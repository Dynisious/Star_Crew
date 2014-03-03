﻿Public Class Clunker
    Inherits Layout

    Public Sub New()
        '-----Ship-----
        NewHull = New Stat(300, 300)
        '--------------
        '-----Helm-----
        NewHelmStats(HelmStats.Acceleration) = New Stat(0.1, 0.1)
        NewHelmStats(HelmStats.Throttle) = New Stat(0, 15)
        NewHelmStats(HelmStats.Turn) = New Stat(Math.PI / 20, Math.PI / 20)
        '--------------
        '-----Batteries-----
        NewBatteryWeapons(BatteryWeapons.Primary) = New Weapon(New Rattler)
        NewBatteryWeapons(BatteryWeapons.Secondary) = New Weapon(New Rattler)
        '-------------------
        '-----Shielding-----
        NewShieldBonuses(Weapon.DamageTypes.Slug) = 1
        NewShields(Shields.Sides.FrontShield) = New Stat(50, 50)
        NewShields(Shields.Sides.LeftShield) = New Stat(50, 50)
        NewShields(Shields.Sides.BackShield) = New Stat(50, 50)
        NewShields(Shields.Sides.RightShield) = New Stat(50, 50)
        '-------------------
        '-----Engineering-----
        NewEngineeringStats(EngineeringStats.Engines) = New Stat(100, 100)
        NewEngineeringStats(EngineeringStats.PowerCore) = New Stat(4, 4)
        '---------------------
    End Sub

End Class
