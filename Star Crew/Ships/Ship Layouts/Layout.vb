Public MustInherit Class Layout

    '-----Ship-----
    Public NewHull As Stat
    '--------------

    '-----Helm-----
    Public Enum HelmStats
        Turn
        Throttle
        Acceleration
        Max
    End Enum
    Public NewHelmStats(HelmStats.Max - 1) As Stat
    '--------------

    '-----Batteries-----
    Public Enum BatteryWeapons
        Primary
        Secondary
        Max
    End Enum
    Public NewBatteryWeapons(BatteryWeapons.Max - 1) As Weapon
    Private TurnScale As Double = 1.25
    '-------------------

    '-----Shielding-----
    Public NewShieldBonuses(Weapon.DamageTypes.Max - 1) As  Double
    Public NewShields(Shields.Sides.Max - 1) As Stat
    '-------------------

    '-----Engineering-----
    Public Enum EngineeringStats
        Engines
        PowerCore
        Capacity
        Max
    End Enum
    Public NewEngineeringStats(EngineeringStats.Max-1) As Stat
    '---------------------

    Public Sub SetLayout(ByRef nShip As Ship)
        '-----Ship-----
        nShip.Hull = NewHull
        '--------------
        '-----Helm-----
        nShip.Helm.TurnSpeed = NewHelmStats(HelmStats.Turn)
        nShip.Helm.Throttle = NewHelmStats(HelmStats.Throttle)
        nShip.Helm.Acceleration = NewHelmStats(HelmStats.Acceleration)
        '--------------
        '-----Batteries-----
        NewBatteryWeapons(BatteryWeapons.Primary).TurnDistance.max = NewBatteryWeapons(BatteryWeapons.Primary).TurnDistance.max * TurnScale
        nShip.Batteries.Primary = NewBatteryWeapons(BatteryWeapons.Primary)
        nShip.Batteries.Primary.Parent = nShip.Batteries
        nShip.Batteries.Secondary = NewBatteryWeapons(BatteryWeapons.Secondary)
        nShip.Batteries.Secondary.Parent = nShip.Batteries
        '-------------------
        '-----Shielding-----
        nShip.Shielding.ShipShields = NewShields
        nShip.Shielding.DefenceBonuses = NewShieldBonuses
        '-------------------
        '-----Engineering-----
        nShip.Engineering.PowerCore = NewEngineeringStats(EngineeringStats.PowerCore)
        nShip.Engineering.Engines = NewEngineeringStats(EngineeringStats.Engines)
        '---------------------
    End Sub

End Class
