Module Server 'Outermost level of the Application
    Private ReadOnly WorkingDirectory As String = "C:\Users\" + Environment.UserName + "\AppData\Roaming\Star Crew\Server"
    Private ReadOnly SaveAddress As String = "Save.save" 'A String value representing the address of the game's save file
    Private ReadOnly Log As String = "Log.log" 'A String value representing the address of the game's error log
    Private ReadOnly ShipsDirectory As String = "Ships" 'A String value representing the address of the game's Ship files
    Public ReadOnly QuarterCircle As Double = (Math.PI / 2) 'A Double value representing a quarter of a Circle
    Public ReadOnly FullCircle As Double = (2 * Math.PI) 'A Double value representing a full Circle
    Public runningApplication As Boolean = True 'A Boolean value indecating if the Server is still open
    Public game As GameEnvironment 'The GameEnvironment object for the Server

    Public Sub Main() 'The initialising code of the application
        Console.WriteLine("-----Star Crew Game Server-----" + Environment.NewLine +
                          "Type help for console commands (not case sensitive)" + Environment.NewLine)
        Console.Title = "Star Crew Server Console"
        Try
            Randomize() 'Set the random sequense of numbers
            Console.WriteLine("Checking directories exist...")
            If FileIO.FileSystem.DirectoryExists(WorkingDirectory) = False Then
                Console.WriteLine("The working directory does not exist. It will be created now.")
                FileIO.FileSystem.CreateDirectory(WorkingDirectory) 'Create the working directory
            End If
            Console.WriteLine("Setting working directory...")
            FileIO.FileSystem.CurrentDirectory = WorkingDirectory 'Set the working directory of the game

            Console.WriteLine("Clearing last log...")
            FileIO.FileSystem.WriteAllText(Log, "This log contains all of the events that occoured during the last Server session", False) 'Clear the error logs
            Console.WriteLine("All directories checked." + Environment.NewLine)

            Console.WriteLine("Initialising Objects...")
            Console.WriteLine("Setting up game environment...")
            Dim saveExists As Boolean = FileIO.FileSystem.FileExists(SaveAddress) 'Get a Boolean indicating whether there is a Save file
            If saveExists Then Console.WriteLine("Loading previous game...")
            game = New GameEnvironment(saveExists) 'Set up game
            Console.WriteLine("Objects have been Initialised.")
        Catch ex As Exception
            Write_To_Log("ERROR : There was an error while initialising the Server. Server will now Close." +
                               Environment.NewLine + ex.ToString())
            End
        End Try
        game.Run_Game() 'Begin the game
        networkThread.Start() 'Start the network

        While runningApplication 'Continue to run the program
            Receive_Server_Commands()
        End While
        Close_Server()
    End Sub

    Public Sub Close_Server()
        runningApplication = False 'Allow the game to begin to close
        game.Finalise() 'Finalise the game
        networkThread.Join() 'Wait for the network to close before exiting
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
                runningApplication = False 'Let the Server Close
            Case ServerCommands.clr.ToString()
                Console.Clear()
            Case ServerCommands.save.ToString()

            Case ServerCommands.kick.ToString()

            Case ServerCommands.bot_add.ToString()

            Case Else 'It was an invalid command
                Console.WriteLine("INVALID COMMAND : Check spelling and try again")
        End Select
    End Sub

    Public Function Normalise_Direction(ByVal nDirection As Double) As Double 'Returns a radian between the range of 0-2*Pi
        nDirection = nDirection Mod FullCircle 'Get the remaineder when the direction is divided by 2*Pi
        Return If((nDirection < 0), (nDirection + FullCircle), nDirection) 'Put the direction within 0 to 2*Pi and return the Normalised direction
    End Function

    Public Sub Write_To_Log(ByVal text As String) 'Write to the log
        FileIO.FileSystem.WriteAllText(Log, (Environment.NewLine + text), True) 'Write the error
    End Sub

    '-----Networking-----
    Public clients As New List(Of ServerClient) 'A List of ServerClient objects to communicate through
    Private _maxPing As Integer = 6000 'An Integer value representing the maximum ping allowed
    Public ReadOnly Property maxPing As Integer
        Get
            Return _maxPing
        End Get
    End Property
    Private networkThread As New Threading.Thread(Sub()
                                                      Dim listener As New Net.Sockets.TcpListener(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) 'Create a TcpListener object to receive connection requests through
                                                      listener.Start() 'Begin listening for connection requests
                                                      Console.WriteLine(Environment.NewLine + "Server is now listening for connection requests on '" +
                                                                        listener.LocalEndpoint.ToString() + "'." + Environment.NewLine + "Network now running.")
                                                      Dim connectMessage As String = Environment.NewLine + "SERVER : Connecting a new Client..." 'The message to log when a new Client is connecting
                                                      Do
                                                          If listener.Pending() Then 'There's a pending connection request
                                                              While listener.Pending() = True 'Loop until all pending connection requests have been added
                                                                  Console.WriteLine(connectMessage)
                                                                  Write_To_Log(connectMessage)
                                                                  Dim successfulConnection As Boolean = False 'A Boolean value indicating if the Client connects successfully
                                                                  Dim client As New ServerClient(listener.AcceptSocket(), successfulConnection) 'Create a new ServerClient object
                                                                  If successfulConnection Then clients.Add(client) 'If the connection was successful keep the ServerClient
                                                              End While
                                                          End If

                                                          Threading.Thread.Sleep(3000) 'Wait for three seconds
                                                          For Each i As ServerClient In clients 'Loop through all ServerClients
                                                              i.Ping_Check() 'Begin Checking this Client's ping
                                                          Next
                                                      Loop While runningApplication 'Loop as long as the application stays open

                                                      listener.Stop()
                                                      For Each i As ServerClient In clients 'Loop through all connected clients
                                                          i.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception)},
                                                                         {Environment.NewLine + "ERROR : There was an exception while sending the Server_Closed_Exception to the " +
                                                                         i.name + ". The " + i.name + " will now disconnect."}) 'Send the reason for the disconnect to all Clients
                                                          i.runClient = False 'Close the Client
                                                      Next
                                                      clients.Clear() 'Clear the list
                                                  End Sub)
    '--------------------
End Module
