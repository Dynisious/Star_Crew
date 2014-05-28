Public Class Screen
    Private WithEvents Tick As New Timer With {.Interval = 100 / 6, .Enabled = False} 'A Timer object that 'ticks' 60 times a second
    Public MyClient As Client 'A Client object
    Public Shared ReadOnly ImageSize As New Point(600, 600) 'The Size of the Bitmap Image displayed on screen

    Public Class MenuScreenLayout 'The GUI layout of the Main Menu
        Public Shared WithEvents btnStartServer As System.Windows.Forms.Button
        Public Shared WithEvents btnStartClient As System.Windows.Forms.Button
        Public Shared WithEvents btnExit As System.Windows.Forms.Button
        Public Shared WithEvents btnLoad As System.Windows.Forms.Button

        Public Sub New()
            ConsoleWindow.OutputScreen.Controls.Clear()
            '-----Initialise Controls-----
            btnStartServer = New System.Windows.Forms.Button()
            btnStartClient = New System.Windows.Forms.Button()
            btnExit = New System.Windows.Forms.Button()
            btnLoad = New System.Windows.Forms.Button()
            '-----------------------------
            '-----Control Settings-----
            '
            'btnStartServer
            '
            btnStartServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnStartServer.Location = New System.Drawing.Point(400, 67)
            btnStartServer.Name = "btnStartServer"
            btnStartServer.Size = New System.Drawing.Size(400, 70)
            btnStartServer.TabIndex = 0
            btnStartServer.Text = "Open Server"
            btnStartServer.UseVisualStyleBackColor = True
            '
            'btnStartClient
            '
            btnStartClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnStartClient.Location = New System.Drawing.Point(400, 220)
            btnStartClient.Margin = New System.Windows.Forms.Padding(3, 100, 3, 3)
            btnStartClient.Name = "btnStartClient"
            btnStartClient.Size = New System.Drawing.Size(400, 70)
            btnStartClient.TabIndex = 1
            btnStartClient.Text = "Open Client"
            btnStartClient.UseVisualStyleBackColor = True
            '
            'btnExit
            '
            btnExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnExit.Location = New System.Drawing.Point(400, 519)
            btnExit.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
            btnExit.Name = "btnExit"
            btnExit.Size = New System.Drawing.Size(400, 70)
            btnExit.TabIndex = 2
            btnExit.Text = "Exit"
            btnExit.UseVisualStyleBackColor = True
            '
            'btnLoad
            '
            btnLoad.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnLoad.Location = New System.Drawing.Point(400, 370)
            btnLoad.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
            btnLoad.Name = "btnLoad"
            btnLoad.Size = New System.Drawing.Size(400, 70)
            btnLoad.TabIndex = 3
            btnLoad.Text = "Load Server"
            btnLoad.UseVisualStyleBackColor = True
            '--------------------------
            '-----Add Controls-----
            ConsoleWindow.OutputScreen.Controls.Add(btnLoad)
            ConsoleWindow.OutputScreen.Controls.Add(btnExit)
            ConsoleWindow.OutputScreen.Controls.Add(btnStartClient)
            ConsoleWindow.OutputScreen.Controls.Add(btnStartServer)
            '----------------------
            My.Computer.Audio.Play(My.Resources.The_Adventure_Begins_Extended, AudioPlayMode.BackgroundLoop)
        End Sub

        Private Shared Sub btnStartServer_Click() Handles btnStartServer.Click 'Starts the Server
            If ConsoleWindow.GameServer.GameWorld IsNot Nothing Then 'There is already a Server Currently Running
                ConsoleWindow.GameServer.ServerLoop = False  'Closes the Server
            End If
            ConsoleWindow.GameServer.StartServer(True) 'Starts the Server
        End Sub

        Private Shared Sub btnStartClient_Click() Handles btnStartClient.Click 'Opens the Client Setup Screen 
            Dim temp As New ClientSetupLayout() 'Sets the Screen's GUI to the ClientSetupLayout layout
        End Sub

        Private Shared Sub btnLoad_Click() Handles btnLoad.Click
            If IO.File.Exists("C:\Users\" + Environment.UserName + "\Desktop\Star Crew Save.save") = True Then
                If ConsoleWindow.GameServer.GameWorld IsNot Nothing Then
                    ConsoleWindow.GameServer.ServerLoop = False
                    ConsoleWindow.ServerThread.Join(30)
                End If
                Using fs As New IO.FileStream("C:\Users\" + Environment.UserName + "\Desktop\Star Crew Save.save", IO.FileMode.Open, IO.FileAccess.Read)
                    Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    ConsoleWindow.GameServer.GameWorld = bf.Deserialize(fs)
                    fs.Close()
                End Using
                Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'The identity of the Mutex
                Dim securityProtocols As New Security.AccessControl.MutexSecurity 'The security settings of the Mutex
                securityProtocols.AddAccessRule(
                    New Security.AccessControl.MutexAccessRule(user,
                                                               Security.AccessControl.MutexRights.Modify Or
                                                               Security.AccessControl.MutexRights.Synchronize,
                                                               Security.AccessControl.AccessControlType.Allow))
                'Allow Threads to Access and Release the Mutex
                Dim bool As Boolean
                ConsoleWindow.GameServer.GameWorld.MessageMutex = New Threading.Mutex(False, "MessageMutex", bool, securityProtocols)
                'Create the Mutex object
                ConsoleWindow.GameServer.GameWorld.GalaxyTimer = New Timer With {.Interval = 100, .Enabled = True} 'Create and start as new Timer
                ConsoleWindow.GameServer.StartServer(False)
                Console.WriteLine("Game has been loaded successfully")
            Else
                Console.WriteLine("Error : No save file was found")
                Beep()
            End If
        End Sub

        Private Shared Sub btnExit_Click() Handles btnExit.Click 'Closes the program
            End
        End Sub

    End Class

    Public Class ClientSetupLayout 'The GUI layout that lets users set up a Client
        Public Shared WithEvents txtIP As System.Windows.Forms.TextBox
        Public Shared WithEvents lblIP As System.Windows.Forms.Label
        Public Shared WithEvents btnEnter As System.Windows.Forms.Button
        Public Shared WithEvents btnMenu As System.Windows.Forms.Button
        Public Shared WithEvents DomainUpDown1 As System.Windows.Forms.DomainUpDown

        Public Sub New()
            ConsoleWindow.OutputScreen.Controls.Clear()
            txtIP = New System.Windows.Forms.TextBox()
            lblIP = New System.Windows.Forms.Label()
            btnEnter = New System.Windows.Forms.Button()
            btnMenu = New System.Windows.Forms.Button()
            DomainUpDown1 = New System.Windows.Forms.DomainUpDown()
            '
            'txtIP
            '
            txtIP.Location = New System.Drawing.Point(500, 230)
            txtIP.Name = "txtIP"
            txtIP.Size = New System.Drawing.Size(200, 20)
            txtIP.TabIndex = 0
            '
            'lblIP
            '
            lblIP.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblIP.Location = New System.Drawing.Point(500, 197)
            lblIP.Name = "lblIP"
            lblIP.Size = New System.Drawing.Size(200, 20)
            lblIP.TabIndex = 1
            lblIP.Text = "Input Server IP"
            lblIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'btnEnter
            '
            btnEnter.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            btnEnter.Location = New System.Drawing.Point(520, 260)
            btnEnter.Name = "btnEnter"
            btnEnter.Size = New System.Drawing.Size(160, 30)
            btnEnter.TabIndex = 2
            btnEnter.Text = "Connect"
            btnEnter.UseVisualStyleBackColor = True
            '
            'btnMenu
            '
            btnMenu.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            btnMenu.Location = New System.Drawing.Point(520, 296)
            btnMenu.Name = "btnMenu"
            btnMenu.Size = New System.Drawing.Size(160, 30)
            btnMenu.TabIndex = 3
            btnMenu.Text = "Main Menu"
            btnMenu.UseVisualStyleBackColor = True
            '
            'DomainUpDown1
            '
            DomainUpDown1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            DomainUpDown1.Items.Add("Helm")
            DomainUpDown1.Items.Add("Batteries")
            DomainUpDown1.Items.Add("Shielding")
            DomainUpDown1.Items.Add("Engineering")
            DomainUpDown1.SelectedIndex = 0
            DomainUpDown1.Location = New System.Drawing.Point(500, 150)
            DomainUpDown1.Name = "DomainUpDown1"
            DomainUpDown1.Size = New System.Drawing.Size(200, 23)
            DomainUpDown1.TabIndex = 4

            ConsoleWindow.OutputScreen.Controls.Add(DomainUpDown1)
            ConsoleWindow.OutputScreen.Controls.Add(btnMenu)
            ConsoleWindow.OutputScreen.Controls.Add(btnEnter)
            ConsoleWindow.OutputScreen.Controls.Add(lblIP)
            ConsoleWindow.OutputScreen.Controls.Add(txtIP)
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click 'Returns to the Main Menu
            Dim temp As New MenuScreenLayout 'Sets the screen's GUI to the MenuScreenLayout Layout
        End Sub

        Private Shared Sub btnEnter_Click() Handles btnEnter.Click 'Attempts to Create a Client object and connect to a specified Server
            Dim count As Integer 'An Integer representing how many '.'s are in the given IP
            Dim lastIndex As Integer 'An Integer representing the last index in the string where a '.' was found
            If txtIP.Text <> "" Then
                While True
                    Dim e As Integer = txtIP.Text.IndexOf(".", lastIndex + 1) 'Get the next index of the '.' in the string
                    If e <> -1 And e < txtIP.TextLength Then 'a '.' was found and it was not at the end of the string
                        lastIndex = e
                        count = count + 1
                    Else 'either a '.' was not found or it was at the end of the IP
                        Exit While
                    End If
                End While

                If count = 3 Then 'A full IP address was given
                    ConsoleWindow.OutputScreen.MyClient = New Client(txtIP.Text, DomainUpDown1.SelectedIndex) 'Create a new Client object
                    If ConsoleWindow.OutputScreen.MyClient.ClientLoop = True Then 'Check if the connection to the Server was successful
                        Dim temp As New GamePlayLayout 'Set the screen's GUI to the GamePlayLayout layout
                    End If
                Else 'A full IP was not given
                    Console.Beep()
                End If
            Else 'No IP was given
                Console.Beep()
            End If
        End Sub

    End Class

    Public Class GamePlayLayout
        Public Shared WithEvents picDisplayGraphics As System.Windows.Forms.PictureBox
        Public Shared WithEvents pnlDisplays As System.Windows.Forms.Panel
        Public Shared WithEvents lblHull As System.Windows.Forms.Label
        Public Shared WithEvents lblThrottle As System.Windows.Forms.Label
        Public Shared WithEvents lblEngines As System.Windows.Forms.Label
        Public Shared WithEvents lblPowerCore As System.Windows.Forms.Label
        Public Shared WithEvents lblSecondary As System.Windows.Forms.Label
        Public Shared WithEvents lblPrimary As System.Windows.Forms.Label
        Public Shared WithEvents lblForward As System.Windows.Forms.Label
        Public Shared WithEvents lblRear As System.Windows.Forms.Label
        Public Shared WithEvents lblRight As System.Windows.Forms.Label
        Public Shared WithEvents lblLeft As System.Windows.Forms.Label
        Public Shared WithEvents lblCoreTemp As System.Windows.Forms.Label
        Public Shared WithEvents lblTempRate As System.Windows.Forms.Label
        Public Shared WithEvents pnlMenuButtons As System.Windows.Forms.Panel
        Public Shared WithEvents btnMainMenu As System.Windows.Forms.Button
        Public Shared WithEvents btnEndGame As System.Windows.Forms.Button
        Public Shared WithEvents UserKeyInterfacer As System.Windows.Forms.Button
        Public Shared WithEvents btnPausePlay As System.Windows.Forms.Button
        Public Shared WithEvents btnSave As System.Windows.Forms.Button
        Public Shared Displaying As Boolean = False

        Public Sub New()
            ConsoleWindow.OutputScreen.Controls.Clear()
            '-----Initialize Controls-----
            picDisplayGraphics = New System.Windows.Forms.PictureBox()
            pnlDisplays = New System.Windows.Forms.Panel()
            lblTempRate = New System.Windows.Forms.Label()
            lblCoreTemp = New System.Windows.Forms.Label()
            lblThrottle = New System.Windows.Forms.Label()
            lblEngines = New System.Windows.Forms.Label()
            lblPowerCore = New System.Windows.Forms.Label()
            lblSecondary = New System.Windows.Forms.Label()
            lblPrimary = New System.Windows.Forms.Label()
            lblForward = New System.Windows.Forms.Label()
            lblRear = New System.Windows.Forms.Label()
            lblRight = New System.Windows.Forms.Label()
            lblLeft = New System.Windows.Forms.Label()
            lblHull = New System.Windows.Forms.Label()
            pnlMenuButtons = New System.Windows.Forms.Panel()
            btnPausePlay = New System.Windows.Forms.Button()
            btnEndGame = New System.Windows.Forms.Button()
            btnMainMenu = New System.Windows.Forms.Button()
            UserKeyInterfacer = New System.Windows.Forms.Button()
            btnSave = New System.Windows.Forms.Button()
            CType(picDisplayGraphics, System.ComponentModel.ISupportInitialize).BeginInit()
            pnlDisplays.SuspendLayout()
            pnlMenuButtons.SuspendLayout()
            '-----------------------------
            Displaying = True

            '-----Control Settings-----
            '
            'picDisplayGraphics
            '
            picDisplayGraphics.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            picDisplayGraphics.Location = New System.Drawing.Point(6, 6)
            picDisplayGraphics.Name = "picDisplayGraphics"
            picDisplayGraphics.Size = New System.Drawing.Size(600, 600)
            picDisplayGraphics.TabIndex = 0
            picDisplayGraphics.TabStop = False
            '
            'pnlDisplays
            '
            pnlDisplays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            pnlDisplays.Controls.Add(lblTempRate)
            pnlDisplays.Controls.Add(lblCoreTemp)
            pnlDisplays.Controls.Add(lblThrottle)
            pnlDisplays.Controls.Add(lblEngines)
            pnlDisplays.Controls.Add(lblPowerCore)
            pnlDisplays.Controls.Add(lblSecondary)
            pnlDisplays.Controls.Add(lblPrimary)
            pnlDisplays.Controls.Add(lblForward)
            pnlDisplays.Controls.Add(lblRear)
            pnlDisplays.Controls.Add(lblRight)
            pnlDisplays.Controls.Add(lblLeft)
            pnlDisplays.Controls.Add(lblHull)
            pnlDisplays.Location = New System.Drawing.Point(612, 6)
            pnlDisplays.Name = "pnlDisplays"
            pnlDisplays.Size = New System.Drawing.Size(560, 338)
            pnlDisplays.TabIndex = 1
            '
            'lblTempRate
            '
            lblTempRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblTempRate.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblTempRate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblTempRate.Location = New System.Drawing.Point(295, 295)
            lblTempRate.Name = "lblTempRate"
            lblTempRate.Size = New System.Drawing.Size(200, 30)
            lblTempRate.TabIndex = 11
            lblTempRate.Text = "Temp Rate: 0"
            lblTempRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblCoreTemp
            '
            lblCoreTemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblCoreTemp.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblCoreTemp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblCoreTemp.Location = New System.Drawing.Point(85, 295)
            lblCoreTemp.Name = "lblCoreTemp"
            lblCoreTemp.Size = New System.Drawing.Size(200, 30)
            lblCoreTemp.TabIndex = 10
            lblCoreTemp.Text = "Core Temp: 0/0"
            lblCoreTemp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblThrottle
            '
            lblThrottle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblThrottle.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblThrottle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblThrottle.Location = New System.Drawing.Point(295, 25)
            lblThrottle.Name = "lblThrottle"
            lblThrottle.Size = New System.Drawing.Size(200, 30)
            lblThrottle.TabIndex = 9
            lblThrottle.Text = "Speed: 0/0"
            lblThrottle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblEngines
            '
            lblEngines.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblEngines.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblEngines.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblEngines.Location = New System.Drawing.Point(295, 247)
            lblEngines.Name = "lblEngines"
            lblEngines.Size = New System.Drawing.Size(200, 30)
            lblEngines.TabIndex = 8
            lblEngines.Text = "Engines: 0/0"
            lblEngines.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblPowerCore
            '
            lblPowerCore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblPowerCore.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblPowerCore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblPowerCore.Location = New System.Drawing.Point(295, 212)
            lblPowerCore.Name = "lblPowerCore"
            lblPowerCore.Size = New System.Drawing.Size(200, 30)
            lblPowerCore.TabIndex = 7
            lblPowerCore.Text = "Power Core: 0/0"
            lblPowerCore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblSecondary
            '
            lblSecondary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblSecondary.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblSecondary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblSecondary.Location = New System.Drawing.Point(85, 247)
            lblSecondary.Name = "lblSecondary"
            lblSecondary.Size = New System.Drawing.Size(200, 30)
            lblSecondary.TabIndex = 6
            lblSecondary.Text = "Secondary: 0/0"
            lblSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblPrimary
            '
            lblPrimary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblPrimary.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblPrimary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblPrimary.Location = New System.Drawing.Point(85, 212)
            lblPrimary.Name = "lblPrimary"
            lblPrimary.Size = New System.Drawing.Size(200, 30)
            lblPrimary.TabIndex = 5
            lblPrimary.Text = "Primary: 0/0"
            lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblForward
            '
            lblForward.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblForward.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblForward.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblForward.Location = New System.Drawing.Point(180, 76)
            lblForward.Name = "lblForward"
            lblForward.Size = New System.Drawing.Size(200, 30)
            lblForward.TabIndex = 4
            lblForward.Text = "Fore: 0/0"
            lblForward.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblRear
            '
            lblRear.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblRear.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblRear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblRear.Location = New System.Drawing.Point(180, 156)
            lblRear.Name = "lblRear"
            lblRear.Size = New System.Drawing.Size(200, 30)
            lblRear.TabIndex = 3
            lblRear.Text = "Aft: 0/0"
            lblRear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblRight
            '
            lblRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblRight.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblRight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblRight.Location = New System.Drawing.Point(295, 116)
            lblRight.Name = "lblRight"
            lblRight.Size = New System.Drawing.Size(200, 30)
            lblRight.TabIndex = 2
            lblRight.Text = "Starbord: 0/0"
            lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblLeft
            '
            lblLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblLeft.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblLeft.Location = New System.Drawing.Point(85, 116)
            lblLeft.Name = "lblLeft"
            lblLeft.Size = New System.Drawing.Size(200, 30)
            lblLeft.TabIndex = 1
            lblLeft.Text = "Port: 0/0"
            lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblHull
            '
            lblHull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblHull.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            lblHull.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            lblHull.Location = New System.Drawing.Point(85, 25)
            lblHull.Name = "lblHull"
            lblHull.Size = New System.Drawing.Size(200, 30)
            lblHull.TabIndex = 0
            lblHull.Text = "Hull: 0/0"
            lblHull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'pnlMenuButtons
            '
            pnlMenuButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            pnlMenuButtons.Controls.Add(btnSave)
            pnlMenuButtons.Controls.Add(btnPausePlay)
            pnlMenuButtons.Controls.Add(btnEndGame)
            pnlMenuButtons.Controls.Add(btnMainMenu)
            pnlMenuButtons.Location = New System.Drawing.Point(612, 350)
            pnlMenuButtons.Name = "pnlMenuButtons"
            pnlMenuButtons.Size = New System.Drawing.Size(560, 256)
            pnlMenuButtons.TabIndex = 3
            '
            'btnPausePlay
            '
            btnPausePlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            btnPausePlay.Font = New System.Drawing.Font("Lucida Console", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            btnPausePlay.Location = New System.Drawing.Point(135, 121)
            btnPausePlay.Name = "btnPausePlay"
            btnPausePlay.Size = New System.Drawing.Size(140, 40)
            btnPausePlay.TabIndex = 3
            btnPausePlay.Text = "Pause"
            btnPausePlay.UseVisualStyleBackColor = True
            '
            'btnEndGame
            '
            btnEndGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            btnEndGame.Font = New System.Drawing.Font("Lucida Console", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            btnEndGame.Location = New System.Drawing.Point(285, 33)
            btnEndGame.Name = "btnEndGame"
            btnEndGame.Size = New System.Drawing.Size(140, 40)
            btnEndGame.TabIndex = 2
            btnEndGame.Text = "Close Game"
            btnEndGame.UseVisualStyleBackColor = True
            '
            'btnMainMenu
            '
            btnMainMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            btnMainMenu.Font = New System.Drawing.Font("Lucida Console", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            btnMainMenu.Location = New System.Drawing.Point(135, 33)
            btnMainMenu.Name = "btnMainMenu"
            btnMainMenu.Size = New System.Drawing.Size(140, 40)
            btnMainMenu.TabIndex = 1
            btnMainMenu.Text = "Main Menu"
            btnMainMenu.UseVisualStyleBackColor = True
            '
            'UserKeyInterfacer
            '
            UserKeyInterfacer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            UserKeyInterfacer.Location = New System.Drawing.Point(20, 20)
            UserKeyInterfacer.Name = "UserKeyInterfacer"
            UserKeyInterfacer.Size = New System.Drawing.Size(40, 40)
            UserKeyInterfacer.TabIndex = 0
            UserKeyInterfacer.UseVisualStyleBackColor = True
            '
            'btnSave
            '
            btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            btnSave.Font = New System.Drawing.Font("Lucida Console", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            btnSave.Location = New System.Drawing.Point(285, 121)
            btnSave.Name = "btnSave"
            btnSave.Size = New System.Drawing.Size(140, 40)
            btnSave.TabIndex = 4
            btnSave.Text = "Save Game"
            btnSave.UseVisualStyleBackColor = True
            '--------------------------

            '-----Add Controls-----
            ConsoleWindow.OutputScreen.Controls.Add(picDisplayGraphics)
            ConsoleWindow.OutputScreen.Controls.Add(pnlDisplays)
            ConsoleWindow.OutputScreen.Controls.Add(pnlMenuButtons)
            ConsoleWindow.OutputScreen.Controls.Add(UserKeyInterfacer)
            '----------------------

            '-----Display Controls-----
            CType(picDisplayGraphics, System.ComponentModel.ISupportInitialize).EndInit()
            pnlDisplays.ResumeLayout(False)
            pnlMenuButtons.ResumeLayout(False)
            '--------------------------
            UserKeyInterfacer.Focus()
            If ConsoleWindow.GameServer.GameWorld IsNot Nothing Then
                btnPausePlay.Enabled = True
                btnSave.Enabled = True
            Else
                btnPausePlay.Enabled = False
                btnSave.Enabled = False
            End If
            If ConsoleWindow.OutputScreen.MyClient.ClientLoop = False Then
                Dim temp As New MenuScreenLayout
            Else
                ConsoleWindow.OutputScreen.Tick.Enabled = True
            End If
        End Sub

        Public Shared Sub btnMainMenu_Click() Handles btnMainMenu.Click 'Returns to the Main Menu
            ConsoleWindow.OutputScreen.MyClient.Tick.Enabled = False 'Stop Updating the Screen's image
            ConsoleWindow.OutputScreen.MyClient.ClientLoop = False 'Lets the Loop finish
            Dim temp As New MenuScreenLayout 'Sets the screen's GUI to the MenuScreenLayout layout
        End Sub

        Private Shared Sub btnEndGame_Click() Handles btnEndGame.Click 'Closes the program
            End
        End Sub

        Private Shared Sub btnPausePlay_Click() Handles btnPausePlay.Click
            If ConsoleWindow.GameServer IsNot Nothing Then
                If ConsoleWindow.GameServer.GameWorld.Paused = False Then
                    ConsoleWindow.GameServer.GameWorld.Paused = True
                    btnPausePlay.Text = "Resume"
                    Console.WriteLine("Game is Paused")
                    My.Computer.Audio.Stop()
                    UserKeyInterfacer.Focus()
                Else
                    ConsoleWindow.GameServer.GameWorld.Paused = False
                    btnPausePlay.Text = "Pause"
                    Console.WriteLine("Game has been Resumed")
                    UserKeyInterfacer.Focus()
                End If
            End If
        End Sub

        Private Shared Sub btnSave_Click() Handles btnSave.Click
            Using fs As New IO.FileStream("C:\Users\" + Environment.UserName + "\Desktop\Star Crew Save.save", IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                bf.Serialize(fs, ConsoleWindow.GameServer.GameWorld)
                fs.Flush()
                fs.Close()
            End Using
            Console.WriteLine("Game has been saved successfully")
            UserKeyInterfacer.Focus()
        End Sub

        Private Shared Sub UserKeyInterfacer_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs) Handles UserKeyInterfacer.PreviewKeyDown
            'Captures PreviewKeyDown events when the User presses a key
            Select Case ConsoleWindow.OutputScreen.MyClient.myMessage.Station 'Selects the Station that the User is in control of
                Case Station.StationTypes.Helm
                    If e.KeyCode = Keys.Up Then 'Speed Up
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.ThrottleUp, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Speed Down
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.ThrottleDown, 1)
                    ElseIf e.KeyCode = Keys.Right Then 'Turn Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.TurnRight, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.TurnLeft, 1)
                    ElseIf e.KeyCode = Keys.J Then 'Warp Drive
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.WarpDrive, 1)
                    ElseIf e.KeyCode = Keys.M Then 'Match Speed
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.MatchSpeed, 1)
                    End If
                Case Station.StationTypes.Batteries
                    If e.KeyCode = Keys.Right Then 'Turn Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.TurnRight, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.TurnLeft, 1)
                    ElseIf e.KeyCode = Keys.M Then 'Set Target
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.SetTarget, 1)
                    End If
                    If e.KeyCode = Keys.A Then 'Fire Primary
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.FirePrimary, 1)
                    End If
                    If e.KeyCode = Keys.D Then 'Fire Secondary
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.FireSecondary, 1)
                    End If
                Case Station.StationTypes.Shielding
                    If e.KeyCode = Keys.Up Then 'Boost Forward
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostForward, 1)
                    ElseIf e.KeyCode = Keys.Right Then 'Boost Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostRight, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Boost Back
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostBack, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Boost Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostLeft, 1)
                    End If
                Case Station.StationTypes.Engineering
                    If e.KeyCode = Keys.Up Then 'Heat
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Engineering.Commands.Heat, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Cool
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Engineering.Commands.Cool, 1)
                    End If
            End Select
            If e.KeyCode = Keys.Z Then
                If ConsoleWindow.OutputScreen.MyClient.Zoom = 100 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 65
                ElseIf ConsoleWindow.OutputScreen.MyClient.Zoom = 65 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 30
                ElseIf ConsoleWindow.OutputScreen.MyClient.Zoom = 30 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 15
                End If
            ElseIf e.KeyCode = Keys.X Then
                If ConsoleWindow.OutputScreen.MyClient.Zoom = 15 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 30
                ElseIf ConsoleWindow.OutputScreen.MyClient.Zoom = 30 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 65
                ElseIf ConsoleWindow.OutputScreen.MyClient.Zoom = 65 Then
                    ConsoleWindow.OutputScreen.MyClient.Zoom = 100
                End If
            End If
        End Sub
        Private Shared Sub UserKeyInterfacer_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles UserKeyInterfacer.KeyUp
            'Captures KeyUp events when the User releases a Key
            Select Case ConsoleWindow.OutputScreen.MyClient.myMessage.Station
                Case Station.StationTypes.Helm
                    If e.KeyCode = Keys.Up Then 'Speed Up
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.ThrottleUp, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Speed Down
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.ThrottleDown, 0)
                    ElseIf e.KeyCode = Keys.Right Then 'Turn Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.TurnRight, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.TurnLeft, 0)
                    ElseIf e.KeyCode = Keys.J Then 'Warp Drive
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.WarpDrive, 0)
                    ElseIf e.KeyCode = Keys.M Then 'Match Speed
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Helm.Commands.MatchSpeed, 0)
                    End If
                Case Station.StationTypes.Batteries
                    If e.KeyCode = Keys.Right Then 'Turn Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.TurnRight, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.TurnLeft, 0)
                    ElseIf e.KeyCode = Keys.M Then 'Set Target
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.SetTarget, 0)
                    End If
                    If e.KeyCode = Keys.A Then 'Fire Primary
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.FirePrimary, 0)
                    End If
                    If e.KeyCode = Keys.D Then 'Fire Secondary
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Battery.Commands.FireSecondary, 0)
                    End If
                Case Station.StationTypes.Shielding
                    If e.KeyCode = Keys.Up Then 'Boost Forward
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostForward, 0)
                    ElseIf e.KeyCode = Keys.Right Then 'Boost Right
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostRight, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Boost Back
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostBack, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Boost Left
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Shields.Commands.BoostLeft, 0)
                    End If
                Case Station.StationTypes.Engineering
                    If e.KeyCode = Keys.Up Then 'Heat
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Engineering.Commands.Heat, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Cool
                        ConsoleWindow.OutputScreen.MyClient.SendCommand(Engineering.Commands.Cool, 0)
                    End If
            End Select
        End Sub

    End Class

    Public Sub Open() 'Creates a application loop on the current thread for the screen
        Windows.Forms.Application.Run(Me)
    End Sub

    Public Sub New() 'Creates the MenuScreenLayout layout
        InitializeComponent()
        '-----Initialise Controls-----
        MenuScreenLayout.btnStartServer = New System.Windows.Forms.Button()
        MenuScreenLayout.btnStartClient = New System.Windows.Forms.Button()
        MenuScreenLayout.btnExit = New System.Windows.Forms.Button()
        MenuScreenLayout.btnLoad = New System.Windows.Forms.Button()
        '-----------------------------
        '-----Control Settings-----
        '
        'btnStartServer
        '
        MenuScreenLayout.btnStartServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnStartServer.Location = New System.Drawing.Point(400, 67)
        MenuScreenLayout.btnStartServer.Name = "btnStartServer"
        MenuScreenLayout.btnStartServer.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnStartServer.TabIndex = 0
        MenuScreenLayout.btnStartServer.Text = "Open Server"
        MenuScreenLayout.btnStartServer.UseVisualStyleBackColor = True
        '
        'btnStartClient
        '
        MenuScreenLayout.btnStartClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnStartClient.Location = New System.Drawing.Point(400, 220)
        MenuScreenLayout.btnStartClient.Margin = New System.Windows.Forms.Padding(3, 100, 3, 3)
        MenuScreenLayout.btnStartClient.Name = "btnStartClient"
        MenuScreenLayout.btnStartClient.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnStartClient.TabIndex = 1
        MenuScreenLayout.btnStartClient.Text = "Open Client"
        MenuScreenLayout.btnStartClient.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        MenuScreenLayout.btnExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnExit.Location = New System.Drawing.Point(400, 519)
        MenuScreenLayout.btnExit.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
        MenuScreenLayout.btnExit.Name = "btnExit"
        MenuScreenLayout.btnExit.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnExit.TabIndex = 2
        MenuScreenLayout.btnExit.Text = "Exit"
        MenuScreenLayout.btnExit.UseVisualStyleBackColor = True
        '
        'btnLoad
        '
        MenuScreenLayout.btnLoad.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnLoad.Location = New System.Drawing.Point(400, 370)
        MenuScreenLayout.btnLoad.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
        MenuScreenLayout.btnLoad.Name = "btnLoad"
        MenuScreenLayout.btnLoad.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnLoad.TabIndex = 3
        MenuScreenLayout.btnLoad.Text = "Load Server"
        MenuScreenLayout.btnLoad.UseVisualStyleBackColor = True
        '--------------------------
        '-----Add Controls-----
        Controls.Add(MenuScreenLayout.btnLoad)
        Controls.Add(MenuScreenLayout.btnExit)
        Controls.Add(MenuScreenLayout.btnStartClient)
        Controls.Add(MenuScreenLayout.btnStartServer)
        ' ''----------------------
        My.Computer.Audio.Play(My.Resources.The_Adventure_Begins_Extended, AudioPlayMode.BackgroundLoop)
    End Sub

    Private Sub UpdateScreen_Handle() Handles Tick.Tick 'Updates the 'stats' displayed on the screen
        If ConsoleWindow.OutputScreen.MyClient.ClientLoop = True And ConsoleWindow.OutputScreen.MyClient.IncomingMessage IsNot Nothing Then
            'There is information to display
            Screen.GamePlayLayout.lblThrottle.Text = "Speed: " +
                CStr(CInt(MyClient.IncomingMessage.Speed.current)) +
                "/" + CStr(CInt(MyClient.IncomingMessage.Speed.max)) 'Displays the current Speed and max Speed
            If MyClient.IncomingMessage.Primary IsNot Nothing Then 'Their is system information to display
                Dim message As ServerMessage = MyClient.IncomingMessage
                If message.Positions(0).Hit = True Then 'Make the Hull label flash orange
                    Screen.GamePlayLayout.lblHull.BackColor = Color.Orange
                Else 'Make the Hull label Transparent
                    Screen.GamePlayLayout.lblHull.BackColor = Color.Transparent
                End If
                Screen.GamePlayLayout.lblHull.Text = "Hull: " +
                    CStr(Math.Round(message.Positions(0).Hull.current, 2)) +
                    "/" + CStr(message.Positions(0).Hull.max) 'Displays the current Hull and max Hull

                Screen.GamePlayLayout.lblForward.Text = "Fore: " +
                    CStr(CInt(message.ForeShield.current)) +
                    "/" + CStr(message.ForeShield.max) 'Displays the current Fore Shield and max Fore Shield
                Screen.GamePlayLayout.lblRight.Text = "Starboard: " +
                    CStr(CInt(message.StarboardShield.current)) +
                    "/" + CStr(message.StarboardShield.max) 'Displays the current Starboard Shield and max Starboard Shield
                Screen.GamePlayLayout.lblRear.Text = "Aft: " +
                    CStr(CInt(message.AftShield.current)) +
                    "/" + CStr(message.AftShield.max) 'Displays the current Aft Shield and max Aft Shield
                Screen.GamePlayLayout.lblLeft.Text = "Port: " +
                    CStr(CInt(message.PortShield.current)) +
                    "/" + CStr(message.PortShield.max) 'Displays the current Port Shield and max Port Shield
                Select Case message.LastHit 'Select which Shield was last hit
                    Case Shields.Sides.FrontShield 'Fore
                        Screen.GamePlayLayout.lblForward.BackColor = Color.LightBlue
                        Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                    Case Shields.Sides.RightShield 'Starboard
                        Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRight.BackColor = Color.LightBlue
                        Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                    Case Shields.Sides.BackShield 'Aft
                        Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRear.BackColor = Color.LightBlue
                        Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                    Case Shields.Sides.LeftShield 'Port
                        Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                        Screen.GamePlayLayout.lblLeft.BackColor = Color.LightBlue
                End Select

                If MyClient.IncomingMessage.Firing = True Then 'Set both Weapons to Flash Light Blue
                    Screen.GamePlayLayout.lblPrimary.BackColor = Color.LightBlue
                    Screen.GamePlayLayout.lblSecondary.BackColor = Color.LightBlue
                Else 'Set both Weapons to be Transparent
                    Screen.GamePlayLayout.lblPrimary.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblSecondary.BackColor = Color.Transparent
                End If
                Screen.GamePlayLayout.lblPrimary.Text = "Primary: " +
                    CStr(MyClient.IncomingMessage.Primary.Integrety.current) +
                    "/" + CStr(MyClient.IncomingMessage.Primary.Integrety.max)
                'Displays the integrety of the Primary Weapon
                Screen.GamePlayLayout.lblSecondary.Text = "Secondary: " +
                    CStr(MyClient.IncomingMessage.Secondary.Integrety.current) +
                    "/" + CStr(MyClient.IncomingMessage.Secondary.Integrety.max)
                'Displays the integrety of the Secondary Weapon

                Screen.GamePlayLayout.lblPowerCore.Text = "Power Core: " +
                    CStr(message.PowerCore.current) +
                    "/" + CStr(message.PowerCore.max)
                'Displays the integrety of the Power Core
                Screen.GamePlayLayout.lblEngines.Text = "Engines: " +
                    CStr(message.Engines.current) +
                    "/" + CStr(message.Engines.max)
                'Displays the integrety of the Engines
                Screen.GamePlayLayout.lblCoreTemp.Text = "Core Temp: " +
                    CStr(Math.Round(message.Heat, 2)) + "*e5/100*e5"
                'Displays the temperature of the Power Core
                Screen.GamePlayLayout.lblTempRate.Text = "Temp Rate: " +
                    CStr(Math.Round(message.Rate, 2)) + "*e5"
                'Displays the rate of increase in the Power Cores Temperature
            Else 'Display Fleet Stats
                Screen.GamePlayLayout.lblHull.Text = "Ship Count: " +
                    CStr(ConsoleWindow.OutputScreen.MyClient.IncomingMessage.ShipCount) + "/" + CStr(Fleet.PopulationCap)
                'The number of Ships in the Fleet
            End If
        ElseIf MyClient.ClientLoop = False Then 'Return to the Main Menu
            Dim temp As New MenuScreenLayout 'Set the screens GUI to the MenuScreenLayout layout
            Tick.Enabled = False 'Stop the Timer to update the Stats
        End If
    End Sub

    Private Sub Screen_FormClosing(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        If ConsoleWindow.GameServer.GameWorld IsNot Nothing Then 'There's an open Server
            ConsoleWindow.GameServer.ServerLoop = False  'Closes the Server
        End If
        End
    End Sub
End Class
