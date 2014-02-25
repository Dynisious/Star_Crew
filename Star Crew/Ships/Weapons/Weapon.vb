Public Class Weapon
    Public Parent As Battery
    Public Enum DamageTypes
        Slug
        Max
    End Enum
    Public Enum Stats
        Damage
        DamageType
        Range
        Ready
        Ammo
        Integrety
        Max
    End Enum
    Public TurnDistance As Stat
    Public TurnSpeed As Stat
    Public WeaponStats(Stats.Max - 1) As Stat

    Public Sub New(ByVal nWeaponLayout As weaponLayout)
        nWeaponLayout.SetLayout(Me)
    End Sub

    Public Sub UpdateWeapon()
        '-----Costs-----
        If WeaponStats(Stats.Ready).current < WeaponStats(Stats.Ready).max Then
            Parent.Parent.Engineering.batteriesDraw =
                Parent.Parent.Engineering.batteriesDraw + (WeaponStats(Stats.Damage).current / 10)
        End If
        '---------------
        If WeaponStats(Stats.Ready).current < WeaponStats(Stats.Ready).max And
            Parent.Power >= WeaponStats(Stats.Damage).current Then
            WeaponStats(Stats.Ready).current = WeaponStats(Stats.Ready).current + 1
            Parent.Power = Parent.Power - WeaponStats(Stats.Damage).current
        End If
    End Sub

    Public Sub FireWeapon(ByVal distance As Integer)
        Randomize()
        If WeaponStats(Stats.Ammo).current > 0 And
            WeaponStats(Stats.Integrety).current > 0 And
            WeaponStats(Stats.Ready).current = WeaponStats(Stats.Ready).max Then
            WeaponStats(Stats.Ready).current = 0
            WeaponStats(Stats.Ammo).current = WeaponStats(Stats.Ammo).current - 1
            Parent.Parent.Target.TakeDamage(Me)
        End If
    End Sub

End Class
