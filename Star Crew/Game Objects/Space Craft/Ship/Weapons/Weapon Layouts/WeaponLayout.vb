Public MustInherit Class WeaponLayout
    Public Damage As StatDbl 'A StatDbl object representing the minimum and maximum damage the Weapon can do
    Public DamageType As Weapon.DamageTypes 'The type of damage this Weapon does
    Public Range As StatInt 'The range that the Weapon can fire
    Public Ready As StatDbl 'A StatDbl object indecating when the Weapon is ready to fire and controling
    'how long it takes to reload the Weapon
    Public Ammo As StatInt 'A StatInt object representing the current and maximum number of times this Weapon can fire
    Public Integrety As StatInt 'A StatInt object representing the Weapons current integrety out of it's
    'maximum integrety
    Public TurnDistance As StatDbl 'A StatDbl object representing the current and maximum values
    'for how far the Weapon can turn on it's mount
    Public TurnSpeed As StatDbl 'A StatDbl object representing the current and maximum values
    'for how quickly the Weapon can turn on it's mount

    Public Sub SetLayout(ByRef nWeapon As Weapon)
        nWeapon.Damage = Damage
        nWeapon.DamageType = DamageType
        nWeapon.Range = Range
        nWeapon.Ready = Ready
        nWeapon.Ammo = Ammo
        nWeapon.Integrety = Integrety
        nWeapon.TurnDistance = TurnDistance
        nWeapon.TurnSpeed = TurnSpeed
    End Sub

End Class
