Public Class Screen 'The object used as a GUI for the Client
    Private Class MenuScreen 'Objects displayed on the menu screen
        Private Shared WithEvents btnHost As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 76), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Host Game"} 'A Button object that when Clicked hosts a Server
        Private Shared WithEvents btnJoin As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 308), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Join Game"} 'A Button object that when Clicked joins an existing Server
        Private Shared WithEvents btnExit As New System.Windows.Forms.Button With {
            .Location = New System.Drawing.Point(400, 540), .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent,
            .Font = New System.Drawing.Font(New System.Drawing.Font("Consolas", 28, System.Drawing.FontStyle.Bold,
                                            System.Drawing.GraphicsUnit.Pixel), System.Drawing.FontStyle.Underline),
            .Size = New System.Drawing.Size(400, 80), .FlatStyle = Windows.Forms.FlatStyle.Flat,
            .TextAlign = Drawing.ContentAlignment.MiddleCenter, .BackgroundImageLayout = Windows.Forms.ImageLayout.Center,
            .Cursor = Windows.Forms.Cursors.Hand, .Text = "Exit"} 'A Button object that when Clicked closes the program

        Public Shared Sub Layout(ByRef scr As Screen) 'Sets the Screen to display the menu screen
            Dim g As System.Drawing.Graphics = scr.CreateGraphics() 'Gets the graphics object for the Screen
            g.Clear(Drawing.Color.Black) 'Clear the screen to black
            scr.Controls.Clear() 'Clear the old list of Controls
            scr.Controls.Add(btnHost) 'Add btnHost
            scr.Controls.Add(btnJoin) 'Add btnJoin
            scr.Controls.Add(btnExit) 'Add btnExit
        End Sub

        Private Shared Sub btnHost_Click() Handles btnHost.Click 'Hosts a new Server
            Process.Start("C:\Users\danie_000\Documents\GitHub\Star_Crew\Star_Crew_Server\Star_Crew_Server\bin\Debug\Star_Crew_Server.exe")
        End Sub
        Private Shared Sub btnHost_MouseEnter() Handles btnHost.MouseEnter 'Changes btnHost's colour when it's moused over
            btnHost.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnHost.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnHost_MouseLeave() Handles btnHost.MouseLeave 'Changes btnHost's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
        End Sub

        Private Shared Sub btnJoin_Click() Handles btnJoin.Click 'Joins an existing Server
            JoinScreen.Layout(Client_Console.OutputScreen) 'Go to the Join Screen
        End Sub
        Private Shared Sub btnJoin_MouseEnter() Handles btnJoin.MouseEnter 'Changes btnJoin's colour when it's moused over
            btnJoin.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnJoin.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnJoin_MouseLeave() Handles btnJoin.MouseLeave 'Changes btnJoin's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
        End Sub

        Private Shared Sub btnExit_Click() Handles btnExit.Click 'Closes the program
            End 'Close the Program
        End Sub
        Private Shared Sub btnExit_MouseEnter() Handles btnExit.MouseEnter 'Changes btnExit's colour when it's moused over
            btnExit.ForeColor = Drawing.Color.Turquoise 'Change the ForeColour
            btnExit.Focus() 'Set focus to this Button
        End Sub
        Private Shared Sub btnExit_MouseLeave() Handles btnExit.MouseLeave 'Changes btnExit's colour when the mouse leaves it
            btnHost.ForeColor = Drawing.Color.DarkTurquoise 'Change the ForeColour
        End Sub

    End Class
    Private Class JoinScreen 'Objects displayed when the Client is going to join an existing game
        Private Shared drpStationSelector As New System.Windows.Forms.ComboBox With {
            .Size = New System.Drawing.Size(125, 40), .Location = New System.Drawing.Point(537, 200), .BackColor = Drawing.Color.LightGray} 'A Combobox object used to select which ShipStation the user wants to connect to
        Private Shared txtIP As New System.Windows.Forms.TextBox With {
            .Size = New System.Drawing.Size(125, 40), .Location = New System.Drawing.Point(537, 250),
            .Text = "Input IP", .Font = New System.Drawing.Font("Consolas", 14, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel),
            .ForeColor = Drawing.Color.DarkTurquoise, .BackColor = Drawing.Color.LightGray}
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

        Public Shared Sub Initialise() 'Set's the initial values for the objects
            drpStationSelector.Items.AddRange({"Batteries", "Shielding", "Engineering"}) 'Add the values to the combobox
            drpStationSelector.SelectedIndex = 0
        End Sub
        Public Shared Sub Layout(ByRef scr As Screen) 'Set's the Screen to display the join screen
            If Client_Console.CommsThread IsNot Nothing Then 'There's a Connector from the last session
                Client_Console.Client.LoopComms = False 'Make sure the comms are not looping
                Client_Console.Client = Nothing 'Clear Client
                Client_Console.CommsThread.Join(300) 'Wait for the Client to close
            End If
            scr.Controls.Clear() 'Clear's the old display
            scr.Controls.Add(drpStationSelector) 'Add drpStationSelector
            scr.Controls.Add(txtIP) 'Add txtIP
            scr.Controls.Add(btnConnect) 'Add btnConnect
            scr.Controls.Add(btnMenu) 'Add btnMeny
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
                Dim nStation As Star_Crew_Shared_Libraries.Shared_Values.StationTypes =
                    Star_Crew_Shared_Libraries.Shared_Values.StationTypes.max 'The ShipStation that the user is connecting to
                Select Case LCase(drpStationSelector.Items(drpStationSelector.SelectedIndex))
                    Case "batteries" 'Their connecting to the batteries
                        nStation = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Battery 'Set nStation
                    Case "shielding" 'Their connecting to the shielding
                        nStation = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Shields 'Set nStation
                    Case "engineering" 'Their connecting to engineering
                        nStation = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Engines 'Set nStation
                End Select
                If nStation = Star_Crew_Shared_Libraries.Shared_Values.StationTypes.max Then 'An error occoured
                    Beep()
                Else
                    Try
                        Client_Console.Client = New Connector(nStation, txtIP.Text, Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort,
                                                              drpStationSelector.SelectedText, False) 'Create a new Connector and connect to the Server
                    Catch ex As Exception 'An exception occoured and the connection failed
                        Client_Console.Client = Nothing 'Clear Client to equal nothing
                        Beep() 'Make a tone to alert the User
                        Console.WriteLine("ERROR : An error occoured when trying to connect to the Server." +
                                          Environment.NewLine + ex.ToString()) 'Write the error to the console
                    End Try
                    If Client_Console.Client Is Nothing Then 'The connection failed
                        Beep() 'Make a tone to alert the User
                    End If
                End If
            Else
                Beep() 'Make a tone to alert the player
                txtIP.ForeColor = Drawing.Color.Red 'Change the fore colour of the bad IP
            End If
        End Sub
        Private Shared Sub btnConnect_MouseEnter() Handles btnConnect.MouseEnter 'Handles the mouse moving into btnConnect
            btnConnect.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
        End Sub
        Private Shared Sub btnConnect_MouseLeave() Handles btnConnect.MouseLeave 'Handles the mouse moving out of btnConnect
            btnConnect.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click 'Handles btnMenu being Clicked
            MenuScreen.Layout(Client_Console.OutputScreen) 'Go to the menu screen
        End Sub
        Private Shared Sub btnMenu_MouseEnter() Handles btnMenu.MouseEnter 'Handles the mouse moving into btnMenu
            btnMenu.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
        End Sub
        Private Shared Sub btnMenu_MouseLeave() Handles btnMenu.MouseLeave 'Handles the mouse moving out of btnMenu
            btnMenu.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
        End Sub

    End Class
    Public Class GameScreen 'Objects displayed when the Client is in game
        Private Shared WithEvents btnMenu As New System.Windows.Forms.Button With {
            .Size = New System.Drawing.Size(200, 45), .Location = New System.Drawing.Point(980, 635),
            .Text = "Main Menu", .FlatStyle = Windows.Forms.FlatStyle.Flat, .ForeColor = Drawing.Color.DarkTurquoise,
            .BackColor = Drawing.Color.Transparent, .Font = New System.Drawing.Font(
                New System.Drawing.Font("Consolas", 18, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Pixel), Drawing.FontStyle.Underline)}

        Public Shared Sub Layout(ByRef scr As Screen, ByVal Hosting As Boolean)
            scr.Controls.Clear() 'Clears the Screen of objects
            scr.Controls.Add(btnMenu) 'Add btnMenu to the Screen
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click 'Handles btnMenu being Clicked
            MenuScreen.Layout(Client_Console.OutputScreen) 'Go to the menu screen
        End Sub
        Private Shared Sub btnMenu_MouseEnter() Handles btnMenu.MouseEnter 'Handles the mouse moving into btnMenu
            btnMenu.ForeColor = Drawing.Color.Turquoise 'Change the fore colour
        End Sub
        Private Shared Sub btnMenu_MouseLeave() Handles btnMenu.MouseLeave 'Handles the mouse moving out of btnMenu
            btnMenu.ForeColor = Drawing.Color.DarkTurquoise 'Change the fore colour
        End Sub

    End Class

    Public Sub New()
        InitializeComponent()
        JoinScreen.Initialise()
        MenuScreen.Layout(Me)
    End Sub

    Public Sub Give_Control() 'Allows the Screen object to receive user inputs
        Windows.Forms.Application.Run(Me)
    End Sub

End Class