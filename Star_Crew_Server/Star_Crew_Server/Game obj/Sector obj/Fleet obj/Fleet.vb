Public Class Fleet
    Inherits Game_Library.Entity
    Public ParentSector As Sector 'A reference to the Sector object that contains the Fleet
    Public index As Integer 'An Integer value representing the Index of the Fleet inside it's Sector
    Private _myAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.nill 'The actual value of myAllegiance
    Public ReadOnly Property myAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'An Integer value representing the Allegiance of the Fleet
        Get
            Return _myAllegiance
        End Get
    End Property
    Public shipList As New List(Of Ship) 'A list of Ship objects that make up the fleet
    Public ReadOnly maximumSize As Integer 'An Integer representing the maximum number of Ships that can be in the list of Ships
    Public speed As New Game_Library.StatDbl(-1, -1, -1) 'A StatDbl representing the Speed of the Fleet
    Private _acceleration As Double 'The actual value of acceleration
    Public ReadOnly Property acceleration As Double 'A Double value representing the acceleration of the Fleet
        Get
            Return _acceleration
        End Get
    End Property
    Public inventory As New List(Of Item) 'A list of Item objects that the Fleet has in it's inventory
    Public shipments As New List(Of Shipment) 'A list of Shipment objects carried by the Fleet
    Public Money As Integer 'An Integer value representing how much money this Fleet has

    Public Sub New(ByVal nAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances)
        _myAllegiance = nAllegiance
    End Sub

    Public Sub Set_Stats() 'Sets the statistics of the Fleet depending on the lowest values inside the list of Ships
        _acceleration = Integer.MaxValue 'Clear the acceleration
        speed.Maximum = Double.MaxValue 'Clear the maximum speed
        For Each i As Ship In shipList 'Loop through all the Ships
            If i.Engineering.Acceleration < acceleration Then 'This is the Slowest Ship
                _acceleration = i.Engineering.Acceleration 'Set the acceleration to the slowest acceleration
            End If
            If i.Engineering.Throttle.Maximum < speed.Maximum Then 'This Ship has a lower maximum speed
                speed.Maximum = i.Engineering.Throttle.Maximum 'Set the maximum speed
            End If
        Next
    End Sub

    Public Sub Remove_Ship(ByVal nIndex As Integer) 'Removes a Ship from the list of Ships at the specified index
        shipList.RemoveAt(nIndex) 'Remove the Ship
        shipList.TrimExcess() 'Clear empty spaces from the list
        If nIndex <> shipList.Count Then 'There are Ships higher in the list
            For i As Integer = nIndex To shipList.Count - 1 'Loop to the end of the list
                shipList(i).FleetIndex = i 'Set the new index
            Next
        End If
        If shipList.Count = 0 Then 'The Fleet is empty
            Destroy() 'Destroy the Fleet
        Else 'The Fleets stats may have changed
            Set_Stats() 'Set the Fleet's stats
        End If
    End Sub

    Public Sub Add_Ship(ByRef nShip As Ship, Optional ByVal nIndex As Integer = -1) 'Adds a Ship object to the list of Ships at the specified index or to the end of the list
        If nIndex = -1 Then 'Add the Ship to the end of the list
            shipList.Add(nShip) 'Add the Ship to the list
            nShip.FleetIndex = (shipList.Count - 1) 'Set the Ship's FleetIndex
        Else 'Insert the Ship at the specified index
            shipList.Insert(nIndex, nShip) 'Insert the Ship into the list
            nShip.FleetIndex = nIndex 'Set the Ship's FleetIndex
        End If
        Set_Stats() 'Change the Fleet's stats
    End Sub

    Public Overrides Sub Destroy() 'Removes all references to the Fleet and calls Destroy() on all Ships inside the Fleet
        For Each i As Ship In shipList 'Loop though all the Ships
            i.Destroy()
        Next
        inventory.Clear() 'Clear the inventory
        shipments.Clear() 'Clear the shipments
        ParentSector.Remove_Fleet(index) 'Remove the Fleet from the Sector
        If Object.ReferenceEquals(Me, Server.GameWorld.ClientFleet) = True Then Server.Game_Over() 'The Client Fleet is dead and the game is over
    End Sub

    Public Overrides Sub Update() 'Sets the Fleets new position and speed
        Dim distance As Integer = Math.Sqrt((X ^ 2) + (Y ^ 2)) 'The distance of the Fleet from the center of the Sector
        Dim direction As Double 'The direction of the Fleet from the center of the Sector
        If X <> 0 Then 'The Fleet is not aligned with the y-axis
            direction = Math.Tanh(Y / X) 'Set the direction
            If X < 0 Then 'The direction is reflected in the line y=x
                direction = direction + Math.PI 'Reflect the direction
            End If
            direction = Server.Normalise_Direction(direction) 'Normalise the direction to be between 0 and 2Pi
        ElseIf Y > 0 Then 'The Fleet is directly above the center of the Sector
            direction = (Math.PI / 2)
        Else 'The Fleet is directly bellow the center of the Sector
            direction = (3 * Math.PI / 2)
        End If
        direction = direction + (2 * Math.PI * (speed.Current / (2 * Math.PI * distance))) + Sector.Rotation 'Add on the radians traveled
        X = distance * Math.Cos(direction) 'Set the new X coordinate
        Y = distance * Math.Cos(direction) 'Set the new Y coordinate
    End Sub

End Class
