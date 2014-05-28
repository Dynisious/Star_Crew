<Serializable()>
Public MustInherit Class Fleet 'A group of Ship objects that flies around a sector
    Inherits SpaceCraft 'The base Class of Ships and Fleets
    Public currentSector As Sector 'The Sector object the Fleet is currently inside
    Public ShipList As New List(Of Ship) 'A List of Ship objects
    Public TurnSpeed As Double 'The Speed at which the Fleet turns
    Public Shared ReadOnly DetectRange As Integer = 200 'The range at which the AI detects other Fleets
    Public Shared ReadOnly InteractRange As Integer = 30 'The range at which Fleets interact with each other
    Public Shared ReadOnly PopulationCap As Integer = 30 'The maximum number of Ships allowed to exist in one Fleet
    Public Enum FleetState
        Target 'The Fleet must go for an actual Fleet
        Wander 'The Fleet can just wander
    End Enum
    Public MovementState As FleetState = FleetState.Wander 'How the Fleet is moving around the Sector

    Public Sub New(ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence,
                   ByVal nFormat As ShipLayout.Formats, ByRef nSpaceStation As SpaceStation)
        MyBase.New(nAllegence, nFormat, nIndex, New Point(nSpaceStation.Position.X + Int(Rnd() * 4 * InteractRange) - (2 * InteractRange),
                                                          nSpaceStation.Position.Y + Int(Rnd() * 4 * InteractRange) - (2 * InteractRange)))
        Randomize()
        MyAllegence = nAllegence 'Sets the Fleets MyAllegence value to the specified Allegience
        Index = nIndex 'Sets the Fleets Index value to the specified index
        For i As Integer = 0 To Int(Rnd() * PopulationCap / 3) + (PopulationCap / 6) + 1 'Creates between 1/3 and 1/2 Ships
            'of the population cap inside the Fleet
            Dim Format As ShipLayout
            Select Case Int(Rnd() * (ShipLayout.Formats.ShipsMax - ShipLayout.Formats.ShipsMin)) + ShipLayout.Formats.ShipsMin
                Case ShipLayout.Formats.Screamer
                    Format = New Screamer
                Case ShipLayout.Formats.Thunder
                    Format = New Thunder
            End Select
            ShipList.Add(New Ship(Format, -1, MyAllegence))
        Next
        SetStats_Handle() 'Sets the Stats of the Fleet
    End Sub

    Public Sub RemoveShip(ByRef nShip As Ship) 'Removes the ship object from the Fleet
        For i As Integer = 0 To ShipList.Count - 1
            If ReferenceEquals(nShip, ShipList(i)) = True Then 'Its the specified Ship object
                ShipList.RemoveAt(i) 'Remove the Ship from the list
                ShipList.TrimExcess() 'Trim the excess space in the list
                Exit Sub
            End If
        Next
    End Sub

    Private Shared Event SetStats() 'Sets the Stats of all Fleets
    Public Shared Sub SetStats_Call() 'Raises the SetStats event
        RaiseEvent SetStats()
    End Sub
    Private Sub SetStats_Handle() Handles MyClass.SetStats 'Sets the Fleets stats
        If ShipList.Count > 0 Then 'There are Ships inside this fleet
            Dim lowestSpeed As Double 'The speed of the slowest Ship inside the fleet
            Dim lowestAcceleration As Double 'The acceleration of the slowest accelerating Ship in the Fleet
            Dim lowestTurnSpeed As Double 'The turning speed of the the Ship with the slowest turning speed
            For Each i As Ship In ShipList
                If i.Dead = False Then 'The ship is alive
                    If i.Helm.Parent.Speed.max < lowestSpeed Or lowestSpeed = 0 Then
                        lowestSpeed = i.Speed.max 'Set the lowest speed
                    End If
                    If i.Acceleration.max < lowestAcceleration Or lowestAcceleration = 0 Then
                        lowestAcceleration = i.Acceleration.max 'Set the lowest acceleration
                    End If
                    If i.Helm.TurnSpeed.current < lowestTurnSpeed Or lowestTurnSpeed = 0 Then
                        lowestTurnSpeed = i.Helm.TurnSpeed.current 'Set the lowest turn speed
                    End If
                End If
            Next
            Speed = New StatDbl(0, lowestSpeed) 'Sets the Fleet's max speed to be 60% of the slowest Ship in the fleet
            Acceleration = New StatDbl(lowestAcceleration * 1.5, 0) 'Sets the Fleet's Acceleration to be 150% of the slowest
            'accelerating Ship in the Fleet
            TurnSpeed = lowestTurnSpeed 'Sets the turn speed of the Fleet to be the same as the slowest turning Ship in the Fleet
        ElseIf Dead = False Then 'The Fleet should be removed
            currentSector.RemoveFleet(Me, True, True) 'Remove the Fleet from the Sector and 'kill' all Ships from it
        End If
    End Sub

    Public Overridable Sub UpdateFleet() 'Updates the Fleet
        If Index <> ConsoleWindow.GameServer.GameWorld.centerFleet.Index Then 'This Fleet is an active AI Fleet
            Dim targetDistance As Integer = DetectRange 'Sets the targets distance to be the maximum range
            Dim targetDirection As Double = -1 'A Double representing the Target's Direction
            Dim targetSpeed As Integer = 3 'Default speed for a fleet to wander at

            For i As Integer = 0 To currentSector.fleetList.Count - 1
                If i < currentSector.fleetList.Count Then
                    Dim nFleet As Fleet = currentSector.fleetList(i) 'The Fleet object being evaluated
                    Dim x As Integer = currentSector.fleetList(i).Position.X - Position.X 'The Target's X position relative to mine
                    Dim y As Integer = nFleet.Position.Y - Position.Y 'The Target's Y position relative to mine
                    Dim distance As Integer = Math.Sqrt((x ^ 2) + (y ^ 2)) 'The Target's distance from me

                    If (distance < targetDistance Or (MovementState = FleetState.Target And targetDistance = DetectRange)) And
                        (nFleet.MyAllegence <> MyAllegence Or (nFleet.ShipList.Count < PopulationCap And ShipList.Count < PopulationCap)) And
                        nFleet.Index <> Index Then 'The target is within detection range and/or it is the closest Fleet and we must Target and it
                        'is an enemy or it is an ally and we can combine Fleets and it is not targeting itself.
                        If distance <= InteractRange Then 'Interact with the fleet
                            MovementState = FleetState.Wander 'The Fleet can do as it wishes
                            If ReferenceEquals(nFleet, ConsoleWindow.GameServer.GameWorld.centerFleet) = False And
                                nFleet.MyAllegence <> MyAllegence Then
                                ConsoleWindow.GameServer.GameWorld.CombatSpace.AutoFight(Me, nFleet) 'Run an automated fight cenario
                                Speed.current = 0 'Set the Speed to 0
                            ElseIf nFleet.MyAllegence = MyAllegence And
                                ReferenceEquals(nFleet, ConsoleWindow.GameServer.GameWorld.centerFleet) = False And
                                ShipList.Count < PopulationCap And
                                nFleet.ShipList.Count < PopulationCap Then 'Combine the two fleets
                                Dim temp As Integer = ShipList.Count 'The number of Ships in this Fleet
                                For e As Integer = 0 To PopulationCap - 1 - temp 'Loop as many times as is necessary to fill up the Fleet
                                    If nFleet.ShipList.Count > 0 Then 'There's still Ships to move
                                        Dim nShip As Ship = nFleet.ShipList(0) 'The Ship getting moved
                                        ShipList.Add(nShip) 'Add the Ship to this Fleet
                                        nFleet.RemoveShip(nShip) 'Remove the Ship from the other Fleet
                                    Else 'The other Fleet no longer exists
                                        currentSector.RemoveFleet(nFleet, True, False) 'Kill the Fleet
                                        Exit For 'Exit the For loop
                                    End If
                                Next
                                Exit Sub 'Exit the Subroutine
                            End If
                        Else 'Fly towards the Target Fleet
                            targetDistance = distance 'The distance of the targeted Fleet
                            '-----Target Direction----- 'Get the Target's direction relative to me
                            If x <> 0 Then
                                targetDirection = Math.Tanh(y / x)
                                If x < 0 Then
                                    targetDirection = targetDirection + Math.PI
                                End If
                                targetDirection = Helm.NormaliseDirection(targetDirection)
                            ElseIf y > 0 Then
                                targetDirection = Math.PI / 2
                            Else
                                targetDirection = (3 * Math.PI) / 2
                            End If
                            '--------------------------
                        End If
                    End If
                End If
            Next

            If targetDistance = DetectRange Then 'No valid target was found so we go for a station
                For Each i As SpaceStation In currentSector.spaceStations
                    Dim X As Integer = i.Position.X - Position.X 'The Target's X position relative to mine
                    Dim Y As Integer = i.Position.Y - Position.Y 'The Target's Y position relative to mine
                    Dim distance As Integer = Math.Sqrt((X ^ 2) + (Y ^ 2)) 'The Target's distance from me
                    If (distance < targetDistance Or targetDistance = DetectRange) Then 'The station is the closest.
                        targetDistance = distance
                        '-----Target Direction----- 'Get the Target's direction relative to me
                        If X <> 0 Then
                            targetDirection = Math.Tanh(Y / X)
                            If X < 0 Then
                                targetDirection = targetDirection + Math.PI
                            End If
                            targetDirection = Helm.NormaliseDirection(targetDirection)
                        ElseIf Y > 0 Then
                            targetDirection = Math.PI / 2
                        Else
                            targetDirection = (3 * Math.PI) / 2
                        End If
                        '--------------------------
                    End If
                Next
            End If

            If targetDirection - Direction < Math.PI / 2 And
                targetDirection - Direction > -Math.PI / 2 Then 'the Fleet is facing the enemy
                targetSpeed = Speed.max 'Accelerate to maximum speed
            End If
            If Helm.NormaliseDirection(targetDirection - Direction + Math.PI) > Math.PI Then 'Turn right to face enemy
                Direction = Direction + TurnSpeed
                If Helm.NormaliseDirection(targetDirection - Direction + Math.PI) < Math.PI Then 'Turn back to be facing the enemy
                    Direction = targetDirection
                Else
                    Direction = Helm.NormaliseDirection(Direction) 'Normalise the direction
                End If
            ElseIf Helm.NormaliseDirection(targetDirection - Direction + Math.PI) < Math.PI Then 'Turn left to face enemy
                Direction = Direction - TurnSpeed
                If Helm.NormaliseDirection(targetDirection - Direction + Math.PI) > Math.PI Then 'Turn back to be facing the enemy
                    Direction = targetDirection
                Else
                    Direction = Helm.NormaliseDirection(Direction) 'Normalise the direction
                End If
            End If
            If Speed.current < targetSpeed Then 'Accelerate up to targetSpeed
                Speed.current = Speed.current + Acceleration.current
                If Speed.current > targetSpeed Then 'Turn back to the targets speed
                    Speed.current = targetSpeed
                End If
                If Speed.current > Speed.max Then 'Turn back to the Fleets maximum speed
                    Speed.current = Speed.max
                End If
            ElseIf Speed.current > targetSpeed Then 'Decelerate down to targetSpeed
                Speed.current = Speed.current - Acceleration.current
                If Speed.current < targetSpeed Then 'Turn up to the targets speed
                    Speed.current = targetSpeed
                End If
            End If
        End If

        Position = New Point(Position.X + (Math.Cos(Direction) * Speed.current),
                             Position.Y + (Math.Sin(Direction) * Speed.current))
        'Update the Fleets position
    End Sub

End Class
