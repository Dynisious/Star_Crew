<Serializable()>
Public Class Helm
    Inherits Station
    Public Direction As Double
    Public TurnSpeed As Stat
    Public Throttle As Stat
    Public Acceleration As Stat
    Private evadeRight As Boolean = False
    Private brakes As Boolean = False
    Public Shared ReadOnly MinimumSpeed As Integer = 5
    Public Shared ReadOnly StandardDistance As Integer = 70
    Public Shared ReadOnly MinimumDistance As Integer = 30
    Public Target As Ship
    Public evadeList(-1) As Double
    Public MatchSpeed As Boolean = False
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
        If Parent IsNot Nothing And PlayerControled = False And Galaxy.Warping <> Galaxy.Warp.Warping Then
            Dim targetDirection As Double
            Dim finalSpeed As Double = MinimumSpeed
            '-----Set Target Direction and Distance-----
            If Target IsNot Nothing Then
                If Target.Dead = True Then
                    Target = Nothing
                    Exit Sub
                End If
                Dim distance As Integer
                Dim opposite As Double = (Target.Position.Y + (Math.Sin(Target.Helm.Direction) * Target.Helm.Throttle.current)) - Parent.Position.Y
                Dim adjacent As Double = (Target.Position.X + (Math.Cos(Target.Helm.Direction) * Target.Helm.Throttle.current)) - Parent.Position.X
                distance = Math.Sqrt((opposite * opposite) + (adjacent * adjacent))

                If adjacent <> 0 Then
                    targetDirection = Math.Tanh(opposite / adjacent)
                    If adjacent < 0 Then
                        targetDirection = targetDirection + Math.PI
                    End If
                    targetDirection = NormalizeDirection(targetDirection)
                ElseIf opposite > 0 Then
                    targetDirection = Math.PI / 2
                Else
                    targetDirection = (3 * Math.PI) / 2
                End If
                '-------------------------------------------

                '-----Speed-----
                If NormalizeDirection(targetDirection - Direction) > (3 * Math.PI) / 4 And
                    NormalizeDirection(targetDirection - Direction) < (5 * Math.PI) / 4 And
                    distance < 100 Then 'The enemy is behind you
                    '-----Steering-----
                    Randomize()
                    If 0 = Int(20 * Rnd()) Then
                        If evadeRight = True Then
                            evadeRight = False
                        Else
                            evadeRight = True
                        End If
                    End If
                    If evadeRight = True Then
                        ReDim Preserve evadeList(evadeList.Length)
                        evadeList(UBound(evadeList)) = (3 * Math.PI) / 2
                    Else
                        ReDim Preserve evadeList(evadeList.Length)
                        evadeList(UBound(evadeList)) = Math.PI / 2
                    End If
                    '------------------

                    finalSpeed = Throttle.max
                ElseIf Throttle.current <> Target.Helm.Throttle.current And
                    distance < MinimumDistance + (((Throttle.current - Target.Helm.Throttle.current) / Acceleration.current) * Throttle.current) And
                    targetDirection - Direction < Math.PI / 2 And
                    targetDirection - Direction > -Math.PI / 2 Then 'Match the enemies speed
                    finalSpeed = Target.Helm.Throttle.current
                ElseIf distance > StandardDistance And targetDirection - Direction < Math.PI / 2 And
                    targetDirection - Direction > -Math.PI / 2 Then 'Charge the enemy
                    finalSpeed = Throttle.max
                End If
                If distance < MinimumDistance Then
                    If targetDirection < Math.PI Then
                        targetDirection = (3 * Math.PI) / 2
                    Else
                        targetDirection = (Math.PI / 2)
                    End If
                End If
                '---------------
            End If

            '-----Evade-----
            Dim offset As Double
            For Each i As Double In evadeList
                offset = i - offset
            Next
            offset = NormalizeDirection(offset)
            targetDirection = NormalizeDirection(targetDirection - offset)
            '---------------

            '-----Steering and Speed-----
            If Math.Sqrt((targetDirection - Direction) ^ 2) < TurnSpeed.current Then
                Direction = targetDirection
            ElseIf NormalizeDirection(targetDirection - Direction) < Math.PI Then
                Direction = NormalizeDirection(Direction + TurnSpeed.current)
            Else
                Direction = NormalizeDirection(Direction - TurnSpeed.current)
            End If
            If ReferenceEquals(Parent, Galaxy.centerShip) Then
                Dim a = 1
            End If

            If Throttle.current < finalSpeed Then
                Throttle.current = Throttle.current + Acceleration.current
                If Throttle.current > finalSpeed Then
                    Throttle.current = finalSpeed
                End If
            Else
                Throttle.current = Throttle.current - Acceleration.current
                If Throttle.current < finalSpeed Then
                    Throttle.current = finalSpeed
                End If
            End If
            '----------------------------
        ElseIf Parent IsNot Nothing Then
            '-----Match Enemies Speed-----
            If MatchSpeed = True And Target IsNot Nothing Then
                If Throttle.current > Target.Helm.Throttle.current Then
                    Throttle.current = Throttle.current - Acceleration.current
                    If Throttle.current < Target.Helm.Throttle.current Then
                        Throttle.current = Target.Helm.Throttle.current
                    End If
                ElseIf Throttle.current < Target.Helm.Throttle.current Then
                    Throttle.current = Throttle.current + Acceleration.current
                    If Throttle.current > Target.Helm.Throttle.current Then
                        Throttle.current = Target.Helm.Throttle.current
                    End If
                    If Throttle.current > Throttle.max Then
                        Throttle.current = Throttle.max
                    End If
                End If
            Else
                MatchSpeed = False
            End If
            '-----------------------------
        End If
    End Sub

    Public Shared Function NormalizeDirection(ByVal nDirecion As Double) As Double
        While nDirecion > (2 * Math.PI) Or nDirecion < 0
            If nDirecion < 0 Then
                nDirecion = nDirecion + (2 * Math.PI)
            Else
                nDirecion = nDirecion - (2 * Math.PI)
            End If
        End While
        Return nDirecion
    End Function

End Class
