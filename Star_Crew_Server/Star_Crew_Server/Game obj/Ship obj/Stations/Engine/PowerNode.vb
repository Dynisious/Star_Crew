Public Class PowerNode 'object that links with other nodes to form paths from the engines to the ShipStations
    Public ParentShip As Ship 'A reference to the Ship that this PowerNode is inside
    Public Operational As Boolean = True 'A Boolean value indecating whether the PowerNode can have powered routed through it
    Public Powered As Boolean = True 'A Boolean value indecating whether the PowerNode is Powered
    Public Repairing As Boolean = False 'A Boolean value indicating whether the PowerNode is being repaired
    Public Index As Integer 'An integer representing the index of the PowerNode in the Ship's list of nodes
    Public Connections() As Integer 'An array of Integers representing the indexes of the other PowerNodes this one connects to
    Public Location As System.Drawing.Point 'A Point object representing the PowerNode's position in the Ship
    Public Type As NodeTypes 'A NodeTypes value indicating what type of PowerNode this one is
    Public Direction As Double 'A Double value representing the direction of the PowerNode from the centre of the Ship
    Public Distance As Integer 'An Integer value representing the distance of the PowerNode from the centre of the Ship
    Public Enum NodeTypes 'The Different types of PowerNodes
        Shields
        Battery
        Engines
        Routing
    End Enum

    Public Sub Update() 'Checks if the PowerNode is receiving power

        If Operational = True Then 'Check if it is receiving power
            Powered = False 'We assume it is not receiving power
            For Each i As Integer In Connections
                If ParentShip.PowerNet(i).Powered = True Then 'This node is receiving power
                    Powered = True
                    Exit For
                End If
            Next
        Else 'The PowerNode cannot receive power
            Powered = False
            If Type = NodeTypes.Engines Then 'Cut Power to everthing
                For Each i As PowerNode In ParentShip.PowerNet
                    i.Powered = False
                Next
                ParentShip.Batteries.Powered = False
                ParentShip.Shielding.Powered = False
            End If
        End If
        Select Case Type
            Case NodeTypes.Battery 'Set all power to the Batteries
                ParentShip.Batteries.Powered = Powered
            Case NodeTypes.Shields 'Set all Power to the Shields
                ParentShip.Shielding.Powered = Powered
        End Select
    End Sub

End Class
