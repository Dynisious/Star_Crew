Public Class Battery 'Object responsible for Aiming and firing the Ship's Weapons and targeting the Closest enemy and detecting upcoming collisions
    Inherits ShipStation

    Public Sub New(ByRef nParent As Ship, ByVal nIntegrity As Game_Library.StatInt, ByVal nRepairCost As Double)
        MyBase.New(nParent, nIntegrity, nRepairCost)
    End Sub

    Protected Overrides Sub Finalise_Destroy() 'Removes all references to the Battery object
        ParentShip.Batteries = Nothing 'Remove the reference
    End Sub

    Public Overrides Sub Update() 'Selects the Closest Enemy, aims the Weapons and Fires the Weapons if necessary and detects upcomming collisions collisions
        If Powered = True Then
            '-----Select Target-----
            Dim evadeCount As Integer = 0 'The difference of how many Ship's need to be evaded to the right or left (negative = right)
            Dim enemyDistances As New List(Of Integer) 'A list of Integer values representing how close the enemy is to the Ship in ascending order
            Dim enemyDirections As New List(Of Double) 'A list of Double values representing the direction of the enemy in object space, ordered by how close they are
            Dim enemyIndexes As New List(Of Integer) 'A list of Integer values representing the CombatIndex of the enemy, ordered by how close they are
            For Each i As Ship In Server.GameWorld.Combat.Combatants 'Loop through all Fleets
                If i.CombatIndex <> ParentShip.CombatIndex Then 'This Ship is not targeting itself
                    '-----Calculate Distance and direction------
                    Dim xOffset As Integer = (i.X - ParentShip.X) 'The offset of the other Ship from this Ship on the x-axis
                    Dim yOffset As Integer = (i.Y - ParentShip.Y) 'The offset of the other Ship from this Ship on the y-axis
                    Dim distance As Integer = Math.Sqrt((xOffset ^ 2) + (yOffset ^ 2)) 'The distance of the other Ship from the enemy Ship
                    Dim direction As Double 'The direction of the other Ship in object space
                    If xOffset <> 0 Then 'They are not aligned on the x-axis
                        direction = Math.Atan(yOffset / xOffset) 'Calculate the direction of the Ship in world space
                        If Math.Sign(xOffset) = -1 Then 'The direction is reflected in the line Y=X
                            direction = direction + Math.PI 'Reflect
                        End If
                    ElseIf yOffset > 0 Then 'The other Ship is directly above this Ship
                        direction = (Math.PI / 2)
                    Else 'The other Ship is directly bellow this Ship
                        direction = (3 * Math.PI / 2)
                    End If
                    direction = Server.Normalise_Direction(direction - ParentShip.Direction) 'Get the direction of the Ship in object space
                    '-------------------------------------------

                    '-----Detect Collisions-----
                    If distance < (2 * ParentShip.MinimumDistance) Then 'This Ship needs to be evaded
                        If direction < Math.PI Then 'They are to the Left
                            evadeCount = evadeCount - 1 'Evade right
                        Else 'They are to the right
                            evadeCount = evadeCount + 1 'Evade left
                        End If
                    End If
                    '--------------------------

                    If i.ParentFleet.myAllegiance <> ParentShip.ParentFleet.myAllegiance Then 'The Ship is an enemy
                        If enemyDistances.Count <> 0 Then 'Theirs at least one distance in the lists
                            For e As Integer = 0 To enemyDistances.Count - 1 'Loop through all the distances
                                If enemyDistances(e) > distance Then 'Insert at this index
                                    enemyDistances.Insert(e, distance) 'Insert the distance
                                    enemyDirections.Insert(e, direction) 'Insert the direction
                                    enemyIndexes.Insert(e, i.CombatIndex) 'Insert the index
                                    Exit For
                                End If
                            Next
                        Else 'The lists are empty
                            enemyDistances.Add(distance) 'Add the distance
                            enemyDirections.Add(direction) 'Add the direction
                            enemyIndexes.Add(i.CombatIndex) 'add the index
                        End If
                    End If
                End If
            Next
            ParentShip.Bridge.EvadeDirection = Math.Sign(evadeCount) 'Decide which way to evade; 1 is right, -1 is left and 0 is none
            ParentShip.target = enemyIndexes(0) 'Set the new target
            ParentShip.targetDirection = Server.Normalise_Direction(enemyDirections(0) + ParentShip.Direction) 'Convert the direction to world space set the new direction
            ParentShip.targetDistance = enemyDistances(0) 'Set the targets distance
            '-----------------------

            '-----Fire Weapons-----
            For Each i As WeaponMount In ParentShip.Mounts 'Loop through all mounts
                If i.MountedWeapon IsNot Nothing Then 'There's a weapon mounted
                    If AIControled = True Or i.MountedWeapon.Index <> Server.GameWorld.ClientInteractions.SelectedWeapon Then 'The AI is in control of all Weapons or it is not the Weapon currently being controlled by a player
                        For e As Integer = 0 To enemyDirections.Count - 1 'loop through all directions
                            Dim relativeDirection As Double = Server.Normalise_Direction(enemyDirections(e) - i.Offset - i.Sweep.Minimum) 'The direction of the enemy relative to the rightmost edge of the Weapon's field of view
                            If relativeDirection < (i.Sweep.Maximum - i.Sweep.Minimum) Then 'This enemy is the closest enemy within the field of view
                                Dim turnOffset As Double = enemyDirections(e) - i.Offset - i.Sweep.Current 'How far the Weapon needs to turn to face the enemy
                                Dim turnRight As Boolean = False 'A Boolean value indicating whether the Weapon should turn right
                                If turnOffset < 0 Then 'Make sure it is positive
                                    turnOffset = -turnOffset
                                    turnRight = True
                                End If
                                If turnOffset < i.MountedWeapon.TurningSpeed Then 'The Weapon can turn to face the enemy
                                    i.Sweep.Current = enemyDirections(e) - i.Offset 'Turn to face the enemy
                                    i.MountedWeapon.Fire_Weapon(Server.GameWorld.Combat.Combatants(enemyIndexes(e)), Server.Normalise_Direction(enemyDirections(e) + ParentShip.Direction), enemyDistances(e)) 'Fire at the enemy
                                ElseIf turnRight = False Then 'The enemy is to the left
                                    i.Sweep.Current = i.Sweep.Current + i.MountedWeapon.TurningSpeed 'Turn left
                                Else 'The enemy is to the right
                                    i.Sweep.Current = i.Sweep.Current - i.MountedWeapon.TurningSpeed 'Turn right
                                End If
                            End If
                        Next
                    End If
                End If
            Next
            '----------------------
        End If
    End Sub

End Class
