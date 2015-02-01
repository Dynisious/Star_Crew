Public Class ServerClient 'An object used to communicate between the Server and a Client
    Inherits Game_Library.Networking.Client
    Private ping As Integer 'An Integer value representing how many milliseconds it takes for a message to receive a reply
    Private pingBegin As Date 'A Date value representing when the ping check began
    Private pingCheckInProgress As Boolean = False 'A Boolean value indicating whether a ping check is in progress
    '-----Sending-----
    Private sendingSemaphore As Threading.Semaphore 'A Semaphore object responsible for allowing the sendThread to loop until all messages are sent and blocking it when there are none to send
    Private sendingMutex As Threading.Mutex 'A Mutex object responsible for syncronising access to sendMessageBuffer
    Private sendMessageBuffers As New List(Of Byte()) 'A List of Byte arrays that are to be sent to the Client
    Private sendErrorMessages As New List(Of String) 'A List of Strings that are the error messages for the message buffers
    Private sendThread As System.Threading.Thread 'A Thread object responsible for sending messages to the Client
    '-----------------
    '-----Receiving-----
    Private receiveThread As System.Threading.Thread 'A Thread object responsible for receiving messages from the Client
    '-------------------
    Public runClient As Boolean = True 'A Boolean value indicating whether to keep the client running
    Private _clientAlive As Boolean = True 'A Boolean value indicating whether the ServerClient is still alive
    Public ReadOnly Property clientAlive As Boolean
        Get
            Return _clientAlive
        End Get
    End Property
    Private myFleet As Fleet 'A Fleet object that this client is in control of
    Private closeReason As String = Environment.NewLine + "SERVER : The " + name + " disconnected for unspecified reasons." 'The reason for the ServerClients disconnect

    Public Sub New(ByRef socket As Net.Sockets.Socket, ByRef successfulConnection As Boolean)
        MyBase.New(socket, maxPing * 2, maxPing * 2)
        Blocking = True
        Try
            Server.Write_To_Log("SERVER : Checking ping from " + RemoteEndPoint.ToString() + "...")
            pingBegin = Date.UtcNow 'The time that the message is to be sent
            Send({1}, Net.Sockets.SocketFlags.None) 'Send a Byte to the Client
            Receive_ByteArray(Net.Sockets.SocketFlags.None, 1) 'Receive a Byte from the Client
            ping = (Date.UtcNow - pingBegin).TotalMilliseconds 'Get the ping of the Client
            If ping <= maxPing Then 'This is an acceptable ping
                Server.Write_To_Log("SERVER : Ping from " + RemoteEndPoint.ToString() + " was " + ping.ToString() + "." +
                                    Environment.NewLine + "SERVER : Initialising ServerClient at " + RemoteEndPoint.ToString() + "...")
                name = Text.ASCIIEncoding.ASCII.GetString(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the name for the Client
                successfulConnection = True 'The connection is successful
                sendingSemaphore = New Threading.Semaphore(0, Integer.MaxValue) 'Create a new Semaphore for sendingSemaphore
                Dim threadSecurity As New Security.AccessControl.MutexSecurity 'Create a new MutexSecurity object
                threadSecurity.AddAccessRule(
                    New Security.AccessControl.MutexAccessRule(Environment.UserName,
                                                               Security.AccessControl.MutexRights.Modify,
                                                               Security.AccessControl.AccessControlType.Allow)) 'Allow the application to wait on the Mutex
                threadSecurity.AddAccessRule(
                    New Security.AccessControl.MutexAccessRule(Environment.UserName,
                                                               Security.AccessControl.MutexRights.Modify,
                                                               Security.AccessControl.AccessControlType.Allow)) 'Allow the application to release the Mutex
                sendingMutex = New Threading.Mutex(False, Nothing, True, threadSecurity) 'Create a new mutex for sendingMutex
                sendThread = New Threading.Thread(AddressOf Sending_Loop) 'Create a new Thread to send messages on
                receiveThread = New Threading.Thread(AddressOf Receiving_Loop) 'Create a new Thread to receive messages on
                sendThread.Start() 'Start sending pending messages
                receiveThread.Start() 'Start receiving messages
                myFleet = New Fleet(Int(Rnd() * Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max), Nothing) 'Create a new Fleet
                myFleet.Switch_Control(AddressOf Player_Update) 'Switch myFleet to be controled by the players input
                Server.game.sectors(Server.game.activeSector).Add_Fleet(myFleet) 'Add myFleet to the game
                Dim message As String = "SERVER : The " + name + " has connected."
                Console.WriteLine(message)
                Server.Write_To_Log(message)
            Else 'The Ping is too high to be accepted
                Dim message As String = "SERVER : The ping from " + RemoteEndPoint.ToString() + " exceeded the maximum."
                Console.WriteLine(message)
                Server.Write_To_Log(message)
                Send(BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Exceeded_Max_Ping_Exception), Net.Sockets.SocketFlags.None) 'Send the exception message to the Client
            End If
        Catch ex As Net.Sockets.SocketException
            Dim message As String = Environment.NewLine + "ERROR : There was an error while connecting a Client to the Server. The Client will now disconnect."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
        Catch ex As Exception
            Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while connecting a Client to the Server. The server will now disconnect."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private playerControl As New Dictionary(Of Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header, Boolean) From {
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up, False},
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down, False},
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right, False},
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left, False},
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Primary, False},
        {Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Missiles, False}} 'The different player inputs
    Private Sub Player_Update() 'Updates myFleet using player input
        For Each i As KeyValuePair(Of Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header, Boolean) In playerControl 'Go through all inputs
            Select Case i.Key
                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up And i.Value
                    myFleet.speed.Current += myFleet.acceleration 'Accelerate
                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down And i.Value
                    myFleet.speed.Current -= myFleet.acceleration 'Decelerate
                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right And i.Value
                    myFleet.direction -= myFleet.turnSpeed 'Turn right
                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left And i.Value
                    myFleet.direction += myFleet.turnSpeed 'Turn left
                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Primary And i.Value

                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Missiles And i.Value

            End Select
        Next
    End Sub

    Public Sub Ping_Check() 'Begins checking the ping of this Client
        If Not pingCheckInProgress Then 'There is not a ping check in progress
            pingCheckInProgress = True
            pingBegin = Date.UtcNow
            Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Ping_Check)},
                         {Environment.NewLine + "ERROR : There was an error while sending the Ping_Check to the " +
                             name + ". The " + name + " will now disconnect."}) 'Begin the ping Check
        End If
    End Sub

    Public Sub Send_Message(ByVal buffers()() As Byte, ByVal errorMessages() As String) 'Safely adds messages to the List of pending messages
        If runClient Then 'New messages are allowed
            sendingMutex.WaitOne() 'Wait for permission to access the send lists
            sendMessageBuffers.AddRange(buffers) 'Add the messages to the List of pending messages
            sendErrorMessages.AddRange(errorMessages) 'Add the error messages to the List of error messages
            sendingSemaphore.Release(buffers.Length) 'Allow the sendThread to begin the next sends
            sendingMutex.ReleaseMutex() 'Release permission to access the send lists
        End If
    End Sub
    Private Sub Sending_Loop() 'Loops sending all pending messages
        Try
            Do
                Do
                    If sendingSemaphore.WaitOne(3000) Then 'There's messages to send
                        Try
                            Send(sendMessageBuffers(0), Net.Sockets.SocketFlags.None) 'Send the message
                        Catch ex As Net.Sockets.SocketException
                            Console.WriteLine(Environment.NewLine + sendErrorMessages(0))
                            Server.Write_To_Log(Environment.NewLine + sendErrorMessages(0) +
                                                      Environment.NewLine + ex.ToString())
                            runClient = False 'Start the Client closing
                            Exit Sub 'Close the sendThread
                        End Try
                        sendingMutex.WaitOne() 'Wait for permission to access the send lists
                        sendMessageBuffers.RemoveAt(0) 'Remove the message
                        sendErrorMessages.RemoveAt(0) 'Remove the error message
                        sendingMutex.ReleaseMutex() 'Release permission to access the send lists
                    End If
                Loop Until sendMessageBuffers.Count = 0 'Loop until all pending messages are sent
            Loop While runClient 'Loop while the Client is alive

            Server.Write_To_Log(Environment.NewLine + "SERVER : Finalising the " + name + "." + closeReason)
            sendingSemaphore.Close() 'Close sendingSemaphore
            sendingMutex.Close() 'Close sendingMutex
            sendMessageBuffers.Clear() 'Clear the pending list
            sendErrorMessages.Clear() 'Clear the error list
            If receiveThread.IsAlive Then receiveThread.Abort() 'Close the receiving thread
            Close()
            Console.WriteLine(closeReason)
            _clientAlive = False
            Server.game.sectors(Server.game.activeSector).Remove_Fleet(myFleet) 'Remove the client's Fleet
            Server.clients.Remove(Me) 'Remove the reference to this ServerClient
        Catch ex As Net.Sockets.SocketException
            Dim message As String = Environment.NewLine + "ERROR : There was an error while sending a message to the " +
                name + ". The " + name + " will now disconnect."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Threading.AbandonedMutexException
            Dim message As String = Environment.NewLine + "ERROR : A Thread exited without releasing sendingMutex. The Server will now close."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
            Server.runningApplication = False 'Close the Server
        Catch ex As Exception
            Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while sending a message to the " +
                name + ". The Server will now close."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private Sub Receiving_Loop() 'Loops receiving all pending messages
        Try
            Do
                Dim header As Integer = Receive_Header(Net.Sockets.SocketFlags.None) 'Receive the message header
                Select Case header 'Act on the header
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                        runClient = False
                        closeReason = Environment.NewLine + "SERVER : The " + name + " disconnected because the Client received a bad message."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting
                        runClient = False
                        closeReason = Environment.NewLine + "SERVER : The " + name + " has disconnected."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Ping_Check
                        ping = (Date.UtcNow - pingBegin).TotalMilliseconds 'Get the new ping from the ping check
                        pingCheckInProgress = False 'Allow a new ping check to begin
                        If ping > maxPing Then 'The Ping is greater than the maximum
                            Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Exceeded_Max_Ping_Exception)},
                                         {Environment.NewLine + "ERROR : There was an error while sending the Exceeded_Max_Ping_Exception to the " +
                                             name + ". The " + name + " will now disconnect."})
                            closeReason = Environment.NewLine + "SERVER : The " + name + " disconnected because the ping exceeded the maximum value."
                        End If
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Primary
                        Receive_ByteArray(Net.Sockets.SocketFlags.None, 1)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Missiles
                        Receive_ByteArray(Net.Sockets.SocketFlags.None, 1)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Heal_Ship
                        Receive_Header(Net.Sockets.SocketFlags.None)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Re_Arm

                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down
                        playerControl(Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down) = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up
                        playerControl(Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up) = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left
                        playerControl(Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left) = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0)
                    Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right
                        playerControl(Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right) = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0)
                    Case Else
                        Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception)},
                                     {Environment.NewLine + "ERROR : There was an error while sending the Bad_Message_Exception to the server. The client will now disconnect."}) 'Send the Bad_Message_Exception to the server
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client disconnected because it received a bad message [" + header.ToString() + "] from the server."
                End Select
            Loop While sendThread.IsAlive 'Run as long as the Client is sending messages
        Catch ex As Game_Library.Networking.Client.ReceiveException When runClient
            Dim message As String = Environment.NewLine + "ERROR : There was an error while receiving a message from the " +
                name + ". The " + name + " will now disconnect."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Net.Sockets.SocketException When runClient
            Dim message As String = Environment.NewLine + "ERROR : There was an error while receiving a message from the " +
                name + ". The " + name + " will now disconnect."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Exception When runClient
            If runClient Then 'Handle the error
                Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while sending a message from the " +
                    name + ". The server will now close."
                Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
                End
            End If
        End Try
    End Sub

    Public Function Generate_Fleet_To_Fleet(ByRef sector As Sector) As Byte()() 'Generates an array of arrays of Bytes representing the message for the client
        Dim fleetsX(sector.fleets.Count - 1) As Integer 'An array of Points that represent the x positions of fleets in the sector
        Dim fleetsY(sector.fleets.Count - 1) As Integer 'An array of Points that represent the y positions of fleets in the sector
        Dim fleetDirections(sector.fleets.Count - 1) As Double 'An array of Doubles that represent the directions that the fleets are facing
        For i As Integer = 0 To sector.fleets.Count - 1 'Loop through all fleets
            fleetsX(i) = sector.fleets(i).X - myFleet.X 'Get the x position of this entities relative to the client's fleet
            fleetsY(i) = sector.fleets(i).Y - myFleet.Y 'Get the y position of this entities relative to the client's fleet
            fleetDirections(i) = sector.fleets(i).direction 'Get the direction that the fleet is facing
        Next
        Dim message As Byte() = Game_Library.Serialisation.ToBytes({myFleet.X, myFleet.Y, myFleet.direction, fleetsX, fleetsY, fleetDirections}) 'Get the Bytes of the message
        Return {BitConverter.GetBytes(Star_Crew_Shared_Libraries.Shared_Values.GameStates.Fleet_Transit), BitConverter.GetBytes(message.Length), message} 'Return the message
    End Function

End Class
