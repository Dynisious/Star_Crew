Imports System.Net
Imports System.Net.Sockets
Public Class Server
    Public GameWorld As New Galaxy
    Public MyListener As New TcpListener("1225")
    Public Clients As New List(Of ServerSideClient)
    Private SendRecieveList As New List(Of Socket)
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
    Private sendBuff() As Byte

    Public Sub StartServer()
        For Each i As ServerSideClient In GameServer.Clients
            If ReferenceEquals(GameServer.MyListener.Server, i) = False Then
                GameServer.RemoveClient(i, True)
            End If
        Next
        GameWorld.StartGame()
        Console.WriteLine("Game is now running")
        If ServerThread.IsAlive = False Then
            ServerThread.Start()
        End If
    End Sub

    Public Sub StartCommunications()
        MyListener.Start()
        Console.WriteLine("Server is now listening on " + MyListener.Server.LocalEndPoint.ToString)

        While True
            Listen()
            Send()
        End While
    End Sub

    Private Sub Listen()
        SendRecieveList.Add(MyListener.Server)
        SendRecieveList.AddRange(Clients)
        '-----Recieve Messages-----
        Socket.Select(SendRecieveList, Nothing, Nothing, -1)
        For Each i As Socket In SendRecieveList
            If ReferenceEquals(i, MyListener.Server) = True Then
                AddClient(MyListener.AcceptSocket())
                Exit For
            Else
                For Each e As ServerSideClient In Clients
                    If ReferenceEquals(i, e) Then
                        e.DecodeMessage()
                        Exit For
                    End If
                Next
            End If
        Next
        '--------------------------
        SendRecieveList.Clear()
        SendRecieveList.TrimExcess()
    End Sub

    Private Sub Send()
        If Clients.Count > 0 Then
            SendRecieveList.AddRange(Clients)
            '-----Send Messages to Clients-----
            Socket.Select(Nothing, SendRecieveList, Nothing, -1)
            For Each i As ServerSideClient In SendRecieveList
                Try
                    Dim byteStream As New IO.MemoryStream()
                    BinarySerializer.Serialize(byteStream, GameWorld.MessageToSend)
                    sendBuff = byteStream.ToArray
                    i.Blocking = True
                    i.Send(BitConverter.GetBytes(sendBuff.Length()))
                    i.Blocking = True
                    i.Send(sendBuff)
                    byteStream.Close()
                Catch ex As SocketException
                    RemoveClient(i, True)
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
            SendRecieveList.Clear()
            SendRecieveList.TrimExcess()
        End If
    End Sub

    Public Sub AddClient(ByRef nSocket As Socket)
        If Clients.Count < 5 Then
            Clients.Add(New ServerSideClient(nSocket.DuplicateAndClose(Process.GetCurrentProcess.Id)))
        Else
            Dim temp As Socket = MyListener.AcceptSocket()
            Console.WriteLine(temp.RemoteEndPoint.ToString + ": Could not be connected. Server is full")
            temp.Close()
        End If
    End Sub

    Public Sub RemoveClient(ByRef nSocket As ServerSideClient, ByVal resetControl As Boolean)
        For Each i As ServerSideClient In Clients
            If ReferenceEquals(i, nSocket) = True Then
                Console.WriteLine(i.MyStation.ToString + ": Client was disconnected")
                If resetControl = True Then
                    Select Case CType(i, ServerSideClient).MyStation
                        Case Station.StationTypes.Helm
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.PlayerControled = False
                        Case Station.StationTypes.Batteries
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Batteries.PlayerControled = False
                        Case Station.StationTypes.Shielding
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.PlayerControled = False
                        Case Station.StationTypes.Engineering
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.PlayerControled = False
                    End Select
                End If
                Clients.Remove(i)
                Clients.TrimExcess()
                Exit For
            End If
        Next
        nSocket.Close()
    End Sub
End Class
