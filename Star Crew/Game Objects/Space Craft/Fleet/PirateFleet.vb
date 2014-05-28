<Serializable()>
Public Class PirateFleet 'A Fleet aligned to Pirates
    Inherits Fleet

    Public Sub New(ByVal nIndex As Integer, ByRef nSpaceStation As SpaceStation)
        MyBase.New(nIndex, Galaxy.Allegence.Pirate, ShipLayout.Formats.Fleet, nSpaceStation)
    End Sub

End Class
