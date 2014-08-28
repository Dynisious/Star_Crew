Public MustInherit Class ShipStation 'The Base class for the four stations on a Ship
    Private _ParentShip As Ship 'The actual value of ParentShip
    Public ReadOnly Property ParentShip As Ship 'The Ship that contains this ShipStation
        Get
            Return _ParentShip
        End Get
    End Property
    Public AIControled As Boolean = True 'A Boolean value indecating whether the Station is AI or Client controled
    Public Powered As Boolean = True 'A Boolean value indecating whether the Station is powered
    Public Integrity As Game_Library.StatInt 'A StatInt object representing the integrity of the ShipStation
    Private _RepairCost As Double 'The actual value of RepairCost
    Public ReadOnly Property RepairCost As Double 'A Double value representing how much power it costs to repair 1 integrity
        Get
            Return _RepairCost
        End Get
    End Property

    Public Sub New(ByRef nParent As Ship, ByVal nIntegrety As Game_Library.StatInt, ByVal nRepairCost As Double)
        _ParentShip = nParent
        Integrity = nIntegrety
        _RepairCost = nRepairCost
    End Sub

    Public Sub Destroy()
        Finalise_Destroy()
        _ParentShip = Nothing
    End Sub

    Protected MustOverride Sub Finalise_Destroy()

    Public MustOverride Sub Update() 'Updates the ShipStation

End Class
