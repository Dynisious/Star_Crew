Public Class Scrap
    Inherits Layout

    Public Sub New()
        '-----Ship-----
        NewHull = New StatDbl(1000, 1000)
        '-----Helm-----
        NewHelmStats(HelmStats.Acceleration) = New StatDbl(0, 0)
        NewHelmStats(HelmStats.Throttle) = New StatDbl(0, 0)
        NewHelmStats(HelmStats.Turn) = New StatDbl(0, 0)
        '--------------
        '-----Batteries-----
        NewBatteryWeapons(BatteryWeapons.Primary) = Nothing
        NewBatteryWeapons(BatteryWeapons.Secondary) = Nothing
        '-------------------
        '-----Shielding-----
        NewShields(Shields.Sides.FrontShield) = New StatDbl(0, 0)
        NewShields(Shields.Sides.LeftShield) = New StatDbl(0, 0)
        NewShields(Shields.Sides.BackShield) = New StatDbl(0, 0)
        NewShields(Shields.Sides.RightShield) = New StatDbl(0, 0)
        '-------------------
        '-----Engineering-----
        NewEngineeringStats(EngineeringStats.Engines) = New StatDbl(1, 1)
        NewEngineeringStats(EngineeringStats.PowerCore) = New StatDbl(0, 0)
        '---------------------
    End Sub

End Class
