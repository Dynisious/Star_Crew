<Serializable()>
Public MustInherit Class SpaceCraft
    Public Index As Integer
    Public Position As Point
    Public MyAllegence As Galaxy.Allegence
    Public Shared ReadOnly SpawnBox As Integer = 6000
    Public Direction As Double
    Public Speed As New StatDbl(0, 0)
    Public Acceleration As New StatDbl(0, 0)
    Public Dead As Boolean = False

End Class
