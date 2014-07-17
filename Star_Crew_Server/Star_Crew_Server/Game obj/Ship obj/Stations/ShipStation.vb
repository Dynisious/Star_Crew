Public MustInherit Class ShipStation 'The Base class for the four stations on a Ship
    Public ParentShip As Ship 'The Ship that contains this ShipStation
    Public AIControled As Boolean = True 'A Boolean value indecating whether the Ship is AI or Client controled
    Public Enum StationTypes 'The different types of Stations on a Ship
        Helm
        Battery
        Shields
        Engines
        max
    End Enum

    Public MustOverride Sub Update() 'Updates the ShipStation object

End Class
