<Serializable()>
Public MustInherit Class Fleet 'A group of Ship objects that flies around a sector
    Inherits SpaceCraft 'The base Class of Ships and Fleets
    Public currentSector As Sector 'The Sector object the Fleet is currently inside
    Public ShipList As New List(Of Ship) 'A List of Ship objects
    Public TurnSpeed As Double 'The Speed at which the Fleet turns
    Public Shared ReadOnly DetectRange As Integer = 400 'The range at which the AI detects other Fleets
    Public Shared ReadOnly InteractRange As Integer = 30 'The range at which Fleets interact with each other
    Public Shared ReadOnly PopulationCap As Integer = 30 'The maximum number of Ships allowed to exist in one Fleet

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
    Public Sub SetStats_Handle() Handles MyClass.SetStats 'Sets the Fleets stats
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
            currentSector.RemoveFleet(Me, True, False) 'Remove the Fleet from the Sector
        End If
    End Sub

    Public Overridable Sub UpdateFleet() 'Updates the Fleet
        For Each i As Ship In ShipList 'Loop through all ships inside the Fleet
            i.Engineering.Update() 'Perform repairs and store power inside the Ship
        Next

        If Index <> ConsoleWindow.GameServer.GameWorld.centerFleet.Index Then 'This Fleet is an active AI Fleet
            Dim targetDistance As Integer = -1 'Sets the targets distance to be the maximum range
            Dim targetDirection As Double = -1 'A Double representing the Target's Direction
            Dim targetSpeed As Integer 'Default speed for a fleet to wander at

            '-----Decide action to take-----
            If TargetLock = False Then 'Select the best target
                Target = Nothing 'Clear the previous target
                For i As Integer = 0 To currentSector.fleetList.Count - 1 'Loop through all the Fleets in the sector
                    If i < currentSector.fleetList.Count And i <> Index Then 'There's still Fleets to loop through and
                        'it is not targeting itself
                        Dim distance As Integer = Math.Sqrt(((currentSector.fleetList(i).Position.X - Position.X) ^ 2) +
                                                            ((currentSector.fleetList(i).Position.Y - Position.Y) ^ 2)) 'The distance
                        'of the potential target relative to the Fleet

                        '-----Target selection-----
                        If distance <= DetectRange Then 'It has the potential of a valid target
                            If (distance < targetDistance Or targetDistance = -1) And
                                (currentSector.fleetList(i).MyAllegence <> MyAllegence Or
                                 (currentSector.fleetList(i).ShipList.Count < PopulationCap And ShipList.Count < PopulationCap)) Then 'It's
                                'a valid target
                                targetDistance = distance 'Set the new distance to compare other potential targets against
                                Target = currentSector.fleetList(i) 'Set the new Target
                            End If
                        End If
                        '--------------------------

                        '-----Interaction-----
                        If distance < InteractRange Then 'Interact with the Fleet
                            If currentSector.fleetList(i).MyAllegence = MyAllegence And ShipList.Count < PopulationCap And
                                currentSector.fleetList(i).ShipList.Count < PopulationCap And
                                i <> ConsoleWindow.GameServer.GameWorld.centerFleet.Index Then 'Combine Fleets
                                For e As Integer = 0 To PopulationCap - ShipList.Count 'Loop as many times as necessary to fill up the Fleet
                                    If currentSector.fleetList(i).ShipList.Count <> 0 Then 'There're still Ships to move
                                        Dim nShip As Ship = currentSector.fleetList(i).ShipList(0) 'The Ship object being moved
                                        ShipList.Add(nShip) 'Add the Ship to the Fleet
                                        currentSector.fleetList(i).RemoveShip(nShip) 'Remove the Ship from the other Fleet
                                    Else 'Exit the Loop
                                        Exit For
                                    End If
                                Next
                                Dim nFleet As Fleet = currentSector.fleetList(i) 'The Fleet to update
                                nFleet.SetStats_Handle() 'Update the Fleet's stats
                            ElseIf currentSector.fleetList(i).MyAllegence <> MyAllegence Then 'attack the Fleet
                                Dim nFleet As Fleet = currentSector.fleetList(i) 'The Fleet to attack
                                ConsoleWindow.GameServer.GameWorld.CombatSpace.AutoFight(Me, nFleet) 'Fight the two Fleets
                            End If
                        End If
                        '---------------------
                    Else 'Exit the Loop
                        Exit For
                    End If
                Next

                If targetDistance = -1 Then 'No valid Fleet target was found
                    For Each i As SpaceStation In currentSector.spaceStations 'Loop through all the Space Stations in the Sector
                        Dim distance As Integer = Math.Sqrt(((i.Position.X - Position.X) ^ 2) +
                                                            ((i.Position.Y - Position.Y) ^ 2)) 'The distance of the Space
                        'Station relative to the Fleet

                        If (distance < targetDistance Or targetDistance = -1) Then 'The Space Station is a potential target
                            Dim validTarget As Boolean = False 'A Boolean value indecating whether the targeted station is valid
                            If i.MyAllegence = MyAllegence Then 'Check that the station is not at full population
                                If i.Population.Count = SpaceStation.PopulationCap Then 'Check to see if the Fleet is part of the population
                                    For Each e As Fleet In i.Population
                                        If e.Index = Index Then 'The Fleet is part of the Space Stations population
                                            validTarget = True 'Allow the Space Station to be targeted
                                            Exit For
                                        End If
                                    Next
                                Else 'The Space Station is not overpopulated
                                    validTarget = True 'Allow the Space Station to be targeted
                                End If
                            Else 'The Station is not allied and an attempt to capture should be made
                                validTarget = True 'Allow the Space Station to be targeted
                            End If
                            If validTarget = True Then
                                targetDistance = distance 'Set the new distance to be compared against other Space Stations
                                Target = i 'Set the new target
                            End If
                        End If
                    Next
                End If
            End If
            '-------------------------------

            If Target IsNot Nothing Then 'Their is a valid target
                '-----Calculate the targeted speed and direction-----
                If Target.Dead = False Then 'The target is alive
                    Dim X As Integer = Target.Position.X - Position.X 'The X coordinate of the target relative to the Fleet
                    Dim Y As Integer = Target.Position.Y - Position.Y 'The Y coordinate of the target relative to the Fleet
                    If X <> 0 Then 'There will not be a divide by 0 error
                        targetDirection = Math.Tanh(Y / X) 'Calculate the direction of the target relative to the Fleet
                        If X < 0 Then 'The calculation is reflected in the line Y=X
                            targetDirection = targetDirection + Math.PI 'Reflect the direction
                        End If
                        targetDirection = Helm.NormaliseDirection(targetDirection) 'Normalise the direction to be within 0 and 2*Pi
                    ElseIf Y > 0 Then 'The target is directly above the Fleet
                        targetDirection = Math.PI / 2
                    Else 'The target is directly bellow the Fleet
                        targetDirection = 3 * Math.PI / 2
                    End If
                    If Helm.NormaliseDirection(targetDirection - Direction + (Math.PI / 2)) < Math.PI Then 'The target is in front of the Fleet
                        targetSpeed = Speed.max 'Accelerate to the maximum speed
                    Else 'Travel at a slow pace
                        targetSpeed = 3 'A Slow pace
                    End If
                Else 'The target is dead
                    Target = Nothing
                    TargetLock = False
                    Exit Sub
                End If
                '----------------------------------------------------
            Else 'There is not target
                targetSpeed = Speed.max 'Accelerate to the maximum speed
                targetDirection = Helm.NormaliseDirection(Direction + (Math.PI / 2)) 'Turn left
            End If

            '-----Adjust Direction-----
            Dim offset As Double = Helm.NormaliseDirection(targetDirection - Direction) 'The offset
            'of the targeted direction relative to the Fleet's current direction
            If offset > Math.PI Then 'Turn back to make it a negative direction
                offset = offset - (2 * Math.PI)
            End If
            If offset > 0 Then 'Turn left
                If offset < TurnSpeed Then 'It's within the turning arc of the Fleet
                    Direction = targetDirection
                Else 'Turn as far as possible
                    Direction = Helm.NormaliseDirection(Direction + TurnSpeed)
                End If
            ElseIf offset < 0 Then 'Turn right
                If offset > -TurnSpeed Then 'It's within the turning arc of the Fleet
                    Direction = targetDirection
                Else 'Turn as far as possible
                    Direction = Helm.NormaliseDirection(Direction - TurnSpeed)
                End If
            End If
            '--------------------------

            '-----Adjust Speed-----
            If targetSpeed > Speed.current Then 'Accelerate to meet the targeted speed
                Speed.current = Speed.current + Acceleration.current 'Accelerate
                If Speed.current > targetSpeed Then 'Turn back to match the targeted speed
                    Speed.current = targetSpeed
                End If
                If Speed.current > Speed.max Then 'Turn back to the maximum speed
                    Speed.current = Speed.max
                End If
            ElseIf targetSpeed < Speed.current Then 'Decelerate to meet the targeted speed
                Speed.current = Speed.current - Acceleration.current 'Decelerate
                If Speed.current < targetSpeed Then 'Turn back to match the targeted speed
                    Speed.current = targetSpeed
                End If
            End If
            '----------------------

            Position = New Point(Position.X + (Math.Cos(Direction) * Speed.current),
                                 Position.Y + (Math.Sin(Direction) * Speed.current)) 'Update the Fleets position
        End If
    End Sub

End Class
