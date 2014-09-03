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

    Public Sub New(ByRef nParent As Ship, ByVal nIntegrity As Game_Library.StatInt, ByVal nRepairCost As Double)
        MyBase.New(nParent, nIntegrity, nRepairCost)
    End Sub

    Protected Overrides Sub Finalise_Destroy() 'Removes all references to the Helm object
        ParentShip.Bridge = Nothing 'Remove the reference
    End Sub

    Public Overrides Sub Update() 'Updates the Ship's Throttle and direction
        If Powered = True Then 'The Helm can opperate
            If AIControled = True Then
                If ParentShip.target <> -1 Then 'Run AI
                    Dim action As Actions = Actions.max 'The action the Ship is performing this update
                    Dim enemy As Ship = Server.GameWorld.Combat.Combatants(ParentShip.target) 'A reference to the currently targeted Ship
                    '-----Choose Action-----
                    If EvadeDirection = 0 Then 'The Ship does not need to evade
                        If Normalise_Direction(ParentShip.targetDirection - (3 * Math.PI / 4)) <= (Math.PI / 2) And
                            ParentShip.targetDistance <= 200 Then
                            action = Actions.Run
                        End If

                        If action = Actions.max Then 'Continue choosing an action
                            If ParentShip.targetDistance > 2 * ParentShip.MinimumDistance Then 'The enemy is far away
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
                            ParentShip.Direction = ParentShip.Direction + (ParentShip.TurnSpeed * EvadeDirection) 'Turn to the right or left accordingly
                            Dim temp As Double = ParentShip.Engineering.Throttle.Current - ParentShip.Engineering.Acceleration 'Calculate the throttle
                            If temp < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                                ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum
                            Else 'The throttle will be within the bounds
                                ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                            End If
                            EvadeDirection = 0 'Reset the evade direction
                        Case Actions.Run 'Run away from the pursuer
                            If Server.Normalise_Direction(ParentShip.targetDirection - ParentShip.Direction) < Math.PI Then 'Turn left to evade them
                                ParentShip.Direction = ParentShip.Direction + ParentShip.TurnSpeed 'Turn left
                            Else 'Turn right to evade them
                                ParentShip.Direction = ParentShip.Direction - ParentShip.TurnSpeed 'Turn right
                            End If
                            If Int(Rnd() * 100) = 0 Then 'Switch from braking to accelerating
                                If Braking = True Then 'Accelerate
                                    Braking = False
                                Else 'Decelerate
                                    Braking = True
                                End If
                            End If
                            If Braking = True Then
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current - ParentShip.Engineering.Acceleration 'Calculate the throttle
                                If temp < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum'
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp
                                End If
                            Else
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current + ParentShip.Engineering.Acceleration 'Calculate the throttle
                                If temp > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is more than the maximum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Set the throttle to the maximum
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp
                                End If
                            End If
                        Case Actions.Charge 'Charge at the enemy
                            If Server.Normalise_Direction(ParentShip.targetDirection - ParentShip.Direction + (Math.PI / 2)) <= Math.PI Then 'The enemy is in front of the Ship
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current + ParentShip.Engineering.Acceleration 'Calculate how much to accelerate
                                If temp > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is more than the maximum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Set the throttle to the maximum
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                                End If
                            Else 'The enemy is behind the Ship
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current - ParentShip.Engineering.Acceleration 'Calculate how much to decelerate
                                If temp < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is less than the minimum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Set the throttle to the minimum
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                                End If
                            End If
                            Dim turnOffset As Double = (ParentShip.targetDirection - ParentShip.Direction) 'How far the Ship needs to turn
                            Dim turnRight As Boolean = False 'A Boolean value representing whether the Ship should turn right
                            If turnOffset < 0 Then 'Make the radian positive
                                turnOffset = -turnOffset
                                turnRight = True
                            End If
                            If turnOffset <= ParentShip.TurnSpeed Then 'Their within turning range
                                ParentShip.Direction = ParentShip.targetDirection 'Turn to face the enemy
                            ElseIf turnRight = False Then 'Turn left
                                ParentShip.Direction = ParentShip.Direction + ParentShip.TurnSpeed 'Turn left
                            Else 'Turn right
                                ParentShip.Direction = ParentShip.Direction - ParentShip.TurnSpeed 'Turn right
                            End If
                        Case Actions.Track 'Track with the enemy and attack them
                            Dim speedOffset As Double = (enemy.Speed - ParentShip.Speed)
                            If speedOffset < 0 Then speedOffset = -speedOffset 'Make the speed positive
                            If speedOffset <= ((ParentShip.Engineering.Integrity.Current / ParentShip.Engineering.Integrity.Maximum) * ParentShip.Engineering.Acceleration) Then 'The Ship can match the enemy's speed
                                Dim temp As Double = (enemy.Speed / (ParentShip.Engineering.Integrity.Current / ParentShip.Engineering.Integrity.Maximum)) 'Calculate the throttle
                                If temp > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is above the maximum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Turn back to maximum throttle
                                ElseIf temp < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is bellow the minimum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Turn back to minimum throttle
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                                End If
                            ElseIf enemy.Speed > ParentShip.Speed Then 'Accelerate to match speed with the enemy
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current + ParentShip.Engineering.Acceleration 'Accelerate
                                If temp > ParentShip.Engineering.Throttle.Maximum Then 'The throttle is above the maximum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Maximum 'Turn back to maximum throttle
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                                End If
                            Else 'Decelerate to match speed with the enemy
                                Dim temp As Double = ParentShip.Engineering.Throttle.Current - ParentShip.Engineering.Acceleration 'Calculate the throttle
                                If temp < ParentShip.Engineering.Throttle.Minimum Then 'The throttle is bellow the minimum
                                    ParentShip.Engineering.Throttle.Current = ParentShip.Engineering.Throttle.Minimum 'Turn back to minimum throttle
                                Else 'The throttle will be within the bounds
                                    ParentShip.Engineering.Throttle.Current = temp 'Set the throttle
                                End If
                            End If
                            Dim turnOffset As Double = (ParentShip.targetDirection - ParentShip.Direction) 'How far the Ship needs to turn
                            Dim turnRight As Boolean = (Math.Sign(turnOffset) = -1) 'A Boolean value representing whether the Ship should turn right
                            turnOffset = turnOffset * Math.Sign(turnOffset) 'Make sure turnOffset is positive
                            If turnOffset <= ParentShip.TurnSpeed Then 'Their within turning range
                                ParentShip.Direction = ParentShip.targetDirection 'Turn to face the enemy
                            ElseIf turnRight = False Then 'Turn left
                                ParentShip.Direction = ParentShip.Direction + ParentShip.TurnSpeed 'Turn left
                            Else 'Turn right
                                ParentShip.Direction = ParentShip.Direction - ParentShip.TurnSpeed 'Turn right
                            End If
                    End Select
                    '------------------------
                End If
            Else 'Run Player support
                Dim a = 1
            End If
        End If
    End Sub

End Class
