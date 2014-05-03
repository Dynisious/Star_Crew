Imports System.Net
Imports System.Net.Sockets
Public Class Server 'Encapslates the Galaxy object of the Server
    Public GameWorld As Galaxy 'A Galaxy object to run the game
    Public MyListener As New TcpListener("1225") 'A TcpListener object to receive connection requests from Clients
    Public Clients As New List(Of ServerSideClient) 'A List of ServerSideClient objects to send and receive messages to and from the Clients
    Private SendRecieveList As New List(Of Socket) 'A modefiable List of Socket objects to filter for use
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter 'A Serialiser object to send messages
    Private sendBuff() As Byte 'An Array of Bytes to send to Clients

    Public Sub StartServer() 'Begins the Server object
        GameWorld = New Galaxy
        GameWorld.StartGame() 'Begin to run the game
        Console.WriteLine("Game is now running") 'Write message to console
        If ConsoleWindow.ServerThread.IsAlive = True Then 'Restart the Server
            ConsoleWindow.ServerThread.Abort()
            ConsoleWindow.ServerThread = New Threading.Thread(AddressOf StartCommunications)
        End If
        ConsoleWindow.ServerThread.Start()
    End Sub

    Public Sub StartCommunications() 'Begins the Communucations
        MyListener.Start() 'Starts Listening for connection requests
        Console.WriteLine("Server is now listening on " + MyListener.Server.LocalEndPoint.ToString) 'Write message to Console

        While True
            Listen() 'Listen for connection requests and messages
            Send() 'Send messages to Clients
        End While
    End Sub

    Private Sub Listen() 'Receive messages and handle connection requests
        SendRecieveList.Add(MyListener.Server)
        SendRecieveList.AddRange(Clients)
        '-----Recieve Messages-----
        Socket.Select(SendRecieveList, Nothing, Nothing, -1) 'Filter out all Socket objects that cannot be read
        For Each i As Socket In SendRecieveList 'Go through all Socket objects left in the List
            If ReferenceEquals(i, MyListener.Server) = True Then 'There is a pending connection request
                AddClient(MyListener.AcceptSocket()) 'Add a new ServerSideClient object to the Clients List
                Exit For
            Else
                For Each e As ServerSideClient In Clients 'Check which ServerSideClient object is ready to Read
                    If ReferenceEquals(i, e) = True Then 'They are the same ServerSideClient object
                        e.DecodeMessage() 'Receive the message
                        Exit For 'Exit the for loop and move on
                    End If
                Next
            End If
        Next
        '--------------------------
        SendRecieveList.Clear() 'Clear the List
        SendRecieveList.TrimExcess() 'Remove all blank spaces
    End Sub

    Private Sub Send() 'Send a message to all Clients
        If Clients.Count > 0 Then
            SendRecieveList.AddRange(Clients) 'Add all the ServerSideClient objects to the List

            '-----Send Messages to Clients-----
            Socket.Select(Nothing, SendRecieveList, Nothing, -1) 'Filter out all ServerSideClients that cannot be Written to
            Dim byteStream As New IO.MemoryStream() 'A MemorStream object to serialise the message to Bytes in
            GameWorld.MessageMutex.WaitOne() 'Wait till the Mutex is free
            BinarySerializer.Serialize(byteStream, GameWorld.MessageToSend) 'Serialise the message into Bytes
            GameWorld.MessageMutex.ReleaseMutex() 'Release the Mutex
            sendBuff = byteStream.ToArray 'Create an Array of Bytes from the MemoryStream
            byteStream.Close() 'Close the MemoryStream

            For Each i As ServerSideClient In SendRecieveList 'Send the message to all available ServerSideClient objects
                Try
                    i.Blocking = True 'Set the ServerSideClient to block
                    i.Send(BitConverter.GetBytes(sendBuff.Length())) 'Send 4 Bytes representing how many bytes will be in the next message
                    i.Blocking = True 'Set the ServerSideClient to block
                    i.Send(sendBuff) 'Send the message
                Catch ex As SocketException
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
            '----------------------------------
            SendRecieveList.Clear() 'Clear the List
            SendRecieveList.TrimExcess() 'Remove all blank spaces
        End If
    End Sub

    Public Sub AddClient(ByRef nSocket As Socket) 'Attempt to add the a ServerSideClient object
        If Clients.Count < 4 Then 'There is less than the maximum number of Clients
            Clients.Add(New ServerSideClient(nSocket.DuplicateAndClose(Process.GetCurrentProcess.Id))) 'Add a new ServerSideClient
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
        nSocket.Close() 'Close the connection
    End Sub
End Class
