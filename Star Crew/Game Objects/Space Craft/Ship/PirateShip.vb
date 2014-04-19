<Serializable()>
Public Class PirateShip
    Inherits Ship

    Public Sub New(ByVal nShipStats As Layout, ByVal nIndex As Integer)
        MyBase.New(nShipStats, nIndex, Galaxy.Allegence.Pirate)
    End Sub

End Class
