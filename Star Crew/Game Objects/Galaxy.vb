Public Class Galaxy
    Public Shared WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Private Shared craftPositions(-1) As GraphicPosition
    Public Shared centerSector As Sector
    Private Shared SectorList(0) As Sector
    Public Enum Warp
        None
        Entering
        Warping
    End Enum
    Public Shared Warping As Warp = Warp.None
    Public Shared WarpCounter As Integer
    Public Enum Scenario
        Battle
        Transit
    End Enum
    Public Shared State As Scenario
    Public Enum Allegence
        Player
        Pirate
    End Enum

    Public Shared Event StartGame()
    Public Shared Sub StartGame_Call()
        RaiseEvent StartGame()
    End Sub
    Public Shared Sub StartGame_Handle() Handles Me.StartGame
        Randomize()
        Warping = Warp.None
        SectorList(0) = New Sector(1)
        Sector.centerFleet = New FriendlyFleet(-1)
        SectorList(0).AddFleet(Sector.centerFleet)
        centerSector = SectorList(0)
        Combat.Generate(centerSector.fleetList(0))
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
    Public Class PlayerControl
        '-----Helm-----
        Public Shared ThrottleUpCheck As Boolean = False
        Public Shared ThrottleDownCheck As Boolean = False
        Public Shared TurnRightCheck As Boolean = False
        Public Shared TurnLeftCheck As Boolean = False
        Public Shared WarpDriveCheck As Boolean = False
        Public Shared MatchSpeedCheck As Boolean = False
        Private Shared Sub ThrottleUp()
            If ThrottleUpCheck = True Then
                Combat.centerShip.Helm.MatchSpeed = False
                Combat.centerShip.Helm.Throttle.current = Combat.centerShip.Helm.Throttle.current + Combat.centerShip.Helm.Acceleration.current
                If Combat.centerShip.Helm.Throttle.current > Combat.centerShip.Helm.Throttle.max Then
                    Combat.centerShip.Helm.Throttle.current = Combat.centerShip.Helm.Throttle.max
                End If
            End If
        End Sub
        Private Shared Sub ThrottleDown()
            If ThrottleDownCheck = True Then
                Combat.centerShip.Helm.MatchSpeed = False
                Combat.centerShip.Helm.Throttle.current = Combat.centerShip.Helm.Throttle.current - Combat.centerShip.Helm.Acceleration.current
                If Combat.centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                    Combat.centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                End If
            End If
        End Sub
        Private Shared Sub TurnRight()
            If TurnRightCheck = True Then
                Combat.centerShip.Helm.Direction = Helm.NormalizeDirection(Combat.centerShip.Helm.Direction + Combat.centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub TurnLeft()
            If TurnLeftCheck = True Then
                Combat.centerShip.Helm.Direction = Helm.NormalizeDirection(Combat.centerShip.Helm.Direction - Combat.centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub WarpDrive()
            If WarpDriveCheck = True Then
                If Combat.centerShip.Helm.Throttle.current > Helm.MinimumSpeed Then
                    Combat.centerShip.Helm.Throttle.current = Combat.centerShip.Helm.Throttle.current - Combat.centerShip.Helm.Acceleration.current
                    If Combat.centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                        Combat.centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                    End If
                End If
                If Combat.centerShip.Helm.Direction < Math.PI Then
                    Combat.centerShip.Helm.Direction = Combat.centerShip.Helm.Direction - Combat.centerShip.Helm.TurnSpeed.current
                    If Combat.centerShip.Helm.Direction < 0 Then
                        Combat.centerShip.Helm.Direction = 0
                    End If
                ElseIf Combat.centerShip.Helm.Direction > Math.PI Then
                    Combat.centerShip.Helm.Direction = Combat.centerShip.Helm.Direction + Combat.centerShip.Helm.TurnSpeed.current
                    If Combat.centerShip.Helm.Direction > 2 * Math.PI Then
                        Combat.centerShip.Helm.Direction = 0
                    End If
                End If
                If Combat.centerShip.Helm.Throttle.current = Helm.MinimumSpeed And
                    Galaxy.Warping <> Galaxy.Warp.Warping And
                    Combat.centerShip.Helm.Direction = 0 Then
                    Galaxy.Warping = Galaxy.Warp.Entering
                End If
            End If
        End Sub
        Private Shared Sub MatchSpeed()
            If MatchSpeedCheck = True Then
                Combat.centerShip.Helm.MatchSpeed = True
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
                Dim centerShip As Ship = Combat.centerShip
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
                Dim centerShip As Ship = Combat.centerShip
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
                For i As Integer = 0 To combat.shipList.Length - 1
                    Dim adjacent As Integer = combat.shipList(i).Position.X - Combat.centerShip.Position.X
                    Dim opposite As Integer = combat.shipList(i).Position.Y - Combat.centerShip.Position.Y
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

                    Dim centerShip As Ship = Combat.centerShip
                    If distance < centerShip.Batteries.Primary.Range.current And distance <> 0 Then
                        If direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current < Battery.PlayerArc / 2 And
                            direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                            centerShip.Batteries.Primary.FireWeapon(distance, combat.shipList(i), direction)
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub FireSecondary()
            If FireSecondaryCheck = True Then
                For i As Integer = 0 To combat.shipList.Length - 1
                    If combat.shipList(i).MyAllegence <> galaxy.allegence.Player Then
                        Dim adjacent As Integer = combat.shipList(i).Position.X - Combat.centerShip.Position.X
                        Dim opposite As Integer = combat.shipList(i).Position.Y - Combat.centerShip.Position.Y
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

                        Dim centerShip As Ship = Combat.centerShip
                        If distance < centerShip.Batteries.Secondary.Range.current And distance <> 0 Then
                            If direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current < Battery.PlayerArc / 2 And
                                direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                                centerShip.Batteries.Secondary.FireWeapon(distance, combat.shipList(i), direction)
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
                For i As Integer = 0 To combat.shipList.Length - 1
                    If combat.shipList(i).MyAllegence <> galaxy.allegence.Player Then
                        Dim distance As Integer = Math.Sqrt(((combat.shipList(i).Position.X - Combat.centerShip.Position.X) ^ 2) + ((combat.shipList(i).Position.Y - Combat.centerShip.Position.Y) ^ 2))
                        If (distance < lastDistance And distance <> 0) Or lastDistance = 0 Then
                            lastDistance = distance
                            Combat.centerShip.Helm.Target = combat.shipList(i)
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
                Combat.centerShip.Shielding.LastHit = Shields.Sides.FrontShield
            End If
        End Sub
        Private Shared Sub RightBoost()
            If RightBoostCheck = True Then
                Combat.centerShip.Shielding.LastHit = Shields.Sides.RightShield
            End If
        End Sub
        Private Shared Sub RearBoost()
            If RearBoostCheck = True Then
                Combat.centerShip.Shielding.LastHit = Shields.Sides.BackShield
            End If
        End Sub
        Private Shared Sub LeftBoost()
            If LeftBoostCheck = True Then
                Combat.centerShip.Shielding.LastHit = Shields.Sides.LeftShield
            End If
        End Sub
        '-------------------

        '-----Engineering-----
        Public Shared HeatCheck As Boolean = False
        Public Shared CoolCheck As Boolean = False
        Private Shared Sub Heat()
            If HeatCheck = True Then
                Combat.centerShip.Engineering.Rate = Combat.centerShip.Engineering.Rate + (Engineering.RateOfChange * 10)
            End If
        End Sub
        Private Shared Sub Cool()
            If CoolCheck = True Then
                Combat.centerShip.Engineering.Rate = Combat.centerShip.Engineering.Rate - (Engineering.RateOfChange * 10)
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

    Public Shared Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        Select Case State
            Case Scenario.Transit

            Case Scenario.Battle
                Combat.UpdateCombatSenario()
                '-----Set Warp-----
                Select Case Warping
                    Case Warp.Entering
                        Warping = Warp.Warping
                        For Each i As Ship In Combat.shipList
                            i.InCombat = False
                        Next
                        WarpCounter = 100
                    Case Warp.Warping
                        If WarpCounter = 0 Then
                            Warping = Warp.None
                            State = Scenario.Transit
                        Else
                            WarpCounter = WarpCounter - 1
                        End If
                End Select
                ReDim craftPositions(UBound(Combat.shipList))
                '------------------
                '-----Set new positions-----
                For i As Integer = 0 To UBound(Combat.shipList)
                    Dim x As Integer = Combat.shipList(i).Position.X - Combat.centerShip.Position.X
                    Dim y As Integer = Combat.shipList(i).Position.Y - Combat.centerShip.Position.Y
                    Dim col As Color
                    Select Case Combat.shipList(i).MyAllegence
                        Case galaxy.allegence.Player
                            col = Color.Green
                        Case galaxy.allegence.Pirate
                            col = Color.Red
                    End Select
                    craftPositions(i) = New GraphicPosition(col, Combat.shipList(i).Hit, Combat.shipList(i).Firing, x, y, Combat.shipList(i).Helm.Direction)
                Next
                '---------------------------
                Server.ServerComms.UpdateServerMessage_Call(Combat.centerShip, craftPositions, Warping)
        End Select
    End Sub

End Class
