Public Class Sector 'An object that represents an area in the Galaxy
    Public Location As System.Drawing.Point 'The Sectors location in the Galaxy
    Public name As String 'A string value representing the name of the Sector
    Public index As Integer 'An intger value representing the Sectors Index in the Galaxy's list
    Public connections As Integer 'An array of Integers representing the indexes of other Sectors this one connects to
    Public MyPlanet As Planet 'A Planet object inside the Sector
    Public fleetList(-1) As Fleet 'A list of Fleet objects inside the Sector
    Public spaceStations(-1) As SpaceStation 'An Array of SpaceStations inside the Sector

    Public Sub Add_Fleet() '(ByRef nFleet As Fleet) 'Adds the Fleet to the list of Fleets and sets their index

    End Sub

    Public Sub Remove_Fleet() '(byval nIndex as Integer) 'Removes a Fleet from the list of Fleets at the specified index

    End Sub

End Class
