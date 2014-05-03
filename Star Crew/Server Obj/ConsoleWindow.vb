Module ConsoleWindow
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
    Public OutputScreen As New Screen 'A Screen object for the GUI
    Public GameServer As New Server 'A Server object to run a Server
    Public client As New Threading.Thread(AddressOf OutputScreen.Open) 'A Thread object for OutputScreen
    Public ServerThread As New Threading.Thread(AddressOf GameServer.StartCommunications) 'A Thread object for GameServer

    Public Sub Main() 'Starts the application
        client.Start() 'Opens the GUI
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
                          "Yellow: Yellow indecates a station which will Repair all ships in a Fleet" + Environment.NewLine +
                          "up to 40% hull." + Environment.NewLine +
                          "Interaction: To interact with a Fleet collide your Fleet into it." + Environment.NewLine +
                          "Objective: The Objective of this game is to eleminate all Enemy Fleets." + Environment.NewLine +
                          Environment.NewLine)

        While True 'Loop for console commands
            Dim str As String = Console.ReadLine().Trim(ChrW(0))
            If str.StartsWith("/") Then
                RunCommand(str)
            End If
        End While
    End Sub

    Private Sub RunCommand(ByVal nCommand As String) 'Run the specified console command
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
                GameServer.StartServer()
            Case commands.Suspend
                If ServerThread.ThreadState = Threading.ThreadState.Running Then
                    ServerThread.Suspend()
                    Console.WriteLine("Game has been paused")
                ElseIf ServerThread.IsAlive = False Then
                    Console.WriteLine("Server is not active type '/start' to start")
                ElseIf ServerThread.ThreadState = Threading.ThreadState.SuspendRequested Then
                    Console.WriteLine("Game is already paused")
                End If
            Case commands.Play
                If ServerThread.ThreadState = Threading.ThreadState.SuspendRequested Then
                    ServerThread.Resume()
                    Console.WriteLine("Game has been resumed")
                ElseIf ServerThread.IsAlive = False Then
                    Console.WriteLine("Server is not active type '/start' to start")
                ElseIf ServerThread.ThreadState = Threading.ThreadState.Running Then
                    Console.WriteLine("Game is already running")
                ElseIf ServerThread.IsAlive = False Then
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
                    For Each i As ServerSideClient In GameServer.Clients
                        If i.MyStation = stationType Then
                            GameServer.RemoveClient(i, True)
                            Console.WriteLine(str + ": Has been kicked")
                            Exit Sub
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

End Module
