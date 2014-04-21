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
    Public OutputScreen As New Screen
    Public client As New Threading.Thread(AddressOf OutputScreen.Open)
    Public comms As New Threading.Thread(AddressOf ServerComms.StartCommunications)

    Public Sub Main()
        client.Start()
        Console.WriteLine("-----Star Crew-----")
        Console.WriteLine("for help with commands type '/help'" + Environment.NewLine +
                          Environment.NewLine +
                          "-----Station Controls-----" + Environment.NewLine +
                          "Helm (Must be maned at all times): Steer the helm with the" + Environment.NewLine +
                          "'Left' and 'Right' arrow keys." + Environment.NewLine +
                          "Speed Up and Down the the 'Up' and 'Down' arrow keys." + Environment.NewLine +
                          "Have the computer Match your targets speed automatically with 'M'." + Environment.NewLine +
                          "Press 'J' to exit combat to Warp Speed." + Environment.NewLine +
                          "Steer the Fleet while flying in the sector as you steer the Helm" + Environment.NewLine +
                          Environment.NewLine +
                          "Batteries: Rotate both Guns with the 'Left' and 'Right' arrow keys." + Environment.NewLine +
                          "Fire the Primary Gun with 'A'." + Environment.NewLine +
                          "Fire the Secondary Gun with 'D'." + Environment.NewLine +
                          "Select a New Target with 'M'." + Environment.NewLine +
                          Environment.NewLine +
                          "Shielding: Set the Shield to Prioritise with the arrow keys." + Environment.NewLine +
                          Environment.NewLine +
                          "Engineering: Control the Power Core's Temperature with the 'Up' and 'Down'" + Environment.NewLine +
                          "Arrow Keys; keep the temperature as close to 50*e^5 as possible or the Power" + Environment.NewLine +
                          "Core will not be at full efficiency." + Environment.NewLine +
                          Environment.NewLine +
                          "-----Sector Mechanics-----" + Environment.NewLine +
                          "Green: Green Fleets are Friendly and if you come close enough to them they" + Environment.NewLine +
                          "will add their forces to yours." + Environment.NewLine +
                          "Red: Red Fleets are the enemy and they will also combine forces to combat" + Environment.NewLine +
                          "yours." + Environment.NewLine +
                          "Yellow: Yellow Fleets are Neutral and will Repair any ships in a Fleet" + Environment.NewLine +
                          "up to 40% hull." + Environment.NewLine +
                          "Interaction: To interact with a Fleet collide your Fleet into it." + Environment.NewLine +
                          "Objective: The Objective of this game is to eleminate all Enemy Fleets." + Environment.NewLine +
                          Environment.NewLine)
        While True
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
                If comms.ThreadState = Threading.ThreadState.Running Then
                    comms.Suspend()
                    Console.WriteLine("Game has been paused")
                ElseIf comms.IsAlive = False Then
                    Console.WriteLine("Server is not active type '/start' to start")
                ElseIf comms.ThreadState = Threading.ThreadState.SuspendRequested Then
                    Console.WriteLine("Game is already paused")
                End If
            Case commands.Play
                If comms.ThreadState = Threading.ThreadState.SuspendRequested Then
                    comms.Resume()
                    Console.WriteLine("Game has been resumed")
                ElseIf comms.IsAlive = False Then
                    Console.WriteLine("Server is not active type '/start' to start")
                ElseIf comms.ThreadState = Threading.ThreadState.Running Then
                    Console.WriteLine("Game is already running")
                ElseIf comms.IsAlive = False Then
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
                            Console.WriteLine("Station not recognised. Check for capitals and spelling")
                    End Select
                    For Each i As Socket In ServerComms.Ports
                        If i.GetType = GetType(ServerSideClient) Then
                            If CType(i, ServerSideClient).MyStation = stationType Then
                                ServerComms.RemoveClient(i, True)
                                Console.WriteLine(str + ": Has been kicked")
                                Exit Sub
                            End If
                        End If
                    Next
                    If stationType <> Station.StationTypes.Max Then
                        Console.WriteLine("That player was not found")
                        stationType = Station.StationTypes.Max
                    End If
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
        For Each i As Socket In ServerComms.Ports
            If ReferenceEquals(ServerComms.MyListener.Server, i) = False Then
                ServerComms.RemoveClient(i, True)
            End If
        Next
        Galaxy.StartGame_Call()
        Console.WriteLine("Game is now running")
        If comms.IsAlive = False Then
            comms.Start()
        End If
    End Sub

    Public Class ServerSideClient
        Inherits Socket
        Public MyStation As Station.StationTypes = Station.StationTypes.Max

        Public Sub New(ByVal nSocketInformation As SocketInformation)
            MyBase.New(nSocketInformation)
        End Sub

        Public Sub DecodeMessage()
            If MyStation = Station.StationTypes.Max Then
                Dim receiveBuff(4) As Byte
                Dim receivedBuffLength As Integer = Receive(receiveBuff, 0, receiveBuff.Length, SocketFlags.None)
                Dim str As Integer = System.Text.Encoding.ASCII.GetString(receiveBuff, 0, receivedBuffLength).Trim(ChrW(0))
                MyStation = CInt(str)
                For i As Integer = 1 To ServerComms.Ports.Count - 1
                    If ReferenceEquals(Me, ServerComms.Ports(i)) = False And CType(ServerComms.Ports(i), ServerSideClient).MyStation = MyStation Then
                        ServerComms.RemoveClient(Me, False)
                        Exit Sub
                    End If
                Next
                Console.WriteLine((MyStation.ToString() + ": Has been connected"))
                Select Case MyStation
                    Case Station.StationTypes.Helm
                        Combat.centerShip.Helm.PlayerControled = True
                    Case Station.StationTypes.Batteries
                        Combat.centerShip.Batteries.PlayerControled = True
                    Case Station.StationTypes.Shielding
                        Combat.centerShip.Shielding.PlayerControled = True
                    Case Station.StationTypes.Engineering
                        Combat.centerShip.Engineering.PlayerControled = True
                End Select
            Else
                Try
                    Using fs As New NetworkStream(Me)
                        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                        Galaxy.RunCommand_Call(bf.Deserialize(fs))
                    End Using
                Catch ex As Runtime.Serialization.SerializationException
                    Server.ServerComms.RemoveClient(Me, True)
                Catch ex As InvalidOperationException
                    Server.ServerComms.RemoveClient(Me, True)
                Catch ex As IO.IOException
                    Server.ServerComms.RemoveClient(Me, True)
                Catch ex As Exception
                    Console.WriteLine()
                    Console.WriteLine("Error : There was an unexpected and unhandled exception.")
                    Console.WriteLine("please submit it as an issue at the URL bellow")
                    Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
                    Console.WriteLine()
                    Console.WriteLine(ex.ToString)
                    Console.WriteLine()
                End Try
            End If
        End Sub
    End Class

    Public Class ServerComms
        Public Shared MessageToSend As ServerMessage
        Public Shared MessageBuff() As Byte
        Public Shared MyListener As New TcpListener("1225")
        Public Shared Ports As New List(Of Socket)
        Private Shared SendRecieveList As New List(Of Socket)

        Public Shared Event UpdateServerMessage(ByVal nCraft As SpaceCraft, ByVal nPositions() As GraphicPosition, ByVal nWarp As Galaxy.Warp, ByVal nState As Galaxy.Scenario)
        Public Shared Sub UpdateServerMessage_Call(ByVal nCraft As SpaceCraft, ByVal nPositions() As GraphicPosition, ByVal nWarp As Galaxy.Warp, ByVal nState As Galaxy.Scenario)
            RaiseEvent UpdateServerMessage(nCraft, nPositions, nWarp, nState)
        End Sub
        Private Shared Sub UpdateServerMessage_Handle(ByVal nCraft As SpaceCraft, ByVal nPositions() As GraphicPosition, ByVal nWarp As Galaxy.Warp, ByVal nState As Galaxy.Scenario) Handles MyClass.UpdateServerMessage
            MessageToSend = New ServerMessage(nCraft, nPositions, nWarp, nState)
        End Sub

        Public Shared Sub StartCommunications()
            Ports.Add(MyListener.Server)
            MyListener.Start()
            Console.WriteLine("Server is now listening on " + MyListener.Server.LocalEndPoint.ToString)

            While True
                Listen()
                Send()
            End While
        End Sub

        Private Shared Sub Listen()
            SendRecieveList.AddRange(Ports)
            '-----Recieve Messages-----
            Socket.Select(SendRecieveList, Nothing, Nothing, -1)
            For Each i As Socket In SendRecieveList
                If ReferenceEquals(i, MyListener.Server) = True Then
                    AddClient(MyListener.AcceptSocket())
                    Exit For
                Else
                    For Each e As Socket In Ports
                        If ReferenceEquals(i, e) Then
                            CType(e, ServerSideClient).DecodeMessage()
                            Exit For
                        End If
                    Next
                End If
            Next
            '--------------------------
            SendRecieveList.Clear()
            SendRecieveList.TrimExcess()
        End Sub

        Private Shared Sub Send()
            SendRecieveList.AddRange(Ports)
            '-----Send Messages to Clients-----
            MessageBuff = ServerMessage.ConstructMessage()
            If SendRecieveList.Count > 1 Then
                Socket.Select(Nothing, SendRecieveList, Nothing, -1)
                For Each i As ServerSideClient In SendRecieveList
                    Try
                        i.Send(MessageBuff)
                    Catch ex As SocketException
                        RemoveClient(i, True)
                    Catch ex As InvalidOperationException
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
            End If
            '----------------------------------
            SendRecieveList.Clear()
            SendRecieveList.TrimExcess()
        End Sub

        Public Shared Sub AddClient(ByRef nSocket As Socket)
            If Ports.Count < 5 Then
                Ports.Add(New ServerSideClient(nSocket.DuplicateAndClose(Process.GetCurrentProcess.Id)))
                Listen()
            Else
                Dim temp As Socket = MyListener.AcceptSocket()
                Console.WriteLine(temp.RemoteEndPoint.ToString + ": Could not be connected. Server is full")
                temp.Close()
            End If
        End Sub

        Public Shared Sub RemoveClient(ByRef nSocket As Socket, ByVal resetControl As Boolean)
            For Each i As Socket In Ports
                If ReferenceEquals(i, nSocket) = True Then
                    Console.WriteLine(CType(i, ServerSideClient).MyStation.ToString + ": Client was disconnected")
                    If resetControl = True Then
                        Select Case CType(i, ServerSideClient).MyStation
                            Case Station.StationTypes.Helm
                                Combat.centerShip.Helm.PlayerControled = False
                            Case Station.StationTypes.Batteries
                                Combat.centerShip.Batteries.PlayerControled = False
                            Case Station.StationTypes.Shielding
                                Combat.centerShip.Shielding.PlayerControled = False
                            Case Station.StationTypes.Engineering
                                Combat.centerShip.Engineering.PlayerControled = False
                        End Select
                    End If
                    Ports.Remove(i)
                    Ports.TrimExcess()
                    Exit For
                End If
            Next
            nSocket.Close()
        End Sub
    End Class

End Module
