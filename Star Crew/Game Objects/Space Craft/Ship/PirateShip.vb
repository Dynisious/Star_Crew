<Serializable()>
Public Class PirateShip 'A Ship aligned with Pirates
    Inherits Ship 'The base Class for all Ships

    Public Sub New(ByVal nShipStats As Layout, ByVal nIndex As Integer)
        MyBase.New(nShipStats, nIndex, Galaxy.Allegence.Pirate)
    End Sub

End Class
