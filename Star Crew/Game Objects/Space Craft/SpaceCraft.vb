<Serializable()>
Public MustInherit Class SpaceCraft
    <NonSerialized()>
    Public Index As Integer
    <NonSerialized()>
    Public Position As Point
    <NonSerialized()>
    Public MyAllegence As Galaxy.Allegence
    <NonSerialized()>
    Public Shared ReadOnly SpawnBox As Integer = 6000
    <NonSerialized()>
    Public Direction As Double
    <NonSerialized()>
    Public Speed As New StatDbl(0, 0)
    <NonSerialized()>
    Public Acceleration As New StatDbl(0, 0)
    <NonSerialized()>
    Public Dead As Boolean = False

End Class
