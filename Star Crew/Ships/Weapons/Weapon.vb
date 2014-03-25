﻿<Serializable()>
Public Class Weapon
    Public Parent As Battery
    Public Enum DamageTypes
        Laser
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

    Public Sub New(ByVal nWeaponLayout As WeaponLayout)
        nWeaponLayout.SetLayout(Me)
    End Sub

    Public Sub UpdateWeapon()
        If WeaponStats(Stats.Ready).current < WeaponStats(Stats.Ready).max Then
            Parent.Parent.Engineering.batteriesDraw =
                Parent.Parent.Engineering.batteriesDraw + (WeaponStats(Stats.Damage).current / WeaponStats(Stats.Ready).max)
            If Parent.Power >= (WeaponStats(Stats.Damage).current / WeaponStats(Stats.Ready).max) Then
                WeaponStats(Stats.Ready).current = WeaponStats(Stats.Ready).current + 1
                Parent.Power = Parent.Power - (WeaponStats(Stats.Damage).current / WeaponStats(Stats.Ready).max)
            End If
        End If
    End Sub

    Public Sub ChangeStats()
        Dim fraction As Double = (WeaponStats(Stats.Integrety).current / WeaponStats(Stats.Integrety).max)
        WeaponStats(Stats.Damage).current = WeaponStats(Stats.Damage).max * fraction
        WeaponStats(Stats.Range).current = WeaponStats(Stats.Range).max * fraction
        TurnSpeed.current = TurnSpeed.max * fraction
    End Sub

    Public Sub FireWeapon(ByVal distance As Integer, ByRef target As Ship)
        Randomize()
        If distance <= WeaponStats(Stats.Range).current And
            WeaponStats(Stats.Ammo).current <> 0 And
            WeaponStats(Stats.Integrety).current > 0 And
            WeaponStats(Stats.Ready).current = WeaponStats(Stats.Ready).max Then
            WeaponStats(Stats.Ready).current = 0
            WeaponStats(Stats.Ammo).current = WeaponStats(Stats.Ammo).current - 1
            target.TakeDamage(Me, Parent.Parent)
            If ReferenceEquals(target, Parent.Parent.Helm.Target) = True And target.Dead = True Then
                Parent.Parent.TargetLock = False
            End If
        End If
    End Sub

End Class
