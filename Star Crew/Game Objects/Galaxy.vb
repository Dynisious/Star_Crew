Public Class Galaxy
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Private craftPositions(-1) As GraphicPosition
    Public centerSector As Sector
    Public Enum Warp
        None
        Entering
        Warping
    End Enum
    Public Warping As Warp = Warp.None
    Public WarpCounter As Integer
    Public Enum Scenario
        Battle
        Transit
    End Enum
    Public State As Scenario = Scenario.Transit
    Public Enum Allegence
        Player
        Pirate
        Neutral
    End Enum
    Public CombatSpace As New Combat
    Public MessageToSend As ServerMessage

    Public Sub StartGame()
        Randomize()
        Warping = Warp.None
        State = Scenario.Transit
        centerSector = New Sector(20)
        Sector.centerFleet = New FriendlyFleet(-1)
        centerSector.fleetList.Insert(0, Sector.centerFleet)
        centerSector.AddFleet(Sector.centerFleet)
        Fleet.SetStats_Call()
        GalaxyTimer.Enabled = True
    End Sub

    Public Sub RunCommand(ByVal clientCommand As ClientMessage)
        Select Case clientCommand.Station
            Case Star_Crew.Station.StationTypes.Helm
                Select Case clientCommand.Command
                    Case Helm.Commands.ThrottleUp
                        If clientCommand.Value = 1 Then
                            ThrottleUpCheck = True
                            ThrottleDownCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            ThrottleUpCheck = False
                        End If
                    Case Helm.Commands.ThrottleDown
                        If clientCommand.Value = 1 Then
                            ThrottleDownCheck = True
                            ThrottleUpCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            ThrottleDownCheck = False
                        End If
                    Case Helm.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            TurnRightCheck = True
                            TurnLeftCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            TurnRightCheck = False
                        End If
                    Case Helm.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            TurnLeftCheck = True
                            TurnRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            TurnLeftCheck = False
                        End If
                    Case Helm.Commands.WarpDrive
                        If clientCommand.Value = 1 Then
                            WarpDriveCheck = True
                            MatchSpeedCheck = False
                            ThrottleUpCheck = False
                            ThrottleDownCheck = False
                            TurnLeftCheck = False
                            TurnRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            WarpDriveCheck = False
                        End If
                    Case Helm.Commands.MatchSpeed
                        If clientCommand.Value = 1 Then
                            MatchSpeedCheck = True
                            WarpDriveCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            MatchSpeedCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Batteries
                Select Case clientCommand.Command
                    Case Battery.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            RotateRightCheck = True
                            RotateLeftCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            RotateRightCheck = False
                        End If
                    Case Battery.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            RotateLeftCheck = True
                            RotateRightCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            RotateLeftCheck = False
                        End If
                    Case Battery.Commands.FirePrimary
                        If clientCommand.Value = 1 Then
                            FirePrimaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            FirePrimaryCheck = False
                        End If
                    Case Battery.Commands.FireSecondary
                        If clientCommand.Value = 1 Then
                            FireSecondaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            FireSecondaryCheck = False
                        End If
                    Case Battery.Commands.SetTarget
                        If clientCommand.Value = 1 Then
                            SelectTargetCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            SelectTargetCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Shielding
                Select Case clientCommand.Command
                    Case Shields.Commands.BoostForward
                        If clientCommand.Value = 1 Then
                            ForwardBoostCheck = True
                            RightBoostCheck = False
                            RearBoostCheck = False
                            LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            ForwardBoostCheck = False
                        End If
                    Case Shields.Commands.BoostRight
                        If clientCommand.Value = 1 Then
                            ForwardBoostCheck = False
                            RightBoostCheck = True
                            RearBoostCheck = False
                            LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            RightBoostCheck = False
                        End If
                    Case Shields.Commands.BoostBack
                        If clientCommand.Value = 1 Then
                            ForwardBoostCheck = False
                            RightBoostCheck = False
                            RearBoostCheck = True
                            LeftBoostCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            RearBoostCheck = False
                        End If
                    Case Shields.Commands.BoostLeft
                        If clientCommand.Value = 1 Then
                            ForwardBoostCheck = False
                            RightBoostCheck = False
                            RearBoostCheck = False
                            LeftBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            LeftBoostCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Engineering
                Select Case clientCommand.Command
                    Case Engineering.Commands.Heat
                        If clientCommand.Value = 1 Then
                            HeatCheck = True
                            CoolCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            HeatCheck = False
                        End If
                    Case Engineering.Commands.Cool
                        If clientCommand.Value = 1 Then
                            CoolCheck = True
                            HeatCheck = False
                        ElseIf clientCommand.Value = 0 Then
                            CoolCheck = False
                        End If
                End Select
        End Select
    End Sub
    '-----Helm-----
    Public ThrottleUpCheck As Boolean = False
    Public ThrottleDownCheck As Boolean = False
    Public TurnRightCheck As Boolean = False
    Public TurnLeftCheck As Boolean = False
    Public WarpDriveCheck As Boolean = False
    Public MatchSpeedCheck As Boolean = False
    Private Sub ThrottleUp()
        If ThrottleUpCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.MatchSpeed = False
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current =
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current +
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Acceleration.current
            If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current >
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.max Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current =
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.max
            End If
        End If
    End Sub
    Private Sub ThrottleDown()
        If ThrottleDownCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.MatchSpeed = False
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current =
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current -
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Acceleration.current
            If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current < Helm.MinimumSpeed Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current = Helm.MinimumSpeed
            End If
        End If
    End Sub
    Private Sub TurnRight()
        If TurnRightCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction =
            Helm.NormalizeDirection(ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction +
                                         ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.TurnSpeed.current)
        End If
    End Sub
    Private Sub TurnLeft()
        If TurnLeftCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction =
                Helm.NormalizeDirection(ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction -
                                        ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.TurnSpeed.current)
        End If
    End Sub
    Private Sub WarpDrive()
        If WarpDriveCheck = True Then
            If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current > Helm.MinimumSpeed Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current =
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current -
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Acceleration.current
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current < Helm.MinimumSpeed Then
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current = Helm.MinimumSpeed
                End If
            End If
            If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction < Math.PI Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.TurnSpeed.current
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction < 0 Then
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction = 0
                End If
            ElseIf ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction > Math.PI Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction + ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.TurnSpeed.current
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction > 2 * Math.PI Then
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction = 0
                End If
            End If
            If ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Parent.Speed.current = Helm.MinimumSpeed And
                Warping <> Galaxy.Warp.Warping And
                ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Direction = 0 Then
                Warping = Galaxy.Warp.Entering
            End If
        End If
    End Sub
    Private Sub MatchSpeed()
        If MatchSpeedCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.MatchSpeed = True
        End If
    End Sub
    '--------------

    '-----Batteries-----
    Public RotateRightCheck As Boolean = False
    Public RotateLeftCheck As Boolean = False
    Public FirePrimaryCheck As Boolean = False
    Public FireSecondaryCheck As Boolean = False
    Public SelectTargetCheck As Boolean = False
    Private Sub RotateRight()
        If RotateRightCheck = True Then
            Dim centerShip As Ship = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip
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
    Private Sub RotateLeft()
        If RotateLeftCheck = True Then
            Dim centerShip As Ship = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip
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
    Private Sub FirePrimary()
        If FirePrimaryCheck = True Then
            For i As Integer = 0 To ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1
                Dim adjacent As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.X
                Dim opposite As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.Y
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

                Dim centerShip As Ship = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip
                If distance < centerShip.Batteries.Primary.Range.current And distance <> 0 Then
                    If direction - centerShip.Direction - centerShip.Batteries.Primary.TurnDistance.current < Battery.PlayerArc / 2 And
                        direction - centerShip.Direction - centerShip.Batteries.Primary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                        centerShip.Batteries.Primary.FireWeapon(distance, ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i), direction)
                        Exit Sub
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub FireSecondary()
        If FireSecondaryCheck = True Then
            For i As Integer = 0 To ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence <> Galaxy.Allegence.Player Then
                    Dim adjacent As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.X
                    Dim opposite As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.Y
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

                    Dim centerShip As Ship = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip
                    If distance < centerShip.Batteries.Secondary.Range.current And distance <> 0 Then
                        If direction - centerShip.Direction - centerShip.Batteries.Secondary.TurnDistance.current < Battery.PlayerArc / 2 And
                            direction - centerShip.Direction - centerShip.Batteries.Secondary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                            centerShip.Batteries.Secondary.FireWeapon(distance, ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i), direction)
                            Exit Sub
                        End If
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SelectTarget()
        If SelectTargetCheck = True Then
            Dim lastDistance As Integer
            For i As Integer = 0 To ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence <> Galaxy.Allegence.Player Then
                    Dim distance As Integer = Math.Sqrt(((ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.X) ^ 2) + ((ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y - ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.Y) ^ 2))
                    If (distance < lastDistance And distance <> 0) Or lastDistance = 0 Then
                        lastDistance = distance
                        ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.Target = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i)
                    End If
                End If
            Next
        End If
    End Sub
    '-------------------

    '-----Shielding-----
    Public ForwardBoostCheck As Boolean = False
    Public RightBoostCheck As Boolean = False
    Public RearBoostCheck As Boolean = False
    Public LeftBoostCheck As Boolean = False
    Private Sub ForwardBoost()
        If ForwardBoostCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.LastHit = Shields.Sides.FrontShield
        End If
    End Sub
    Private Sub RightBoost()
        If RightBoostCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.LastHit = Shields.Sides.RightShield
        End If
    End Sub
    Private Sub RearBoost()
        If RearBoostCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.LastHit = Shields.Sides.BackShield
        End If
    End Sub
    Private Sub LeftBoost()
        If LeftBoostCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.LastHit = Shields.Sides.LeftShield
        End If
    End Sub
    '-------------------

    '-----Engineering-----
    Public HeatCheck As Boolean = False
    Public CoolCheck As Boolean = False
    Private Sub Heat()
        If HeatCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.Rate = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.Rate + (Engineering.RateOfChange * 10)
        End If
    End Sub
    Private Sub Cool()
        If CoolCheck = True Then
            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.Rate = ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.Rate - (Engineering.RateOfChange * 10)
        End If
    End Sub
    '---------------------

    Public Sub RunPlayerControls()
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

    Public Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        Select Case State
            Case Scenario.Transit
                If ThrottleUpCheck = True Then
                    Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.current + Sector.centerFleet.Acceleration.current
                    If Sector.centerFleet.Speed.current > Sector.centerFleet.Speed.max Then
                        Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.max
                    End If
                End If
                If ThrottleDownCheck = True Then
                    Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.current - Sector.centerFleet.Acceleration.current
                    If Sector.centerFleet.Speed.current < 0 Then
                        Sector.centerFleet.Speed.current = 0
                    End If
                End If
                If TurnRightCheck = True Then
                    Sector.centerFleet.Direction = Helm.NormalizeDirection(Sector.centerFleet.Direction + Sector.centerFleet.TurnSpeed)
                End If
                If TurnLeftCheck = True Then
                    Sector.centerFleet.Direction = Helm.NormalizeDirection(Sector.centerFleet.Direction - Sector.centerFleet.TurnSpeed)
                End If
                Fleet.UpdateFleet_Call()

                ReDim craftPositions(centerSector.fleetList.Count - 1)
                '-----Set new positions-----
                For i As Integer = 0 To centerSector.fleetList.Count - 1
                    Dim x As Integer = centerSector.fleetList(i).Position.X - Sector.centerFleet.Position.X
                    Dim y As Integer = centerSector.fleetList(i).Position.Y - Sector.centerFleet.Position.Y
                    craftPositions(i) = New GraphicPosition(centerSector.fleetList(i).MyAllegence, False, False, x, y, centerSector.fleetList(i).Direction)
                Next
                '---------------------------
                MessageToSend = New ServerMessage(-1, Nothing, craftPositions, Warping, State,
                                                  Sector.centerFleet.Speed, Sector.centerFleet.Direction)
            Case Scenario.Battle
                CombatSpace.UpdateCombatSenario()
                '-----Set Warp-----
                Select Case Warping
                    Case Warp.Entering
                        Warping = Warp.Warping
                        For Each i As Ship In CombatSpace.shipList
                            i.InCombat = False
                        Next
                        WarpCounter = 50
                    Case Warp.Warping
                        If WarpCounter = 0 Then
                            Warping = Warp.None
                            WarpDriveCheck = False
                            State = Scenario.Transit
                            Sector.centerFleet.Position = New Point(Sector.centerFleet.Position.X + (Math.Cos(Sector.centerFleet.Direction) * 2 * Fleet.DetectRange),
                                                                    Sector.centerFleet.Position.Y + (Math.Sin(Sector.centerFleet.Direction) * 2 * Fleet.DetectRange))
                            Fleet.SetStats_Call()
                        Else
                            WarpCounter = WarpCounter - 1
                        End If
                End Select
                ReDim craftPositions(CombatSpace.shipList.Count - 1)
                '------------------
                '-----Set new positions-----
                For i As Integer = 0 To ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1
                    Dim x As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X -
                        ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.X
                    Dim y As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y -
                        ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.Y
                    craftPositions(i) = New GraphicPosition(ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence,
                                                            ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Hit,
                                                            ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Firing,
                                                            x, y, ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Direction)
                Next
                '---------------------------
                Dim targetIndex As Integer = If(CombatSpace.centerShip.Helm.Target IsNot Nothing, CombatSpace.centerShip.Helm.Target.Index, -1)
                MessageToSend = New ServerMessage(targetIndex, CombatSpace.centerShip, craftPositions, Warping, State,
                                                  CombatSpace.centerShip.Speed, CombatSpace.centerShip.Direction)
        End Select
    End Sub

End Class
