Public Class Fleet
    Inherits Game_Library.Entity
    Public ParentSector As Sector 'A reference to the Sector object that contains the Fleet
    Public index As Integer 'An Integer value representing the Index of the Fleet inside it's Sector
    Public shipList As New List(Of Ship) 'A list of Ship objects that make up the fleet
    Public maximumSize As Integer 'An Integer representing the maximum number of Ships that can be in the list of Ships
    Public speed As Game_Library.StatDbl 'A StatDbl representing the Speed of the Fleet
    Public acceleration As Double 'A Double value representing the acceleration of the Fleet
    Public turnSpeed As Double 'A Double value representing the turning speed of the Fleet
    Public inventory As New List(Of Item) 'A list of Item objects that the Fleet has in it's inventory
    Public shipments As New List(Of Shipment) 'A list of Shipment objects carried by the Fleet

    Public Sub Set_Stats() 'Sets the turning speed, acceleration and speed statistics of the Fleet depending on the lowest values inside the list of Ships

    End Sub

    Public Sub Remove_Ship(ByVal nIndex As Integer) 'Removes a Ship from the list of Ships at the specified index

    End Sub

    Public Sub Add_Ship(ByRef nShip As Ship, Optional ByVal nIndex As Integer = -1) 'Adds a Ship object to the list of Ships at the specified index or to the end of the list

    End Sub

    Public Overrides Sub Destroy() 'Removes all references to the Fleet and calls Destroy() on all Ships inside the Fleet

    End Sub

    Public Overrides Sub Update() 'Updates all Engine stations so that subsystems get repaired and sets the Fleets new position, direction and speed

    End Sub

End Class
