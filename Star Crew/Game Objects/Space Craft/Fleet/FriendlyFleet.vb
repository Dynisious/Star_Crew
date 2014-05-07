<Serializable()>
Public Class FriendlyFleet 'A Fleet aligned to the Player
    Inherits Fleet 'The base Class for all Fleets

    Public Sub New(ByVal nIndex As Integer)
        MyBase.New(nIndex, Galaxy.Allegence.Friendly, ShipLayout.Formats.Fleet)
    End Sub

End Class
