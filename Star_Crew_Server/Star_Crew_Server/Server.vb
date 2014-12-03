Module Server 'Runs the Game and allows Clients to connect and interact
    Public Combat As New CombatSpace 'A CombatSpace object to store all connected Ship's
    Public Comms As New Communications 'A Communications object that receives new messages and stores the Server Clients
    Private ReadOnly workingDirectory As String = "C:\Users\" + Environment.UserName + "\AppData\Roaming\Star Crew"
    Private ReadOnly saveAddress As String = "Save.save" 'A String value representing the address of the game's save file
    Private ReadOnly errorLog As String = "Server Error Log.log" 'A String value representing the address of the game's error log
    Private ReadOnly shipsDirectory As String = "Ships" 'A String value representing the address of the game's Ship files
    Public ReadOnly QuarterCircle As Double = (Math.PI / 2) 'A Double value representing a quarter of a Circle
    Public ReadOnly FullCircle As Double = (2 * Math.PI) 'A Double value representing a full Circle
    Public Enum ShipStats 'The different stats of a Ship
        Type
        Trackable
        Physics
        Hull
        Throttle_Maximum
        Acceleration
        Turn_Speed
        Hitbox_X_Distance
        Hitbox_Y_Distance
        Allegiance
        Shield_Recharge_Ticks
        Shield_Minimum
        Shield_Recharge_Value
        Shield_Maximum
        max
    End Enum
    Private _ShipsTypes(-1) As Dictionary(Of ShipStats, Object)
    Public ReadOnly Property ShipsTypes As Dictionary(Of ShipStats, Object)()
        Get
            Return _ShipsTypes
        End Get
    End Property

    Public Sub Main() 'The initialising code of the application
        Console.WriteLine("-----Star Crew Game Server-----" + Environment.NewLine +
                          "Type /help for console commands (not case sensitive)" + Environment.NewLine)
        Console.Title = "Star Crew Server Console"
        Console.WriteLine("Initialising Objects...")
        Try
            Randomize() 'Set the random sequense of numbers
            Console.WriteLine("Checking Directories exist...")
            If FileIO.FileSystem.DirectoryExists(workingDirectory) = False Then
                Console.WriteLine("The Working Directory does not exist. It will be created now.")
                FileIO.FileSystem.CreateDirectory(workingDirectory) 'Create the working directory
            End If
            Console.WriteLine("Setting working directory...")
            FileIO.FileSystem.CurrentDirectory = workingDirectory 'Set the working directory of the game

            Console.WriteLine("Clearing last error log...")
            FileIO.FileSystem.WriteAllText(errorLog, "This error log contains all of the errors that occoured during the last Server session", False) 'Clear the error logs
            Console.WriteLine("Initialising mutexes...")
            Dim newMutex As Boolean 'Necessary for creating a new Mutex
            Dim mutexSecurity As New System.Security.AccessControl.MutexSecurity 'The security object for the Mutex
            Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'The name of the current user
            Dim rule As New System.Security.AccessControl.MutexAccessRule(
                user, System.Security.AccessControl.MutexRights.Modify Or
                System.Security.AccessControl.MutexRights.Synchronize,
                System.Security.AccessControl.AccessControlType.Allow) 'The security rule for the Mutex
            mutexSecurity.AddAccessRule(rule) 'Add the rule to the Security
            Comms.InteractWithClients = New System.Threading.Mutex(False, Nothing, newMutex, mutexSecurity) 'Create a new Mutex to control access to the network
            Console.WriteLine("Objects have been Initialised")
            Console.WriteLine("Initialising Game...")
            Console.WriteLine("Game has been initialised")
            Server.Comms.Initialise_Communications() 'Initialise the network
        Catch ex As Exception
            Write_To_Error_Log("ERROR : There was an error while initialising the Server. Server will now Close." +
                               Environment.NewLine + ex.ToString())
            End
        End Try

        '-----Testing-----
        Combat.Ticker.Start()
        Combat.gameThread.Start()
        For i As Integer = 1 To 2
            Combat.adding.Add(New AIShip(Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Pirate_Alliance))
        Next
        '-----------------

        While True 'Continue to run the program
            Receive_Server_Commands()
        End While
    End Sub
    Public Sub Load_Ships() 'Load the Ships
        Console.WriteLine("Checking Ships directory exists...")
        If FileIO.FileSystem.DirectoryExists(shipsDirectory) = False Then 'The settings file does not exist
            Console.WriteLine("The Ships directory does not exist. It will be created now.")
            FileIO.FileSystem.CreateDirectory(shipsDirectory) 'Create the ShipsDirectory
        Else 'Load settings from file
            Console.WriteLine("Loading Ships...")
            Try
                Dim files As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = FileIO.FileSystem.GetFiles(shipsDirectory) 'Get the Ship files in the directory
                ReDim _ShipsTypes(files.Count - 1) 'Redimentionalise the array
                For i As Integer = 0 To files.Count - 1 'Loop through all Ships
                    Using reader As New IO.StreamReader(shipsDirectory + "\" + files(i))
                        Dim text As String = FileIO.FileSystem.ReadAllText(files(i)) 'Get the text for the Ship
                        Dim index As Integer = text.IndexOf(ShipStats.Acceleration.ToString()) + ShipStats.Acceleration.ToString().Length 'Get the starting index of the Ship
                        settingElements(0) = text.Substring(index, (text.IndexOf(";", index) - index)) 'Set the value
                        reader.Close()
                    End Using
                Next
            Catch ex As Exception
                Console.WriteLine("ERROR : There was an error while loading the Ships. Some Ships may not have been loaded.")
                Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while loading user settings. Some settings may not have been loaded." +
                                   Environment.NewLine + ex.ToString())
            End Try
        End If
    End Sub

    Public Sub Finalise_Server() 'Closes the Server objects
        Comms.serverComms = False
        Comms.CommsThread.Join(5000)
        End
    End Sub

    Public Enum ServerCommands 'An enumorator of console commands for the Server
        help 'Displays all console commands
        close 'Close the program
        clr 'Clears the console window
        save 'Saves the current game
        kick 'Kicks a specified client
        bot_add 'Adds a specified number of AI Ships
    End Enum
    Public Sub Receive_Server_Commands() 'Runs console commands for the Server
        Dim command As String = Mid(LCase(Console.ReadLine()), 1) + " " 'Get the entered command
        Dim firstSpace As Integer = command.IndexOf(" ")
        Select Case Left(command, firstSpace) 'Gets the command
            Case ServerCommands.help.ToString()
                Console.WriteLine(Environment.NewLine +
                    "help:              Displays help for all commands." + Environment.NewLine +
                    "close:             Closes the Server." + Environment.NewLine +
                    "clr:               Clears the console of text." + Environment.NewLine +
                    "save:              !NOT IMPLIMENTED!" + Environment.NewLine +
                    "kick:              !NOT IMPLIMENTED!" + Environment.NewLine +
                    "bot_add <number> <faction>:    Adds the specified" + Environment.NewLine +
                    "                               number of AI Ships" + Environment.NewLine +
                    "                               alligned to the specified faction.")
            Case ServerCommands.close.ToString()
                Finalise_Server()
            Case ServerCommands.clr.ToString()
                Console.Clear()
            Case ServerCommands.save.ToString()

            Case ServerCommands.kick.ToString()

            Case ServerCommands.bot_add.ToString()
                Try
                    Dim secondSpace As Integer = command.IndexOf(" ", firstSpace + 1) 'Get the index of the seconds space
                    Dim num As Integer = CInt(Mid(command, firstSpace + 1, (secondSpace - firstSpace))) - 1
                    Dim alleg As Integer = CInt(Mid(command, secondSpace + 1, (command.IndexOf(" ", secondSpace + 1) - secondSpace)))
                    For i As Integer = 0 To num 'Loop through all indexes
                        Combat.adding.Add(New AIShip(alleg)) 'Add a new AI Ship
                    Next
                Catch ex As Exception
                    Console.WriteLine("ERROR : Invalid value 'number'. Check input and try again.")
                End Try
            Case Else 'It was an invalid command
                Console.WriteLine("INVALID COMMAND : Check spelling and try again")
        End Select
    End Sub

    Public Function Normalise_Direction(ByVal nDirection As Double) As Double 'Returns a radian between the range of 0-2*Pi
        nDirection = nDirection Mod FullCircle 'Get the remaineder when the direction is divided by 2*Pi
        If nDirection < 0 Then nDirection = nDirection + FullCircle 'Put the direction within 0 to 2*Pi
        Return nDirection 'Return the Normalised direction
    End Function

    Public Sub Write_To_Error_Log(ByVal text As String) 'Write an error to the error log
        FileIO.FileSystem.WriteAllText(errorLog, (Environment.NewLine + text), True) 'Write the error
    End Sub

    Public Sub End_Game(ByVal Message As String) 'Ends the game
        If Message <> "" Then Console.WriteLine(Message) 'Write the message to the console
    End Sub

End Module
