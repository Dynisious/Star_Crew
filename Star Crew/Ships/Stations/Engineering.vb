Public Class Engineering
    Inherits Station
    Public batteriesDraw As Integer
    Public shieldingDraw As Integer
    Public Engines As Stat
    Public PowerCore As Stat

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        Power = Power + PowerCore.current
        Dim primaryDraw As Integer = ((Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).max -
             Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current) * 10)
        Dim secondaryDraw As Integer = ((Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).max -
             Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current) * 10)
        Dim enginesDraw As Integer = ((Engines.max - Engines.current) * 10)
        Dim totalPowerDraw As Integer = batteriesDraw + shieldingDraw + enginesDraw + primaryDraw + secondaryDraw

        If totalPowerDraw <> 0 And PlayerControled = False Then
            Dim usablePower As Integer = Power

            '-----Batteries-----
            Dim batteriesCost As Integer = usablePower * (batteriesDraw / totalPowerDraw)
            If batteriesCost > batteriesDraw Then
                batteriesCost = batteriesDraw
            End If
            Parent.Batteries.Influx = batteriesCost
            '-------------------

            '-----Shielding-----
            Dim shieldingCost As Integer = usablePower * (shieldingDraw / totalPowerDraw)
            If shieldingCost > shieldingDraw Then
                shieldingCost = shieldingDraw
            End If
            Parent.Shielding.Influx = shieldingCost
            '-------------------

            '-----Primary Weapon-----
            Dim primaryCost As Integer = usablePower * (primaryDraw / totalPowerDraw)
            If primaryCost > primaryDraw Then
                primaryCost = primaryDraw
            End If
            Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current =
                Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current + primaryCost
            '------------------------

            '-----Secondary Weapon-----
            Dim secondaryCost As Integer = usablePower * (secondaryDraw / totalPowerDraw)
            If primaryCost > primaryDraw Then
                primaryCost = primaryDraw
            End If
            Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current =
                Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current + secondaryCost
            '------------------------

            '-----Engines-----
            Dim enginesCost As Integer = usablePower * (enginesDraw / totalPowerDraw)
            If enginesCost > enginesDraw Then
                enginesCost = enginesDraw
            End If
            Engines.current = Engines.current + enginesCost
            '-----------------

            Power = usablePower - batteriesCost - shieldingCost - primaryCost - secondaryCost - enginesCost
        End If
    End Sub

End Class
