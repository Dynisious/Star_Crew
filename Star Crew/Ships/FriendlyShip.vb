<Serializable()>
Public Class FriendlyShip
    Inherits Ship

    Public Sub New(ByRef nParent As Galaxy, ByVal nShipStats As Layout)
        MyBase.New(nParent, nShipStats, Allegence.Player)
    End Sub

End Class
