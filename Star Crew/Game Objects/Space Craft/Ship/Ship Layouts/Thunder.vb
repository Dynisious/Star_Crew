Public Class Thunder
    Inherits ShipLayout

    Public Sub New()
        Format = ShipLayout.Formats.Thunder
        Hull = New StatDbl(200, 200)
        Speed = New StatDbl(Helm.MinimumSpeed, 10)
        Acceleration = New StatDbl(0.15, 0.15)

        TurnSpeed = New StatDbl(Math.PI / 30, Math.PI / 30)

        PrimaryMount = Math.PI / 6
        SecondaryMount = -Math.PI / 6
        Primary = New Weapon(New Thumper)
        Secondary = New Weapon(New Rattler)
        BatteriesPower = New StatInt(0, 40)

        ShieldingStats = New HeriaShields
        ShieldingPower = New StatInt(0, 60)

        EngineeringStats = New CorrierEngine
        EngineeringPower = New StatInt(0, 150)
    End Sub

End Class
