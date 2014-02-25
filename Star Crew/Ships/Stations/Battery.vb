Public Class Battery
    Inherits Station
    Public Primary As Weapon
    Public Secondary As Weapon

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        Power = Power + Influx
        Parent.Engineering.batteriesDraw = 0
        Parent.Target = Nothing
        Primary.UpdateWeapon()
        Secondary.UpdateWeapon()
        Dim Opposite As Integer
        Dim Adjacent As Integer
        Dim targetDistance As Integer
        Dim targetDirection As Double

        If playerControled = False Then
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
                If nIndex < UBound(shipsIndexs) Then
                    If ReferenceEquals(Parent.Parent.xList(nIndex), Parent) = False And
                        Parent.Parent.xList(nIndex).MyAllegence <> Parent.MyAllegence Then
                        Parent.Target = Parent.Parent.xList(nIndex)
                        targetDistance = shipsIndexs(nIndex)
                        Opposite = Parent.Parent.xList(nIndex).Position.Y - Parent.Position.Y
                        Adjacent = Parent.Parent.xList(nIndex).Position.X - Parent.Position.X
                        If Adjacent <> 0 Then
                            targetDirection = Math.Tanh(Opposite / Adjacent)
                            If Adjacent < 0 Then
                                targetDirection = targetDirection + Math.PI
                            End If
                            targetDirection = Helm.NormalizeDirection(targetDirection)
                        End If
                        Exit While
                    Else
                        nIndex = nIndex + 1
                    End If
                Else
                    End
                End If
            End While
            If nIndex <> UBound(shipsIndexs) Then
                For i As Integer = nIndex + 1 To UBound(shipsIndexs)
                    If shipsIndexs(i) < shipsIndexs(i - 1) And
                        ReferenceEquals(Parent.Parent.xList(i), Parent) = False And
                    Parent.Parent.xList(i).MyAllegence <> Parent.MyAllegence Then
                        Parent.Target = Parent.Parent.xList(i)
                        targetDistance = shipsIndexs(i)
                        Opposite = Parent.Parent.xList(i).Position.Y - Parent.Position.Y
                        Adjacent = Parent.Parent.xList(i).Position.X - Parent.Position.X
                        targetDirection = Math.Tanh(Opposite / Adjacent)
                        If Adjacent < 0 Then
                            targetDirection = targetDirection + Math.PI
                        End If
                        targetDirection = Helm.NormalizeDirection(targetDirection)
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
                    Primary.TurnDistance.current = targetDistance - Parent.Helm.Direction
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
                    Secondary.TurnDistance.current = targetDistance - Parent.Helm.Direction
                End If
                If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then
                    Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                End If
            Else
                Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current
                If Helm.NormalizeDirection(Secondary.TurnDistance.current + Parent.Helm.Direction) < targetDirection Then
                    Secondary.TurnDistance.current = Secondary.TurnDistance.current - Parent.Helm.Direction
                End If
                If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then
                    Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                End If
            End If
            '-------------------
            '-----------------------

            '-----Fire at Target-----
            '-----Primary-----
            Dim primaryDirection As Double = (targetDirection - Parent.Helm.Direction - Primary.TurnDistance.current)
            If ((Math.PI / 8) >= primaryDirection Or (7 * Math.PI / 8) <= primaryDirection) And
                targetDistance <= Primary.WeaponStats(Weapon.Stats.Range).current Then
                Primary.FireWeapon(targetDistance)
            End If
            '-----------------

            '-----Secondary-----
            Dim secondaryDirection As Double = (targetDirection - Parent.Helm.Direction - Secondary.TurnDistance.current)
            If ((Math.PI / 8) >= secondaryDirection Or (7 * Math.PI / 8) <= secondaryDirection) And
                targetDistance <= Secondary.WeaponStats(Weapon.Stats.Range).current Then
                Secondary.FireWeapon(targetDistance)
            End If
            '-------------------
            '------------------------
        End If
    End Sub

End Class
