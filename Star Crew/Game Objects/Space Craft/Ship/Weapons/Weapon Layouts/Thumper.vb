Public Class Thumper
    Inherits WeaponLayout

    Public Sub New()
        Damage = New StatDbl(19, 19)
        DamageType = Weapon.DamageTypes.Slug
        Ammunition = New StatInt(5, 5)
        Integrety = New StatInt(110, 110)
        Range = New StatInt(100, 100)
        Ready = New StatDbl(4, 4)
        TurnDistance = New StatDbl(0, (2 * Math.PI) / 4)
        TurnSpeed = New StatDbl((Math.PI / 40), (Math.PI / 40))
    End Sub

End Class
