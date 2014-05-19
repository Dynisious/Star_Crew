<Serializable()>
Public MustInherit Class Station
    Public WithEvents Parent As Ship
    Public PlayerControled As Boolean = False
    Public Enum StationTypes
        Helm
        Batteries
        Shielding
        Engineering
        Max
    End Enum
    Public Power As Double
    Public Influx As Integer

    Public Sub New(ByRef nParent As Ship)
        Parent = nParent
    End Sub

    Public MustOverride Sub Update()

End Class
