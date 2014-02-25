Public Class Helm
    Inherits Station
    Public Direction As Double
    Public TurnSpeed As Stat
    Public Throttle As Stat
    Public Acceleration As Stat
    Private EvadeRight As Boolean = True
    Private Brakes As Boolean = False

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If playerControled = False Then
            Dim opposite As Double = (Parent.Target.Position.Y + (Math.Sin(Parent.Target.Helm.Direction) * Parent.Target.Helm.Throttle.current)) - Parent.Position.Y
            Dim tangent As Double = (Parent.Target.Position.X + (Math.Cos(Parent.Target.Helm.Direction) * Parent.Target.Helm.Throttle.current)) - Parent.Position.X
            Dim distance As Integer = Math.Sqrt((opposite * opposite) + (tangent * tangent))
            Dim targetDirection As Double

            If tangent <> 0 Then
                targetDirection = Math.Tanh(opposite / tangent)
                If tangent < 0 Then
                    targetDirection = targetDirection + Math.PI
                End If
                targetDirection = NormalizeDirection(targetDirection)

                '-----Speed-----
                If NormalizeDirection(targetDirection - Direction) >= (6 * Math.PI) / 8 And
                    NormalizeDirection(targetDirection - Direction) <= (10 * Math.PI) / 8 And
                    distance < 150 And
                    distance < 200 Then 'The enemy is behind you

                    '-----Steering-----
                    Randomize()
                    If 0 = Int(100 * Rnd()) Then
                        If EvadeRight = True Then
                            EvadeRight = False
                        Else
                            EvadeRight = True
                        End If
                    End If
                    If 0 = Int(100 * Rnd()) Then
                        If Brakes = True Then
                            Brakes = False
                        Else
                            Brakes = True
                        End If
                    End If
                    If EvadeRight = True Then
                        Direction = NormalizeDirection(Direction - TurnSpeed.current)
                    Else
                        Direction = NormalizeDirection(Direction + TurnSpeed.current)
                    End If
                    '------------------
                    If Brakes = False Then
                        Throttle.current = Throttle.current + Acceleration.current
                    Else
                        Throttle.current = Throttle.current - Acceleration.current
                    End If
                    If Throttle.current > Throttle.max Then
                        Throttle.current = Throttle.max
                    ElseIf Throttle.current < 5 Then
                        Throttle.current = 5
                    End If

                ElseIf distance < 65 Then 'Slow down from the enemy

                    '-----Steering-----
                    Randomize()
                    If targetDirection > Math.PI Then
                        Direction = NormalizeDirection(Direction - TurnSpeed.current)
                    Else
                        Direction = NormalizeDirection(Direction + TurnSpeed.current)
                    End If
                    '------------------
                    Throttle.current = Throttle.current - Acceleration.current
                    If Throttle.current < 5 Then
                        Throttle.current = 5
                    End If

                ElseIf Throttle.current <> Parent.Target.Helm.Throttle.current And
                    distance < 70 Then 'Match the enemies speed

                    '-----Steering-----
                    Steering(targetDirection)
                    '------------------
                    If Throttle.current < Parent.Target.Helm.Throttle.current Then
                        Throttle.current = Throttle.current + Acceleration.current
                        If Throttle.current > Parent.Target.Helm.Throttle.current Then
                            Throttle.current = Parent.Target.Helm.Throttle.current
                        End If
                    ElseIf Throttle.current > Parent.Target.Helm.Throttle.current Then
                        Throttle.current = Throttle.current - Acceleration.current
                        If Throttle.current < Parent.Target.Helm.Throttle.current Then
                            Throttle.current = Parent.Target.Helm.Throttle.current
                        End If
                    End If

                ElseIf distance > 70 And
                    ((NormalizeDirection(targetDirection - Direction) >= 0 And
                      NormalizeDirection(targetDirection - Direction) <= (2 * Math.PI) / 4) Or
                     NormalizeDirection(targetDirection - Direction) >= (6 * Math.PI) / 4) Then 'Charge the enemy

                    '-----Steering-----
                    Steering(targetDirection)
                    '------------------
                    Throttle.current = Throttle.current + Acceleration.current
                    If Throttle.current > Throttle.max Then
                        Throttle.current = Throttle.max
                    End If

                Else

                    '-----Steering-----
                    Steering(targetDirection)
                    '------------------
                    Throttle.current = Throttle.current - Acceleration.current
                    If Throttle.current < 5 Then
                        Throttle.current = 5
                    End If

                End If
                '---------------
            End If
        End If

        Direction = NormalizeDirection(Direction)
    End Sub

    Private Sub Steering(ByVal targetDirection As Double)
        If NormalizeDirection(targetDirection - Direction) <= Math.PI Then
            Direction = NormalizeDirection(Direction + TurnSpeed.current)
        Else
            Direction = NormalizeDirection(Direction - TurnSpeed.current)
        End If
    End Sub

    Public Shared Function NormalizeDirection(ByVal nDirection As Double) As Double
        Do Until nDirection >= 0 And
            nDirection <= 2 * Math.PI
            If nDirection < 0 Then
                nDirection = nDirection + (2 * Math.PI)
            ElseIf nDirection > 2 * Math.PI Then
                nDirection = nDirection - (2 * Math.PI)
            End If
        Loop
        Return nDirection
    End Function

End Class
