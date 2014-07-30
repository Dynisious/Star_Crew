Public Class Sector 'An object that represents an area in the Galaxy
    Public Location As System.Drawing.Point 'The Sectors location in the Galaxy
    Public name As String 'A string value representing the name of the Sector
    Public index As Integer 'An intger value representing the Sectors Index in the Galaxy's list
    Public connections As Integer 'An array of Integers representing the indexes of other Sectors this one connects to
    Public MyPlanet As Planet 'A Planet object inside the Sector
    Public fleetList As New List(Of Fleet) 'A list of Fleet objects inside the Sector
    Public spaceStations(-1) As SpaceStation 'An Array of SpaceStations inside the Sector
    Public Shared ReadOnly Rotation As Double = (2 * Math.PI / 200) 'A Double value representing how many radians a Sector rotates per update

    Public Sub Add_Fleet(ByRef nFleet As Fleet) 'Adds the Fleet to the list of Fleets and sets their index
        fleetList.Add(nFleet) 'Add the Fleet to the list
        nFleet.index = fleetList.Count - 1 'Set the Fleet's index
        For Each i As Shipment In nFleet.shipments
            If i.SectorIndex = index Then 'The Shipment is addressed to this Sector
                nFleet.Money = nFleet.Money + i.value 'Add the reward money to the Fleet
                nFleet.shipments.RemoveAt(i.Index) 'Remove the Shipment from the list
                nFleet.shipments.TrimExcess() 'Remove blank spaces from the list
            End If
        Next
    End Sub

    Public Sub Remove_Fleet(ByVal nIndex As Integer) 'Removes a Fleet from the list of Fleets at the specified index
        fleetList.RemoveAt(nIndex) 'Remove the Fleet
        fleetList.TrimExcess() 'Removes all blank spaces from the list
        If nIndex <> fleetList.Count Then 'There are Fleets higher in the list
            For i As Integer = nIndex To fleetList.Count - 1 'Loop through the rest of the list
                fleetList(i).index = i 'Set the new index
            Next
        End If
    End Sub

End Class
