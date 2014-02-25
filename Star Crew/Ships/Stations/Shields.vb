Public Class Shields
    Inherits Station
    Public Tuning As Weapon.DamageTypes
    Public DefenceBonuses(Weapon.DamageTypes.Max - 1) As Double
    Public Enum Sides
        FrontShield
        RightShield
        BackShield
        LeftShield
        Max
    End Enum
    Public ShipShields(Sides.Max - 1) As Stat
    Public DamagePerSide(Sides.Max - 1) As Integer
    Public IncomingDamageTypes(Weapon.DamageTypes.Max - 1) As Integer
    Public LastHit As Sides 'The side that was last hit

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Function DeflectHit(ByVal side As Sides, ByVal nWeapon As Weapon) As Integer
        DamagePerSide(side) = DamagePerSide(side) + nWeapon.WeaponStats(Weapon.Stats.Damage).current
        IncomingDamageTypes(nWeapon.WeaponStats(Weapon.Stats.DamageType).current) =
            IncomingDamageTypes(nWeapon.WeaponStats(Weapon.Stats.DamageType).current) +
            nWeapon.WeaponStats(Weapon.Stats.Damage).current

        Dim recivedDamage As Integer = nWeapon.WeaponStats(Weapon.Stats.Damage).current
        If Tuning = nWeapon.WeaponStats(Weapon.Stats.DamageType).current Then
            recivedDamage = recivedDamage * DefenceBonuses(Tuning)
        End If
        If shipShields(side).current > recivedDamage Then
            ShipShields(side).current = ShipShields(side).current - recivedDamage
            recivedDamage = 0
        Else
            recivedDamage = recivedDamage - shipShields(side).current
            shipShields(side).current = 0
        End If
        Return recivedDamage
    End Function

    Public Overrides Sub Update()
        Dim totalDamage As Integer = DamagePerSide(Sides.FrontShield) +
            DamagePerSide(Sides.LeftShield) + DamagePerSide(Sides.BackShield) +
            DamagePerSide(Sides.RightShield) 'The total damage that has been taken by all sides

        '-----Send off the costs-----
        Parent.Engineering.shieldingDraw = 0
        For i As Integer = 0 To 3
            Parent.Engineering.shieldingDraw = Parent.Engineering.shieldingDraw +
              (ShipShields(i).max - ShipShields(i).current)
        Next
        '----------------------------

        If PlayerControled = False Then
            '-----Distribute the power-----
            If totalDamage <> 0 Then
                '-----Add up the power-----
                Dim usablePower As Integer = Power + Influx + (ShipShields(Sides.FrontShield).current +
                    ShipShields(Sides.LeftShield).current +
                    ShipShields(Sides.BackShield).current + ShipShields(Sides.RightShield).current)
                '--------------------------

                For i As Integer = 0 To 3
                    ShipShields(i).current = usablePower * (DamagePerSide(i) / totalDamage)
                    If ShipShields(i).current > ShipShields(i).max Then
                        ShipShields(i).current = ShipShields(i).max
                    End If
                Next
                '------------------------------

                '-----See if theres remaining usablePower-----
                usablePower = usablePower - ShipShields(Sides.FrontShield).current -
                    ShipShields(Sides.LeftShield).current - ShipShields(Sides.BackShield).current -
                    ShipShields(Sides.RightShield).current
                '---------------------------------------

                '-----Remaining usablePower-----
                If usablePower > 0 Then 'Theres spare usablePower
                    Dim nCost As Integer = ShipShields(LastHit).max - ShipShields(LastHit).current
                    If nCost > usablePower Then
                        nCost = usablePower
                    End If
                    ShipShields(LastHit).current = ShipShields(LastHit).current + nCost
                    usablePower = usablePower - nCost
                    If ShipShields(LastHit).current > ShipShields(LastHit).max Then
                        ShipShields(LastHit).current = ShipShields(LastHit).max
                    End If
                    While usablePower <> 0 'Distribute remaining power
                        Dim maxed As Integer
                        For i As Integer = 0 To Sides.Max - 1
                            If ShipShields(i).current = ShipShields(i).max Then
                                maxed = maxed + 1
                            End If
                        Next
                        If maxed = 4 Then
                            Exit While
                        Else
                            Dim factor As Integer = 4 - maxed
                            For i As Integer = 0 To Sides.Max - 1
                                If ShipShields(i).current <> ShipShields(i).max Then
                                    ShipShields(i).current = ShipShields(i).current + (usablePower / factor)
                                End If
                            Next
                            usablePower = 0
                            For i As Integer = 0 To Sides.Max - 1
                                If ShipShields(i).current > ShipShields(i).max Then
                                    usablePower = usablePower + (ShipShields(i).current - ShipShields(i).max)
                                    ShipShields(i).current = ShipShields(i).max
                                End If
                            Next
                        End If
                    End While
                    Power = usablePower
                End If
            End If
            '-------------------------

            For i As Integer = 0 To Sides.Max - 1
                If DamagePerSide(i) > 0 Then
                    DamagePerSide(i) = DamagePerSide(i) - 1
                End If
            Next
            For i As Integer = 0 To Weapon.DamageTypes.Max - 1
                If IncomingDamageTypes(i) > 0 Then
                    IncomingDamageTypes(i) = IncomingDamageTypes(i) - 1
                End If
            Next
        End If
    End Sub

End Class
