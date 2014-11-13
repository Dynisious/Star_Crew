Public Class ServerClient
    Inherits Game_Library.Networking.Client
    Public Craft As Ship 'The Ship that this Client controls
    Public receivingAlive As Boolean = True 'Keeps the receiving thread open
    Public sendingAlive As Boolean = True 'Keeps the sending thread open
    Private receiveThread As System.Threading.Thread 'A Thread object used to receive messages from the Client
    Private sendThread As System.Threading.Thread 'A Thread object used to send messages to the Client
    Private sendingSemaphore As System.Threading.Semaphore 'A Semaphore object used to block the sendThread until data is ready
    Private accessSendList As System.Threading.Mutex 'A Mutex object used to synchronise access to SendList
    Private sendList As New List(Of Byte()) 'A List object of Bytes to be sent
    Private errorList As New List(Of String) 'A List of strings that represent the error messages for the messages to be sent
    Private waitToClose As System.Threading.Mutex 'A Semaphore object used to block the receiveThread from closing until the SendThread is closed
    Private disconnecting = False 'A Boolean value indicating whether the Sending thread should continue sending or close for a disconnect
    Private _throttleUp As Boolean = False 'The actual value of throttleUp
    Public ReadOnly Property throttleUp As Boolean 'A Boolean value indicating whether the throttle needs to increase
        Get
            Return _throttleUp
        End Get
    End Property
    Private _throttleDown As Boolean = False 'The actual value of throttleDown
    Public ReadOnly Property throttleDown As Boolean 'A Boolean value indicating whether the throttle needs to decrease
        Get
            Return _throttleDown
        End Get
    End Property
    Private _turnRight As Boolean = False 'The actual value of turnRight
    Public ReadOnly Property turnRight As Boolean 'A Boolean value indicating whether the Ship needs to turn right
        Get
            Return _turnRight
        End Get
    End Property
    Private _turnLeft As Boolean = False 'The actual value of turnLeft
    Public ReadOnly Property turnLeft As Boolean 'A Boolean value indicating whether the ship needs to turn left
        Get
            Return _turnLeft
        End Get
    End Property
    Private _fireWeapons As Boolean = False 'The actual value of fireWeapons
    Public ReadOnly Property fireWeapons As Boolean 'A Boolean value indicating whether the Ship needs to fire Weapons
        Get
            Return _fireWeapons
        End Get
    End Property

    Public Sub New(ByRef nSocket As System.Net.Sockets.Socket, ByVal index As Integer, ByRef successfulConnection As Boolean)
        MyBase.New(nSocket, 300, -1, "Unnamed Client", index)
        Console.WriteLine(Environment.NewLine + "Server : Connecting a new Client at '" + RemoteEndPoint.ToString() + "'...")
        successfulConnection = False 'The connection is not yet successful
        Name = Text.ASCIIEncoding.ASCII.GetString(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Get the name of the Ship
        sendingSemaphore = New System.Threading.Semaphore(0, 1) 'Create a new Semaphore for the sending thread
        accessSendList = New System.Threading.Mutex(False) 'Create a new Mutex for synchronising access to SendList
        waitToClose = New System.Threading.Mutex(False) 'Create a new Mutex for stopping the Socket from closing until all sends are completed
        sendThread = New System.Threading.Thread(AddressOf Sending) 'Create a new Thread for sending messages
        sendThread.Start()
        receiveThread = New System.Threading.Thread(AddressOf Receiving) 'Create a new Thread for receiving messages
        receiveThread.Start() 'Start the thread
        successfulConnection = True 'The connection was successful
        Craft = New PlayerShip(Me)
        Dim temp As String = ("Server : The '" + Name + "' has been connected.")
        Server.Combat.adding.Add(Craft)
        Console.WriteLine(temp) 'Write that the Client is connected and the seconds for the ping
        Server.Write_To_Error_Log(Environment.NewLine + temp) 'Write that the Client is connected and the seconds for the ping
        If successfulConnection = False Then Close() 'Close the socket
    End Sub

    Public Sub Send_Message(ByVal messages()() As Byte, ByVal errorMessages() As String)
        Try
            accessSendList.WaitOne()
            If sendList.Count = 0 Then sendingSemaphore.Release() 'Release the semaphore so that the sendThread starts sending
            sendList.AddRange(messages) 'Add the messages to the list
            errorList.AddRange(errorMessages) 'Add the error messages to the List
            accessSendList.ReleaseMutex()
        Catch ex As Exception
            Server.Write_To_Error_Log("ERROR : There was an error while calling Send_Message() in the " + Name + ". Server will now close.")
            End
        End Try
    End Sub
    Private Sub Sending() 'Sends each of the elements in data
        Try
            waitToClose.WaitOne() 'Hold the mutex until the Send thread is closed
            Do
                sendingSemaphore.WaitOne()
                Do
                    accessSendList.WaitOne()
                    Try
                        Send(sendList(0), Net.Sockets.SocketFlags.None) 'Send the message
                        sendList.RemoveAt(0)
                        errorList.RemoveAt(0)
                    Catch ex As Exception
                        Server.Write_To_Error_Log(Environment.NewLine + errorList(0) + Environment.NewLine + ex.ToString()) 'Write to the error log
                        End
                    End Try
                    accessSendList.ReleaseMutex()
                Loop Until sendList.Count = 0 Or disconnecting 'Loop while there's messages to send
            Loop While sendingAlive 'Loop while the sending is alive
            waitToClose.ReleaseMutex() 'The Send thread is closed
        Catch ex As Net.Sockets.SocketException
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while sending a message to the " +
                                      Name + ". The " + Name + " will now disconnect." + Environment.NewLine + ex.ToString())
            sendingAlive = False
            receivingAlive = False
            disconnecting = True
        Catch ex As Exception
            Server.Write_To_Error_Log("ERROR : There was an unexpected and unhandled error while sending a message to the " + Name + ". Server will now close.")
            End
        End Try
    End Sub

    Private Sub Receiving() 'Receives messages
        Try
            While receivingAlive 'Loop while comms are open
                If Available <> 0 Then 'Theres data to receive
                    Select Case Receive_Header(Net.Sockets.SocketFlags.None) 'Decide what to do with the received header
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                            receivingAlive = False 'Close the Client
                            sendingAlive = False 'Close the Client
                            disconnecting = True
                            Dim temp As String = (Environment.NewLine + "Server : The " + Name + " is disconnecting because it received a bad message from the Server.")
                            Console.WriteLine(temp)
                            Server.Write_To_Error_Log(temp)
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting
                            receivingAlive = False 'Close the Client
                            sendingAlive = False 'Close the Client
                            disconnecting = True
                            Dim temp As String = (Environment.NewLine + "Server : The " + Name + " is disconnecting from the Server.")
                            Console.WriteLine(temp)
                            Server.Write_To_Error_Log(temp)
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Weapons
                            _fireWeapons = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Set the value
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down
                            _throttleDown = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Set the value
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up
                            _throttleUp = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Set the value
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left
                            _turnLeft = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Set the value
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right
                            _turnRight = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Set the value
                        Case Else
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : The Client sent an unknown message header. The " + Name + " will now disconnect.")
                            Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception)},
                                         {"ERROR : There was an error sending the Bad_Message_Exception to the " + Name + ". Server will now close."})
                            sendingAlive = False
                            receivingAlive = False
                    End Select
                End If
            End While
        Catch ex As Net.Sockets.SocketException
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while receiving a message from the " +
                                      Name + ". The " + Name + " will now disconnect." + Environment.NewLine + ex.ToString())
            sendingAlive = False
            receivingAlive = False
            disconnecting = True
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while receiving a message from the " +
                                      Name + ". The Server will now close." + Environment.NewLine + ex.ToString())
            End
        End Try

        Finalise_Client()
    End Sub

    Private Sub Finalise_Client()
        Try
            waitToClose.WaitOne() 'Wait till the send thread is done before closing
            If Not Craft.Dead Then Craft.Dead = True 'Kill the Client craft
            Server.Comms.InteractWithClients.WaitOne() 'Wait till this Client has the right to interact with Clients
            Server.Comms.clientList.RemoveAt(Index) 'Remove this Client
            If Index <> Server.Comms.clientList.Count Then 'There're Clients whos indexs have changed
                For i As Integer = Index To Server.Comms.clientList.Count - 1 'Loop through all Clients
                    Server.Comms.clientList(i).Index = i 'Set the Client's index
                Next
            End If
            Server.Comms.InteractWithClients.ReleaseMutex() 'Release the mutex
            Close() 'Close the socket
            waitToClose.ReleaseMutex()
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while closing the connection to the " + Name + ". The Server will now close." +
                                      Environment.NewLine + ex.ToString())
            End
        End Try
        Console.WriteLine(Environment.NewLine + "Server : The " + Name + " has now disconnected.")
    End Sub

    Public Sub Generate_Message(ByVal objects As List(Of Ship)) 'Creates a message for the ServerClient to send
        If Craft.Dead And receivingAlive Then 'The craft is dead and the Client needs to disconnect
            Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception)}, {"ERROR : There was an error while sending the Client_Kicked_Exception to the " + Name + ". Server will now close."}) 'Send the Disconnect message
            sendingAlive = False
            receivingAlive = False
        ElseIf Craft.CombatIndex <> -1 Then 'The craft is still alive and messages need to be sent
            Dim positions(objects.Count - 1) As System.Drawing.Point 'An array of point objects to be sent to the Client
            Dim directions(objects.Count - 1) As Double 'An array of double values to be sent to the Client
            Dim allegiances(objects.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'An array of alleiances values to be sent to the Client
            Dim types(objects.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'An array of ObjectType values to be sent to the Client
            Dim hits(objects.Count - 1) As Boolean 'An array of Boolean values to be sent to the Client
            For i As Integer = 0 To objects.Count - 1 'Loop through all Indexs
                positions(i) = New System.Drawing.Point((objects(i).X - Craft.X), (objects(i).Y - Craft.Y)) 'Create a point object relative to the object
                directions(i) = objects(i).Direction 'Set the object's direction
                allegiances(i) = -1 'Set the object's allegiance
                types(i) = objects(i).Type
                hits(i) = objects(i).hit 'Set the object's hit state
            Next
            allegiances(Craft.CombatIndex) = 1 'Set the craft's allegiance
            Dim message As Byte() = Game_Library.Serialisation.ToBytes({positions, directions, allegiances, types, hits, Craft.Throttle.Current,
                                                                        Craft.Throttle.Maximum, Craft.CombatIndex, Craft.firing, Craft.Hull.Current,
                                                                        Craft.Hull.Maximum, Craft.Primary.Ammunition.Current, Craft.Primary.Ammunition.Maximum}) 'Generate the message to send to the Client
            Dim errorMessages() As String = {"ERROR : There was an error while sending the Ship_To_Ship header to the " + Name + ". Server will now close.",
                                             "ERROR : There was an error while sending the Ship_To_Ship message length to the " + Name + ". Server will now close.",
                                             "ERROR : There was an error while sending the Ship_To_Ship message to the " + Name + ". Server will now close."}
            Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship),
                            BitConverter.GetBytes(message.Length), message}, errorMessages)
        End If
    End Sub

End Class
