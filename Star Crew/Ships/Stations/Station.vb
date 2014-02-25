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
    Public Type As StationTypes
    Public Power As Integer
    Public Influx As Integer

    Public Sub New(ByRef nParent As Ship)
        parent = nParent
    End Sub

    Public MustOverride Sub Update()

End Class
