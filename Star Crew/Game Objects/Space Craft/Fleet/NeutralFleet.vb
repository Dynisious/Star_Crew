<Serializable()>
Public Class NeutralFleet 'A Fleet with no alignment
    Inherits Fleet 'The base Class for all Fleets

    Public Sub New(ByVal nIndex As Integer)
        MyBase.New(nIndex, Galaxy.Allegence.Neutral, ShipLayout.Formats.Station)
    End Sub

    Public Shared Sub Heal(ByRef nFleet As Fleet)
        For Each i As Ship In nFleet.ShipList
            If i.Hull.current < i.Hull.max * 0.4 Then
                i.Hull.current = i.Hull.max * 0.4
            End If
        Next
    End Sub

End Class
