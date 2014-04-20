Public Class Clunker
    Inherits Layout

    Public Sub New()
        '-----Ship-----
        NewHull = New StatDbl(200, 200)
        '--------------
        '-----Helm-----
        NewHelmStats(HelmStats.Acceleration) = New StatDbl(0.1, 0.1)
        NewHelmStats(HelmStats.Speed) = New StatDbl(0, 15)
        NewHelmStats(HelmStats.Turn) = New StatDbl(Math.PI / 20, Math.PI / 20)
        '--------------
        '-----Batteries-----
        TurnScale = 1.25
        NewBatteryWeapons(BatteryWeapons.Primary) = New Weapon(New Rattler)
        NewBatteryWeapons(BatteryWeapons.Secondary) = New Weapon(New Rattler)
        '-------------------
        '-----Shielding-----
        NewShields(Shields.Sides.FrontShield) = New StatDbl(30, 30)
        NewShields(Shields.Sides.LeftShield) = New StatDbl(15, 15)
        NewShields(Shields.Sides.BackShield) = New StatDbl(15, 15)
        NewShields(Shields.Sides.RightShield) = New StatDbl(15, 15)
        '-------------------
        '-----Engineering-----
        NewEngineeringStats(EngineeringStats.Engines) = New StatDbl(100, 100)
        NewEngineeringStats(EngineeringStats.PowerCore) = New StatDbl(3, 3)
        '---------------------
    End Sub

End Class
