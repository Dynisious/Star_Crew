Public MustInherit Class Shipment
    Inherits Item
    Private _SectorIndex As Integer 'The actual value of SectorIndex
    Public ReadOnly Property SectorIndex As Integer 'An Integer value representing the index of the Sector the Shipment is addressed to
        Get
            Return _SectorIndex
        End Get
    End Property
    Private _value As Integer 'The actual value of value
    Public ReadOnly Property value As Integer 'An Integer value representing the reward value of this Shipment
        Get
            Return _value
        End Get
    End Property

    Sub New(ByVal nSectorIndex As Integer, ByVal nValue As Integer)
        _SectorIndex = nSectorIndex
        _value = nValue
    End Sub

    Public Overrides Sub Use(ByRef data As Object)

    End Sub
End Class
