Module Server 'Runs the Game and allows Clients to connect and interact
    Public Combat As New CombatSpace 'A CombatSpace object to store all connected Ship's
    Public Comms As New Communications 'A Communications object that receives new messages and stores the Server Clients
    Private ReadOnly workingDirectory As String = "C:\Users\" + Environment.UserName + "\AppData\Roaming\Star Crew Server"
    Private ReadOnly saveAddress As String = "Star Crew Save.save" 'A String value representing the address of the game's save file
    Private ReadOnly errorLog As String = "Error Log.log" 'A String value representing the address of the game's error log
    Public ReadOnly QuarterCircle As Double = (Math.PI / 2) 'A Double value representing a quarter of a Circle
    Public ReadOnly FullCircle As Double = (2 * Math.PI) 'A Double value representing a full Circle

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
            Comms.interactWithClients = New System.Threading.Mutex(False, Nothing, newMutex, mutexSecurity) 'Create a new Mutex to control access to the network
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
        For i As Integer = 1 To 3
            Combat.adding.Add(New AIShip)
        Next
        '-----------------

        While Receive_Server_Commands() = True  'Continue to run the program
        End While
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
    Public Function Receive_Server_Commands() As Boolean 'Runs console commands for the Server
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
                    "bot_add <number>:  Adds the specified number of AI Ships.")
            Case ServerCommands.close.ToString()
                Finalise_Server()
            Case ServerCommands.clr.ToString()
                Console.Clear()
            Case ServerCommands.save.ToString()

            Case ServerCommands.kick.ToString()

            Case ServerCommands.bot_add.ToString()
                Dim num As Integer = CInt(Mid(command, firstSpace + 1, (command.IndexOf(" ", firstSpace + 1) - firstSpace))) - 1
                For i As Integer = 0 To num 'Loop through all indexes
                    Combat.adding.Add(New AIShip()) 'Add a new AI Ship
                Next
            Case Else 'It was an invalid command
                Console.WriteLine("INVALID COMMAND : Check spelling and try again")
        End Select
        Return True
    End Function

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
