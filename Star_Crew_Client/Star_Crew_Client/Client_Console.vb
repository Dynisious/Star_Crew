Module Client_Console 'Used to output messages to the console for error handling etc for the Client
    Public WithEvents OutputScreen As Screen 'A Screen object used as the GUI for the Client
    Public Client As Connector 'A Connector object used to connect to a Server
    Private ScreenThread As New System.Threading.Thread((Sub()
                                                             OutputScreen = New Screen()
                                                             OutputScreen.Give_Control()
                                                         End Sub)) 'Give start the message loop for the form
    Private ReadOnly WorkingDirectory As String = "C:\Users\" + Environment.UserName + "\AppData\Roaming\Star Crew Client"
    Private ReadOnly ErrorLog As String = "Error Log.log" 'A String value representing the address of the game's error log
    Private ReadOnly SettingFile As String = "Settings.txt" 'A String value representing the address of the game's settings file
    Public Enum Settings 'An enumerator of Settings
        Ship_Name 'The Name of the Ship
        Throttle_Up 'The Key to throttle up the Ship
        Throttle_Down 'The Key to throttle down the Ship
        Turn_Right 'The Key to turn the Ship right
        Turn_Left 'The Key to turn the Ship left
        Fire_Weapon 'The Key to fire the Ship's Weapon
        Zoom_Out 'The Key to zoom out on the screen
        Zoom_In 'The Key to zoom in on the screen
    End Enum
    Private ReadOnly settingNames() As String = {
        "Ship Name:",
        "Throttle Up:",
        "Throttle Down:",
        "Turn Right:",
        "Turn Left:",
        "Fire Weapon:",
        "Zoom Out:",
        "Zoom In:"}
    Public settingElements() As Object = {
        "VTC Unnamed Ship",
        Windows.Forms.Keys.W,
        Windows.Forms.Keys.S,
        Windows.Forms.Keys.A,
        Windows.Forms.Keys.D,
        Windows.Forms.Keys.ControlKey,
        Windows.Forms.Keys.Z,
        Windows.Forms.Keys.X}

    Sub Main()
        Console.WriteLine("-----Star Crew Client-----")
        Console.WriteLine("Initialising Objects...")
        Randomize() 'Set the random sequense of numbers
        Console.WriteLine("Checking Directories exist...")
        If FileIO.FileSystem.DirectoryExists(WorkingDirectory) = False Then
            Console.WriteLine("The Working Directory does not exist. It will be created now.")
            FileIO.FileSystem.CreateDirectory(WorkingDirectory) 'Create the working directory
        End If
        Console.WriteLine("Setting working directory...")
        FileIO.FileSystem.CurrentDirectory = WorkingDirectory 'Set the working directory of the game
        Console.WriteLine("Clearing last error log...")
        FileIO.FileSystem.WriteAllText(ErrorLog, "This error log contains all of the errors that occoured during the last session", False) 'Clear the error logs
        Console.WriteLine("Checking Settings exist...")
        If FileIO.FileSystem.FileExists(SettingFile) = False Then 'The settings file does not exist
            Console.WriteLine("The Settings file does not exist. It will be created now.")
            Save_Settings()
        Else 'Load settings from file
            Console.WriteLine("Loading Settings...")
            Try
                Dim text As String = FileIO.FileSystem.ReadAllText(SettingFile) 'Get the text for the settings
                Dim index As Integer = text.IndexOf(settingNames(0)) + settingNames(0).Length 'Get the starting index of the setting
                Dim converter As New System.Windows.Forms.KeysConverter
                settingElements(0) = text.Substring(index, (text.IndexOf(";", index) - index)) 'Set the setting
                index = text.IndexOf(settingNames(1)) + settingNames(1).Length 'Get the starting index of the setting
                settingElements(1) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(2)) + settingNames(2).Length 'Get the starting index of the setting
                settingElements(2) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(3)) + settingNames(3).Length 'Get the starting index of the setting
                settingElements(3) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(4)) + settingNames(4).Length 'Get the starting index of the setting
                settingElements(4) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(5)) + settingNames(5).Length 'Get the starting index of the setting
                settingElements(5) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(6)) + settingNames(6).Length 'Get the starting index of the setting
                settingElements(6) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(7)) + settingNames(7).Length 'Get the starting index of the setting
                settingElements(7) = converter.ConvertFromString(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
            Catch ex As Exception
                Console.WriteLine("ERROR : There was an error while loading user settings. Some settings may not have been loaded.")
                Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while loading user settings. Some settings may not have been loaded." +
                                   Environment.NewLine + ex.ToString())
            End Try
        End If
        '-----Write Settings-----
        Screen.SettingsScreen.txtShipName.Text = "SHIP NAME: " + settingElements(0)
        Screen.SettingsScreen.txtThrottleUp.Text = "THROTTLE UP: " + CType(settingElements(1), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtThrottleDown.Text = "THROTTLE DOWN: " + CType(settingElements(2), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtTurnLeft.Text = "TURN LEFT: " + CType(settingElements(3), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtTurnRight.Text = "TURN RIGHT: " + CType(settingElements(4), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtFireWeapon.Text = "FIRE WEAPON: " + CType(settingElements(5), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtZoomOut.Text = "ZOOM OUT: " + CType(settingElements(6), Windows.Forms.Keys).ToString()
        Screen.SettingsScreen.txtZoomIn.Text = "ZOOM IN: " + CType(settingElements(7), Windows.Forms.Keys).ToString()
        '------------------------
        Console.WriteLine("Objects have been Initialised")
        Console.Title = "Star Crew Client Console"

        ScreenThread.Start() 'Makes the Screen object visible and gives it control over this thread
        While True
            Receive_Console_Commands()
        End While
    End Sub

    Public Enum ServerCommands 'An enumorator of console commands for the Server
        help 'Displays all console commands
        close 'Close the program
        clr 'Clears the console window
        heal 'Adds health to the Client's Ship
    End Enum
    Public Sub Receive_Console_Commands() 'Runs console commands for the Server
        Dim command As String = Mid(LCase(Console.ReadLine()), 1) + " " 'Get the entered command
        Dim firstSpace As Integer = command.IndexOf(" ")
        Select Case Left(command, firstSpace) 'Gets the command
            Case ServerCommands.help.ToString()
                Console.WriteLine(Environment.NewLine +
                    "help:          Displays help for all commands." + Environment.NewLine +
                    "close:         Closes the Server." + Environment.NewLine +
                    "clr:           Clears the console of text." + Environment.NewLine +
                    "heal <number>: Adds the specified number of hitpoints to the Client Ship.")
            Case ServerCommands.close.ToString()
                End
            Case ServerCommands.clr.ToString()
                Console.Clear()
            Case ServerCommands.heal.ToString()
                Try
                    Dim num As Integer = CInt(Mid(command, firstSpace + 1, (command.IndexOf(" ", firstSpace + 1) - firstSpace))) - 1
                    Client.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Heal_Ship), BitConverter.GetBytes(num)},
                                        {"ERROR : There was an error sending the Heal_Ship message header to the Server. Client will now disconnect.",
                                         "ERROR : There was an error sending the value for the Heal_Ship message to the Server. Client will now disconnect."})
                Catch ex As Exception
                    Console.WriteLine("ERROR : Invalid value 'number'. Check input and try again.")
                End Try
            Case Else 'It was an invalid command
                Console.WriteLine("INVALID COMMAND : Check spelling and try again")
        End Select
    End Sub

    Sub Save_Settings()
        Dim converter As New System.Windows.Forms.KeysConverter
        FileIO.FileSystem.WriteAllText(SettingFile, (
                                       "If a mistake is made while editing this file the settings will return to default." + Environment.NewLine +
                                       settingNames(0) + converter.ConvertToString(settingElements(0)) + ";" + Environment.NewLine +
                                       settingNames(1) + converter.ConvertToString(settingElements(1)) + ";" + Environment.NewLine +
                                       settingNames(2) + converter.ConvertToString(settingElements(2)) + ";" + Environment.NewLine +
                                       settingNames(3) + converter.ConvertToString(settingElements(3)) + ";" + Environment.NewLine +
                                       settingNames(4) + converter.ConvertToString(settingElements(4)) + ";" + Environment.NewLine +
                                       settingNames(5) + converter.ConvertToString(settingElements(5)) + ";" + Environment.NewLine +
                                       settingNames(6) + converter.ConvertToString(settingElements(6)) + ";" + Environment.NewLine +
                                       settingNames(7) + converter.ConvertToString(settingElements(7)) + ";"), False)
    End Sub

    Sub Write_To_Error_Log(ByVal text As String)
        FileIO.FileSystem.WriteAllText(ErrorLog, (Environment.NewLine + text), True)
    End Sub

    Sub Close_Client() Handles OutputScreen.FormClosing
        If Client_Console.OutputScreen.Server IsNot Nothing Then
            If Client_Console.OutputScreen.Server.HasExited = False Then Client_Console.OutputScreen.Server.CloseMainWindow() 'Make Sure the Server is not left open
        End If
        End 'Close the Program
    End Sub

End Module
