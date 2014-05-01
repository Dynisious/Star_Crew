﻿Public Class Helm
    Inherits Station
    Public TurnSpeed As StatDbl
    Private evadeRight As Boolean = False
    Private brakes As Boolean = False
    Public Shared ReadOnly MinimumSpeed As Integer = 5
    Public Shared ReadOnly StandardDistance As Integer = 140
    Public Shared ReadOnly MinimumDistance As Integer = 60
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
        If Parent IsNot Nothing And PlayerControled = False And ConsoleWindow.GameServer.GameWorld.Warping <> Galaxy.Warp.Warping Then
            Dim targetDirection As Double
            Dim finalSpeed As Double = MinimumSpeed
            '-----Set Target Direction and Distance-----
            If Target IsNot Nothing Then
                If Target.Dead = True Then
                    Target = Nothing
                    Exit Sub
                End If
                Dim distance As Integer
                Dim opposite As Double = (Target.Position.Y + (Math.Sin(Target.Direction) * Target.Helm.Parent.Speed.current)) - Parent.Position.Y
                Dim adjacent As Double = (Target.Position.X + (Math.Cos(Target.Direction) * Target.Helm.Parent.Speed.current)) - Parent.Position.X
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
                If NormalizeDirection(targetDirection - Parent.Direction) > (3 * Math.PI) / 4 And
                    NormalizeDirection(targetDirection - Parent.Direction) < (5 * Math.PI) / 4 And
                    distance < 200 Then 'The enemy is behind you
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

                    finalSpeed = Parent.Speed.max
                ElseIf Parent.Speed.current <> Target.Helm.Parent.Speed.current And
                    distance < MinimumDistance + (((Parent.Speed.current - Target.Helm.Parent.Speed.current) / Parent.Acceleration.current) * Parent.Speed.current) And
                    targetDirection - Parent.Direction < Math.PI / 2 And
                    targetDirection - Parent.Direction > -Math.PI / 2 Then 'Match the enemies speed
                    finalSpeed = Target.Helm.Parent.Speed.current
                ElseIf distance > StandardDistance And targetDirection - Parent.Direction < Math.PI / 2 And
                    targetDirection - Parent.Direction > -Math.PI / 2 Then 'Charge the enemy
                    finalSpeed = Parent.Speed.max
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
            If Math.Sqrt((targetDirection - Parent.Direction) ^ 2) < TurnSpeed.current Then
                Parent.Direction = targetDirection
            ElseIf NormalizeDirection(targetDirection - Parent.Direction) < Math.PI Then
                Parent.Direction = NormalizeDirection(Parent.Direction + TurnSpeed.current)
            Else
                Parent.Direction = NormalizeDirection(Parent.Direction - TurnSpeed.current)
            End If

            If Parent.Speed.current < finalSpeed Then
                Parent.Speed.current = Parent.Speed.current + Parent.Acceleration.current
                If Parent.Speed.current > finalSpeed Then
                    Parent.Speed.current = finalSpeed
                End If
            Else
                Parent.Speed.current = Parent.Speed.current - Parent.Acceleration.current
                If Parent.Speed.current < finalSpeed Then
                    Parent.Speed.current = finalSpeed
                End If
            End If
            '----------------------------
        ElseIf Parent IsNot Nothing Then
            '-----Match Enemies Speed-----
            If MatchSpeed = True And Target IsNot Nothing Then
                If Parent.Speed.current > Target.Speed.current Then
                    Parent.Speed.current = Parent.Speed.current - Parent.Acceleration.current
                    If Parent.Speed.current < Target.Speed.current Then
                        Parent.Speed.current = Target.Helm.Parent.Speed.current
                    End If
                ElseIf Parent.Speed.current < Target.Speed.current Then
                    Parent.Speed.current = Parent.Speed.current + Parent.Acceleration.current
                    If Parent.Speed.current > Target.Speed.current Then
                        Parent.Speed.current = Target.Speed.current
                    End If
                    If Parent.Speed.current > Parent.Speed.max Then
                        Parent.Speed.current = Parent.Speed.max
                    End If
                End If
            Else
                MatchSpeed = False
            End If
            '-----------------------------
        End If
    End Sub

    Public Shared Function NormalizeDirection(ByVal nDirecion As Double) As Double
        nDirecion = nDirecion Mod 2 * Math.PI
        If nDirecion < 0 Then
            nDirecion = nDirecion + (2 * Math.PI)
        End If
        Return nDirecion
    End Function

End Class