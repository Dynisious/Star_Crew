﻿Public Class Battery
    Inherits Station
    Public Primary As Weapon
    Public Secondary As Weapon
    Public Shared ReadOnly HitArc As Double = Math.PI / 5
    Public Shared ReadOnly PlayerArc As Double = HitArc * 1.5
    Public Enum Commands
        TurnRight
        TurnLeft
        FirePrimary
        FireSecondary
        SetTarget
    End Enum

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power = Power + Influx
            Parent.Engineering.batteriesDraw = 0
            Primary.UpdateWeapon()
            Secondary.UpdateWeapon()

            If PlayerControled = False And ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count <> 1 Then
                Dim shipDirections(ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1) As Double
                Dim shipDistances(ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1) As Integer
                Parent.Helm.Target = Nothing
                Dim Opposite As Integer
                Dim Adjacent As Integer
                Dim target As Ship = Nothing
                Dim targetDistance As Integer
                Dim targetDirection As Double
                ReDim shipDirections(ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1)
                ReDim shipDistances(ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count - 1)
                ReDim Parent.Helm.evadeList(-1)
                '-----Set Target Distances and Directions-----
                For i As Integer = 0 To shipDistances.Length - 1
                    Opposite = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.Y - Parent.Position.Y
                    Adjacent = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).Position.X - Parent.Position.X
                    If Adjacent <> 0 Then
                        shipDirections(i) = Math.Tanh(Opposite / Adjacent)
                        If Adjacent < 0 Then
                            shipDirections(i) = shipDirections(i) + Math.PI
                        End If
                        shipDirections(i) = Helm.NormalizeDirection(shipDirections(i))
                    ElseIf Opposite > 0 Then
                        shipDirections(i) = Math.PI / 2
                    Else
                        shipDirections(i) = (3 * Math.PI) / 2
                    End If
                    shipDistances(i) = Math.Sqrt((Adjacent ^ 2) + (Opposite ^ 2))

                    If ReferenceEquals(Parent, ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i)) = False And shipDistances(i) < Helm.MinimumDistance And
                        shipDirections(i) - Parent.Direction < (3 * Math.PI) / 4 And
                        shipDirections(i) - Parent.Direction > -(3 * Math.PI) / 4 Then
                        ReDim Preserve Parent.Helm.evadeList(Parent.Helm.evadeList.Length)
                        Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) = shipDirections(i)
                    End If
                Next
                '---------------------------------------------

                '-----Target Selection-----
                Dim lastDistance As Integer
                For i As Integer = 0 To shipDistances.Length - 1
                    '-----Select Target to Shoot-----
                    If (shipDistances(i) <= lastDistance Or lastDistance = 0) And
                            ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i).MyAllegence <> Parent.MyAllegence Then
                        If Parent.TargetLock = False Then
                            Parent.Helm.Target = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i)
                        End If
                        If (shipDirections(i) - Parent.Direction) < (Math.PI / 2) And
                            (shipDirections(i) - Parent.Direction) > -(Math.PI / 2) Then
                            lastDistance = shipDistances(i)
                            target = ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList(i)
                            targetDirection = shipDirections(i)
                            targetDistance = shipDistances(i)
                        End If
                    End If
                    '--------------------------------
                Next
                '--------------------------

                If target IsNot Nothing Then
                    '-----Aim at Target-----
                    '-----Primary-----
                    If Primary.TurnDistance.current > (targetDirection - Parent.Direction) Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current - Primary.TurnSpeed.current
                        If Primary.TurnDistance.current < (targetDirection - Parent.Direction) Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction)
                        End If
                        If Primary.TurnDistance.current < -(Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = -(Primary.TurnDistance.max / 2)
                        End If
                    ElseIf Primary.TurnDistance.current < (targetDirection - Parent.Direction) Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current + Primary.TurnSpeed.current
                        If Primary.TurnDistance.current > (targetDirection - Parent.Direction) Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction)
                        End If
                        If Primary.TurnDistance.current > (Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = (Primary.TurnDistance.max / 2)
                        End If
                    End If
                    '-----------------

                    '-----Secondary-----
                    If Secondary.TurnDistance.current > (targetDirection - Parent.Direction) Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current
                        If Secondary.TurnDistance.current < (targetDirection - Parent.Direction) Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction)
                        End If
                        If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                        End If
                    ElseIf Secondary.TurnDistance.current < (targetDirection - Parent.Direction) Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current + Secondary.TurnSpeed.current
                        If Secondary.TurnDistance.current > (targetDirection - Parent.Direction) Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction)
                        End If
                        If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                        End If
                    End If
                    '-------------------
                    '-----------------------

                    '-----Fire at Target-----
                    '-----Primary-----
                    If targetDirection - Parent.Direction - Primary.TurnDistance.current > -(HitArc / 2) And
                        targetDirection - Parent.Direction - Primary.TurnDistance.current < (HitArc / 2) Then
                        Primary.FireWeapon(targetDistance, target, targetDirection)
                    End If
                    '-----------------

                    '-----Secondary-----
                    If targetDirection - Parent.Direction - Secondary.TurnDistance.current > -(HitArc / 2) And
                        targetDirection - Parent.Direction - Secondary.TurnDistance.current < (HitArc / 2) Then
                        Secondary.FireWeapon(targetDistance, target, targetDirection)
                    End If
                    '-------------------
                    '------------------------
                End If
            Else
                ReDim Parent.Helm.evadeList(-1)
                If Parent.Helm.Target IsNot Nothing Then
                    If Parent.Helm.Target.Dead = True Then
                        Parent.Helm.Target = Nothing
                    End If
                End If
            End If
        End If
    End Sub

End Class
