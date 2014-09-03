Public Class Sector 'An object that represents an area in the Galaxy
    Private _Location As System.Drawing.Point 'The actual value of Location
    Public ReadOnly Property Location As System.Drawing.Point 'The Sectors location in the Galaxy
        Get
            Return _Location
        End Get
    End Property
    Private _index As Integer 'The actual value of index
    Public ReadOnly Property index As Integer 'An intger value representing the Sectors Index in the Galaxy's list
        Get
            Return index
        End Get
    End Property
    Private _Connections(-1) As Integer 'The actual value of Connections
    Public ReadOnly Property Connections As Integer() 'An array of Integers representing the indexes of other Sectors this one connects to
        Get
            Return _Connections
        End Get
    End Property
    Private _MyPlanet As Planet 'The actual value of MyPlanet
    Public ReadOnly Property MyPlanet As Planet 'A Planet object inside the Sector
        Get
            Return _MyPlanet
        End Get
    End Property
    Public FleetList As New List(Of Fleet) 'A list of Fleet objects inside the Sector
    Private _SpaceStations(-1) As SpaceStation 'The actual value of SpaceStations
    Public ReadOnly Property SpaceStations As SpaceStation() 'An Array of SpaceStations inside the Sector
        Get
            Return _SpaceStations
        End Get
    End Property
    Private _myAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances =
        Star_Crew_Shared_Libraries.Shared_Values.Allegiances.nill  'The actual value of myAllegiance
    Public ReadOnly Property myAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances
        Get
            Return _myAllegiance
        End Get
    End Property
    Public Shared ReadOnly Rotation As Double = (2 * Math.PI / 200) 'A Double value representing how many radians a Sector rotates per update
    Private Shared ReadOnly SpawnBox As Integer = 6000 'An Integer value representing the square area that the Sectors spawn inside

    Public Sub New(ByVal nIndex As Integer)
        _MyPlanet = New Planet(Me, nIndex) 'Create a new Planet object
        _index = nIndex 'Set the Sectors index
        _Location = New System.Drawing.Point(Int(Rnd() * SpawnBox), Int(Rnd() / SpawnBox)) 'Set the Sector's location
    End Sub

    Public Sub Add_Fleet(ByRef nFleet As Fleet) 'Adds the Fleet to the list of Fleets and sets their index
        FleetList.Add(nFleet) 'Add the Fleet to the list
        nFleet.index = FleetList.Count - 1 'Set the Fleet's index
        For Each i As Shipment In nFleet.shipments
            If i.SectorIndex = index Then 'The Shipment is addressed to this Sector
                nFleet.Money = nFleet.Money + i.value 'Add the reward money to the Fleet
                nFleet.shipments.RemoveAt(i.Index) 'Remove the Shipment from the list
                nFleet.shipments.TrimExcess() 'Remove blank spaces from the list
            End If
        Next
        If nFleet.myAllegiance <> myAllegiance Then 'The Sectors allegience is contested
            _myAllegiance = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Contested 'Set the Sector's allegiance to be conteseted
            If FleetList.Count <> 1 Then 'Combat needs to be initiated
                For Each i As Fleet In FleetList 'Loop through all Fleets
                    If Object.ReferenceEquals(Server.GameWorld.ClientFleet, i) = True Then 'Engage in Ship to Ship combat
                        Server.GameWorld.Combat.Generate_Scenario(FleetList) 'Generate the combat scenario
                    End If
                Next
            Else 'The new Fleet owns the Sector
                _myAllegiance = nFleet.myAllegiance 'Set the Sector's allegiance to the new Fleet's
            End If
        End If
    End Sub

    Public Sub Remove_Fleet(ByVal nIndex As Integer) 'Removes a Fleet from the list of Fleets at the specified index
        FleetList.RemoveAt(nIndex) 'Remove the Fleet
        FleetList.TrimExcess() 'Removes all blank spaces from the list
        If nIndex <> FleetList.Count Then 'There are Fleets higher in the list
            For i As Integer = nIndex To FleetList.Count - 1 'Loop through the rest of the list
                FleetList(i).index = i 'Set the new index
            Next
        End If
        If FleetList.Count = 0 Then _myAllegiance =
            Star_Crew_Shared_Libraries.Shared_Values.Allegiances.nill 'There's no Fleets inside the Sector
    End Sub

    Public Sub Update() 'Updates performs any AI_Combat for the Sector
        If myAllegiance = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Contested Then 'AI combat is necessary
            Dim attacker As Fleet = FleetList(Int(Rnd() * FleetList.Count)) 'Select an Attacking Fleet
            For Each i As Fleet In FleetList
                If i.myAllegiance <> attacker.myAllegiance Then
                    Server.GameWorld.Combat.AI_Combat(attacker, i) 'Fight the two Fleets
                    Exit For
                End If
            Next
            For Each i As Fleet In FleetList
                If i.myAllegiance <> FleetList(0).myAllegiance Then 'The Sector is still being contested over
                    Exit Sub 'Exit the Sub
                End If
            Next
            _myAllegiance = FleetList(0).myAllegiance 'The Sector is no longer being contested
        End If
    End Sub

End Class
