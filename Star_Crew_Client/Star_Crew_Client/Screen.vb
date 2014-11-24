Public Class Screen 'The object used as a GUI for the Client
    Public Server As System.Diagnostics.Process 'The Server that the Client hosts
    Public sendKeys As Boolean = False 'A Boolean value indicating whether the Client should send keystrokes to the Server
    Public Class MenuScreen 'Objects displayed on the menu screen
        Private Shared WithEvents btnHost As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 48), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Host Game"} 'A Button object that when Clicked hosts a Server
        Private Shared WithEvents btnJoin As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 223), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Join Game"} 'A Button object that when Clicked joins an existing Server
        Private Shared WithEvents btnSettings As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 398), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Settings"} 'A Button object that when Clicked hosts a Server
        Private Shared WithEvents btnExit As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 573), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Exit"} 'A Button object that when Clicked closes the program
        Public Shared WithEvents btnBackToGame As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(1025, 623), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 16, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(150, 27), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Resume Game"} 'A Button object that when Clicked returns to the GameScreen

        Public Shared Sub Layout(ByRef scr As Screen) 'Sets the Screen to display the menu screen
            Dim g As System.Drawing.Graphics = scr.CreateGraphics() 'Gets the graphics object for the Screen
            g.Clear(Drawing.Color.Black) 'Clear the screen to black
            scr.Controls.Clear() 'Clear the old list of Controls
            scr.Controls.Add(btnHost) 'Add btnHost
            scr.Controls.Add(btnJoin) 'Add btnJoin
            scr.Controls.Add(btnSettings) 'Add btnSettings
            scr.Controls.Add(btnExit) 'Add btnExit
            If Client_Console.Client IsNot Nothing Then
                scr.Controls.Add(btnBackToGame) 'Add btnBackToGame
                btnBackToGame.Enabled = True
            Else
                btnBackToGame.Enabled = False
            End If
        End Sub

        Private Shared Sub btnHost_Click() Handles btnHost.Click 'Hosts a new Server
            If Client_Console.Client IsNot Nothing Then 'There's a Connector from the last session
                If Client_Console.OutputScreen.Server IsNot Nothing Then
                    If Client_Console.OutputScreen.Server.HasExited = False Then Client_Console.OutputScreen.Server.CloseMainWindow() 'Make Sure the Server is not left open
                End If
                Client_Console.Client.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting)},
                                                   {"ERROR : There was an error sending the Client_Disconnecting message to the Server. Client will now close."})
                Client_Console.Client.sendingAlive = False
                Client_Console.Client.receivingAlive = False
            End If
            Client_Console.OutputScreen.Server = Process.Start("Server\Star_Crew_Server.exe")
            Client_Console.Client = New Connector("127.0.0.1", Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort)
        End Sub
        Private Shared Sub btnHost_MouseEnter() Handles btnHost.MouseEnter 'Changes btnHost's colour when it's moused over
            btnHost.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnHost.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnHost_MouseLeave() Handles btnHost.MouseLeave 'Changes btnHost's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Private Shared Sub btnJoin_Click() Handles btnJoin.Click 'Joins an existing Server
            JoinScreen.Layout(Client_Console.OutputScreen) 'Go to the Join Screen
            If Client_Console.Client IsNot Nothing Then 'There's a Connector from the last session
                If Client_Console.OutputScreen.Server IsNot Nothing Then
                    If Client_Console.OutputScreen.Server.HasExited = False Then Client_Console.OutputScreen.Server.CloseMainWindow() 'Make Sure the Server is not left open
                End If
                Client_Console.Client.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting)},
                                                   {"ERROR : There was an error sending the Client_Disconnecting message to the Server. Client will now close."})
                Client_Console.Client.sendingAlive = False
                Client_Console.Client.receivingAlive = False
            End If
        End Sub
        Private Shared Sub btnJoin_MouseEnter() Handles btnJoin.MouseEnter 'Changes btnJoin's colour when it's moused over
            btnJoin.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnJoin.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnJoin_MouseLeave() Handles btnJoin.MouseLeave 'Changes btnJoin's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Private Shared Sub btnSettings_Click() Handles btnSettings.Click 'Opens the Settings screen
            SettingsScreen.Layout(Client_Console.OutputScreen)
        End Sub
        Private Shared Sub btnSettings_MouseEnter() Handles btnSettings.MouseEnter 'Changes btnJoin's colour when it's moused over
            btnSettings.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnSettings.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnSettings_MouseLeave() Handles btnSettings.MouseLeave 'Changes btnJoin's colour when the mouse leaves it
            btnSettings.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Private Shared Sub btnExit_Click() Handles btnExit.Click 'Closes the program
            Client_Console.Close_Client()
        End Sub
        Private Shared Sub btnExit_MouseEnter() Handles btnExit.MouseEnter 'Changes btnExit's colour when it's moused over
            btnExit.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnExit.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnExit_MouseLeave() Handles btnExit.MouseLeave 'Changes btnExit's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Public Shared Sub btnBackToGame_Click() Handles btnBackToGame.Click 'Opens the GameScreenLayout
            GameScreen.Layout(Client_Console.OutputScreen)
            btnBackToGame.Enabled = False
        End Sub

    End Class
    Public Class JoinScreen 'Objects displayed when the Client is going to join an existing game
        Private Shared WithEvents txtIP As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(125, 40), .Location = New System.Drawing.Point(537, 250),
            .Text = "Input IP", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Private Shared WithEvents btnConnect As New System.Windows.Forms.Button With {
            .Size = New System.Drawing.Size(150, 45), .Location = New System.Drawing.Point(525, 300),
            .Text = "Connect", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                New System.Drawing.Font("Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel), Drawing.FontStyle.Underline)}
        Private Shared WithEvents btnMenu As New System.Windows.Forms.Button With {
            .Size = New System.Drawing.Size(200, 45), .Location = New System.Drawing.Point(500, 360),
            .Text = "Main Menu", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                New System.Drawing.Font("Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel), Drawing.FontStyle.Underline)}

        Public Shared Sub Layout(ByRef scr As Screen) 'Set's the Screen to display the join screen
            scr.Controls.Clear() 'Clear's the old display
            scr.Controls.Add(txtIP) 'Add txtIP
            scr.Controls.Add(btnConnect) 'Add btnConnect
            scr.Controls.Add(btnMenu) 'Add btnMeny
            txtIP.SelectAll()
        End Sub

        Private Shared Sub txtIP_Enter(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtIP.KeyDown 'Handles the enter key being clicked
            If e.KeyCode = Windows.Forms.Keys.Enter Then btnConnect.PerformClick() 'Click the button
        End Sub
        Private Shared Sub txtIP_MouseClick() Handles txtIP.MouseClick 'Handles mousing over txtIP
            txtIP.Focus()
            txtIP.SelectAll()
        End Sub

        Private Shared Sub btnConnect_Click() Handles btnConnect.Click 'Handles btnConnect being Clicked
            txtIP.ForeColor = Drawing.Color.DarkTurquoise 'Reset the fore colour
            Dim count As Integer 'A count of how many "."s are in the string
            Dim nextIndex As Integer 'The last index where a "." was found
            For i As Integer = 1 To 3
                If nextIndex = txtIP.Text.Length Then Exit For 'There are no more spaces in the string
                nextIndex = txtIP.Text.IndexOf(".", nextIndex) + 1 'Find the next instance and record the index
                count = count + 1
            Next
            If count = 3 Then 'It's a valid IP
                Try
                    Client_Console.Client = New Connector(txtIP.Text, Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) 'Create a new Connector and connect to the Server
                Catch ex As System.Net.Sockets.SocketException
                    Console.WriteLine(Environment.NewLine + "ERROR : There was an error while connecting to the Server located at " +
                                      txtIP.Text + ":" + CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) + ". Check address and try again.")
                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while connecting to the Server located at " + txtIP.Text +
                                                      ":" + CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) + ". Check address and try again." +
                                                      Environment.NewLine + ex.ToString())
                    Client_Console.Client = Nothing
                Catch ex As Exception
                    Console.WriteLine(Environment.NewLine + "ERROR : There was an unecpected and unhandled exception while connecting to the Sever located at " +
                                      txtIP.Text + ":" + CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) + ". Client will now close.")
                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unecpected and unhandled exception while connecting to the Sever located at " + txtIP.Text +
                                                      ":" + CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) + ". Client will now close." +
                                                      Environment.NewLine + ex.ToString())
                    Client_Console.Client = Nothing
                End Try
                If Client_Console.Client Is Nothing Then Beep()
            Else
                Beep() 'Make a tone to alert the player
                txtIP.ForeColor = Drawing.Color.Red 'Change the fore colour of the bad IP
            End If
        End Sub
        Private Shared Sub btnConnect_MouseEnter() Handles btnConnect.MouseEnter 'Handles the mouse moving into btnConnect
            btnConnect.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
            btnConnect.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnConnect_MouseLeave() Handles btnConnect.MouseLeave 'Handles the mouse moving out of btnConnect
            btnConnect.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click 'Handles btnMenu being Clicked
            MenuScreen.Layout(Client_Console.OutputScreen) 'Go to the menu screen
        End Sub
        Private Shared Sub btnMenu_MouseEnter() Handles btnMenu.MouseEnter 'Handles the mouse moving into btnMenu
            btnMenu.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
            btnMenu.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnMenu_MouseLeave() Handles btnMenu.MouseLeave 'Handles the mouse moving out of btnMenu
            btnMenu.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

    End Class
    Public Class GameScreen 'Objects displayed when the Client is in game
        Public Shared WithEvents btnMenu As New System.Windows.Forms.Button With {
            .Size = New System.Drawing.Size(200, 45), .Location = New System.Drawing.Point(980, 635), .TabStop = False,
            .Text = "Main Menu", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                New System.Drawing.Font("Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel), Drawing.FontStyle.Underline)}
        Public Shared lblHull As New System.Windows.Forms.Label With {
            .Size = New System.Drawing.Size(300, 45), .Location = New System.Drawing.Point(875, 5),
            .Text = "HULL: 0/0", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                "Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel)}
        Public Shared lblThrottle As New System.Windows.Forms.Label With {
            .Size = New System.Drawing.Size(300, 45), .Location = New System.Drawing.Point(875, 60),
            .Text = "THROTTLE: 0/0", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                "Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel)}
        Public Shared lblAmmunition As New System.Windows.Forms.Label With {
            .Size = New System.Drawing.Size(300, 45), .Location = New System.Drawing.Point(875, 115),
            .Text = "AMMUNITION: 0/0", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                "Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel)}
        Public Shared lblTargetDistance As New System.Windows.Forms.Label With {
            .Size = New System.Drawing.Size(300, 45), .Location = New System.Drawing.Point(875, 160),
            .Text = "TARGET DISTANCE: 0m", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                "Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel)}

        Public Shared Sub Layout(ByRef scr As Screen)
            scr.Controls.Clear() 'Clears the Screen of objects
            scr.Controls.Add(btnMenu) 'Add btnMenu to the Screen
            btnMenu.Enabled = True
            scr.Controls.Add(lblHull) 'Add lblHull to the Screen
            scr.Controls.Add(lblThrottle) 'Add lblThrottle to the Screen
            scr.Controls.Add(lblAmmunition) 'Add lblAmmunition to the Screen
            scr.Controls.Add(lblTargetDistance) 'Add lblTargetDistance to the Screen
            scr.ActiveControl = Nothing 'Clear the active control
            scr.sendKeys = True 'Send keystrokes to the Server
        End Sub

        Public Shared Sub btnMenu_Click() Handles btnMenu.Click 'Handles btnMenu being Clicked
            MenuScreen.Layout(Client_Console.OutputScreen) 'Go to the menu screen
            Client_Console.OutputScreen.sendKeys = False 'Stop sending keys to the Server
            btnMenu.Enabled = False
        End Sub
        Private Shared Sub btnMenu_MouseEnter() Handles btnMenu.MouseEnter 'Handles the mouse moving into btnMenu
            btnMenu.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
            btnMenu.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnMenu_MouseLeave() Handles btnMenu.MouseLeave 'Handles the mouse moving out of btnMenu
            btnMenu.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Public Shared Sub lblHull_Set_Text(ByVal text As String)
            lblHull.Text = text
        End Sub
        Public Shared Sub lblThrottle_Set_Text(ByVal text As String)
            lblThrottle.Text = text
        End Sub
        Public Shared Sub lblAmmunition_Set_Text(ByVal text As String)
            lblAmmunition.Text = text
        End Sub
        Public Shared Sub lblTargetDistance_Set_Text(ByVal text As String)
            lblTargetDistance.Text = text
        End Sub

    End Class
    Public Class SettingsScreen
        Private Shared WithEvents pnlSettings As New System.Windows.Forms.Panel With {
            .Size = New System.Drawing.Size(950, 600), .Location = New System.Drawing.Point(50, 50),
            .BorderStyle = Windows.Forms.BorderStyle.FixedSingle, .BackColor = Drawing.Color.Transparent,
            .AutoScroll = True}
        Public Shared WithEvents txtShipName As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 8),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtThrottleUp As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 58),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtThrottleDown As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 108),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtTurnRight As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 158),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtTurnLeft As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 208),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtFireWeapon As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 258),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtZoomOut As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 308),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Public Shared WithEvents txtZoomIn As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(225, 40), .Location = New System.Drawing.Point(10, 358),
            .Text = "SETTINGS ERROR", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.FromArgb(55, 22, 95, 95), .BackColor = Drawing.Color.LightGray}
        Private Shared WithEvents btnMenu As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(1025, 623), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 16, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(150, 27), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Main Menu"} 'A Button object that when Clicked returns to the main menu

        Public Shared Sub Layout(ByRef scr As Screen) 'Sets the Screen to display the menu screen
            Dim g As System.Drawing.Graphics = scr.CreateGraphics() 'Gets the graphics object for the Screen
            g.Clear(Drawing.Color.Black) 'Clear the screen to black
            scr.Controls.Clear() 'Clear the old list of Controls
            scr.Controls.Add(pnlSettings) 'Add pnlSettings
            pnlSettings.Controls.Add(txtShipName) 'Add txtShipName
            pnlSettings.Controls.Add(txtThrottleUp) 'Add txtThrottleUp
            pnlSettings.Controls.Add(txtThrottleDown) 'Add txtThrottleDown
            pnlSettings.Controls.Add(txtTurnLeft) 'Add txtTurnLeft
            pnlSettings.Controls.Add(txtTurnRight) 'Add txtTurnRight
            pnlSettings.Controls.Add(txtFireWeapon) 'Add txtFireWeapon
            pnlSettings.Controls.Add(txtZoomOut) 'Add txtZoomOut
            pnlSettings.Controls.Add(txtZoomIn) 'Add txtZoomIn
            scr.Controls.Add(btnMenu) 'Add btnMenu
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click 'Handles btnMenu being Clicked
            MenuScreen.Layout(Client_Console.OutputScreen) 'Go to the menu screen
            Client_Console.Save_Settings() 'Saves the Settings
        End Sub
        Private Shared Sub btnMenu_MouseEnter() Handles btnMenu.MouseEnter 'Handles the mouse moving into btnMenu
            btnMenu.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
            btnMenu.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnMenu_MouseLeave() Handles btnMenu.MouseLeave 'Handles the mouse moving out of btnMenu
            btnMenu.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
            Client_Console.OutputScreen.ActiveControl = Nothing
        End Sub

        Private Shared Sub txtShipName_TextChanged() Handles txtShipName.TextChanged
            If txtShipName.Text.Contains("SHIP NAME:") Then 'The title is there
                If txtShipName.Text.Substring(10) <> " " Then txtShipName.Text.Insert(10, " ") 'Insert the space
                txtShipName.Select(txtShipName.Text.Length, 0) 'Set the cursor at the end of the text
            Else 'The title is missing
                txtShipName.Text = "SHIP NAME: " + txtShipName.Text 'Set the text
                txtShipName.Select(txtShipName.Text.Length, 0) 'Set the cursor at the end of the text
            End If
            If txtShipName.Text.Length > 11 Then Client_Console.settingElements(0) =
                txtShipName.Text.Substring(11, (txtShipName.Text.Length - 11)) 'Set the setting
        End Sub
        Private Shared Sub txtThrottleUp_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtThrottleUp.KeyUp
            txtThrottleUp.Text = "THROTTLE UP: " + e.KeyCode.ToString()
            Client_Console.settingElements(1) = e.KeyCode
        End Sub
        Private Shared Sub txtThrottleDown_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtThrottleDown.KeyUp
            txtThrottleDown.Text = "THROTTLE DOWN: " + e.KeyCode.ToString()
            Client_Console.settingElements(2) = e.KeyCode
        End Sub
        Private Shared Sub txtTurnLeft_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtTurnLeft.KeyUp
            txtTurnLeft.Text = "TURN LEFT: " + e.KeyCode.ToString()
            Client_Console.settingElements(4) = e.KeyCode
        End Sub
        Private Shared Sub txtTurnRight_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtTurnRight.KeyUp
            txtTurnRight.Text = "TURN RIGHT: " + e.KeyCode.ToString()
            Client_Console.settingElements(3) = e.KeyCode
        End Sub
        Private Shared Sub txtFireWeapon_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtFireWeapon.KeyUp
            txtFireWeapon.Text = "FIRE WEAPON: " + e.KeyCode.ToString()
            Client_Console.settingElements(5) = e.KeyCode
        End Sub
        Private Shared Sub txtZoomOut_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtZoomOut.KeyUp
            txtZoomOut.Text = "ZOOM OUT: " + e.KeyCode.ToString()
            Client_Console.settingElements(6) = e.KeyCode
        End Sub
        Private Shared Sub txtZoomIn_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtZoomIn.KeyUp
            txtZoomIn.Text = "ZOOM IN: " + e.KeyCode.ToString()
            Client_Console.settingElements(7) = e.KeyCode
        End Sub

    End Class

    Public Sub New()
        InitializeComponent()
        Me.KeyPreview = True
        MenuScreen.Layout(Me)
    End Sub

    Public Sub Give_Control() 'Allows the Screen object to receive user inputs
        Windows.Forms.Application.Run(Me)
    End Sub

    Private Delegate Sub Escape_Key()
    Private Sub Keys_Down(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If sendKeys Then 'At the game screen
            Select Case e.KeyCode
                Case Windows.Forms.Keys.Escape
                    Dim d As New Escape_Key(AddressOf GameScreen.btnMenu_Click)
                    GameScreen.btnMenu.Invoke(d)
                Case Client_Console.settingElements(6) 'Zoom Out Key
                    If Client_Console.Client.Scaler < 1 Then Client_Console.Client.Scaler += 0.03
                Case Client_Console.settingElements(7) 'Zoom In Key
                    If Client_Console.Client.Scaler > 0.5 Then Client_Console.Client.Scaler -= 0.03
                Case Else
                    For i As Integer = 1 To 5 'Loop through all controls
                        If e.KeyCode = Client_Console.settingElements(i) Then 'The key has been found
                            i -= 1 'Take one from i
                            Dim shipControl As Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header =
                                (Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.max + i) 'The header value for the message
                            Dim temp As String = ("ERROR : There was an error while sending the " + shipControl.ToString() +
                                                  " KeyDown event to the Server. Client will now close.") 'The error message for if the send fails
                            If Client_Console.Client.values(i) = False Then 'This will not be a repeat message
                                Client_Console.Client.values(i) = True
                                Client_Console.Client.Send_Message({BitConverter.GetBytes(shipControl), BitConverter.GetBytes(True)}, {temp, temp})
                            End If
                            Exit Sub 'The key has been handled
                        End If
                    Next
            End Select
        Else 'At the menu screen
            Select Case e.KeyCode
                Case Windows.Forms.Keys.Escape
                    Dim d As New Escape_Key(AddressOf MenuScreen.btnBackToGame_Click)
                    MenuScreen.btnBackToGame.Invoke(d)
            End Select
        End If
    End Sub
    Private Sub Keys_Up(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        If sendKeys Then
            For i As Integer = 1 To 5 'Loop through all controls
                If e.KeyCode = Client_Console.settingElements(i) Then 'The key has been found
                    i -= 1 'Take one from i
                    Dim shipControl As Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header =
                        (Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.max + i) 'The header value for the message
                    Dim temp As String = ("ERROR : There was an error while sending the " + shipControl.ToString() +
                                          " KeyUp event to the Server. Client will now close.") 'The error message for if the send fails
                    Client_Console.Client.values(i) = False
                    Client_Console.Client.Send_Message({BitConverter.GetBytes(shipControl),
                                                        BitConverter.GetBytes(False)}, {temp, temp})
                    Exit Sub 'The key has been handled
                End If
            Next
        End If
    End Sub

End Class