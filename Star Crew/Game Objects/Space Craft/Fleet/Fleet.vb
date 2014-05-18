﻿Public MustInherit Class Fleet 'A group of Ship objects that flies around a sector
    Inherits SpaceCraft 'The base Class of Ships and Fleets
    Public currentSector As Sector 'The Sector object the Fleet is currently inside
    Public ShipList As New List(Of Ship) 'A List of Ship objects
    Public TurnSpeed As Double 'The Speed at which the Fleet turns
    Public Shared ReadOnly DetectRange As Integer = 200 'The range at which the AI detects other Fleets
    Public Shared ReadOnly InteractRange As Integer = 30 'The range at which Fleets interact with each other
    Public Enum FleetState
        Target 'The Fleet must go for an actual Fleet
        Wander 'The Fleet can just wander
    End Enum
    Public MovementState As FleetState = FleetState.Wander

    Public Sub New(ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence, ByVal nFormat As ShipLayout.Formats)
        MyBase.New(nAllegence, nFormat, nIndex, New Point(Int(Rnd() * SpawnBox / 3), Int(Rnd() * SpawnBox / 3)))
        Randomize()
        MyAllegence = nAllegence 'Sets the Fleets MyAllegence value to the specified Allegience
        Index = nIndex 'Sets the Fleets Index value to the specified index
        For i As Integer = 0 To Int(Rnd() * 11) + 9 'Creates between 10 and 20 Ships alligned to the Fleet
            Dim Format As ShipLayout
            Select Case Int(Rnd() * (ShipLayout.Formats.ShipsMax - ShipLayout.Formats.ShipsMin)) + ShipLayout.Formats.ShipsMin
                Case ShipLayout.Formats.Screamer
                    Format = New Screamer
                Case ShipLayout.Formats.Thunder
                    Format = New Thunder
            End Select
            ShipList.Add(New Ship(Format, -1, MyAllegence))
        Next
        SetStats_Handle()
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
        If Index <> Sector.centerFleet.Index Then 'This Fleet is an active AI Fleet
            Dim targetDistance As Integer = DetectRange 'Sets the targets distance to be the maximum range
            Dim targetDirection As Double = -1 'A Double representing the Target's Direction
            Dim targetSpeed As Integer = 3 'Default speed for a fleet to wander at

            For Each i As Fleet In currentSector.fleetList
                Dim x As Integer = i.Position.X - Position.X 'The Target's X position relative to mine
                Dim y As Integer = i.Position.Y - Position.Y 'The Target's Y position relative to mine
                Dim distance As Integer = Math.Sqrt((x ^ 2) + (y ^ 2)) 'The Target's distance from me

                If (distance < targetDistance Or (i.MovementState = FleetState.Target And targetDistance = DetectRange)) And
                    (i.MyAllegence <> MyAllegence Or (i.ShipList.Count < 50 And ShipList.Count < 50)) And
                    i.Index <> Index Then 'The target is within detection range and/or it is the closest Fleet and we must Target and it
                    'is an enemy or it is an ally and we can combine Fleets and it is not targeting itself.
                    If distance <= InteractRange Then 'Interact with the fleet
                        MovementState = FleetState.Wander 'The Fleet can do as it wishes
                        If ReferenceEquals(i, Sector.centerFleet) = False And i.MyAllegence <> MyAllegence Then
                            'Run an automated fight cenario
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.AutoFight(Me, i)
                            Exit Sub
                        ElseIf i.MyAllegence = MyAllegence And ReferenceEquals(i, Sector.centerFleet) = False And
                            ShipList.Count < 50 And i.ShipList.Count < 50 Then 'Combine the two fleets
                            Dim temp As Integer = ShipList.Count
                            For e As Integer = 0 To 49 - temp
                                If i.ShipList.Count > 0 Then
                                    ShipList.Add(i.ShipList(0))
                                    Dim nShip As Ship = i.ShipList(0)
                                    i.RemoveShip(nShip)
                                Else
                                    currentSector.RemoveFleet(i, True, False)
                                    Exit For
                                End If
                            Next
                            Exit Sub
                        End If
                    Else 'Fly towards the Target Fleet
                        targetDistance = distance
                        '-----Target Direction----- 'Get the Target's direction relative to me
                        If x <> 0 Then
                            targetDirection = Math.Tanh(y / x)
                            If x < 0 Then
                                targetDirection = targetDirection + Math.PI
                            End If
                            targetDirection = Helm.NormalizeDirection(targetDirection)
                        ElseIf y > 0 Then
                            targetDirection = Math.PI / 2
                        Else
                            targetDirection = (3 * Math.PI) / 2
                        End If
                        '--------------------------
                    End If
                End If
            Next

            If targetDistance = DetectRange Then 'No valid target was found so we go for a station
                For Each i As SpaceStation In currentSector.spaceStations
                    Dim x As Integer = i.Position.X - Position.X 'The Target's X position relative to mine
                    Dim y As Integer = i.Position.Y - Position.Y 'The Target's Y position relative to mine
                    Dim distance As Integer = Math.Sqrt((x ^ 2) + (y ^ 2)) 'The Target's distance from me
                    If (distance < targetDistance Or targetDistance = DetectRange) Then 'The station is the closest.
                        targetDistance = distance
                        '-----Target Direction----- 'Get the Target's direction relative to me
                        If x <> 0 Then
                            targetDirection = Math.Tanh(y / x)
                            If x < 0 Then
                                targetDirection = targetDirection + Math.PI
                            End If
                            targetDirection = Helm.NormalizeDirection(targetDirection)
                        ElseIf y > 0 Then
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
            If Helm.NormalizeDirection(targetDirection - Direction + Math.PI) > Math.PI Then 'Turn right to face enemy
                Direction = Direction + TurnSpeed
                If Helm.NormalizeDirection(targetDirection - Direction + Math.PI) < Math.PI Then
                    Direction = targetDirection
                Else
                    Direction = Helm.NormalizeDirection(Direction)
                End If
            ElseIf Helm.NormalizeDirection(targetDirection - Direction + Math.PI) < Math.PI Then 'Turn left to face enemy
                Direction = Direction - TurnSpeed
                If Helm.NormalizeDirection(targetDirection - Direction + Math.PI) > Math.PI Then
                    Direction = targetDirection
                Else
                    Direction = Helm.NormalizeDirection(Direction)
                End If
            End If
            If Speed.current < targetSpeed Then 'Accelerate up to targetSpeed
                Speed.current = Speed.current + Acceleration.current
                If Speed.current > targetSpeed Then
                    Speed.current = targetSpeed
                End If
            ElseIf Speed.current > targetSpeed Then 'Decelerate down to targetSpeed
                Speed.current = Speed.current - Acceleration.current
                If Speed.current < targetSpeed Then
                    Speed.current = targetSpeed
                End If
            End If
        End If

        Position = New Point(Position.X + (Math.Cos(Direction) * Speed.current),
                             Position.Y + (Math.Sin(Direction) * Speed.current))
        'Update the Fleets position
    End Sub

End Class
