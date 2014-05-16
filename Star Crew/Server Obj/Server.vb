Imports System.Net
Imports System.Net.Sockets
Public Class Server 'Encapslates the Galaxy object of the Server
    Public GameWorld As Galaxy 'A Galaxy object to run the game
    Public MyListener As New TcpListener("1225") 'A TcpListener object to receive connection requests from Clients
    Public Clients As New List(Of ServerSideClient) 'A List of ServerSideClient objects to send and receive messages to and from the Clients
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter 'A Serialiser object to send messages
    Private sendBuff() As Byte 'An Array of Bytes to send to Clients
    Private sendReceiveList As New List(Of ServerSideClient) 'A List of the Clients to communicate with at a particular time
    Public ServerLoop As Boolean = True 'A Boolean to keep the Server Looping

    Public Sub StartServer() 'Begins the Server object
        Console.WriteLine("Initialising...")
        ServerLoop = True 'Let the Server Loop until it is disposed of
        ConsoleWindow.ServerThread = New Threading.Thread(AddressOf StartCommunications) 'Create a new Thread for the Server
        GameWorld = New Galaxy
        GameWorld.StartGame() 'Begin to run the game
        Console.WriteLine("Game is now running") 'Write message to console
        ConsoleWindow.ServerThread.Start()
    End Sub

    Public Sub CloseServer()
        ServerLoop = False 'Lets the Servers Loop finish
        GameWorld.GalaxyTimer.Stop() 'Stops the Galaxy Object from Updating
        If Clients.Count > 0 Then 'There's Clients to dispose of
            For i As Integer = 0 To Clients.Count - 1  'Remove any Clients left behind from the last session
                Dim temp As ServerSideClient = Clients(0)
                RemoveClient(temp, True)
            Next
        End If
        MyListener.Stop()
        GameWorld.MessageMutex.Close() 'Closes the Mutex Object
    End Sub

    Public Sub StartCommunications() 'Begins the Communucations
        MyListener.Start() 'Starts Listening for connection requests
        Console.WriteLine("Server is now listening on " + MyListener.Server.LocalEndPoint.ToString) 'Write message to Console

        While ServerLoop = True
            If MyListener.Pending() = True Then 'There's a pending connection request
                AddClient(MyListener.AcceptSocket()) 'Add the new Client
            End If
            If Clients.Count > 0 Then 'There's Clients to talk with
                Send() 'Send messages to Clients
                Listen() 'Listen for messages
            End If
        End While
    End Sub

    Private Sub Listen() 'Receive messages and handle connection requests
        '-----Recieve Messages-----
        sendReceiveList.AddRange(Clients) 'Add the Clients to the List
        Socket.Select(sendReceiveList, Nothing, Nothing, -1) 'Wait till there's data to read
        For Each i As ServerSideClient In sendReceiveList
            i.DecodeMessage() 'Receive the message
        Next
        sendReceiveList.Clear() 'Clear the List
        sendReceiveList.TrimExcess() 'Remove all spare spots from the List
        '--------------------------
    End Sub

    Private Sub Send() 'Send a message to all Clients
        If Clients.Count > 0 Then
            '-----Send Messages to Clients-----
            sendReceiveList.AddRange(Clients) 'Add the Clients to the List
            Socket.Select(Nothing, sendReceiveList, Nothing, -1) 'Filter out all Sockets that can't be sent to right now
            Dim byteStream As New IO.MemoryStream() 'A MemorStream object to serialise the message to Bytes in
            '            Console.WriteLine("Server Wait")
            GameWorld.MessageMutex.WaitOne() 'Wait till the Mutex is free
            BinarySerializer.Serialize(byteStream, GameWorld.MessageToSend) 'Serialise the message into Bytes
            GameWorld.MessageMutex.ReleaseMutex() 'Release the Mutex
            sendBuff = byteStream.ToArray 'Create an Array of Bytes from the MemoryStream
            byteStream.Close() 'Close the MemoryStream

            For Each i As ServerSideClient In sendReceiveList 'Send the message to all available ServerSideClient objects
                Try
                    i.Blocking = True 'Set the ServerSideClient to block
                    i.Send(BitConverter.GetBytes(sendBuff.Length())) 'Send 4 Bytes representing how many bytes will be in the next message
                    i.Blocking = True 'Set the ServerSideClient to block
                    i.Send(sendBuff) 'Send the message
                Catch ex As SocketException
                    Console.WriteLine()
                    Console.WriteLine("Error : There was an problem communicating with the Client.")
                    Console.WriteLine()
                    Console.WriteLine(ex.ToString())
                    RemoveClient(i, True) 'The Client has disconnected, remove the ServerSideClient object
                Catch ex As Exception
                    Console.WriteLine()
                    Console.WriteLine("Error : There was an unexpected and unhandled exception.")
                    Console.WriteLine("please submit it as an issue at the URL bellow")
                    Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
                    Console.WriteLine()
                    Console.WriteLine(ex.ToString)
                    Console.WriteLine()
                End Try
            Next
            sendReceiveList.Clear() 'Clear the List
            sendReceiveList.TrimExcess() 'Remove spare spots from the List
            '----------------------------------
        End If
    End Sub

    Public Sub AddClient(ByRef nSocket As Socket) 'Attempt to add the a ServerSideClient object
        If Clients.Count < 4 Then 'There is less than the maximum number of Clients
            Clients.Add(New ServerSideClient(nSocket.DuplicateAndClose(Process.GetCurrentProcess.Id))) 'Add a new ServerSideClient
            Clients(Clients.Count - 1).DecodeMessage() 'Finalise the Client's connection
        Else 'There is no more Space for Clients
            Console.WriteLine(nSocket.RemoteEndPoint.ToString + ": Could not be connected. Server is full") 'Write the message to the console
            nSocket.Close() 'Close the connection
        End If
    End Sub

    Public Sub RemoveClient(ByRef nSocket As ServerSideClient, ByVal resetControl As Boolean) 'Remove the specified ServerSideClient from the List
        For Each i As ServerSideClient In Clients 'Find the object
            If ReferenceEquals(i, nSocket) = True Then 'It is the same ServerSideClient object
                Console.WriteLine(i.MyStation.ToString + ": Client was disconnected") 'Write message to the Console
                If resetControl = True Then 'Reset the control of the Station to be AI
                    Select Case i.MyStation
                        Case Station.StationTypes.Helm 'Reset the Helm
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.PlayerControled = False
                        Case Station.StationTypes.Batteries 'Reset the Batteries
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Batteries.PlayerControled = False
                        Case Station.StationTypes.Shielding 'Reset Shielding
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.PlayerControled = False
                        Case Station.StationTypes.Engineering 'Reset the Engineering
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.PlayerControled = False
                    End Select
                End If
                Clients.Remove(i) 'Remove the ServerSideClient from the List
                Clients.TrimExcess() 'Remove the blank space
                Exit For 'Exit the for loop
            End If
        Next
        nSocket.Disconnect(False)
        nSocket.Close() 'Close the connection
    End Sub
End Class
