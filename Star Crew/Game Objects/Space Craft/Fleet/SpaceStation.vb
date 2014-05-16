<Serializable()>
Public Class SpaceStation 'A Fleet with no alignment
    Inherits SpaceCraft 'The base Class of Ships, Fleets and Stations
    Public currentSector As Sector 'The Sector object the Fleet is currently inside

    Public Sub New(ByVal nIndex As Integer, ByRef nSector As Sector)
        MyBase.New(Galaxy.Allegence.Neutral, ShipLayout.Formats.Station, nIndex, New Point(Int(Rnd() * SpawnBox), Int(Rnd() * SpawnBox)))
        currentSector = nSector
    End Sub

    Public Shared Sub Heal(ByRef nFleet As Fleet)
        For Each i As Ship In nFleet.ShipList
            If i.Hull.current < i.Hull.max * 0.4 Then
                i.Hull.current = i.Hull.max * 0.4
            End If
        Next
    End Sub

    Public Sub UpdateStation()
        Dim count As Integer 'How many Fleets are near by the SpaceStation
        For Each i As Fleet In currentSector.fleetList
            Dim distance As Integer = Math.Sqrt(((Position.X - i.Position.X) ^ 2) + ((Position.Y - i.Position.Y) ^ 2))
            If distance < Fleet.InteractRange Then
                'Heal the Fleet and set the allegience of the Station
                SpaceStation.Heal(i)
                MyAllegence = i.MyAllegence
                count = count + 1
                If count > 3 Then 'The Station is over-populated
                    i.MovementState = Fleet.FleetState.Target
                End If
            End If
            If distance < Fleet.DetectRange / 2 Then 'The Fleet is counted as part of the stations population
                count = count + 1
                If count > 3 Then 'The Station is over-populated
                    i.MovementState = Fleet.FleetState.Target
                End If
            End If
        Next
        If MyAllegence <> Galaxy.Allegence.Neutral Then
            If 0 = Int(Rnd() * 60) Then 'Spawn in a new Fleet every 6 seconds roughly
                Select Case MyAllegence
                    Case Galaxy.Allegence.Friendly
                        currentSector.AddFleet(New FriendlyFleet(currentSector.fleetList.Count))
                    Case Galaxy.Allegence.Pirate
                        currentSector.AddFleet(New PirateFleet(currentSector.fleetList.Count))
                End Select
                currentSector.fleetList(currentSector.fleetList.Count - 1).Position =
                    New Point(Position.X + Int((Rnd() * 2 * Fleet.InteractRange) - Fleet.InteractRange),
                              Position.Y + Int((Rnd() * 2 * Fleet.InteractRange) - Fleet.InteractRange))
            End If
        End If
    End Sub

End Class
