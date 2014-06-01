<Serializable()>
Public MustInherit Class Station
    Public Parent As Ship 'The parent Ship that the Station is a part of
    Public PlayerControled As Boolean = False 'A Boolean value indecating if a Player is controling this station or if it is AI controlled
    Public Enum StationTypes
        Helm
        Batteries
        Shielding
        Engineering
        Max
    End Enum
    Public Power As StatInt 'A StatInt object indecating the current and maximum units of Power Stored within the Station
    Public Influx As Integer 'An integer value indecating how many units of power the Station receives per update

    Public Sub New(ByRef nParent As Ship)
        Parent = nParent
    End Sub

    Public MustOverride Sub Update() 'A Subroutine that updates the Station

End Class
