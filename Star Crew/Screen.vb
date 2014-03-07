Public Class Screen
    Private WithEvents tick As New Timer With {.Interval = 10, .Enabled = False}
    Public Shared MyClient As Client

    Public Class MenuScreenLayout
        Public Shared WithEvents btnStartServer As System.Windows.Forms.Button
        Public Shared WithEvents btnStartClient As System.Windows.Forms.Button
        Public Shared WithEvents btnExit As System.Windows.Forms.Button

        Public Sub New()
            Server.OutputScreen.Controls.Clear()
            btnStartServer = New System.Windows.Forms.Button()
            btnStartClient = New System.Windows.Forms.Button()
            btnExit = New System.Windows.Forms.Button()
            '
            'btnStartServer
            '
            btnStartServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnStartServer.Location = New System.Drawing.Point(400, 100)
            btnStartServer.Name = "btnStartServer"
            btnStartServer.Size = New System.Drawing.Size(400, 70)
            btnStartServer.TabIndex = 0
            btnStartServer.Text = "Open Server"
            btnStartServer.UseVisualStyleBackColor = True
            '
            'btnStartClient
            '
            btnStartClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
            btnStartClient.Location = New System.Drawing.Point(400, 273)
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
            btnExit.Location = New System.Drawing.Point(400, 446)
            btnExit.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
            btnExit.Name = "btnExit"
            btnExit.Size = New System.Drawing.Size(400, 70)
            btnExit.TabIndex = 2
            btnExit.Text = "Exit"
            btnExit.UseVisualStyleBackColor = True
            Server.OutputScreen.Controls.Add(btnExit)
            Server.OutputScreen.Controls.Add(btnStartClient)
            Server.OutputScreen.Controls.Add(btnStartServer)
        End Sub

        Private Shared Sub btnStartServer_Click() Handles btnStartServer.Click
            Server.StartServer()
        End Sub

        Private Shared Sub btnStartClient_Click() Handles btnStartClient.Click
            Dim temp As New ClientSetupLayout()
        End Sub

        Private Shared Sub btnExit_Click() Handles btnExit.Click
            Server.comms.Abort()
            If MyClient IsNot Nothing Then
                MyClient.comms.Abort()
            End If
            End
        End Sub

    End Class

    Public Class ClientSetupLayout
        Public Shared WithEvents txtIP As System.Windows.Forms.TextBox
        Public Shared WithEvents lblIP As System.Windows.Forms.Label
        Public Shared WithEvents btnEnter As System.Windows.Forms.Button
        Public Shared WithEvents btnMenu As System.Windows.Forms.Button
        Public Shared WithEvents DomainUpDown1 As System.Windows.Forms.DomainUpDown

        Public Sub New()
            Server.OutputScreen.Controls.Clear()
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
            DomainUpDown1.Location = New System.Drawing.Point(500, 150)
            DomainUpDown1.Name = "DomainUpDown1"
            DomainUpDown1.Size = New System.Drawing.Size(200, 23)
            DomainUpDown1.TabIndex = 4

            Server.OutputScreen.Controls.Add(DomainUpDown1)
            Server.OutputScreen.Controls.Add(btnMenu)
            Server.OutputScreen.Controls.Add(btnEnter)
            Server.OutputScreen.Controls.Add(lblIP)
            Server.OutputScreen.Controls.Add(txtIP)
        End Sub

        Private Shared Sub btnMenu_Click() Handles btnMenu.Click
            Dim temp As New MenuScreenLayout
        End Sub

        Private Shared Sub btnEnter_Click() Handles btnEnter.Click
            Dim count As Integer
            Dim lastIndex As Integer
            While True
                Dim e As Integer = txtIP.Text.IndexOf(".", lastIndex + 1)
                If e <> -1 And e < txtIP.TextLength Then
                    lastIndex = e
                    count = count + 1
                Else
                    Exit While
                End If
            End While

            If count = 3 And DomainUpDown1.SelectedIndex <> -1 Then
                MyClient = New Client(txtIP.Text, DomainUpDown1.SelectedIndex)
                If MyClient.connected = True Then
                    Dim temp As New GamePlayLayout
                End If
            End If
        End Sub

    End Class

    Public Class GamePlayLayout
        Public Shared Displaying As Boolean = False
        Public Shared WithEvents picDisplay As System.Windows.Forms.PictureBox
        Public Shared WithEvents pnlStats As System.Windows.Forms.Panel
        Public Shared WithEvents pnlControls As System.Windows.Forms.Panel
        Public Shared WithEvents lblHull As System.Windows.Forms.Label
        Public Shared WithEvents lblLeftShield As System.Windows.Forms.Label
        Public Shared WithEvents lblRightShield As System.Windows.Forms.Label
        Public Shared WithEvents lblForwardShield As System.Windows.Forms.Label
        Public Shared WithEvents lblRearShield As System.Windows.Forms.Label
        Public Shared WithEvents lblPrimary As System.Windows.Forms.Label
        Public Shared WithEvents lblSecondary As System.Windows.Forms.Label
        Public Shared WithEvents lblPowerCore As System.Windows.Forms.Label
        Public Shared WithEvents lblEngines As System.Windows.Forms.Label

        Public Sub New()
            Server.OutputScreen.Controls.Clear()
            Displaying = True
            picDisplay = New System.Windows.Forms.PictureBox()
            pnlStats = New System.Windows.Forms.Panel()
            pnlControls = New System.Windows.Forms.Panel()
            lblHull = New System.Windows.Forms.Label()
            lblLeftShield = New System.Windows.Forms.Label()
            lblRightShield = New System.Windows.Forms.Label()
            lblForwardShield = New System.Windows.Forms.Label()
            lblRearShield = New System.Windows.Forms.Label()
            lblPrimary = New System.Windows.Forms.Label()
            lblSecondary = New System.Windows.Forms.Label()
            lblPowerCore = New System.Windows.Forms.Label()
            lblEngines = New System.Windows.Forms.Label()
            CType(picDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
            pnlStats.SuspendLayout()
            '
            'picDisplay
            '
            picDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            picDisplay.Location = New System.Drawing.Point(12, 12)
            picDisplay.Name = "picDisplay"
            picDisplay.Size = New System.Drawing.Size(600, 600)
            picDisplay.TabIndex = 0
            picDisplay.TabStop = False
            '
            'pnlStats
            '
            pnlStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            pnlStats.Controls.Add(lblEngines)
            pnlStats.Controls.Add(lblPowerCore)
            pnlStats.Controls.Add(lblSecondary)
            pnlStats.Controls.Add(lblPrimary)
            pnlStats.Controls.Add(lblRearShield)
            pnlStats.Controls.Add(lblForwardShield)
            pnlStats.Controls.Add(lblRightShield)
            pnlStats.Controls.Add(lblLeftShield)
            pnlStats.Controls.Add(lblHull)
            pnlStats.Location = New System.Drawing.Point(618, 12)
            pnlStats.Name = "pnlStats"
            pnlStats.Size = New System.Drawing.Size(554, 320)
            pnlStats.TabIndex = 1
            '
            'pnlControls
            '
            pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            pnlControls.Location = New System.Drawing.Point(618, 338)
            pnlControls.Name = "pnlControls"
            pnlControls.Size = New System.Drawing.Size(554, 274)
            pnlControls.TabIndex = 2
            '
            'lblHull
            '
            lblHull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblHull.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblHull.Location = New System.Drawing.Point(3, 5)
            lblHull.Name = "lblHull"
            lblHull.Size = New System.Drawing.Size(166, 27)
            lblHull.TabIndex = 0
            lblHull.Text = "Hull: 0/0"
            lblHull.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblLeftShield
            '
            lblLeftShield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblLeftShield.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblLeftShield.Location = New System.Drawing.Point(17, 228)
            lblLeftShield.Name = "lblLeftShield"
            lblLeftShield.Size = New System.Drawing.Size(166, 27)
            lblLeftShield.TabIndex = 1
            lblLeftShield.Text = "Left: 0/0"
            lblLeftShield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblRightShield
            '
            lblRightShield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblRightShield.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblRightShield.Location = New System.Drawing.Point(189, 228)
            lblRightShield.Name = "lblRightShield"
            lblRightShield.Size = New System.Drawing.Size(166, 27)
            lblRightShield.TabIndex = 2
            lblRightShield.Text = "Right: 0/0"
            lblRightShield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblForwardShield
            '
            lblForwardShield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblForwardShield.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblForwardShield.Location = New System.Drawing.Point(103, 182)
            lblForwardShield.Name = "lblForwardShield"
            lblForwardShield.Size = New System.Drawing.Size(166, 27)
            lblForwardShield.TabIndex = 3
            lblForwardShield.Text = "Forward: 0/0"
            lblForwardShield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblRearShield
            '
            lblRearShield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblRearShield.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblRearShield.Location = New System.Drawing.Point(103, 280)
            lblRearShield.Name = "lblRearShield"
            lblRearShield.Size = New System.Drawing.Size(166, 27)
            lblRearShield.TabIndex = 4
            lblRearShield.Text = "Rear: 0/0"
            lblRearShield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblPrimary
            '
            lblPrimary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblPrimary.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblPrimary.Location = New System.Drawing.Point(114, 69)
            lblPrimary.Name = "lblPrimary"
            lblPrimary.Size = New System.Drawing.Size(166, 27)
            lblPrimary.TabIndex = 5
            lblPrimary.Text = "Primary: 0/0"
            lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblSecondary
            '
            lblSecondary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblSecondary.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblSecondary.Location = New System.Drawing.Point(114, 106)
            lblSecondary.Name = "lblSecondary"
            lblSecondary.Size = New System.Drawing.Size(166, 27)
            lblSecondary.TabIndex = 6
            lblSecondary.Text = "Secondary: 0/0"
            lblSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblPowerCore
            '
            lblPowerCore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblPowerCore.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblPowerCore.Location = New System.Drawing.Point(327, 69)
            lblPowerCore.Name = "lblPowerCore"
            lblPowerCore.Size = New System.Drawing.Size(166, 27)
            lblPowerCore.TabIndex = 7
            lblPowerCore.Text = "Power Core: 0/0"
            lblPowerCore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblEngines
            '
            lblEngines.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            lblEngines.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
            lblEngines.Location = New System.Drawing.Point(327, 106)
            lblEngines.Name = "lblEngines"
            lblEngines.Size = New System.Drawing.Size(166, 27)
            lblEngines.TabIndex = 8
            lblEngines.Text = "Engines: 0/0"
            lblEngines.TextAlign = System.Drawing.ContentAlignment.MiddleCenter

            Server.OutputScreen.Controls.Add(Me.pnlControls)
            Server.OutputScreen.Controls.Add(Me.pnlStats)
            Server.OutputScreen.Controls.Add(Me.picDisplay)
            CType(Me.picDisplay, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlStats.ResumeLayout(False)
            Server.OutputScreen.tick.Enabled = True
        End Sub

    End Class

    Public Sub Open()
        Application.Run(Me)
    End Sub

    Public Sub New()
        InitializeComponent()
        MenuScreenLayout.btnStartServer = New System.Windows.Forms.Button()
        MenuScreenLayout.btnStartClient = New System.Windows.Forms.Button()
        MenuScreenLayout.btnExit = New System.Windows.Forms.Button()
        '
        'btnStartServer
        '
        MenuScreenLayout.btnStartServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnStartServer.Location = New System.Drawing.Point(400, 100)
        MenuScreenLayout.btnStartServer.Name = "btnStartServer"
        MenuScreenLayout.btnStartServer.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnStartServer.TabIndex = 0
        MenuScreenLayout.btnStartServer.Text = "Open Server"
        MenuScreenLayout.btnStartServer.UseVisualStyleBackColor = True
        '
        'btnStartClient
        '
        MenuScreenLayout.btnStartClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        MenuScreenLayout.btnStartClient.Location = New System.Drawing.Point(400, 273)
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
        MenuScreenLayout.btnExit.Location = New System.Drawing.Point(400, 446)
        MenuScreenLayout.btnExit.Margin = New System.Windows.Forms.Padding(3, 100, 3, 100)
        MenuScreenLayout.btnExit.Name = "btnExit"
        MenuScreenLayout.btnExit.Size = New System.Drawing.Size(400, 70)
        MenuScreenLayout.btnExit.TabIndex = 2
        MenuScreenLayout.btnExit.Text = "Exit"
        MenuScreenLayout.btnExit.UseVisualStyleBackColor = True
        Me.Controls.Add(MenuScreenLayout.btnExit)
        Me.Controls.Add(MenuScreenLayout.btnStartClient)
        Me.Controls.Add(MenuScreenLayout.btnStartServer)
    End Sub

    Private Sub DisplayStats() Handles tick.Tick
        If GamePlayLayout.Displaying = True And MyClient.Message IsNot Nothing Then
            Screen.GamePlayLayout.picDisplay.Image = MyClient.Message.bmp
            Screen.GamePlayLayout.lblHull.Text = "Hull: " + CStr(MyClient.Message.ship.Hull.current) + "/" + CStr(MyClient.Message.ship.Hull.max)
            Screen.GamePlayLayout.lblForwardShield.Text = "Forward: " + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.FrontShield).current) + "/" + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.FrontShield).max)
            Screen.GamePlayLayout.lblRightShield.Text = "Right: " + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.RightShield).current) + "/" + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.RightShield).max)
            Screen.GamePlayLayout.lblRearShield.Text = "Rear: " + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.BackShield).current) + "/" + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.BackShield).max)
            Screen.GamePlayLayout.lblLeftShield.Text = "Left: " + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.LeftShield).current) + "/" + CStr(MyClient.Message.ship.Shielding.ShipShields(Shields.Sides.LeftShield).max)
            Screen.GamePlayLayout.lblPrimary.Text = "Primary: " + CStr(MyClient.Message.ship.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current) + "/" + CStr(MyClient.Message.ship.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).max)
            Screen.GamePlayLayout.lblSecondary.Text = "Secondary: " + CStr(MyClient.Message.ship.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current) + "/" + CStr(MyClient.Message.ship.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).max)
            Screen.GamePlayLayout.lblPowerCore.Text = "Power Core: " + CStr(MyClient.Message.ship.Engineering.PowerCore.current) + "/" + CStr(MyClient.Message.ship.Engineering.PowerCore.max)
            Screen.GamePlayLayout.lblEngines.Text = "Engines: " + CStr(MyClient.Message.ship.Engineering.Engines.current) + "/" + CStr(MyClient.Message.ship.Engineering.Engines.max)
        End If
    End Sub

    Private Sub Screen_FormClosing(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        Server.comms.Abort()
        If MyClient IsNot Nothing Then
            MyClient.comms.Abort()
        End If
        End
    End Sub
End Class
