Public Class PlayerShip
    Inherits Ship
    Private Client As ServerClient 'A reference to the ServerClient object in control of the Ship

    Public Sub New(ByRef nClient As ServerClient)
        Client = nClient
        _Gun = New Rattler(Me)
    End Sub

    Public Overrides Sub Destroy()
        MyBase.Destroy()
        Client = Nothing
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        If Client IsNot Nothing Then 'Change the Ship
            If Client.throttleUp Then Throttle.Current += Acceleration
            If Client.throttleDown Then Throttle.Current -= Acceleration
            If Client.turnRight Then Direction += TurnSpeed
            If Client.turnLeft Then Direction -= TurnSpeed
            If Client.fireWeapons Then
                Dim inLine As New List(Of Ship) With {.Capacity = Server.Combat.ShipList.Count - 1} 'A List of Ship objects that are in the line with the weapon's round in order of closest to furthest
                Dim distances As New List(Of Integer) With {.Capacity = Server.Combat.ShipList.Count - 1} 'A List of Integers that representing the ranges of the Ship objects in inLine
                For i As Integer = 0 To Server.Combat.ShipList.Count - 1
                    If Server.Combat.ShipList(i).CombatIndex <> CombatIndex Then 'It is not targeting itself
                        Dim targetX As Integer = Server.Combat.ShipList(i).X - X 'The x coord of the target relative to the Ship
                        Dim targetY As Integer = Server.Combat.ShipList(i).Y - Y 'The y coord of the target relative to the Ship
                        Dim targetDistance As Integer = Math.Sqrt((targetX ^ 2) + (targetY ^ 2)) 'The distance from the center of the Ship to the center of the object
                        Dim targetDirection As Double = Math.Atan2(targetY, targetX) 'Get the direction to the target in world space
                        Dim hitDistance As Integer = targetDistance -
                            Server.Combat.ShipList(i).Get_Collision_Radia(targetDirection) 'Get the distance to hit the target
                        If hitDistance < Gun.Range Then 'It's possible to hit the target
                            Dim cord As Integer = Server.Combat.ShipList(i).Get_FOV_Cord(targetDirection) 'Get half of the cord that extends across the FOV
                            Dim FOVAngle As Double = Math.Asin(cord / targetDistance) 'Get half the angle that the object takes up of the Ship's FOV
                            If (Server.Normalise_Direction(targetDirection - Direction + FOVAngle) < (2 * FOVAngle)) Then 'The Ship can Shoot the target
                                If inLine.Count = 0 Then 'This is the first target
                                    inLine.Add(Server.Combat.ShipList(i)) 'Add the target to the list
                                    distances.Add(targetDistance) 'Add the distance to the list
                                ElseIf hitDistance < distances(0) Then 'It is the closest viable target
                                    inLine.Insert(0, Server.Combat.ShipList(i)) 'Insert the target at the front of the List
                                    distances.Insert(0, targetDistance) 'Record the distance
                                Else 'Insert it at the apropriate index
                                    For index As Integer = 0 To distances.Count - 1 'Loop through all targets
                                        If distances(index) > targetDistance Then 'This target is closer
                                            inLine.Insert(index, Server.Combat.ShipList(i)) 'Insert the target
                                            distances.Insert(index, targetDistance) 'Insert the distance
                                            Exit For
                                        End If
                                        If index = distances.Count - 1 Then 'This is the furthest
                                            inLine.Add(Server.Combat.ShipList(i)) 'Insert the target
                                            distances.Add(targetDistance) 'Insert the distance
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    End If
                Next
                If inLine.Count <> 0 Then 'Target(s) was/were found
                    Gun.Fire(inLine.ToArray) 'Damage the target
                End If
            End If
        End If
    End Sub

End Class
