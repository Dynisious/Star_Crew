<Serializable()>
Public Class FriendlyShip
    Inherits Ship

    Public Sub New(ByVal nShipStats As Layout, ByVal nIndex As Integer)
        MyBase.New(nShipStats, nIndex, Allegence.Player)
    End Sub

End Class
