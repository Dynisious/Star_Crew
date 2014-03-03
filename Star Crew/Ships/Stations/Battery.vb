<Serializable()>
Public Class Battery
    Inherits Station
    Public Primary As Weapon
    Public Secondary As Weapon
    Public batteriesTarget As Ship
    Public evadeTarget As Boolean = False

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power = Power + Influx
            evadeTarget = False
            Parent.Engineering.batteriesDraw = 0
            Parent.Target = Nothing
            batteriesTarget = Nothing
            Primary.UpdateWeapon()
            Secondary.UpdateWeapon()
            Dim Opposite As Integer
            Dim Adjacent As Integer
            Dim targetDistance As Integer
            Dim targetDirection As Double

            If PlayerControled = False Then
                '-----Target Selection-----
                Dim shipsIndexs(-1) As Integer
                For Each i As Ship In Parent.Parent.xList
                    Opposite = i.Position.Y - Parent.Position.Y
                    Adjacent = i.Position.X - Parent.Position.X
                    ReDim Preserve shipsIndexs(shipsIndexs.Length)
                    shipsIndexs(UBound(shipsIndexs)) = Math.Sqrt((Opposite * Opposite) + (Adjacent * Adjacent))
                Next
                Dim nIndex As Integer
                While True
                    If ReferenceEquals(Parent.Parent.xList(nIndex), Parent) = False Then
                        Parent.Target = Parent.Parent.xList(nIndex)
                        If Parent.MyAllegence <> Parent.Parent.xList(nIndex).MyAllegence Then
                            targetDistance = shipsIndexs(nIndex)
                            Opposite = Parent.Parent.xList(nIndex).Position.Y - Parent.Position.Y
                            Adjacent = Parent.Parent.xList(nIndex).Position.X - Parent.Position.X
                            If Adjacent <> 0 Then
                                targetDirection = Math.Tanh(Opposite / Adjacent)
                                If Adjacent < 0 Then
                                    targetDirection = targetDirection + Math.PI
                                End If
                                targetDirection = Helm.NormalizeDirection(targetDirection)
                                If Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) < Math.PI / 2 Or
                                    Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) > (6 * Math.PI) / 4 Then
                                    batteriesTarget = Parent.Parent.xList(nIndex)
                                End If
                            End If
                        End If
                        Exit While
                    Else
                        nIndex = nIndex + 1
                    End If
                End While
                If nIndex <> UBound(shipsIndexs) Then
                    For i As Integer = nIndex + 1 To UBound(shipsIndexs)
                        If shipsIndexs(i) < shipsIndexs(i - 1) And
                            ReferenceEquals(Parent.Parent.xList(i), Parent) = False Then
                            If shipsIndexs(i) < 30 Then
                                Parent.Target = Parent.Parent.xList(i)
                                evadeTarget = True
                            End If
                            If Parent.MyAllegence <> Parent.Parent.xList(i).MyAllegence Then
                                If evadeTarget = False Then
                                    Parent.Target = Parent.Parent.xList(i)
                                End If
                                targetDistance = shipsIndexs(i)
                                Opposite = Parent.Parent.xList(i).Position.Y - Parent.Position.Y
                                Adjacent = Parent.Parent.xList(i).Position.X - Parent.Position.X
                                If Adjacent <> 0 Then
                                    targetDirection = Math.Tanh(Opposite / Adjacent)
                                    If Adjacent < 0 Then
                                        targetDirection = targetDirection + Math.PI
                                    End If
                                    targetDirection = Helm.NormalizeDirection(targetDirection)
                                    If Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) < Math.PI / 2 Or
                                        Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) > (6 * Math.PI) / 4 Then
                                        batteriesTarget = Parent.Parent.xList(i)
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
                '--------------------------

                '-----Aim at Target-----
                '-----Primary-----
                If Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) < Math.PI Then
                    Primary.TurnDistance.current = Primary.TurnDistance.current + Primary.TurnSpeed.current
                    If Helm.NormalizeDirection(Primary.TurnDistance.current + Parent.Helm.Direction) > targetDirection Then
                        Primary.TurnDistance.current = Helm.NormalizeDirection(targetDistance - Parent.Helm.Direction)
                    End If
                    If Primary.TurnDistance.current > (Primary.TurnDistance.max / 2) Then
                        Primary.TurnDistance.current = (Primary.TurnDistance.max / 2)
                    End If
                Else
                    Primary.TurnDistance.current = Primary.TurnDistance.current - Primary.TurnSpeed.current
                    If Helm.NormalizeDirection(Primary.TurnDistance.current + Parent.Helm.Direction) < targetDirection Then
                        Primary.TurnDistance.current = Helm.NormalizeDirection(targetDistance - Parent.Helm.Direction)
                    End If
                    If Primary.TurnDistance.current < -(Primary.TurnDistance.max / 2) Then
                        Primary.TurnDistance.current = -(Primary.TurnDistance.max / 2)
                    End If
                End If
                '-----------------

                '-----Secondary-----
                If Helm.NormalizeDirection(targetDirection - Parent.Helm.Direction) < Math.PI Then
                    Secondary.TurnDistance.current = Secondary.TurnDistance.current + Secondary.TurnSpeed.current
                    If Helm.NormalizeDirection(Secondary.TurnDistance.current + Parent.Helm.Direction) > targetDirection Then
                        Secondary.TurnDistance.current = Helm.NormalizeDirection(targetDistance - Parent.Helm.Direction)
                    End If
                    If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then
                        Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                    End If
                Else
                    Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current
                    If Helm.NormalizeDirection(Secondary.TurnDistance.current + Parent.Helm.Direction) < targetDirection Then
                        Secondary.TurnDistance.current = Helm.NormalizeDirection(targetDistance - Parent.Helm.Direction)
                    End If
                    If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then
                        Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                    End If
                End If
                '-------------------
                '-----------------------
            End If

            '-----Fire at Target-----
            '-----Primary-----
            Dim primaryDirection As Double = (targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current)
            If ((2 * Math.PI / 10) >= primaryDirection Or (18 * Math.PI / 10) <= primaryDirection) And
                batteriesTarget IsNot Nothing And
                targetDistance <= Primary.WeaponStats(Weapon.Stats.Range).current Then
                Primary.FireWeapon(targetDistance)
            End If
            '-----------------

            '-----Secondary-----
            Dim secondaryDirection As Double = (targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current)
            If ((2 * Math.PI / 10) >= secondaryDirection Or (18 * Math.PI / 10) <= secondaryDirection) And
                batteriesTarget IsNot Nothing And
                targetDistance <= Secondary.WeaponStats(Weapon.Stats.Range).current Then
                Secondary.FireWeapon(targetDistance)
            End If
            '-------------------
            '------------------------
        End If
    End Sub

End Class
