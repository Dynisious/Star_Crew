Imports System.Net
Imports System.Net.Sockets

Module Server
    Private Enum commands
        Close
        Help
        Start
        Suspend
        Play
        KickPlayer
        Clear
        max
    End Enum
    Private commandList As New Dictionary(Of commands, String) From {
        {commands.Close, "/cls"},
        {commands.Help, "/help"},
        {commands.Start, "/start"},
        {commands.Suspend, "/stop"},
        {commands.Play, "/play"},
        {commands.KickPlayer, "/kick"},
        {commands.Clear, "/clr"}
    }
    Private helpList As New Dictionary(Of commands, String) From {
        {commands.Close, "/cls :      Closes the application"},
        {commands.Help, "/help :     Displays help with console commands"},
        {commands.Start, "/start :      Starts the server"},
        {commands.Suspend, "/stop :     Pauses the execution of the game"},
        {commands.Play, "/play :        Resumes the execution of the game"},
        {commands.KickPlayer, "/kick :      Removes the player from the game"},
        {commands.Clear, "/clr :        Clears the screen"}
    }
    Private MyListener As New TcpListener("1225")
    Private Ports(0) As Socket
    Private Clients(-1) As ServerSideClient
    Public GameWorld As New Galaxy
    Public OutputScreen As New Screen
    Public Running As Boolean = True
    Public client As New Threading.Thread(AddressOf OutputScreen.open)
    Public comms As New Threading.Thread(AddressOf StartComms)

    Private Class ServerSideClient
        Public MySocket As Socket
        Public MyStation As Station.StationTypes = Station.StationTypes.Max

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
                Console.WriteLine((MyStation.ToString() + ": Has been connected"))
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

    Public Sub Main()
        client.Start()
        Console.WriteLine("-----Star Crew-----")
        Console.WriteLine("for help with commands type '/help'")
        Console.WriteLine()
        While Running = True
            Dim str As String = Console.ReadLine().Trim(ChrW(0))
            If str.StartsWith("/") Then
                RunCommand(str)
            End If
        End While
    End Sub

    Private Sub RunCommand(ByVal nCommand As String)
        Console.WriteLine()
        Dim command As commands = commands.max
        For Each i As KeyValuePair(Of commands, String) In commandList
            If i.Value = nCommand Then
                command = i.Key
                Exit For
            End If
        Next

        Select Case command
            Case commands.Close
                End
            Case commands.Help
                Console.WriteLine("All commands must be lowercase")
                For Each i As KeyValuePair(Of commands, String) In helpList
                    Console.WriteLine(i.Value)
                Next
                Console.WriteLine()
            Case commands.Start
                StartServer()
            Case commands.Suspend
                If comms.IsAlive = True Then
                    comms.Suspend()
                Else
                    Console.WriteLine("Server is not active type '/start' to start")
                End If
            Case commands.Play
                If comms.IsAlive = True Then
                    comms.Resume()
                Else
                    Console.WriteLine("Server is not active type '/start' to start")
                End If
            Case commands.KickPlayer
                Console.WriteLine("Type 'helm' 'batteries' 'shielding' 'engineering' or 'cancel'")
                Dim stationType As Station.StationTypes = Station.StationTypes.Max
                Dim str As String
                While stationType = Station.StationTypes.Max
                    str = Console.ReadLine().Trim(ChrW(0))
                    Select Case str
                        Case "helm"
                            stationType = Station.StationTypes.Helm
                        Case "batteries"
                            stationType = Station.StationTypes.Batteries
                        Case "shielding"
                            stationType = Station.StationTypes.Shielding
                        Case "engineering"
                            stationType = Station.StationTypes.Engineering
                        Case "cancel"
                            Console.WriteLine("Kicking canceled")
                            Exit Sub
                        Case Else
                            WriteLine("Station not recognised. Check for capitals and spelling")
                    End Select
                    For Each i As ServerSideClient In Clients
                        If i.mystation = stationType Then
                            removeClient(i.MySocket)
                            WriteLine(str + " Has been kicked")
                            Exit Sub
                        End If
                    Next
                End While
            Case commands.Clear
                Console.Clear()
                Console.WriteLine("-----Star Crew-----")
                Console.WriteLine("for help with commands type '/help'")
                Console.WriteLine()
            Case commands.max
                Console.WriteLine("Error: Command not recognised")
                Console.WriteLine("Check spelling and capitals and try again")
        End Select
    End Sub

    Public Sub StartServer()
        GameWorld.CreateWorld()
        Console.WriteLine("Game is now running")
        If comms.IsAlive = False Then
            comms.Start()
        End If
    End Sub

    Private Sub StartComms()
        Ports(0) = MyListener.Server
        MyListener.Start()
        Console.WriteLine("Server is listening on " + MyListener.Server.LocalEndPoint.ToString())

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
            Console.WriteLine()
            Console.WriteLine("Client could not connect: Server full")
            Console.WriteLine()
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
