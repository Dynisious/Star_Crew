Public MustInherit Class WeaponLayout
    Public Damage As StatDbl
    Public DamageType As Weapon.DamageTypes
    Public Range As StatInt
    Public Ready As StatDbl
    Public Ammo As StatInt
    Public Integrety As StatInt
    Public TurnDistance As StatDbl
    Public TurnSpeed As StatDbl

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
