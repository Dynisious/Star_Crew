Module Server 'Runs the Game and allows Clients to connect and interact
    Public GameWorld As Galaxy 'The Galaxy object that encompasses the game
    Public Comms As Communications 'A Communications object that handles networking for the Server
    Public UseNetwork As System.Threading.Mutex 'A Mutex object used to synchronise the use of the network
    Public CommsThread As System.Threading.Thread 'A System.Threading.Mutex object that allows either the game or the communications to be in control during an interaction
    Private ReadOnly saveAddress As String = "Star_Crew_Save.save" 'A string value representing the address of the games save file

    Public Sub Main() 'The initialising code of the application
        Console.WriteLine("-----Star Crew Game Server-----" + Environment.NewLine +
                          "Type /help for console commands (not case sensitive)" + Environment.NewLine)
        Console.Title = "Star Crew Server Console"
        Console.WriteLine("Initialising Objects...")
        Randomize() 'Set the random sequense of numbers
        Dim newMutex As Boolean 'Necessary for creating a new Mutex
        Dim mutexSecurity As New System.Security.AccessControl.MutexSecurity 'The security object for the Mutex
        Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'The name of the current user
        Dim rule As New System.Security.AccessControl.MutexAccessRule(
            user, System.Security.AccessControl.MutexRights.Modify Or
            System.Security.AccessControl.MutexRights.Synchronize,
            System.Security.AccessControl.AccessControlType.Allow) 'The security rule for the Mutex
        mutexSecurity.AddAccessRule(rule) 'Add the rule to the Security
        UseNetwork = New System.Threading.Mutex(False, "UseNetwork", newMutex, mutexSecurity) 'Create a new Mutex to control access to the network
        Console.WriteLine("Objects have been Initialised")
        Console.WriteLine("Initialising Game...")
        If System.IO.File.Exists(saveAddress) = True Then 'There's a previous game
            GameWorld = Game_Library.Serialisation.LoadFromFile(saveAddress) 'Load the previous game
        Else 'Create a new Game
            GameWorld = New Galaxy 'Start the new game
        End If
        GameWorld.GalaxyTick.Start() 'Starts the game updating
        Console.WriteLine("Game has been initialised")
        Comms = New Communications 'Create a new communications object
        Comms.Initialise_Communications() 'Initialise the network

        '-----Testing-----
        GameWorld.ClientFleet = New Fleet(Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Emperial_Forces) 'Create a new Fleet for the Client
        Dim temp As New Fleet(Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Pirate_Alliance)
        For i As Integer = 1 To 20
            GameWorld.ClientFleet.Add_Ship(New Screamer(GameWorld.ClientFleet))
            temp.Add_Ship(New Screamer(temp))
        Next
        GameWorld.FocusedSector.Add_Fleet(GameWorld.ClientFleet)
        GameWorld.FocusedSector.Add_Fleet(temp)
        '-----------------

        While Receive_Server_Commands() = True  'Continue to run the program
        End While
        Finalise_Server() 'Close the Server objects
    End Sub

    Public Sub Finalise_Server() 'Closes the Server objects
        GameWorld.GalaxyTick.Stop() 'Stops the Galaxy from Updating
        Comms.Finalise() 'Closes the Communications
    End Sub

    Public Enum ServerCommands 'An enumorator of console commands for the Server
        help 'Displays all console commands
        close 'Close the program
        clr 'Clears the console window
        save 'Saves the current game
        kick 'Kicks a specified client
    End Enum
    Public Function Receive_Server_Commands() As Boolean 'Runs console commands for the Server
        Dim command As String = Mid(LCase(Console.ReadLine()), 2) + " " 'Get the entered command
        Dim lastSpace As Integer = command.IndexOf(" ")
        Dim val As Boolean = True 'The value to return
        Select Case Left(command, lastSpace) 'Gets the command
            Case ServerCommands.help.ToString()
                Console.WriteLine(Environment.NewLine +
                    "/help:     Displays help for all commands" + Environment.NewLine +
                    "/close:    Closes the Server" + Environment.NewLine +
                    "/clr:      Clears the console of text" + Environment.NewLine +
                    "/save:     Saves the current game" + Environment.NewLine +
                    "/kick:     (e.g. /Kick b) kicks the connected Client at the specified station:" + Environment.NewLine +
                    "           'b', 's' and 'e'")
            Case ServerCommands.close.ToString()
                val = False
            Case ServerCommands.clr.ToString()
                Console.Clear()
            Case ServerCommands.save.ToString()
                Game_Library.Serialisation.SaveToFile(saveAddress, System.IO.FileMode.OpenOrCreate, GameWorld)
            Case ServerCommands.kick.ToString()
                If lastSpace + 1 <> command.Length Then 'There's a character there
                    Dim station As Star_Crew_Shared_Libraries.Shared_Values.StationTypes 'The station to be kicked
                    Select Case Mid(command, lastSpace + 2, 1)
                        Case "h" 'Trying to kick helm
                            Console.WriteLine("Helm cannot be kicked")
                            Return True
                            Exit Function 'Exit the function
                        Case "b"
                            station = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Battery
                        Case "s"
                            station = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Shields
                        Case "e"
                            station = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Engines
                        Case Else
                            Console.WriteLine("Please enter either 'b' 's' 'e'")
                            Return True
                            Exit Function
                    End Select
                    For Each i As ServerClient In Comms.ClientList 'Loop through all connected Clients
                        If i.Station = station Then 'This is the Client
                            UseNetwork.WaitOne() 'Wait till the network is clear
                            Comms.Remove_Client(i.Index, Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception,
                                                ("Server : The " + i.Name + " Client at " + i.RemoteEndPoint.ToString() + " was kicked.")) 'Kick the Client
                            UseNetwork.ReleaseMutex() 'Clear the network
                            Return True
                            Exit Function
                        End If
                    Next
                    Console.WriteLine("There is no Client connected to {0}", station.ToString()) 'Write to the Server
                Else
                    Console.WriteLine("Please specify a station with either 'b', 's' or 'e' e.g. /kick b")
                End If
            Case Else 'It was an invalid command
                Console.WriteLine("INVALID COMMAND : Check spelling and try again")
        End Select
        Console.WriteLine()
        Return val
    End Function

    Public Function Normalise_Direction(ByVal nDirection As Double) As Double 'Returns a radian between the range of 0-2*Pi
        nDirection = nDirection Mod (2 * Math.PI) 'Get the remaineder when the direction is divided by 2*Pi
        If nDirection < 0 Then nDirection = nDirection + (2 * Math.PI) 'Put the direction within 0 to 2*Pi
        Return nDirection 'Return the Normalised direction
    End Function

    Public Sub Game_Over() 'Ends the game because the Clients have been defeated
        Console.WriteLine("GAME OVER!!!")
        GameWorld.GalaxyTick.Stop()
    End Sub

End Module
