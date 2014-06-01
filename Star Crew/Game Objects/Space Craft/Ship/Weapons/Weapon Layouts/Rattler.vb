<Serializable()>
Public Class Rattler
    Inherits WeaponLayout

    Public Sub New()
        Damage = New StatDbl(13, 13)
        DamageType = Weapon.DamageTypes.Laser
        Ammunition = New StatInt(10, 10)
        Integrety = New StatInt(100, 100)
        Range = New StatInt(70, 70)
        Ready = New StatDbl(3, 3)
        TurnDistance = New StatDbl(0, (2 * Math.PI) / 5)
        TurnSpeed = New StatDbl((Math.PI / 40), (Math.PI / 40))
    End Sub

End Class
