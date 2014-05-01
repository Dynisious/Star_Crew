﻿<Serializable()>
Public Class Weapon
    <NonSerialized()>
    Public Parent As Battery
    Public Enum DamageTypes
        Laser
        Max
    End Enum
    <NonSerialized()>
    Public Damage As StatDbl
    <NonSerialized()>
    Public DamageType As DamageTypes
    Public Range As StatInt
    <NonSerialized()>
    Public Ready As StatDbl
    <NonSerialized()>
    Public Ammo As StatInt
    Public Integrety As StatInt
    Public TurnDistance As StatDbl
    <NonSerialized()>
    Public TurnSpeed As StatDbl

    Public Sub New(ByVal nWeaponLayout As WeaponLayout)
        nWeaponLayout.SetLayout(Me)
    End Sub

    Public Sub UpdateWeapon()
        If Ready.current < Ready.max Then
            Parent.Parent.Engineering.batteriesDraw =
                Parent.Parent.Engineering.batteriesDraw + (Damage.current / Ready.max)
            If Parent.Power >= (Damage.current / Ready.max) Then
                Ready.current = Ready.current + 1
                Parent.Power = Parent.Power - (Damage.current / Ready.max)
            End If
        End If
    End Sub

    Public Sub ChangeStats()
        Dim fraction As Double = (Integrety.current / Integrety.max)
        Damage.current = Damage.max * fraction
        Range.current = Range.max * fraction
        TurnSpeed.current = TurnSpeed.max * fraction
    End Sub

    Public Sub FireWeapon(ByVal distance As Integer, ByRef target As Ship, ByVal direction As Double)
        Randomize()
        If distance <= Range.current And
            Ammo.current <> 0 And
            Integrety.current > 0 And
            Ready.current = Ready.max Then
            Ready.current = 0
            Ammo.current = Ammo.current - 1
            target.TakeDamage(Me, Parent.Parent, direction)
            Parent.Parent.Firing = True
            If ReferenceEquals(target, Parent.Parent.Helm.Target) = True And target.Dead = True Then
                Parent.Parent.TargetLock = False
            End If
        End If
    End Sub

End Class