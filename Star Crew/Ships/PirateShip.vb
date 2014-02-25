Public Class PirateShip
    Inherits Ship

    Public Sub New(ByRef nParent As Galaxy, ByVal nShipStats As Layout)
        MyBase.New(nParent, nShipStats, Allegence.Pirate)
    End Sub

End Class
