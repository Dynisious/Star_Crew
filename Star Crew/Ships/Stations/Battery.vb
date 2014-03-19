<Serializable()>
Public Class Battery
    Inherits Station
    Public Primary As Weapon
    Public Secondary As Weapon
    Public batteriesTarget As Ship
    Public Shared ReadOnly HitArc As Double = Math.PI / 5
    Public evading As Boolean = False

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power = Power + Influx
            evading = False
            Parent.Engineering.batteriesDraw = 0
            Parent.Target = Nothing
            batteriesTarget = Nothing
            Primary.UpdateWeapon()
            Secondary.UpdateWeapon()
            Dim Opposite As Integer
            Dim Adjacent As Integer
            Dim targetDistance As Integer
            Dim targetDirection As Double
            Dim shipDirections(Parent.Parent.xList.Length - 1) As Double
            Dim shipDistances(Parent.Parent.xList.Length - 1) As Integer

            If PlayerControled = False Then
                '-----Set Target Distances and Directions-----
                For i As Integer = 0 To shipDistances.Length - 1
                    Opposite = Parent.Parent.xList(i).Position.Y - Parent.Position.Y
                    Adjacent = Parent.Parent.xList(i).Position.X - Parent.Position.X
                    shipDistances(i) = Math.Sqrt((Opposite ^ 2) + (Adjacent ^ 2))
                    If Adjacent <> 0 Then
                        shipDirections(i) = Helm.NormalizeDirection(Math.Tanh(Opposite / Adjacent))
                        If Adjacent < 0 Then
                            shipDirections(i) = Helm.NormalizeDirection(shipDirections(i) + Math.PI)
                        End If
                    ElseIf Opposite > 0 Then
                        shipDirections(i) = Math.PI / 2
                    Else
                        shipDirections(i) = (3 * Math.PI) / 2
                    End If
                Next
                '---------------------------------------------

                '-----Target Selection-----
                Dim lastIndex As Integer
                '-----Initial Selection-----
                If ReferenceEquals(Parent, Parent.Parent.xList(0)) = False Then
                    Parent.Target = Parent.Parent.xList(0)
                Else
                    Parent.Target = Parent.Parent.xList(1)
                    lastIndex = 1
                End If
                If Parent.Target.MyAllegence <> Parent.MyAllegence Then
                    targetDirection = shipDirections(lastIndex)
                    targetDistance = shipDistances(lastIndex)
                    batteriesTarget = Parent.Parent.xList(lastIndex)
                End If
                '---------------------------
                '-----Selection-----
                If lastIndex <> UBound(Parent.Parent.xList) Then
                    For i As Integer = lastIndex + 1 To shipDistances.Length - 1
                        If ReferenceEquals(Parent, Parent.Parent.xList(i)) = False Then
                            '-----Check Evading------
                            If shipDistances(i) < Helm.MinimumDistance Then
                                evading = True
                                Parent.Target = Parent.Parent.xList(i)
                            End If
                            '------------------------
                            '-----Select Target to Shoot-----
                            If Parent.Parent.xList(i).MyAllegence <> Parent.MyAllegence Then
                                If shipDistances(i) < shipDistances(lastIndex) And
                                    (shipDirections(i) - Parent.Helm.Direction) < (Math.PI / 4) And
                                    (shipDirections(i) - Parent.Helm.Direction) > -(Math.PI / 4) Then
                                    '----------
                                    lastIndex = i
                                    targetDirection = shipDirections(i)
                                    targetDistance = shipDistances(i)
                                    batteriesTarget = Parent.Parent.xList(i)
                                    If evading = False Then
                                        Parent.Target = Parent.Parent.xList(i)
                                    End If
                                    '----------
                                End If
                            End If
                            '--------------------------------
                        End If
                    Next
                    '-------------------
                End If
                '--------------------------

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
            End If

            If batteriesTarget IsNot Nothing Then
                '-----Fire at Target-----
                '-----Primary-----
                If targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current > -(HitArc / 2) And
                    targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current < (HitArc / 2) Then
                    Primary.FireWeapon(targetDistance)
                End If
                '-----------------

                '-----Secondary-----
                If targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current > -(HitArc / 2) And
                    targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current < (HitArc / 2) Then
                    Secondary.FireWeapon(targetDistance)
                End If
                '-------------------
                '------------------------
            End If
        End If
    End Sub

End Class
