<Serializable()>
Public MustInherit Class Fleet
    Inherits SpaceCraft
    Public currentSector As Sector
    Public ShipList As New List(Of Ship)
    Public Shared ReadOnly TurnSpeed As Double = ((2 * Math.PI) / 90)
    Public Shared ReadOnly InteractRange As Integer = 20
    Private TurnRight = True

    Public Sub New(ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence)
        Randomize()
        MyAllegence = nAllegence
        Index = nIndex
        For i As Integer = 0 To Int(Rnd() * 15) + 14
            Select Case nAllegence
                Case Galaxy.Allegence.Player
                    ShipList.Add(New FriendlyShip(New Clunker, i))
                Case Galaxy.Allegence.Pirate
                    ShipList.Add(New PirateShip(New Clunker, i))
            End Select
        Next
        Position = New Point(Int(Rnd() * SpawnBox), Int(Rnd() * SpawnBox))
    End Sub

    Public Sub RemoveShip(ByRef nShip As Ship)
        For i As Integer = 0 To ShipList.Count
            If ReferenceEquals(nShip, ShipList(i)) = True Then
                ShipList.RemoveAt(i)
                ShipList.TrimExcess()
                Exit Sub
            End If
        Next
    End Sub

    Private Shared Event SetStats()
    Public Shared Sub SetStats_Call()
        RaiseEvent SetStats()
    End Sub
    Private Sub SetStats_Handle() Handles MyClass.SetStats
        If ShipList.Count <> 0 Then
            Dim lowestSpeed As Double
            Dim lowestAcceleration As Double
            For Each i As Ship In ShipList
                If i.Helm.Parent.Speed.max < lowestSpeed Or lowestSpeed = 0 Then
                    lowestSpeed = i.Helm.Parent.Speed.max
                End If
                If i.Acceleration.max < lowestAcceleration Or lowestAcceleration = 0 Then
                    lowestAcceleration = i.Acceleration.max
                End If
            Next
            Speed = New StatDbl(0, lowestSpeed * 0.8)
            Acceleration = New StatDbl(lowestAcceleration, 0)
        ElseIf Dead = False Then
            currentSector.RemoveFleet(Me)
        End If
    End Sub

    Private Shared Event FleetUpdate()
    Public Shared Sub UpdateFleet_Call()
        RaiseEvent FleetUpdate()
    End Sub
    Public Overridable Sub UpdateFleet_Handle() Handles MyClass.FleetUpdate
        If ReferenceEquals(Me, Sector.centerFleet) = False And Dead = False Then
            Dim hasTarget As Boolean = False
            Dim targetDistance As Integer
            Dim targetDirection As Double
            Dim targetSpeed As Integer = 3

            If MyAllegence = Galaxy.Allegence.Neutral Then
                targetSpeed = 0
            End If

            For Each i As Fleet In currentSector.fleetList
                Dim x As Integer = i.Position.X - Position.X
                Dim y As Integer = i.Position.Y - Position.Y
                Dim distance As Integer = Math.Sqrt((x ^ 2) + (y ^ 2))
                If (distance <= targetDistance Or targetDistance = 0) And ReferenceEquals(Me, i) = False Then
                    If distance <= InteractRange Then
                        If i.MyAllegence = Galaxy.Allegence.Neutral Then
                            NeutralFleet.Heal(Me)
                        ElseIf ReferenceEquals(i, Sector.centerFleet) = False And i.MyAllegence <> MyAllegence Then
                            Combat.AutoFight(Me, i)
                            Exit Sub
                        ElseIf i.MyAllegence = MyAllegence And ReferenceEquals(i, Sector.centerFleet) = False Then
                            ShipList.AddRange(i.ShipList)
                            currentSector.RemoveFleet(i)
                            Exit Sub
                        End If
                    Else
                        hasTarget = True
                        targetDistance = distance
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
                    End If
                End If
            Next

            If hasTarget = True Then
                If targetDirection - Direction + (Math.PI / 2) < Math.PI Then
                    targetSpeed = Speed.max
                End If
                If targetDirection - Direction > 0 Then
                    Direction = Direction + TurnSpeed
                    If targetDirection - Direction < 0 Then
                        Direction = targetDirection
                    Else
                        Direction = Helm.NormalizeDirection(Direction)
                    End If
                ElseIf targetDirection - Direction < 0 Then
                    Direction = Direction - TurnSpeed
                    If targetDirection - Direction > 0 Then
                        Direction = targetDirection
                    Else
                        Direction = Helm.NormalizeDirection(Direction)
                    End If
                End If
            Else
                If Int(Rnd() * 30) = 0 Then
                    If TurnRight = True Then
                        TurnRight = False
                    Else
                        TurnRight = True
                    End If
                End If
                If TurnRight = True Then
                    Direction = Direction + TurnSpeed
                Else
                    Direction = Direction - TurnSpeed
                End If
                Direction = Helm.NormalizeDirection(Direction)
            End If
            If Speed.current < targetSpeed Then
                Speed.current = Speed.current + Acceleration.current
                If Speed.current > targetSpeed Then
                    Speed.current = targetSpeed
                End If
            ElseIf Speed.current > targetSpeed Then
                Speed.current = Speed.current - Acceleration.current
                If Speed.current < targetSpeed Then
                    Speed.current = targetSpeed
                End If
            End If
        ElseIf Dead = False Then
            For Each i As Fleet In currentSector.fleetList
                Dim distance As Integer = Math.Sqrt(((i.Position.X - Position.X) ^ 2) + ((i.Position.Y - Position.Y) ^ 2))
                If distance <= InteractRange And ReferenceEquals(i, Me) = False Then
                    Select Case i.MyAllegence
                        Case Galaxy.Allegence.Pirate
                            Combat.Generate(i)
                        Case Galaxy.Allegence.Neutral
                            NeutralFleet.Heal(Me)
                        Case Galaxy.Allegence.Player
                            ShipList.AddRange(i.ShipList)
                            currentSector.RemoveFleet(i)
                            Exit Sub
                    End Select
                End If
            Next
        End If

        Position = New Point(Position.X + (Math.Cos(Direction) * Speed.current),
                             Position.Y + (Math.Sin(Direction) * Speed.current))
    End Sub

End Class
