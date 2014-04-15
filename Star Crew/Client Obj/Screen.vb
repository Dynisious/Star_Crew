﻿Public Class Screen
    Private Shared WithEvents Tick As New Timer With {.Interval = 10, .Enabled = False}
    Public Shared MyClient As Client
    Public Shared ReadOnly ImageSize As New Point(600, 600)

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
            If txtIP.Text <> "" Then
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
                    If Client.Connected = True Then
                        Dim temp As New GamePlayLayout
                    End If
                Else
                    Console.Beep()
                End If
            Else
                Console.Beep()
            End If
        End Sub

    End Class

    Public Class GamePlayLayout
        Public Shared Displaying As Boolean = False
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

        Public Sub New()
            Server.OutputScreen.Controls.Clear()
            '-----Initialize Controls-----
            picDisplayGraphics = New System.Windows.Forms.PictureBox()
            pnlDisplays = New System.Windows.Forms.Panel()
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
            lblCoreTemp = New System.Windows.Forms.Label()
            lblTempRate = New System.Windows.Forms.Label()
            pnlMenuButtons = New System.Windows.Forms.Panel()
            btnEndGame = New System.Windows.Forms.Button()
            btnMainMenu = New System.Windows.Forms.Button()
            UserKeyInterfacer = New System.Windows.Forms.Button()
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
            lblThrottle.Text = "Throttle: 0/0"
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
            pnlMenuButtons.Controls.Add(btnEndGame)
            pnlMenuButtons.Controls.Add(btnMainMenu)
            pnlMenuButtons.Location = New System.Drawing.Point(612, 350)
            pnlMenuButtons.Name = "pnlMenuButtons"
            pnlMenuButtons.Size = New System.Drawing.Size(560, 256)
            pnlMenuButtons.TabIndex = 3
            '
            'btnEndGame
            '
            btnEndGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            btnEndGame.Font = New System.Drawing.Font("Lucida Console", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            btnEndGame.Location = New System.Drawing.Point(285, 19)
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
            btnMainMenu.Location = New System.Drawing.Point(135, 19)
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
            '--------------------------

            '-----Add Controls-----
            Server.OutputScreen.Controls.Add(picDisplayGraphics)
            Server.OutputScreen.Controls.Add(pnlDisplays)
            Server.OutputScreen.Controls.Add(pnlMenuButtons)
            Server.OutputScreen.Controls.Add(UserKeyInterfacer)
            '----------------------

            '-----Display Controls-----
            CType(picDisplayGraphics, System.ComponentModel.ISupportInitialize).EndInit()
            pnlDisplays.ResumeLayout(False)
            pnlMenuButtons.ResumeLayout(False)
            '--------------------------
            UserKeyInterfacer.Focus()
            If Client.Connected = False Then
                Dim temp As New MenuScreenLayout
            Else
                Tick.Enabled = True
            End If
        End Sub

        Public Shared Sub btnMainMenu_Click() Handles btnMainMenu.Click
            Client.MyConnector.Close()
            Dim temp As New MenuScreenLayout
        End Sub

        Private Shared Sub btnEndGame_Click() Handles btnEndGame.Click
            End
        End Sub

        Private Shared Sub UserKeyInterfacer_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs) Handles UserKeyInterfacer.PreviewKeyDown
            Select Case Client.myMessage.Station
                Case Station.StationTypes.Helm
                    If e.KeyCode = Keys.Up Then 'Throttle Up
                        Client.SendCommand_Call(Helm.Commands.ThrottleUp, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Throttle Down
                        Client.SendCommand_Call(Helm.Commands.ThrottleDown, 1)
                    ElseIf e.KeyCode = Keys.Right Then 'Turn Right
                        Client.SendCommand_Call(Helm.Commands.TurnRight, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        Client.SendCommand_Call(Helm.Commands.TurnLeft, 1)
                    ElseIf e.KeyCode = Keys.J And e.Control = True Then 'Warp Drive
                        Client.SendCommand_Call(Helm.Commands.WarpDrive, 1)
                    ElseIf e.KeyCode = Keys.M Then 'Match Speed
                        Client.SendCommand_Call(Helm.Commands.MatchSpeed, 1)
                    End If
                Case Station.StationTypes.Batteries
                    If e.KeyCode = Keys.Right Then 'Turn Right
                        Client.SendCommand_Call(Battery.Commands.TurnRight, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        Client.SendCommand_Call(Battery.Commands.TurnLeft, 1)
                    ElseIf e.KeyCode = Keys.M Then 'Set Target
                        Client.SendCommand_Call(Battery.Commands.SetTarget, 1)
                    End If
                    If e.KeyCode = Keys.A Then 'Fire Primary
                        Client.SendCommand_Call(Battery.Commands.FirePrimary, 1)
                    End If
                    If e.KeyCode = Keys.D Then 'Fire Secondary
                        Client.SendCommand_Call(Battery.Commands.FireSecondary, 1)
                    End If
                Case Station.StationTypes.Shielding
                    If e.KeyCode = Keys.Up Then 'Boost Forward
                        Client.SendCommand_Call(Shields.Commands.BoostForward, 1)
                    ElseIf e.KeyCode = Keys.Right Then 'Boost Right
                        Client.SendCommand_Call(Shields.Commands.BoostRight, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Boost Back
                        Client.SendCommand_Call(Shields.Commands.BoostBack, 1)
                    ElseIf e.KeyCode = Keys.Left Then 'Boost Left
                        Client.SendCommand_Call(Shields.Commands.BoostLeft, 1)
                    End If
                Case Station.StationTypes.Engineering
                    If e.KeyCode = Keys.Up Then 'Heat
                        Client.SendCommand_Call(Engineering.Commands.Heat, 1)
                    ElseIf e.KeyCode = Keys.Down Then 'Cool
                        Client.SendCommand_Call(Engineering.Commands.Cool, 1)
                    End If
            End Select
        End Sub
        Private Shared Sub UserKeyInterfacer_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles UserKeyInterfacer.KeyUp
            Select Case Client.myMessage.Station
                Case Station.StationTypes.Helm
                    If e.KeyCode = Keys.Up Then 'Throttle Up
                        Client.SendCommand_Call(Helm.Commands.ThrottleUp, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Throttle Down
                        Client.SendCommand_Call(Helm.Commands.ThrottleDown, 0)
                    ElseIf e.KeyCode = Keys.Right Then 'Turn Right
                        Client.SendCommand_Call(Helm.Commands.TurnRight, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        Client.SendCommand_Call(Helm.Commands.TurnLeft, 0)
                    ElseIf e.KeyCode = Keys.J And e.Control = True Then 'Warp Drive
                        Client.SendCommand_Call(Helm.Commands.WarpDrive, 0)
                    ElseIf e.KeyCode = Keys.M Then 'Match Speed
                        Client.SendCommand_Call(Helm.Commands.MatchSpeed, 0)
                    End If
                Case Station.StationTypes.Batteries
                    If e.KeyCode = Keys.Right Then 'Turn Right
                        Client.SendCommand_Call(Battery.Commands.TurnRight, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Turn Left
                        Client.SendCommand_Call(Battery.Commands.TurnLeft, 0)
                    ElseIf e.KeyCode = Keys.M Then 'Set Target
                        Client.SendCommand_Call(Battery.Commands.SetTarget, 0)
                    End If
                    If e.KeyCode = Keys.A Then 'Fire Primary
                        Client.SendCommand_Call(Battery.Commands.FirePrimary, 0)
                    End If
                    If e.KeyCode = Keys.D Then 'Fire Secondary
                        Client.SendCommand_Call(Battery.Commands.FireSecondary, 0)
                    End If
                Case Station.StationTypes.Shielding
                    If e.KeyCode = Keys.Up Then 'Boost Forward
                        Client.SendCommand_Call(Shields.Commands.BoostForward, 0)
                    ElseIf e.KeyCode = Keys.Right Then 'Boost Right
                        Client.SendCommand_Call(Shields.Commands.BoostRight, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Boost Back
                        Client.SendCommand_Call(Shields.Commands.BoostBack, 0)
                    ElseIf e.KeyCode = Keys.Left Then 'Boost Left
                        Client.SendCommand_Call(Shields.Commands.BoostLeft, 0)
                    End If
                Case Station.StationTypes.Engineering
                    If e.KeyCode = Keys.Up Then 'Heat
                        Client.SendCommand_Call(Engineering.Commands.Heat, 0)
                    ElseIf e.KeyCode = Keys.Down Then 'Cool
                        Client.SendCommand_Call(Engineering.Commands.Cool, 0)
                    End If
            End Select
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
        Controls.Add(MenuScreenLayout.btnExit)
        Controls.Add(MenuScreenLayout.btnStartClient)
        Controls.Add(MenuScreenLayout.btnStartServer)
        Focus()
    End Sub

    Private Shared Sub UpdateScreen_Handle() Handles Tick.Tick
        If Client.Connected = True And Client.serversMessage IsNot Nothing Then
            Screen.GamePlayLayout.lblHull.Text = "Hull: " + CStr(Math.Round(Client.serversMessage.Ship.Hull.current, 2)) + "/" + CStr(Client.serversMessage.Ship.Hull.max)
            Screen.GamePlayLayout.lblThrottle.Text = "Throttle: " + CStr(CInt(Client.serversMessage.Ship.Helm.Throttle.current)) + "/" + CStr(CInt(Client.serversMessage.Ship.Helm.Throttle.max))

            Screen.GamePlayLayout.lblForward.Text = "Fore: " + CStr(CInt(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.FrontShield).current)) + "/" + CStr(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.FrontShield).max)
            Screen.GamePlayLayout.lblRight.Text = "Starboard: " + CStr(CInt(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.RightShield).current)) + "/" + CStr(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.RightShield).max)
            Screen.GamePlayLayout.lblRear.Text = "Aft: " + CStr(CInt(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.BackShield).current)) + "/" + CStr(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.BackShield).max)
            Screen.GamePlayLayout.lblLeft.Text = "Port: " + CStr(CInt(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.LeftShield).current)) + "/" + CStr(Client.serversMessage.Ship.Shielding.ShipShields(Shields.Sides.LeftShield).max)
            Select Case Client.serversMessage.Ship.Shielding.LastHit
                Case Shields.Sides.FrontShield
                    Screen.GamePlayLayout.lblForward.BackColor = Color.LightBlue
                    Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                Case Shields.Sides.RightShield
                    Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRight.BackColor = Color.LightBlue
                    Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                Case Shields.Sides.BackShield
                    Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRear.BackColor = Color.LightBlue
                    Screen.GamePlayLayout.lblLeft.BackColor = Color.Transparent
                Case Shields.Sides.LeftShield
                    Screen.GamePlayLayout.lblForward.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRight.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblRear.BackColor = Color.Transparent
                    Screen.GamePlayLayout.lblLeft.BackColor = Color.LightBlue
            End Select
            Screen.GamePlayLayout.lblPrimary.Text = "Primary: " + CStr(Client.serversMessage.Ship.Batteries.Primary.Integrety.current) + "/" + CStr(Client.serversMessage.Ship.Batteries.Primary.Integrety.max)
            Screen.GamePlayLayout.lblSecondary.Text = "Secondary: " + CStr(Client.serversMessage.Ship.Batteries.Secondary.Integrety.current) + "/" + CStr(Client.serversMessage.Ship.Batteries.Secondary.Integrety.max)
            Screen.GamePlayLayout.lblPowerCore.Text = "Power Core: " + CStr(Client.serversMessage.Ship.Engineering.PowerCore.current) + "/" + CStr(Client.serversMessage.Ship.Engineering.PowerCore.max)
            Screen.GamePlayLayout.lblEngines.Text = "Engines: " + CStr(Client.serversMessage.Ship.Engineering.Engines.current) + "/" + CStr(Client.serversMessage.Ship.Engineering.Engines.max)
            Screen.GamePlayLayout.lblCoreTemp.Text = "Core Temp: " + CStr(Math.Round(Client.serversMessage.Ship.Engineering.Heat, 2)) + "*e^5/100*e^5"
            Screen.GamePlayLayout.lblTempRate.Text = "Temp Rate: " + CStr(Math.Round(Client.serversMessage.Ship.Engineering.Rate, 2)) + "*e^5"
        ElseIf Client.Connected = False Then
            Dim temp As New MenuScreenLayout
            Tick.Enabled = False
        End If
    End Sub

    Private Sub Screen_FormClosing(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        Server.comms.Abort()
        End
    End Sub
End Class