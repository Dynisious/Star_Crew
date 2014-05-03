<Serializable()>
Public Class PirateFleet 'A Fleet aligned to Pirates
    Inherits Fleet

    Public Sub New(ByVal nIndex As Integer)
        MyBase.New(nIndex, Galaxy.Allegence.Pirate)
    End Sub

End Class
