<Serializable()>
Public Class Helm 'Controls the Steering and Speed of the Ship
    Inherits Station
    Public TurnSpeed As StatDbl 'A StatDbl object representing the current and maximum values for the Ship's turn speed
    Private brakes As Boolean = False 'A Boolean value indecating whether the Ship is braking or accelerating to avoid a persueer
    Public Shared ReadOnly MinimumSpeed As Integer = 5 'The minimum speed a Ship is allowed to move at
    Public Shared ReadOnly MinimumDistance As Integer = 60 'The minimum distance allowed between one Ship and another
    Public Target As Ship 'The Ship object that is being targeted by the Helm
    Public evasion As Int16 'A 16 bit integer representing which direction the Ship should turn to avoid a collision
    Public MatchSpeed As Boolean = False 'A boolean value indecation whether or not the Ship should be trying to match it's target's speed
    Public Enum Commands
        ThrottleUp
        ThrottleDown
        TurnLeft
        TurnRight
        WarpDrive
        MatchSpeed
    End Enum

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If PlayerControled = False And ConsoleWindow.GameServer.GameWorld.Warping <> Galaxy.Warp.Warping Then 'This station is AI controlled
            'and the player is not in 'Warp'
            Dim targetDirection As Double 'The direction of the target relative to the Ship in world space
            Dim finalSpeed As Double = MinimumSpeed 'Set the default speed for the Ship

            '-----Set Target Direction and Distance-----
            If Target IsNot Nothing And Target.Dead = False Then 'There is a target to point towards
                Dim Y As Double = (Target.Position.Y + (Math.Sin(Target.Direction) * Target.Helm.Parent.Speed.current)) -
                    Parent.Position.Y 'The Y coordinate of the target relative to the Ship in one update
                Dim X As Double = (Target.Position.X + (Math.Cos(Target.Direction) * Target.Helm.Parent.Speed.current)) -
                    Parent.Position.X 'The X coordinate of the target relative to the Ship in one update
                Dim distance As Integer = Math.Sqrt((Y ^ 2) + (X ^ 2)) 'The distance of the target relative to the Ship
                'in one update

                If X <> 0 Then 'There will not be a divide by 0 error
                    targetDirection = Math.Tanh(Y / X) 'Calculate the targets direction
                    If X < 0 Then 'The calculation produced a direction reflected in a Y axis
                        targetDirection = targetDirection + Math.PI
                    End If
                    targetDirection = NormaliseDirection(targetDirection) 'Normalise the direction to be between 0 and 2*Pi
                ElseIf Y > 0 Then 'The target is directly above the Ship
                    targetDirection = Math.PI / 2
                Else 'The target is directly bellow the Ship
                    targetDirection = (3 * Math.PI) / 2
                End If
                '-------------------------------------------

                '-----Speed-----
                If NormaliseDirection(targetDirection - Parent.Direction) > (3 * Math.PI) / 4 And
                    NormaliseDirection(targetDirection - Parent.Direction) < (5 * Math.PI) / 4 And
                    distance < 200 Then 'The enemy is behind you and too close
                    '-----Steering-----
                    Dim turnSide = Helm.NormaliseDirection(-(targetDirection - Parent.Direction)) 'If in the first quadrant turn right
                    If turnSide < Math.PI Then
                        targetDirection = Helm.NormaliseDirection(Parent.Direction - (Math.PI / 2)) 'Turn left
                    Else
                        targetDirection = Helm.NormaliseDirection(Parent.Direction + (Math.PI / 2)) 'Turn right
                    End If
                    '------------------

                    '-----Speed-----
                    If brakes = True And 0 = Int(30 * Rnd()) Then 'Accelerate
                        brakes = False
                    ElseIf 0 = Int(50 * Rnd()) Then 'Decelerate
                        brakes = True
                    End If
                    If brakes = False Then 'Set the final speed to be the maximum speed
                        finalSpeed = Parent.Speed.max
                    End If
                    '---------------
                ElseIf Parent.Speed.current <> Target.Helm.Parent.Speed.current And
                    distance - Helm.MinimumDistance < (((Parent.Speed.current - Target.Speed.current) ^ 2) / Parent.Acceleration.current) And
                    distance > MinimumDistance And
                    targetDirection - Parent.Direction < Math.PI / 2 And
                    targetDirection - Parent.Direction > -Math.PI / 2 Then 'Match the enemies speed
                    finalSpeed = Target.Helm.Parent.Speed.current
                ElseIf distance < MinimumDistance Then 'Turn away from the target
                    If targetDirection - Parent.Direction > 0 Then 'Turn right
                        targetDirection = Helm.NormaliseDirection(Parent.Direction + (3 * Math.PI / 2))
                    Else 'Turn left
                        targetDirection = Helm.NormaliseDirection(Parent.Direction + Math.PI)
                    End If
                ElseIf distance > (((Parent.Speed.current - Target.Speed.current) ^ 2) / Parent.Acceleration.current) And
                    targetDirection - Parent.Direction < Math.PI / 2 And
                    targetDirection - Parent.Direction > -Math.PI / 2 Then 'Charge the enemy
                    finalSpeed = Parent.Speed.max
                End If
                '---------------
            Else
                Target = Nothing
            End If

            '-----Evade-----
            If evasion <> 0 Then 'Evade collisions
                If evasion < 0 Then
                    targetDirection = Helm.NormaliseDirection(Parent.Direction - (Math.PI / 2)) 'Turn left
                Else
                    targetDirection = Helm.NormaliseDirection(Parent.Direction + (Math.PI / 2)) 'Turn right
                End If
            End If
            evasion = 0 'Clear the evasion
            '---------------

            '-----Steering and Speed-----
            If Math.Sqrt((targetDirection - Parent.Direction) ^ 2) < TurnSpeed.current Then 'The target is within the Ship's abilty to turn
                'to face it in this update
                Parent.Direction = targetDirection
            ElseIf NormaliseDirection(targetDirection - Parent.Direction) < Math.PI Then 'Turn left to face it
                Parent.Direction = NormaliseDirection(Parent.Direction + TurnSpeed.current)
            Else 'Turn right to face it
                Parent.Direction = NormaliseDirection(Parent.Direction - TurnSpeed.current)
            End If

            If Parent.Speed.current < finalSpeed Then 'Accelerate to meet the targeted speed
                Parent.Speed.current = Parent.Speed.current + Parent.Acceleration.current
                If Parent.Speed.current > finalSpeed Then 'Turn back the speed to the targeted speed
                    Parent.Speed.current = finalSpeed
                End If
            Else 'Decelerate to meet the targeted speed
                Parent.Speed.current = Parent.Speed.current - Parent.Acceleration.current
                If Parent.Speed.current < finalSpeed Then 'Turn back the speed to the Targeted speed
                    Parent.Speed.current = finalSpeed
                End If
            End If
            '----------------------------
        Else 'The Helm is player controlled
            '-----Match Enemies Speed-----
            If MatchSpeed = True And Target IsNot Nothing Then 'Change the speed to match the targets speed
                If Parent.Speed.current > Target.Speed.current Then 'Decelerate to match the targets speed
                    Parent.Speed.current = Parent.Speed.current - Parent.Acceleration.current
                    If Parent.Speed.current < Target.Speed.current Then 'Turn back to match the targets speed
                        Parent.Speed.current = Target.Speed.current
                    End If
                ElseIf Parent.Speed.current < Target.Speed.current Then 'Accelerate to match the targets speed
                    Parent.Speed.current = Parent.Speed.current + Parent.Acceleration.current
                    If Parent.Speed.current > Target.Speed.current Then 'Turn back to match the targets speed
                        Parent.Speed.current = Target.Speed.current
                    End If
                    If Parent.Speed.current > Parent.Speed.max Then 'Turn the speed back to the maximum possible speed
                        Parent.Speed.current = Parent.Speed.max
                    End If
                End If
            Else 'The Ship should not try to match a targets speed
                MatchSpeed = False
            End If
            '-----------------------------
        End If
    End Sub

    Public Shared Function NormaliseDirection(ByVal nDirecion As Double) As Double 'A Shared Function that takes a radian and normalises it
        'to be within the bounds of 0 and 2*Pi
        nDirecion = nDirecion Mod 2 * Math.PI 'Returns the remainder after the direction is divided by 2*Pi
        If nDirecion < 0 Then 'The radian is within the 3 or 4 quadrant but is currently a negative
            nDirecion = nDirecion + (2 * Math.PI) 'Make the radian positive
        End If
        Return nDirecion 'Return the new direction
    End Function

End Class
