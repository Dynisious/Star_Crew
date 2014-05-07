<Serializable()>
Public Class PirateFleet 'A Fleet aligned to Pirates
    Inherits Fleet

    Public Sub New(ByVal nIndex As Integer)
        MyBase.New(nIndex, Galaxy.Allegence.Pirate, ShipLayout.Formats.Fleet)
    End Sub

End Class
