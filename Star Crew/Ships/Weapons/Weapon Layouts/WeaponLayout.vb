Public MustInherit Class WeaponLayout
    Public nTurnDistance As Stat
    Public nTurnSpeed As Stat
    Public stats(Weapon.Stats.Max - 1) As Stat

    Public Sub SetLayout(ByRef nWeapon As Weapon)
        For i As Integer = 0 To Weapon.Stats.Max - 1
            nWeapon.WeaponStats(i) = stats(i)
        Next
        nWeapon.TurnDistance = nTurnDistance
        nWeapon.TurnSpeed = nTurnSpeed
    End Sub

End Class
