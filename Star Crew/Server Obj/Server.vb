Imports System.Net
Imports System.Net.Sockets

Module Server
    Private MyListener As New TcpListener("1225")
    Private Ports(0) As Socket
    Private Clients(-1) As ServerSideClient
    Public GameWorld As New Galaxy
    Public comms As New Threading.Thread(AddressOf StartComms)

    Private Class ServerSideClient
        Public MySocket As Socket
        Private MyStation As Station.StationTypes = Station.StationTypes.Max

        Public Sub New(ByRef nSocket As Socket)
            MySocket = nSocket
        End Sub

        Public Sub DecodeMessage()
            Dim receiveBuff(MySocket.ReceiveBufferSize) As Byte
            Dim receivedBuffLength As Integer = MySocket.Receive(receiveBuff, receiveBuff.Length, SocketFlags.None)
            ReDim Preserve receiveBuff(receivedBuffLength)
            If MyStation = Station.StationTypes.Max Then
                Dim str As String = System.Text.Encoding.ASCII.GetString(receiveBuff, 0, receivedBuffLength).Trim(ChrW(0))
                If CInt(str) < Station.StationTypes.Max Then
                    MyStation = CInt(str)
                End If
                Screen.ConsoleWindow.WriteLine((MyStation.ToString() + ": Has been connected"))
                'Select Case MyStation
                '    Case Station.StationTypes.Helm
                '        Galaxy.clientShip.Helm.PlayerControled = True
                '    Case Station.StationTypes.Batteries
                '        Galaxy.clientShip.Batteries.PlayerControled = True
                '    Case Station.StationTypes.Shielding
                '        Galaxy.clientShip.Shielding.PlayerControled = True
                '    Case Station.StationTypes.Engineering
                '        Galaxy.clientShip.Engineering.PlayerControled = True
                'End Select
            Else
                Using fs As New IO.MemoryStream(receiveBuff)
                    Dim bf As System.Runtime.Serialization.Formatters.Binary.BinaryFormatter = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    Select Case MyStation
                        Case Station.StationTypes.Helm
                            Galaxy.clientShip.HelmMessage(bf.Deserialize(fs))
                        Case Station.StationTypes.Batteries
                            Galaxy.clientShip.BatteriesMessage(bf.Deserialize(fs))
                        Case Station.StationTypes.Shielding
                            Galaxy.clientShip.ShieldingMessage(bf.Deserialize(fs))
                        Case Station.StationTypes.Engineering
                            Galaxy.clientShip.EngineeringMessage(bf.Deserialize(fs))
                    End Select
                End Using
            End If
        End Sub
    End Class

    Public Sub StartServer()
        GameWorld.CreateWorld()
        Screen.ConsoleWindow.WriteLine("Game is now running")
        If comms.IsAlive = False Then
            comms.Start()
        End If
        Screen.ConsoleWindow.WriteLine("Server is listening on port 1225")
    End Sub

    Private Sub StartComms()
        Ports(0) = MyListener.Server
        MyListener.Start()

        While True
            ListenForMessage()
        End While
    End Sub

    Private Sub ListenForMessage()
        Dim socketList As New ArrayList
        For i As Integer = 0 To UBound(Ports)
            socketList.Add(Ports(i))
        Next
        Socket.Select(socketList, Nothing, Nothing, -1)

        For Each i As Socket In socketList
            If ReferenceEquals(i, MyListener.Server) = True Then
                AddClient()
            Else
                For e As Integer = 0 To UBound(Clients)
                    If ReferenceEquals(i, Clients(e).MySocket) Then
                        Clients(e).DecodeMessage()
                        Exit For
                    End If
                Next
            End If
        Next

        Server.GameWorld.CanSend = False
        socketList.Clear()
        For i As Integer = 0 To UBound(Ports)
            socketList.Add(Ports(i))
        Next
        Socket.Select(Nothing, Nothing, socketList, 50000)
        For Each i As Socket In socketList
            removeClient(i)
        Next
        socketList.Clear()
        For i As Integer = 0 To UBound(Ports)
            socketList.Add(Ports(i))
        Next
        Socket.Select(Nothing, socketList, Nothing, -1)
        While Server.GameWorld.CanSend = False
        End While

        For Each i As Socket In socketList
            If ReferenceEquals(i, MyListener.Server) = False Then
                Try
                    i.Send(ServerMessage.NewMessage())
                Catch ex As Net.Sockets.SocketException
                    removeClient(i)
                End Try
            End If
        Next

    End Sub

    Private Sub AddClient()
        If UBound(Ports) < 4 Then
            ReDim Preserve Ports(Ports.Length)
            Ports(UBound(Ports)) = MyListener.AcceptSocket()
            ReDim Preserve Clients(Clients.Length)
            Clients(UBound(Clients)) = New ServerSideClient(Ports(UBound(Ports)))
        Else
            MyListener.Stop()
            MyListener.Start()
            Screen.ConsoleWindow.WriteLine()
            Screen.ConsoleWindow.WriteLine("Client could not connect: Server full")
            Screen.ConsoleWindow.WriteLine()
        End If
    End Sub

    Private Sub removeClient(ByRef nSocket As Socket)
        nSocket.Close()
        Dim index As Integer = Array.IndexOf(Ports, nSocket)
        If index <> UBound(Ports) Then
            For e As Integer = index To UBound(Ports)
                If e <> UBound(Ports) Then
                    Ports(e) = Ports(e + 1)
                End If
            Next
        End If
        ReDim Preserve Ports(UBound(Ports) - 1)
        For e As Integer = 0 To UBound(Clients)
            If ReferenceEquals(Clients(e).MySocket, nSocket) = True Then
                If e <> UBound(Clients) Then
                    For f As Integer = e To UBound(Clients)
                        If f <> UBound(Clients) Then
                            Clients(f) = Clients(f + 1)
                        End If
                    Next
                    ReDim Preserve Clients(UBound(Clients) - 1)
                End If
                Exit For
            End If
        Next
    End Sub

End Module
