Public Class Scrap
    Inherits Layout

    Public Sub New()
        '-----Ship-----
        NewHull = New Stat(1000, 1000)
        '-----Helm-----
        NewHelmStats(HelmStats.Acceleration) = New Stat(0, 0)
        NewHelmStats(HelmStats.Throttle) = New Stat(0, 0)
        NewHelmStats(HelmStats.Turn) = New Stat(0, 0)
        '--------------
        '-----Batteries-----
        NewBatteryWeapons(BatteryWeapons.Primary) = New Weapon(New Blank)
        NewBatteryWeapons(BatteryWeapons.Secondary) = New Weapon(New Blank)
        '-------------------
        '-----Shielding-----
        NewShields(Shields.Sides.FrontShield) = New Stat(0, 0)
        NewShields(Shields.Sides.LeftShield) = New Stat(0, 0)
        NewShields(Shields.Sides.BackShield) = New Stat(0, 0)
        NewShields(Shields.Sides.RightShield) = New Stat(0, 0)
        '-------------------
        '-----Engineering-----
        NewEngineeringStats(EngineeringStats.Engines) = New Stat(1, 1)
        NewEngineeringStats(EngineeringStats.PowerCore) = New Stat(0, 0)
        '---------------------
    End Sub

End Class
