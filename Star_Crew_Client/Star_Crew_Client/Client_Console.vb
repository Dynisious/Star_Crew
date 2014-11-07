Module Client_Console 'Used to output messages to the console for error handling etc for the Client
    Public WithEvents OutputScreen As New Screen 'A Screen object used as the GUI for the Client
    Public Client As Connector 'A Connector object used to connect to a Server
    Public CommsThread As System.Threading.Thread 'A Thread object used to run the communications
    Private ReadOnly WorkingDirectory As String = "C:\Users\" + Environment.UserName + "\AppData\Roaming\Star Crew Client"
    Private ReadOnly ErrorLog As String = "Error Log.log" 'A String value representing the address of the game's error log
    Private ReadOnly SettingFile As String = "Settings.txt" 'A String value representing the address of the game's settings file
    Private ReadOnly settingNames() As String = {
        "Ship Name:",
        "Throttle Up:",
        "Throttle Down:",
        "Turn Left:",
        "Turn Right:",
        "Fire Weapon:"}
    Public settingElements() As Object = {
        "VTC Unnamed Ship",
        Windows.Forms.Keys.W,
        Windows.Forms.Keys.S,
        Windows.Forms.Keys.A,
        Windows.Forms.Keys.D,
        Windows.Forms.Keys.ControlKey}

    Sub Main()
        Console.WriteLine("-----Star Crew Client-----")
        Console.WriteLine("Initialising Objects...")
        Randomize() 'Set the random sequense of numbers
        Console.WriteLine("Checking Directories exist...")
        If FileIO.FileSystem.DirectoryExists(workingDirectory) = False Then
            Console.WriteLine("The Working Directory does not exist. It will be created now.")
            FileIO.FileSystem.CreateDirectory(workingDirectory) 'Create the working directory
        End If
        Console.WriteLine("Setting working directory...")
        FileIO.FileSystem.CurrentDirectory = workingDirectory 'Set the working directory of the game
        Console.WriteLine("Clearing last error log...")
        FileIO.FileSystem.WriteAllText(ErrorLog, "This error log contains all of the errors that occoured during the last session", False) 'Clear the error logs
        Console.WriteLine("Checking Settings exist...")
        If FileIO.FileSystem.FileExists(SettingFile) = False Then 'The settings file does not exist
            Console.WriteLine("The Settings file does not exist. It will be created now.")
            FileIO.FileSystem.WriteAllText(SettingFile,
                                           ("If a mistake is made while editing this file the settings will return to default." + Environment.NewLine +
                                            settingNames(0) + settingElements(0) + ";" + Environment.NewLine +
                                            settingNames(1) + settingElements(1).ToString() + ";" + Environment.NewLine +
                                            settingNames(2) + settingElements(2).ToString() + ";" + Environment.NewLine +
                                            settingNames(3) + settingElements(3).ToString() + ";" + Environment.NewLine +
                                            settingNames(4) + settingElements(4).ToString() + ";" + Environment.NewLine +
                                            settingNames(5) + settingElements(5).ToString() + ";"), False)
        Else 'Load settings from file
            Console.WriteLine("Loading Settings...")
            Try
                Dim text As String = FileIO.FileSystem.ReadAllText(SettingFile) 'Get the text for the settings
                Dim index As Integer = text.IndexOf(settingNames(0)) + settingNames(0).Length 'Get the starting index of the setting
                settingElements(0) = text.Substring(index, (text.IndexOf(";", index) - index)) 'Set the setting
                index = text.IndexOf(settingNames(1)) + settingNames(1).Length 'Get the starting index of the setting
                settingElements(1) = CInt(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(2)) + settingNames(2).Length 'Get the starting index of the setting
                settingElements(2) = CInt(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(3)) + settingNames(3).Length 'Get the starting index of the setting
                settingElements(3) = CInt(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(4)) + settingNames(4).Length 'Get the starting index of the setting
                settingElements(4) = CInt(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
                index = text.IndexOf(settingNames(5)) + settingNames(5).Length 'Get the starting index of the setting
                settingElements(5) = CInt(text.Substring(index, (text.IndexOf(";", index) - index))) 'Set the setting
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
        '------------------------
        Console.WriteLine("Objects have been Initialised")
        Console.Title = "Star Crew Client Console"

        OutputScreen.Give_Control() 'Makes the Screen object visible and gives it control over this thread
    End Sub

    Sub Save_Settings()
        FileIO.FileSystem.WriteAllText(SettingFile, (
                                       "If a mistake is made while editing this file the settings will return to default." + Environment.NewLine +
                                       settingNames(0) + settingElements(0) + ";" + Environment.NewLine +
                                       settingNames(1) + settingElements(1).ToString() + ";" + Environment.NewLine +
                                       settingNames(2) + settingElements(2).ToString() + ";" + Environment.NewLine +
                                       settingNames(3) + settingElements(3).ToString() + ";" + Environment.NewLine +
                                       settingNames(4) + settingElements(4).ToString() + ";" + Environment.NewLine +
                                       settingNames(5) + settingElements(5).ToString() + ";"), False)
    End Sub

    Sub Write_To_Error_Log(ByVal text As String)
        FileIO.FileSystem.WriteAllText(errorLog, (Environment.NewLine + text), True)
    End Sub

    Sub Close_Client() Handles OutputScreen.FormClosing
        If Client_Console.OutputScreen.Server IsNot Nothing Then
            If Client_Console.OutputScreen.Server.HasExited = False Then Client_Console.OutputScreen.Server.CloseMainWindow() 'Make Sure the Server is not left open
        End If
        End 'Close the Program
    End Sub

End Module
