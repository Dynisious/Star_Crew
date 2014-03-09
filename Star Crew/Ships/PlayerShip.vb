<Serializable()>
Public Class PlayerShip
    Inherits Ship

    Public Sub New(ByVal nShipStats As Layout)
        MyBase.New(Nothing, nShipStats, Allegence.Player)
    End Sub
End Class
