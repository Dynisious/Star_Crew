Public Class Client 'An object used to communicate between the Client_Console and a Client
    Inherits Game_Library.Networking.Client
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
    Public runClient As Boolean = True 'A Boolean value indicating whether the Client should continue to run
    Private _clientAlive As Boolean = True 'A Boolean value indicating whether the Client is still alive
    Private closeReason As String = Environment.NewLine + "CLIENT : The client disconnected for unspecified reasons." 'The reason for the Clients disconnect
    Public ReadOnly Property clientAlive As Boolean
        Get
            Return _clientAlive
        End Get
    End Property

    Public Sub New(ByVal nIP As String, ByVal nPort As Integer)
        MyBase.New(nIP, nPort, -1, -1, Client_Console.settingElements(Client_Console.Settings.Ship_Name))
        Blocking = True
        Try
            Receive_ByteArray(Net.Sockets.SocketFlags.None, 1) 'Wait till a Byte is received
            Send({1}, Net.Sockets.SocketFlags.None) 'Send a Byte to the Server
            Dim buff() As Byte = Text.ASCIIEncoding.ASCII.GetBytes(name) 'Convert the name into Bytes
            Send(BitConverter.GetBytes(buff.Length), Net.Sockets.SocketFlags.None) 'Send the number of Bytes to be received to the Server
            Send(buff, Net.Sockets.SocketFlags.None) 'Send the bytes to the Server
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
            Dim message As String = Environment.NewLine + "CLIENT : The " + name + " has connected to the server at '" + RemoteEndPoint.ToString() + "'."
            Client_Console.Write_To_Log(message)
            Console.WriteLine(message)
        Catch ex As Net.Sockets.SocketException
            Dim message As String = Environment.NewLine + "ERROR : There was an error while connecting to a server at '" +
                RemoteEndPoint.ToString() + "'. The Client will now disconnect."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
        Catch ex As Exception
            Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while connecting to a server at '" +
                RemoteEndPoint.ToString() + "'. The Client will now close."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Public Sub Send_Message(ByVal buffers()() As Byte, ByVal errorMessages() As String) 'Safely adds messages to the List of pending messages
        If runClient = True Then 'New messages are allowed
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
                    If sendingSemaphore.WaitOne(3000) Then 'There are messages to send
                        Try
                            Send(sendMessageBuffers(0), Net.Sockets.SocketFlags.None) 'Send the message
                        Catch ex As Net.Sockets.SocketException
                            Console.WriteLine(Environment.NewLine + sendErrorMessages(0))
                            Client_Console.Write_To_Log(Environment.NewLine + sendErrorMessages(0) +
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

            Client_Console.Write_To_Log(Environment.NewLine + "CLIENT : Finalising the " + name + "." + closeReason)
            sendingSemaphore.Close() 'Close sendingSemaphore
            sendingMutex.Close() 'Close sendingMutex
            sendMessageBuffers.Clear() 'Clear the pending list
            sendErrorMessages.Clear() 'Clear the error list
            If receiveThread.IsAlive Then receiveThread.Abort() 'Close the thread
            Close()
            Console.WriteLine(closeReason)
            _clientAlive = False 'The client is no longer alive
        Catch ex As Net.Sockets.SocketException
            Dim message As String = Environment.NewLine + "ERROR : There was an error while sending a message to the server. The client will now disconnect."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Threading.AbandonedMutexException
            Dim message As String = Environment.NewLine + "ERROR : A Thread exited without releasing sendingMutex. The client will now disconnect."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
            Client_Console.runningApplication = False 'Close the Client_Console
        Catch ex As Exception
            Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while sending a message to the server. The client will now close."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
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
                        closeReason = Environment.NewLine + "CLIENT : The client disconnected because the server received a bad message."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client was kicked."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Ping_Check
                        Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Ping_Check)},
                                     {Environment.NewLine + "ERROR : There was an error while sending the Ping_Check to the server. The client will now disconnect."})
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client disconnected because the server closed."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Lost
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client was defeated."
                    Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Exceeded_Max_Ping_Exception
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client disconnected because the ping exceeded the maximum allowed by the server."
                    Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Fleet_Transit
                        MessageRendering.Render(True, Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message
                    Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Ship_To_Ship_Combat
                        MessageRendering.Render(False, Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message
                    Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Trading
                        Receive_ByteArray(Net.Sockets.SocketFlags.None) 'Receive the message
                    Case Else
                        Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception)},
                                     {Environment.NewLine + "ERROR : There was an error while sending the Bad_Message_Exception to the server. The client will now disconnect."}) 'Send the Bad_Message_Exception to the server
                        runClient = False
                        closeReason = Environment.NewLine + "CLIENT : The client disconnected because it received a bad message [" + header.ToString() + "] from the server."
                End Select
            Loop While sendThread.IsAlive 'Run as long as the Client is sending messages
        Catch ex As Game_Library.Networking.Client.ReceiveException When runClient
            Dim message As String = Environment.NewLine + "ERROR : There was an error while receiving a message from the server. The client will now disconnect."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Net.Sockets.SocketException When runClient
            Dim message As String = Environment.NewLine + "ERROR : There was an error while receiving a message from the server. The client will now disconnect."
            Console.WriteLine(message)
            Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
            runClient = False 'Close the Client
        Catch ex As Exception
            If runClient Then 'Handle the error
                Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while sending a message from the server. The client will now close."
                Client_Console.Write_To_Log(message + Environment.NewLine + ex.ToString())
                End
            End If
        End Try
    End Sub

    Public values(5) As Boolean 'An array of 6 Booleans used to prevent multiple presses of the same keys

End Class
