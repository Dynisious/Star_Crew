Public Class Galaxy 'Encapsulates and runs the Ships and Fleets of the Application
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False} 'A Timer object that 'ticks' 10 times a second
    Private craftPositions(-1) As GraphicPosition 'An Array of GraphicPosition objects that represents the Ships sent to the Clients
    Public centerSector As Sector 'A reference to a Sector object which the Players Fleet currently inhabits
    Public Enum Warp 'An Enumerator of the stages of 'warp'
        None 'Normal Space
        Entering 'Enter 'Warp'
        Warping 'Actively 'Warping'
    End Enum
    Public Warping As Warp = Warp.None 'The current state of 'warp' the Galaxy is in
    Public WarpCounter As Integer 'An Integer representing how many 10ths of a second the galaxy 'warps'
    Public Enum Scenario 'An Enumorator of the States of travel the Player can be in
        Battle 'Battling Ship to Ship
        Transit 'Moving the Fleet to the next battle
    End Enum
    Public State As Scenario = Scenario.Transit 'The current state of the Galaxy
    Public Paused As Boolean = False
    Public Enum Allegence 'An Enumorator of the different allegencies in the game
        Friendly 'Alligned with the Players
        Pirate 'Against the Player
        Neutral 'Acts as a Station
        max 'The maximum bounds of this enumerator
    End Enum
    Public CombatSpace As New Combat 'A Combat object that handles the 'Battle' state
    Public MessageToSend As ServerMessage 'A ServerMessage object to be serialised and sent to the Clients
    Public MessageMutex As Threading.Mutex 'A Mutex object to syncronize manipulation of the MessageToSend object
    Private MutexCreated As Boolean 'A Boolean value indecating whether the Mutex was successfully created

    Public Sub StartGame()
        Randomize()
        Warping = Warp.None 'Reset the 'warp' stage
        State = Scenario.Transit 'Reset the State of the Galaxy
        centerSector = New Sector(20) 'A new Sector object with 20 AI Fleet objects
        Sector.centerFleet = New FriendlyFleet(-1) 'Add a Fleet for the Players to control
        centerSector.AddFleet(Sector.centerFleet, 0) 'Add the Player controled Fleet into the Sector
        Fleet.SetStats_Call() 'Set the initial Stats of all Fleets
        Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'The identity of the Mutex
        Dim securityProtocols As New Security.AccessControl.MutexSecurity 'The security settings of the Mutex
        securityProtocols.AddAccessRule(
            New Security.AccessControl.MutexAccessRule(user,
                                                       Security.AccessControl.MutexRights.Modify Or
                                                       Security.AccessControl.MutexRights.Synchronize,
                                                       Security.AccessControl.AccessControlType.Allow))
        'Allow Threads to Access and Release the Mutex
        MessageMutex = New Threading.Mutex(False, "MessageMutex", MutexCreated, securityProtocols) 'Create the Mutex object
        GalaxyTimer.Enabled = True 'Start the Timer responsible for updating the game
    End Sub

    Public Sub RunCommand(ByVal clientCommand As ClientMessage) 'Change a Boolean value indecating whether the Client is pressing a key or not
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
    Public ThrottleUpCheck As Boolean = False 'True: Accelerate the Ship or Fleet
    Public ThrottleDownCheck As Boolean = False 'True: Decelerate the Ship or Fleet
    Public TurnRightCheck As Boolean = False 'True: Turn the Ship or Fleet right
    Public TurnLeftCheck As Boolean = False 'True: Turn the Ship or Fleet left
    Public WarpDriveCheck As Boolean = False 'True: Attempt to enter 'warp'
    Public MatchSpeedCheck As Boolean = False 'True: Match the speed of the target automatically
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
    Public RotateRightCheck As Boolean = False 'True: Rotate the Weapons Right
    Public RotateLeftCheck As Boolean = False 'True: Rotate the Weapons Left
    Public FirePrimaryCheck As Boolean = False 'True: Fire the Primary Weapon
    Public FireSecondaryCheck As Boolean = False 'True: Fire the Secondary Weapon
    Public SelectTargetCheck As Boolean = False 'True: Select the closest Target
    Private Sub RotateRight()
        If RotateRightCheck = True Then
            '-----Primary-----
            CombatSpace.centerShip.Batteries.Primary.TurnDistance.current =
                CombatSpace.centerShip.Batteries.Primary.TurnDistance.current +
                CombatSpace.centerShip.Batteries.Primary.TurnSpeed.current
            If CombatSpace.centerShip.Batteries.Primary.TurnDistance.current > CombatSpace.centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                CombatSpace.centerShip.Batteries.Primary.TurnDistance.current = CombatSpace.centerShip.Batteries.Primary.TurnDistance.max / 2
            End If
            '-----------------

            '-----Secondary-----
            CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current =
                CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current +
                CombatSpace.centerShip.Batteries.Secondary.TurnSpeed.current
            If CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current > CombatSpace.centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current = CombatSpace.centerShip.Batteries.Secondary.TurnDistance.max / 2
            End If
            '-------------------
        End If
    End Sub
    Private Sub RotateLeft()
        If RotateLeftCheck = True Then
            '-----Primary-----
            CombatSpace.centerShip.Batteries.Primary.TurnDistance.current =
                CombatSpace.centerShip.Batteries.Primary.TurnDistance.current -
                CombatSpace.centerShip.Batteries.Primary.TurnSpeed.current
            If CombatSpace.centerShip.Batteries.Primary.TurnDistance.current < -CombatSpace.centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                CombatSpace.centerShip.Batteries.Primary.TurnDistance.current = -CombatSpace.centerShip.Batteries.Primary.TurnDistance.max / 2
            End If
            '-----------------

            '-----Secondary-----
            CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current =
                CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current -
                CombatSpace.centerShip.Batteries.Secondary.TurnSpeed.current
            If CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current < -CombatSpace.centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                CombatSpace.centerShip.Batteries.Secondary.TurnDistance.current = -CombatSpace.centerShip.Batteries.Secondary.TurnDistance.max / 2
            End If
            '-------------------
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
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence <> Galaxy.Allegence.Friendly Then
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
                If ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence <> Galaxy.Allegence.Friendly Then
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
    Public ForwardBoostCheck As Boolean = False 'True: Prioritise power to the Fore Shield
    Public RightBoostCheck As Boolean = False 'True: Prioritise power to the Starboard Shield
    Public RearBoostCheck As Boolean = False 'True: Priorities power to the Aft Shield
    Public LeftBoostCheck As Boolean = False 'True: Priorities power to the Port Shield
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
    Public HeatCheck As Boolean = False 'True: Increase the rate at which the heat increases
    Public CoolCheck As Boolean = False 'True: Decrease the rate at which the heat increases
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

    Public Sub RunPlayerControls() 'Implement the actions the player is attempting
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

    Public Sub UpdateGalaxy() Handles GalaxyTimer.Tick 'Update the Galaxy object
        Dim stateToSend As Integer = State
        If Paused = True Then
            stateToSend = -1
        End If
        Select Case State
            Case Scenario.Transit 'Update Fleets
                If stateToSend <> -1 Then
                    If ThrottleUpCheck = True Then 'Accelerate the Players Fleet
                        Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.current + Sector.centerFleet.Acceleration.current
                        If Sector.centerFleet.Speed.current > Sector.centerFleet.Speed.max Then
                            Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.max
                        End If
                    End If
                    If ThrottleDownCheck = True Then 'Decelerate the Players Fleet
                        Sector.centerFleet.Speed.current = Sector.centerFleet.Speed.current - Sector.centerFleet.Acceleration.current
                        If Sector.centerFleet.Speed.current < 0 Then
                            Sector.centerFleet.Speed.current = 0
                        End If
                    End If
                    If TurnRightCheck = True Then 'Turn the Players Fleet right
                        Sector.centerFleet.Direction = Helm.NormalizeDirection(Sector.centerFleet.Direction + Sector.centerFleet.TurnSpeed)
                    End If
                    If TurnLeftCheck = True Then 'Turn the Players Fleet left
                        Sector.centerFleet.Direction = Helm.NormalizeDirection(Sector.centerFleet.Direction - Sector.centerFleet.TurnSpeed)
                    End If
                    centerSector.UpdateSector()

                    ReDim craftPositions(centerSector.fleetList.Count + centerSector.spaceStations.Length - 1) 'Clear and resize the Array of GraphicPosition objects apropriately
                    '-----Set new positions----- 'Create new GraphicPosition positions for the Fleets and SpaceStations
                    For i As Integer = 0 To centerSector.fleetList.Count - 1
                        Dim x As Integer = centerSector.fleetList(i).Position.X - Sector.centerFleet.Position.X 'The X coordinate relative to the
                        'center Fleet
                        Dim y As Integer = centerSector.fleetList(i).Position.Y - Sector.centerFleet.Position.Y 'The Y coordinate relative to the
                        'center Fleet
                        craftPositions(i) = New GraphicPosition(centerSector.fleetList(i).MyAllegence, centerSector.fleetList(i).Format,
                                                                False, False, x, y, centerSector.fleetList(i).Direction) 'Create a new GraphicPosition object
                    Next
                    For i As Integer = 0 To centerSector.spaceStations.Length - 1
                        Dim X As Integer = centerSector.spaceStations(i).Position.X - Sector.centerFleet.Position.X 'The X coordinate relative to the
                        'center Fleet
                        Dim Y As Integer = centerSector.spaceStations(i).Position.Y - Sector.centerFleet.Position.Y 'The Y coordinate relative to the
                        'center Fleet
                        craftPositions(centerSector.fleetList.Count + i) = New GraphicPosition(centerSector.spaceStations(i).MyAllegence,
                                                                                                ShipLayout.Formats.Station, False, False,
                                                                                                X, Y, 0)
                    Next
                    '---------------------------
                End If
                
                MessageMutex.WaitOne() 'Wait till the Mutex is free
                MessageToSend = New ServerMessage(-1, Sector.centerFleet.Speed, Sector.centerFleet.Direction,
                                                       Sector.centerFleet.ShipList.Count, Nothing, craftPositions,
                                                       Warping, stateToSend) 'Update the ServerMessage object
                MessageMutex.ReleaseMutex() 'Release the Mutex
            Case Scenario.Battle
                If stateToSend <> -1 Then
                    CombatSpace.UpdateCombatSenario() 'Update the Combat object
                    '-----Set Warp----- 'See what 'warp' actions are necessary
                    Select Case Warping
                        Case Warp.Entering 'The Player wants to enter 'warp'
                            Warping = Warp.Warping 'Set the warp
                            For Each i As Ship In CombatSpace.shipList
                                i.InCombat = False 'Take all ships out of combat
                            Next
                            WarpCounter = 50 'Set the count down to 50 10ths of a second
                        Case Warp.Warping 'The Player is 'warping'
                            If WarpCounter = 0 Then 'Exit 'warp'
                                Warping = Warp.None 'Reset the 'warp' state
                                WarpDriveCheck = False 'Force the release of the warp key
                                State = Scenario.Transit 'Resest the Galaxy's state
                                Sector.centerFleet.Position = New Point(Sector.centerFleet.Position.X +
                                                                        (Math.Cos(Sector.centerFleet.Direction) * 200),
                                                                        Sector.centerFleet.Position.Y +
                                                                        (Math.Sin(Sector.centerFleet.Direction) * 200))
                                'Move the Players fleet out of detection range in case they're running
                                Fleet.SetStats_Call() 'Update the Stats of all Fleets
                            Else 'Count down
                                WarpCounter = WarpCounter - 1
                            End If
                    End Select
                    ReDim craftPositions(CombatSpace.shipList.Count - 1) 'Clear and resize the Array of GraphicPosition objects apropriately
                    '------------------

                    '-----Set new positions----- 'Create new GraphicPosition objects
                    For i As Integer = 0 To ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1
                        Dim x As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X -
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.X 'The X coordinate of the Ship relative to the
                        'Players Ship
                        Dim y As Integer = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y -
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Position.Y 'The Y coordinate of the Ship relative to the
                        'Players Ship
                        craftPositions(i) = New GraphicPosition(CombatSpace.shipList(i).MyAllegence, CombatSpace.shipList(i).Format,
                                                                CombatSpace.shipList(i).Hit, CombatSpace.shipList(i).Firing,
                                                                x, y, CombatSpace.shipList(i).Direction) 'A GraphicPosition object to send to the
                        'Client's representing the Ship
                        'Create a new GraphicPosition object
                    Next
                    '---------------------------
                End If
                Dim targetIndex As Integer = If(CombatSpace.centerShip.Helm.Target IsNot Nothing, CombatSpace.centerShip.Helm.Target.Index, -1)
                'Get the index of the Player's targeted Ship if there is one

                MessageMutex.WaitOne() 'Wait for the Mutex to be free
                MessageToSend = New ServerMessage(targetIndex, CombatSpace.centerShip.Speed, CombatSpace.centerShip.Direction,
                                                      -1, CombatSpace.centerShip, craftPositions, Warping, stateToSend) 'Update the
                'ServerMessage object
                MessageMutex.ReleaseMutex() 'Release the Mutex
        End Select
    End Sub

End Class
