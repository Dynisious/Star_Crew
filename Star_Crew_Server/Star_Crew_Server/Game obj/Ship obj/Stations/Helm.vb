Public Class Helm 'Object that controls the throttle and direction of the Ship
    Inherits ShipStation
    Public EvadeDirection As Int16 = 0 'An Int16 value that indicates whether to avoid a collision/enemy to the right or left
    Public Braking As Boolean = False 'A Boolean value indecating whether the Ship is meant to Decelerate to escape an enemy
    Private Enum Actions 'An enumerator of the different Actions the Helm can take
        Evade 'Avoid an upcomming collision
        Run 'Run away from a pursurer
        Charge 'Charge at the targetd enemy
        Track 'Pursue the targeted enemy at a effective distance
        max 'Nothing
    End Enum

    Public Overrides Sub Update() 'Updates the Ship's Throttle and direction
        If AIControled = True Then 'Run AI
            Dim action As Actions = Actions.max 'The action the Ship is performing this update
            Dim enemy As Ship = Server.GameWorld.Combat.Combatants(ParentShip.target) 'A reference to the currently targeted Ship
            Dim enemyDirection As Double 'The direction in world space of the enemy relative to the Ship
            '-----Choose Action-----
            If EvadeDirection = 0 Then 'The Ship does not need to evade
                '-----Set Initial Values-----
                Dim xOffset As Integer = (enemy.X - ParentShip.X) 'The offset of the enemy relative to the Ship on the x-axis
                Dim yOffset As Integer = (enemy.X - ParentShip.X) 'The offset of the enemy relative to the Ship on the y-axis
                Dim enemyDistance As Integer = Math.Sqrt((xOffset ^ 2) + (yOffset ^ 2)) 'The distance of the enemy from the Ship
                If enemy.X <> ParentShip.X Then 'They are not verticaly aligned
                    enemyDirection = Math.Tanh(yOffset / xOffset) 'Calculate enemyDirection
                    If xOffset < 0 Then 'enemyDirection is reflected in the line y=x
                        enemyDirection = enemyDirection + Math.PI 'Rotate the direction a half turn
                    End If
                    enemyDirection = Server.Normalise_Direction(enemyDirection) 'Normalises the direction to be within the range of 0-2*Pi
                ElseIf enemy.Y > ParentShip.Y Then 'The enemy is directly above the Ship
                    enemyDirection = Math.PI / 2
                Else 'The enemy is directly bellow the Ship
                    enemyDirection = 3 * Math.PI / 2
                End If
                '----------------------------

                If Normalise_Direction(enemyDirection - (3 * Math.PI / 4)) <= (Math.PI / 2) And
                    enemyDistance <= 200 Then
                    action = Actions.Run
                End If

                If action = Actions.max Then 'Continue choosing an action
                    If enemyDistance > 2 * ParentShip.MinimumDistance Then 'The enemy is far away
                        action = Actions.Charge
                    Else 'The enemy is within range
                        action = Actions.Track
                    End If
                End If
            Else 'The Ship needs to evade
                action = Actions.Evade
            End If
            '-----------------------

            '-----Perform Action-----
            Select Case action
                Case Actions.Evade 'Evade the up-coming collision
                    ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction + (ParentShip.TurnSpeed * EvadeDirection)) 'Turn to the right or left accordingly
                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current - ParentShip.Acceleration 'Decelerate
                    If ParentShip.Engineering.Throttle.Current < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum
                    End If
                    EvadeDirection = 0 'Reset the evade direction
                Case Actions.Run 'Run away from the pursuer
                    If Server.Normalise_Direction(enemyDirection - ParentShip.Direction) < Math.PI Then 'Turn left to evade them
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction + ParentShip.TurnSpeed) 'Turn left
                    Else 'Turn right to evade them
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction - ParentShip.TurnSpeed) 'Turn right
                    End If
                    If Int(Rnd() * 50) = 0 Then 'Switch from braking to accelerating
                        If Braking = True Then 'Accelerate
                            Braking = False
                        Else 'Decelerate
                            Braking = True
                        End If
                    End If
                    If Braking = True Then
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current - ParentShip.Acceleration 'Decelerate
                        If ParentShip.Engineering.Throttle.Current < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum
                        End If
                    Else
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current + ParentShip.Acceleration 'Accelerate
                        If ParentShip.Engineering.Throttle.Current > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is more than the maximum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Set the throttle to the maximum
                        End If
                    End If
                Case Actions.Charge 'Charge at the enemy
                    If Server.Normalise_Direction(enemyDirection - ParentShip.Direction + (Math.PI / 2)) <= Math.PI Then 'The enemy is in front of the Ship
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current + ParentShip.Acceleration 'Accelerate
                        If ParentShip.Engineering.Throttle.Current > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is more than the maximum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Set the throttle to the maximum
                        End If
                    Else 'The enemy is behind the Ship
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current - ParentShip.Acceleration 'Accelerate
                        If ParentShip.Engineering.Throttle.Current < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum
                        End If
                    End If
                    Dim turnOffset As Double = (enemyDirection - ParentShip.Direction) 'How far the Ship needs to turn
                    If turnOffset < 0 Then turnOffset = -turnOffset 'Make the radian positive
                    If turnOffset <= ParentShip.TurnSpeed Then 'Their within turning range
                        ParentShip.Direction = enemyDirection 'Turn to face the enemy
                    ElseIf Server.Normalise_Direction(enemyDirection - ParentShip.Direction) < Math.PI Then 'Turn left
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction + ParentShip.TurnSpeed) 'Turn left
                    Else 'Turn right
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction - ParentShip.TurnSpeed) 'Turn right
                    End If
                Case Actions.Track 'Track with the enemy and attack them
                    Dim speedOffset As Double = (enemy.Speed - ParentShip.Speed)
                    If speedOffset < 0 Then speedOffset = -speedOffset 'Make the speed positive
                    If speedOffset <= ((ParentShip.Engineering.Integrety.Current / ParentShip.Engineering.Integrety.Maximum) * ParentShip.Acceleration) Then 'The Ship can match the enemy's speed
                        ParentShip.Engineering.Throttle.Current = (enemy.Speed / (ParentShip.Engineering.Integrety.Current / ParentShip.Engineering.Integrety.Maximum)) 'Change throttle to match the enemy's speed
                        If ParentShip.Engineering.Throttle.Current > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is above the maximum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Turn back to maximum throttle
                        ElseIf ParentShip.Engineering.Throttle.Current < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is bellow the minimum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Turn back to minimum throttle
                        End If
                    ElseIf enemy.Speed > ParentShip.Speed Then 'Accelerate to match speed with the enemy
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current + ParentShip.Acceleration 'Accelerate
                        If ParentShip.Engineering.Throttle.Current > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is above the maximum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Turn back to maximum throttle
                        End If
                    Else 'Decelerate to match speed with the enemy
                        ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Current - ParentShip.Acceleration 'Decelerate
                        If ParentShip.Engineering.Throttle.Current < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is bellow the minimum
                            ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Turn back to minimum throttle
                        End If
                    End If
                    Dim turnOffset As Double = (enemyDirection - ParentShip.Direction) 'How far the Ship needs to turn
                    If turnOffset < 0 Then turnOffset = -turnOffset 'Make the radian positive
                    If turnOffset <= ParentShip.TurnSpeed Then 'Their within turning range
                        ParentShip.Direction = enemyDirection 'Turn to face the enemy
                    ElseIf Server.Normalise_Direction(enemyDirection - ParentShip.Direction) < Math.PI Then 'Turn left
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction + ParentShip.TurnSpeed) 'Turn left
                    Else 'Turn right
                        ParentShip.Direction = Server.Normalise_Direction(ParentShip.Direction - ParentShip.TurnSpeed) 'Turn right
                    End If
            End Select
            '------------------------
        Else 'Run Player support

        End If
    End Sub

End Class
