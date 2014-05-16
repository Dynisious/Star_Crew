<Serializable()>
Public Class Sector 'Encapsulates a group of Fleets that will interact with each other
    Public Shared centerFleet As Fleet 'The Players Fleet to control
    Public fleetList As New List(Of Fleet) 'A List of Fleet objects
    Public spaceStations(2) As SpaceStation 'An Array of three SpaceStation Objects

    Public Sub New(ByVal fleetCount As Integer)
        If fleetCount <> 0 Then 'Create the specified number of Fleets with random Allegencies
            For i As Integer = 0 To fleetCount - 1
                If Int(Rnd() * 2) = 0 Then
                    AddFleet(New FriendlyFleet(i))
                Else
                    AddFleet(New PirateFleet(i))
                End If
            Next
        End If
        For i As Integer = 0 To 2 'Create three neutral Fleets
            spaceStations(i) = New SpaceStation(i, Me)
        Next
    End Sub

    Public Sub AddFleet(ByRef nFleet As Fleet, Optional ByVal InsertIndex As Integer = -1) 'Add a Fleet object to the List
        If InsertIndex = -1 Then 'Add the Fleet to the end of the List
            nFleet.Index = fleetList.Count
            fleetList.Add(nFleet)
        Else 'Add the Fleet at the specified index
            fleetList.Insert(InsertIndex, nFleet)
            For i As Integer = 0 To fleetList.Count - 1
                fleetList(i).Index = i
            Next
        End If
        nFleet.currentSector = Me 'Set the Fleet to exist inside this Sector
        If ReferenceEquals(nFleet, centerFleet) = True Then 'This is the centerSector now
            ConsoleWindow.GameServer.GameWorld.centerSector = Me
        End If
    End Sub

    Public Sub RemoveFleet(ByRef nFleet As Fleet, ByVal KillFleet As Boolean, ByVal KillShips As Boolean) 'Remove the Specified Fleet from the List
        fleetList.RemoveAt(nFleet.Index) 'Remove the Fleet
        fleetList.TrimExcess() 'Remove the blank space
        For i As Integer = 0 To fleetList.Count - 1 'Update the Fleets indexs
            fleetList(i).Index = i
        Next
        If KillFleet = True Then 'Kill the Fleet
            nFleet.Dead = True 'Set the Fleet to be dead
            If KillShips = True Then 'All Ships inside the Fleet need to be destroyed
                For i As Integer = 0 To nFleet.ShipList.Count - 1 'Destroy all the Ships
                    If i < nFleet.ShipList.Count Then
                        nFleet.ShipList(i).DestroyShip()
                    End If
                Next
                nFleet.ShipList.Clear() 'Clear the List
                nFleet.ShipList.TrimExcess() 'Remove all blank spaces
            End If
        End If
    End Sub

    Public Sub UpdateSector()
        For i As Integer = 0 To fleetList.Count - 1 'Update all Fleets
            If i < fleetList.Count Then
                fleetList(i).UpdateFleet()
            End If
        Next
        For Each i As SpaceStation In spaceStations 'Update all Fleets
            i.UpdateStation()
        Next
    End Sub

End Class
