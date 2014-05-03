<Serializable()>
Public Class FriendlyShip 'A ship aligned to the Player
    Inherits Ship 'The base Class for all Ships

    Public Sub New(ByVal nShipStats As Layout, ByVal nIndex As Integer)
        MyBase.New(nShipStats, nIndex, Galaxy.Allegence.Player)
    End Sub

End Class
