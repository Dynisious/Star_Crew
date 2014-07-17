Public MustInherit Class Engines 'Object responsible for the routing of power through a Ship
    Inherits ShipStation
    Public Generation As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum power output
    Public Integrety As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum integrety of the engines
    Public Throttle As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Ship's throttle

    Public Sub Destroy() 'Removes all references to the Engines object

    End Sub

    Public Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    End Function

    Public Overrides Sub Update() 'Distributes power around the Ship and repairs damaged PowerNodes

    End Sub

End Class
