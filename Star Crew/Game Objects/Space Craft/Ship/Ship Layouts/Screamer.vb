﻿Public Class Screamer
    Inherits ShipLayout

    Public Sub New()
        Format = ShipLayout.Formats.Screamer 'Set the Format for the Layout
        Hull = New StatDbl(125, 125)
        Speed = New StatDbl(Helm.MinimumSpeed, 15)
        Acceleration = New StatDbl(0.3, 0.3)

        TurnSpeed = New StatDbl(Math.PI / 15, Math.PI / 15)

        PrimaryMount = Math.PI / 8
        SecondaryMount = -Math.PI / 8
        Primary = New Weapon(New Rattler)
        Secondary = New Weapon(New Rattler)

        ShieldingStats = New HeriaShields

        EngineeringStats = New KnifeEdge

    End Sub

End Class
