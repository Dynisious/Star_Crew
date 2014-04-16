<Serializable()>
Public Class Battery
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

            If PlayerControled = False And Galaxy.xList.Length <> 1 Then
                Dim shipDirections(Galaxy.xList.Length - 1) As Double
                Dim shipDistances(Galaxy.xList.Length - 1) As Integer
                Parent.Helm.Target = Nothing
                Dim Opposite As Integer
                Dim Adjacent As Integer
                Dim target As Ship = Nothing
                Dim targetDistance As Integer
                Dim targetDirection As Double
                ReDim shipDirections(Galaxy.xList.Length - 1)
                ReDim shipDistances(Galaxy.xList.Length - 1)
                ReDim Parent.Helm.evadeList(-1)
                '-----Set Target Distances and Directions-----
                For i As Integer = 0 To shipDistances.Length - 1
                    Opposite = Galaxy.xList(i).Position.Y - Parent.Position.Y
                    Adjacent = Galaxy.xList(i).Position.X - Parent.Position.X
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

                    If ReferenceEquals(Parent, Galaxy.xList(i)) = False And shipDistances(i) < Helm.MinimumDistance And
                        shipDirections(i) - Parent.Helm.Direction < (3 * Math.PI) / 4 And
                        shipDirections(i) - Parent.Helm.Direction > -(3 * Math.PI) / 4 Then
                        ReDim Preserve Parent.Helm.evadeList(Parent.Helm.evadeList.Length)
                        Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) = shipDirections(i)
                    End If
                Next
                '---------------------------------------------

                '-----Target Selection-----
                Dim lastDistance As Integer
                For i As Integer = 0 To shipDistances.Length - 1
                    '-----Select Target to Shoot-----
                    If ReferenceEquals(Parent, Galaxy.xList(i)) = False Then
                        If (shipDistances(i) <= lastDistance Or lastDistance = 0) And
                            Galaxy.xList(i).MyAllegence <> Parent.MyAllegence Then
                            If Parent.TargetLock = False Then
                                Parent.Helm.Target = Galaxy.xList(i)
                            End If
                            If (shipDirections(i) - Parent.Helm.Direction) < (Math.PI / 2) And
                                (shipDirections(i) - Parent.Helm.Direction) > -(Math.PI / 2) Then
                                lastDistance = shipDistances(i)
                                target = Galaxy.xList(i)
                                targetDirection = shipDirections(i)
                                targetDistance = shipDistances(i)
                            End If
                        End If
                    End If
                    '--------------------------------
                Next
                '--------------------------

                If target IsNot Nothing Then
                    '-----Aim at Target-----
                    '-----Primary-----
                    If Primary.TurnDistance.current > (targetDirection - Parent.Helm.Direction) Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current - Primary.TurnSpeed.current
                        If Primary.TurnDistance.current < (targetDirection - Parent.Helm.Direction) Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Helm.Direction)
                        End If
                        If Primary.TurnDistance.current < -(Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = -(Primary.TurnDistance.max / 2)
                        End If
                    ElseIf Primary.TurnDistance.current < (targetDirection - Parent.Helm.Direction) Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current + Primary.TurnSpeed.current
                        If Primary.TurnDistance.current > (targetDirection - Parent.Helm.Direction) Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Helm.Direction)
                        End If
                        If Primary.TurnDistance.current > (Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = (Primary.TurnDistance.max / 2)
                        End If
                    End If
                    '-----------------

                    '-----Secondary-----
                    If Secondary.TurnDistance.current > (targetDirection - Parent.Helm.Direction) Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current
                        If Secondary.TurnDistance.current < (targetDirection - Parent.Helm.Direction) Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Helm.Direction)
                        End If
                        If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                        End If
                    ElseIf Secondary.TurnDistance.current < (targetDirection - Parent.Helm.Direction) Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current + Secondary.TurnSpeed.current
                        If Secondary.TurnDistance.current > (targetDirection - Parent.Helm.Direction) Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Helm.Direction)
                        End If
                        If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                        End If
                    End If
                    '-------------------
                    '-----------------------

                    '-----Fire at Target-----
                    '-----Primary-----
                    If targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current > -(HitArc / 2) And
                        targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current < (HitArc / 2) Then
                        Primary.FireWeapon(targetDistance, target)
                    End If
                    '-----------------

                    '-----Secondary-----
                    If targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current > -(HitArc / 2) And
                        targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current < (HitArc / 2) Then
                        Secondary.FireWeapon(targetDistance, target)
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
