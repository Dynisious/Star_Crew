Public Class Battery 'Object responsible for Aiming and firing the Ship's Weapons and targeting the Closest enemy and detecting upcoming collisions
    Inherits ShipStation
    Public Powered As Boolean = True 'A Boolean value indicating whether or not the Station is receiving power

    Public Overrides Sub Update() 'Selects the Closest Enemy, aims the Weapons and Fires the Weapons if necessary and detects upcomming collisions collisions
        '-----Select Target-----
        Dim evadeCount As Integer = 0 'The difference of how many Ship's need to be evaded to the right or left (negative = right)
        Dim targetDistance As Integer = -1 'The last distance calculated so to compare against other distances so as to know whether to change the target
        Dim targetDirection As Double = -1 'The direction of the targeted enemy in radians in object space
        For Each i As Ship In Server.GameWorld.Combat.Combatants 'Loop through all Ships
            If i.CombatIndex <> ParentShip.CombatIndex Then 'This Ship is not targeting itself
                Dim xOffset As Integer = (i.X - ParentShip.X) 'The offset of the other Ship from this Ship on the x-axis
                Dim yOffset As Integer = (i.Y - ParentShip.Y) 'The offset of the other Ship from this Ship on the y-axis
                Dim distance As Integer = Math.Sqrt((xOffset ^ 2) + (yOffset ^ 2)) 'The distance of the other Ship from the enemy Ship
                If distance < (2 * ParentShip.MinimumDistance) Then 'This Ship needs to be evaded
                    If xOffset <> 0 Then 'They are not aligned on the x-axis
                        Dim direction As Double = Math.Tanh(yOffset / xOffset) 'Calculate the direction of the Ship in world space
                        If xOffset < 0 Then 'The direction is reflected in the lin y=x
                            direction = direction + Math.PI 'Reflect
                        End If
                        direction = Server.Normalise_Direction(direction - ParentShip.Direction) 'Get the direction of the Ship in object space
                        If direction < Math.PI Then 'They are to the Left
                            evadeCount = evadeCount - 1 'Evade right
                        Else 'They are to the right
                            evadeCount = evadeCount + 1 'Evade left
                        End If
                    Else 'They are aligned on the x-axis
                        evadeCount = evadeCount - 1 'Evade right
                    End If
                End If
                If ((distance < targetDistance) Or (targetDistance = -1)) And (i.myAllegiance <> ParentShip.myAllegiance) Then 'The Ship is either closer or there is no targeted Ship yet and the Ship is an enemy
                    ParentShip.target = i.CombatIndex 'Set the new target
                    targetDistance = distance 'Set the new targetDistance
                    If xOffset <> 0 Then 'The Ships are not aligned on the x-axis

                    ElseIf yOffset > 0 Then 'The enemy is directly above the Ship

                    Else 'The enemy is directly bellow the Ship

                    End If
                End If
            End If
        Next
        If evadeCount < 0 Then 'Evade right
            ParentShip.Bridge.EvadeDirection = -1
        ElseIf evadeCount > 0 Then 'Evade left
            ParentShip.Bridge.EvadeDirection = 1
        End If
        '-----------------------

        '-----Fire at target-----
        If targetDistance <> -1 Then
        End If
        '------------------------
    End Sub

End Class
