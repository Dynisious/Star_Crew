<Serializable()>
Public Class PlayerShip
    Inherits Ship

    Public Sub New(ByVal nShipStats As Layout)
        MyBase.New(Nothing, nShipStats, Allegence.Player)
    End Sub

    Public Sub HelmMessage(ByVal nMessage As Helm)

    End Sub

    Public Sub BatteriesMessage(ByVal nMessage As Battery)

    End Sub

    Public Sub ShieldingMessage(ByVal nMessage As Shields)

    End Sub

    Public Sub EngineeringMessage(ByVal nMessage As Engineering)

    End Sub
End Class
