Public Class Rattler
    Inherits WeaponLayout

    Public Sub New()
        stats(Weapon.Stats.Damage) = New Stat(15, 15)
        stats(Weapon.Stats.DamageType) = New Stat(Weapon.DamageTypes.Slug, Weapon.DamageTypes.Slug)
        stats(Weapon.Stats.Ammo) = New Stat(5000, 5000)
        stats(Weapon.Stats.Integrety) = New Stat(100, 100)
        stats(Weapon.Stats.Range) = New Stat(70, 70)
        stats(Weapon.Stats.Ready) = New Stat(3, 3)
        nTurnDistance = New Stat(0, (2 * Math.PI) / 5)
        nTurnSpeed = New Stat((Math.PI / 40), (Math.PI / 40))
    End Sub

End Class
