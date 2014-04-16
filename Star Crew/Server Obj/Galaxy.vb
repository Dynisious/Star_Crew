Public Class Galaxy
    Public Shared WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Public Shared centerShip As Ship
    Public Shared ShipCount As Integer = 50
    Public Shared xList(-1) As Ship
    Private Shared shipPositions(-1) As GraphicPosition
    Public Enum Warp
        None
        Entering
        Exiting
        Warping
    End Enum
    Public Shared Warping As Warp = Warp.None

    Private Shared Event StartGame()
    Public Shared Sub StartGame_Call()
        RaiseEvent StartGame()
    End Sub
    Public Shared Sub StartGame_Handle() Handles Me.StartGame
        Warping = Warp.None
        ReDim xList(ShipCount - 1)
        Randomize()

        centerShip = New FriendlyShip(New Clunker, 0)
        xList(0) = centerShip
        xList(0).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        xList(1) = New PirateShip(New Clunker, 1)
        xList(1).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        For i As Integer = 2 To UBound(xList)
            If Int(2 * Rnd()) = 0 Then
                xList(i) = New FriendlyShip(New Clunker, i)
            Else
                xList(i) = New PirateShip(New Clunker, i)
            End If
            xList(i).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        Next
        GalaxyTimer.Enabled = True
    End Sub

    Private Shared Event RunCommand(ByVal clientCommand As ClientMessage)
    Public Shared Sub RunCommand_Call(ByVal clientCommand As ClientMessage)
        RaiseEvent RunCommand(clientCommand)
    End Sub
    Private Shared Sub RunCommand_Handle(ByVal clientCommand As ClientMessage) Handles Me.RunCommand
        Select Case clientCommand.Station
            Case Star_Crew.Station.StationTypes.Helm
                Select Case clientCommand.Command
                    Case Helm.Commands.ThrottleUp
                        If clientCommand.Value = 1 Then
                            PlayerControl.ThrottleUpCheck = True
                            PlayerControl.ThrottleDownCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ThrottleUpCheck = False
                        End If
                    Case Helm.Commands.ThrottleDown
                        If clientCommand.Value = 1 Then
                            PlayerControl.ThrottleDownCheck = True
                            PlayerControl.ThrottleUpCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ThrottleDownCheck = False
                        End If
                    Case Helm.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.TurnRightCheck = True
                            PlayerControl.TurnLeftCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.TurnRightCheck = False
                        End If
                    Case Helm.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.TurnLeftCheck = True
                            PlayerControl.TurnRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.TurnLeftCheck = False
                        End If
                    Case Helm.Commands.WarpDrive
                        If clientCommand.Value = 1 Then
                            PlayerControl.WarpDriveCheck = True
                            PlayerControl.MatchSpeedCheck = False
                            PlayerControl.ThrottleUpCheck = False
                            PlayerControl.ThrottleDownCheck = False
                            PlayerControl.TurnLeftCheck = False
                            PlayerControl.TurnRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.WarpDriveCheck = False
                        End If
                    Case Helm.Commands.MatchSpeed
                        If clientCommand.Value = 1 Then
                            PlayerControl.MatchSpeedCheck = True
                            PlayerControl.WarpDriveCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.MatchSpeedCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Batteries
                Select Case clientCommand.Command
                    Case Battery.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.RotateRightCheck = True
                            PlayerControl.RotateLeftCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RotateRightCheck = False
                        End If
                    Case Battery.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.RotateLeftCheck = True
                            PlayerControl.RotateRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RotateLeftCheck = False
                        End If
                    Case Battery.Commands.FirePrimary
                        If clientCommand.Value = 1 Then
                            PlayerControl.FirePrimaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.FirePrimaryCheck = False
                        End If
                    Case Battery.Commands.FireSecondary
                        If clientCommand.Value = 1 Then
                            PlayerControl.FireSecondaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.FireSecondaryCheck = False
                        End If
                    Case Battery.Commands.SetTarget
                        If clientCommand.Value = 1 Then
                            PlayerControl.SelectTargetCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.SelectTargetCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Shielding
                Select Case clientCommand.Command
                    Case Shields.Commands.BoostForward
                        If clientCommand.Value = 1 Then
                            PlayerControl.ForwardBoostCheck = True
                            PlayerControl.RightBoostCheck = False
                            PlayerControl.RearBoostCheck = False
                            PlayerControl.LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ForwardBoostCheck = False
                        End If
                    Case Shields.Commands.BoostRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.ForwardBoostCheck = False
                            PlayerControl.RightBoostCheck = True
                            PlayerControl.RearBoostCheck = False
                            PlayerControl.LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RightBoostCheck = False
                        End If
                    Case Shields.Commands.BoostBack
                        If clientCommand.Value = 1 Then
                            PlayerControl.ForwardBoostCheck = False
                            PlayerControl.RightBoostCheck = False
                            PlayerControl.RearBoostCheck = True
                            PlayerControl.LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RearBoostCheck = False
                        End If
                    Case Shields.Commands.BoostLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.ForwardBoostCheck = False
                            PlayerControl.RightBoostCheck = False
                            PlayerControl.RearBoostCheck = False
                            PlayerControl.LeftBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.LeftBoostCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Engineering
                Select Case clientCommand.Command
                    Case Engineering.Commands.Heat
                        If clientCommand.Value = 1 Then
                            PlayerControl.HeatCheck = True
                            PlayerControl.CoolCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.HeatCheck = False
                        End If
                    Case Engineering.Commands.Cool
                        If clientCommand.Value = 1 Then
                            PlayerControl.CoolCheck = True
                            PlayerControl.HeatCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.CoolCheck = False
                        End If
                End Select
        End Select
    End Sub
    Private Class PlayerControl
        '-----Helm-----
        Public Shared ThrottleUpCheck As Boolean = False
        Public Shared ThrottleDownCheck As Boolean = False
        Public Shared TurnRightCheck As Boolean = False
        Public Shared TurnLeftCheck As Boolean = False
        Public Shared WarpDriveCheck As Boolean = False
        Public Shared MatchSpeedCheck As Boolean = False
        Private Shared Sub ThrottleUp()
            If ThrottleUpCheck = True Then
                centerShip.Helm.MatchSpeed = False
                centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current + centerShip.Helm.Acceleration.current
                If centerShip.Helm.Throttle.current > centerShip.Helm.Throttle.max Then
                    centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.max
                End If
            End If
        End Sub
        Private Shared Sub ThrottleDown()
            If ThrottleDownCheck = True Then
                centerShip.Helm.MatchSpeed = False
                centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current - centerShip.Helm.Acceleration.current
                If centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                    centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                End If
            End If
        End Sub
        Private Shared Sub TurnRight()
            If TurnRightCheck = True Then
                centerShip.Helm.Direction = Helm.NormalizeDirection(centerShip.Helm.Direction + centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub TurnLeft()
            If TurnLeftCheck = True Then
                centerShip.Helm.Direction = Helm.NormalizeDirection(centerShip.Helm.Direction - centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub WarpDrive()
            If WarpDriveCheck = True Then
                If centerShip.Helm.Throttle.current > Helm.MinimumSpeed Then
                    centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current - centerShip.Helm.Acceleration.current
                    If centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                        centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                    End If
                End If
                If centerShip.Helm.Direction < Math.PI Then
                    centerShip.Helm.Direction = centerShip.Helm.Direction - centerShip.Helm.TurnSpeed.current
                    If centerShip.Helm.Direction < 0 Then
                        centerShip.Helm.Direction = 0
                    End If
                ElseIf centerShip.Helm.Direction > Math.PI Then
                    centerShip.Helm.Direction = centerShip.Helm.Direction + centerShip.Helm.TurnSpeed.current
                    If centerShip.Helm.Direction > 2 * Math.PI Then
                        centerShip.Helm.Direction = 0
                    End If
                End If
                If Galaxy.centerShip.Helm.Throttle.current = Helm.MinimumSpeed And
                    Galaxy.Warping <> Galaxy.Warp.Warping And
                    Galaxy.centerShip.Helm.Direction = 0 Then
                    Galaxy.Warping = Galaxy.Warp.Entering
                End If
            End If
        End Sub
        Private Shared Sub MatchSpeed()
            If MatchSpeedCheck = True Then
                centerShip.Helm.MatchSpeed = True
            End If
        End Sub
        '--------------

        '-----Batteries-----
        Public Shared RotateRightCheck As Boolean = False
        Public Shared RotateLeftCheck As Boolean = False
        Public Shared FirePrimaryCheck As Boolean = False
        Public Shared FireSecondaryCheck As Boolean = False
        Public Shared SelectTargetCheck As Boolean = False
        Private Shared Sub RotateRight()
            If RotateRightCheck = True Then
                '-----Primary-----
                centerShip.Batteries.Primary.TurnDistance.current =
                    centerShip.Batteries.Primary.TurnDistance.current +
                    centerShip.Batteries.Primary.TurnSpeed.current
                If centerShip.Batteries.Primary.TurnDistance.current > centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                    centerShip.Batteries.Primary.TurnDistance.current = centerShip.Batteries.Primary.TurnDistance.max / 2
                End If
                '-----------------
                If centerShip.Batteries.Primary.TurnDistance.current > centerShip.Batteries.Secondary.TurnDistance.current Then
                    '-----Secondary-----
                    centerShip.Batteries.Secondary.TurnDistance.current =
                        centerShip.Batteries.Secondary.TurnDistance.current +
                        centerShip.Batteries.Secondary.TurnSpeed.current
                    If centerShip.Batteries.Secondary.TurnDistance.current > centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                        centerShip.Batteries.Secondary.TurnDistance.current = centerShip.Batteries.Secondary.TurnDistance.max / 2
                    End If
                    '-------------------
                End If
            End If
        End Sub
        Private Shared Sub RotateLeft()
            If RotateLeftCheck = True Then
                '-----Primary-----
                centerShip.Batteries.Primary.TurnDistance.current =
                    centerShip.Batteries.Primary.TurnDistance.current -
                    centerShip.Batteries.Primary.TurnSpeed.current
                If centerShip.Batteries.Primary.TurnDistance.current < -centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                    centerShip.Batteries.Primary.TurnDistance.current = -centerShip.Batteries.Primary.TurnDistance.max / 2
                End If
                '-----------------
                If centerShip.Batteries.Primary.TurnDistance.current < centerShip.Batteries.Secondary.TurnDistance.current Then
                    '-----Secondary-----
                    centerShip.Batteries.Secondary.TurnDistance.current =
                        centerShip.Batteries.Secondary.TurnDistance.current -
                        centerShip.Batteries.Secondary.TurnSpeed.current
                    If centerShip.Batteries.Secondary.TurnDistance.current < -centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                        centerShip.Batteries.Secondary.TurnDistance.current = -centerShip.Batteries.Secondary.TurnDistance.max / 2
                    End If
                    '-------------------
                End If
            End If
        End Sub
        Private Shared Sub FirePrimary()
            If FirePrimaryCheck = True Then
                For i As Integer = 0 To xList.Length - 1
                    Dim adjacent As Integer = xList(i).Position.X - centerShip.Position.X
                    Dim opposite As Integer = xList(i).Position.Y - centerShip.Position.Y
                    Dim distance = Math.Sqrt((opposite ^ 2) + (adjacent ^ 2))
                    Dim direction As Double
                    If adjacent <> 0 Then
                        direction = Math.Tanh(opposite / adjacent)
                        If adjacent < 0 Then
                            direction = direction + Math.PI
                        End If
                        direction = Helm.NormalizeDirection(direction)
                    ElseIf opposite > 0 Then
                        direction = Math.PI / 2
                    Else
                        direction = (3 * Math.PI) / 2
                    End If

                    If distance < centerShip.Batteries.Primary.Range.current And distance <> 0 Then
                        If direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current < Battery.PlayerArc / 2 And
                            direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                            centerShip.Batteries.Primary.FireWeapon(distance, xList(i))
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub FireSecondary()
            If FireSecondaryCheck = True Then
                For i As Integer = 0 To xList.Length - 1
                    If xList(i).MyAllegence <> Ship.Allegence.Player Then
                        Dim adjacent As Integer = xList(i).Position.X - centerShip.Position.X
                        Dim opposite As Integer = xList(i).Position.Y - centerShip.Position.Y
                        Dim distance = Math.Sqrt((opposite ^ 2) + (adjacent ^ 2))
                        Dim direction As Double
                        If adjacent <> 0 Then
                            direction = Math.Tanh(opposite / adjacent)
                            If adjacent < 0 Then
                                direction = direction + Math.PI
                            End If
                            direction = Helm.NormalizeDirection(direction)
                        ElseIf opposite > 0 Then
                            direction = Math.PI / 2
                        Else
                            direction = (3 * Math.PI) / 2
                        End If

                        If distance < centerShip.Batteries.Secondary.Range.current And distance <> 0 Then
                            If direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current < Battery.PlayerArc / 2 And
                                direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                                centerShip.Batteries.Secondary.FireWeapon(distance, xList(i))
                                Exit Sub
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub SelectTarget()
            If SelectTargetCheck = True Then
                Dim lastDistance As Integer
                For i As Integer = 0 To xList.Length - 1
                    If xList(i).MyAllegence <> Ship.Allegence.Player Then
                        Dim distance As Integer = Math.Sqrt(((xList(i).Position.X - centerShip.Position.X) ^ 2) + ((xList(i).Position.Y - centerShip.Position.Y) ^ 2))
                        If (distance < lastDistance And distance <> 0) Or lastDistance = 0 Then
                            lastDistance = distance
                            centerShip.Helm.Target = xList(i)
                        End If
                    End If
                Next
            End If
        End Sub
        '-------------------

        '-----Shielding-----
        Public Shared ForwardBoostCheck As Boolean = False
        Public Shared RightBoostCheck As Boolean = False
        Public Shared RearBoostCheck As Boolean = False
        Public Shared LeftBoostCheck As Boolean = False
        Private Shared Sub ForwardBoost()
            If ForwardBoostCheck = True Then
                centerShip.Shielding.LastHit = Shields.Sides.FrontShield
            End If
        End Sub
        Private Shared Sub RightBoost()
            If RightBoostCheck = True Then
                centerShip.Shielding.LastHit = Shields.Sides.RightShield
            End If
        End Sub
        Private Shared Sub RearBoost()
            If RearBoostCheck = True Then
                centerShip.Shielding.LastHit = Shields.Sides.BackShield
            End If
        End Sub
        Private Shared Sub LeftBoost()
            If LeftBoostCheck = True Then
                centerShip.Shielding.LastHit = Shields.Sides.LeftShield
            End If
        End Sub
        '-------------------

        '-----Engineering-----
        Public Shared HeatCheck As Boolean = False
        Public Shared CoolCheck As Boolean = False
        Private Shared Sub Heat()
            If HeatCheck = True Then
                centerShip.Engineering.Rate = centerShip.Engineering.Rate + (Engineering.RateOfChange * 10)
            End If
        End Sub
        Private Shared Sub Cool()
            If CoolCheck = True Then
                centerShip.Engineering.Rate = centerShip.Engineering.Rate - (Engineering.RateOfChange * 10)
            End If
        End Sub
        '---------------------

        Public Shared Sub RunPlayerControls()
            '-----Helm-----
            ThrottleUp()
            ThrottleDown()
            TurnRight()
            TurnLeft()
            WarpDrive()
            MatchSpeed()
            '--------------
            '-----Batteries-----
            RotateRight()
            RotateLeft()
            FirePrimary()
            FireSecondary()
            SelectTarget()
            '-------------------
            '-----Shielding-----
            ForwardBoost()
            RightBoost()
            RearBoost()
            LeftBoost()
            '-------------------
            '-----Engineering-----
            Heat()
            Cool()
            '---------------------
        End Sub
    End Class

    Public Shared Sub RemoveShip(ByRef nShip As Ship)
        For i As Integer = nShip.Index To UBound(xList)
            If i <> UBound(xList) Then
                xList(i) = xList(i + 1)
                xList(i).Index = i
            End If
        Next
        ReDim Preserve xList(UBound(xList) - 1)
    End Sub

    Public Shared Sub Recenter()
        For Each i As Ship In xList
            If i.MyAllegence = Ship.Allegence.Player Then
                centerShip = i
                For Each e As ServerSideClient In Server.ServerComms.Ports
                    Select Case e.MyStation
                        Case Station.StationTypes.Helm
                            centerShip.Helm.PlayerControled = True
                        Case Station.StationTypes.Batteries
                            centerShip.Batteries.PlayerControled = True
                        Case Station.StationTypes.Shielding
                            centerShip.Batteries.PlayerControled = True
                        Case Station.StationTypes.Engineering
                            centerShip.Engineering.PlayerControled = True
                    End Select
                Next
                Exit Sub
            End If
        Next
        GalaxyTimer.Enabled = False
        Console.WriteLine("Player is Defeated")
    End Sub

    Public Shared Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        Dim friendly As Integer
        Dim enemy As Integer
        PlayerControl.RunPlayerControls()
        Ship.UpdateShip_Call()
        For Each i As Ship In xList
            If i.MyAllegence = Ship.Allegence.Player Then
                friendly = friendly + 1
            ElseIf i.MyAllegence = Ship.Allegence.Pirate Then
                enemy = enemy + 1
            End If
        Next

        '-----Set Warp-----
        Select Case Warping
            Case Warp.Entering
                Warping = Warp.Warping
            Case Warp.Exiting
                Warping = Warp.None
        End Select
        ReDim shipPositions(UBound(xList))
        '------------------

        '-----Set new positions-----
        For i As Integer = 0 To UBound(xList)
            Dim x As Integer = xList(i).Position.X - centerShip.Position.X
            Dim y As Integer = xList(i).Position.Y - centerShip.Position.Y
            Dim col As Color
            Select Case xList(i).MyAllegence
                Case Ship.Allegence.Player
                    col = Color.Green
                Case Ship.Allegence.Pirate
                    col = Color.Red
            End Select
            shipPositions(i) = New GraphicPosition(col, xList(i).Hit, xList(i).Firing, x, y, xList(i).Helm.Direction)
        Next
        '---------------------------
        Server.ServerComms.UpdateServerMessage_Call(centerShip, shipPositions, Warping)
    End Sub

End Class
