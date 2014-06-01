<Serializable()>
Public Class Weapon
    Public Parent As Battery 'The Battery object that this Weapon is a part of
    Public Enum DamageTypes
        Laser
        Slug
        Max
    End Enum
    Public Damage As StatDbl 'A StatDbl object representing the current and maximum damage done by the Weapon
    Public DamageType As DamageTypes 'The type of damage this Weapon does
    Public Range As StatInt 'A StatInt object representing the current and maximum range of this Weapon
    Public Ready As StatDbl 'A StatDbl object representing whether the Weapon is ready to fire and how
    'long it takes to reload the Weapon
    Public Ammunition As StatInt 'A StatInt object representing the current and maximum number of times the Weapon can shoot
    Public Integrety As StatInt 'A StatInt object representing the current integrety of the Weapon out of it's maximum
    Public TurnDistance As StatDbl 'A StatDbl object representing the current and maximum values for the distances the Weapon can turn
    Public TurnSpeed As StatDbl 'A StatDbl object representing the current and maximum values for how quickly the Weapon turn
    Private reloadCount As Integer 'A counter to limit the round generation to as many tenths of a second as the Weapon does damage

    Public Sub New(ByVal nWeaponLayout As WeaponLayout)
        nWeaponLayout.SetLayout(Me)
    End Sub

    Public Sub UpdateWeapon()
        If Ammunition.current < Ammunition.max Then 'Get more rounds ready to fire
            Parent.Parent.Engineering.batteriesDraw =
                Parent.Parent.Engineering.batteriesDraw + (Damage.max * (Ammunition.max - Ammunition.current)) - Parent.Power.current
            'Tell engineering how much Power is needed to generate the shots
            If Parent.Power.current >= Damage.max And reloadCount = Damage.max Then 'Generate another round
                Ammunition.current = Ammunition.current + 1
                Parent.Power.current = Parent.Power.current - Damage.max
            ElseIf reloadCount < Damage.max Then 'Add another tenth of a second to the count
                reloadCount = reloadCount + 1
            End If
        End If
        If Ready.current < Ready.max Then 'Continue to reload the Weapon
            Ready.current = Ready.current + 1 'Add another tenth of a second to the count
        End If
    End Sub

    Public Sub ChangeStats()
        Dim fraction As Double = (Integrety.current / Integrety.max) 'The fraction at which all the stats should opperate
        Damage.current = Damage.max * fraction 'Change the damage
        Range.current = Range.max * fraction 'Change the range
        TurnSpeed.current = TurnSpeed.max * fraction 'Change the turn speed
    End Sub

    Public Sub FireWeapon(ByVal distance As Integer, ByRef target As Ship, ByVal direction As Double) 'Attempt to fire the Weapon
        If distance <= Range.current And Ammunition.current <> 0 And
            Integrety.current > 0 And Ready.current = Ready.max Then 'The target is within range, there's ammunition, the Weapon
            'is opperable and the Weapon is ready to be fired
            Ready.current = 0 'Reset the count so that the Weapon begins to reload
            Ammunition.current = Ammunition.current - 1 'Decrease the number of available Shots by 1
            target.TakeDamage(Me, Parent.Parent, direction) 'Let the Target calculate what damage is done to it
            Parent.Parent.Firing = True 'The Ship knows that it is firing
            If ReferenceEquals(target, Parent.Parent.Target) = True And target.Dead = True Then 'Unlock the Ship to
                'freely target Ships
                Parent.Parent.TargetLock = False
            End If
        End If
    End Sub

End Class
