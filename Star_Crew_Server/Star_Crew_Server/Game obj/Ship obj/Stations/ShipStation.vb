Public MustInherit Class ShipStation 'The Base class for the four stations on a Ship
    Public ParentShip As Ship 'The Ship that contains this ShipStation
    Public AIControled As Boolean = True 'A Boolean value indecating whether the Station is AI or Client controled
    Public Powered As Boolean = True 'A Boolean value indecating whether the Station is powered
    Public Integrity As Game_Library.StatInt 'A StatInt object representing the integrity of the ShipStation
    Public RepairCost As Double 'A Double value representing how much power it costs to repair 1 integrity
    Public Enum StationTypes 'The different types of Stations on a Ship
        Helm
        Battery
        Shields
        Engines
        max
    End Enum

    Public MustOverride Sub Update() 'Updates the ShipStation

End Class
