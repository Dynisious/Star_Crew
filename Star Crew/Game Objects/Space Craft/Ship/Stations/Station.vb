<Serializable()>
Public MustInherit Class Station
    <NonSerialized()>
    Public WithEvents Parent As Ship
    <NonSerialized()>
    Public PlayerControled As Boolean = False
    Public Enum StationTypes
        Helm
        Batteries
        Shielding
        Engineering
        Max
    End Enum
    <NonSerialized()>
    Public Power As Double
    <NonSerialized()>
    Public Influx As Integer

    Public Sub New(ByRef nParent As Ship)
        Parent = nParent
    End Sub

    Public MustOverride Sub Update()

End Class
