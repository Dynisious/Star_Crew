<Serializable()>
Public Class SpaceStation 'A Fleet with no alignment
    Inherits SpaceCraft 'The base Class of Ships, Fleets and Stations
    Public currentSector As Sector 'The Sector object the Fleet is currently inside

    Public Sub New(ByVal nIndex As Integer, ByRef nSector As Sector, ByVal nAllegience As Galaxy.Allegence)
        MyBase.New(nAllegience, ShipLayout.Formats.Station, nIndex, New Point(Int(Rnd() * SpawnBox), Int(Rnd() * SpawnBox)))
        currentSector = nSector
    End Sub

    Public Shared Sub Heal(ByRef nFleet As Fleet)
        For Each i As Ship In nFleet.ShipList
            If i.Hull.current < i.Hull.max * 2 / 3 Then
                i.Hull.current = i.Hull.max * 2 / 3
            End If
        Next
    End Sub

    Public Sub UpdateStation()
        Dim pop As New List(Of Fleet) 'The Fleets nearby the SpaceStation
        For Each i As Fleet In currentSector.fleetList
            Dim distance As Integer = Math.Sqrt(((Position.X - i.Position.X) ^ 2) + ((Position.Y - i.Position.Y) ^ 2))
            If distance < Fleet.InteractRange * 2 Then
                'Heal the Fleet and set the allegience of the Station
                SpaceStation.Heal(i)
                MyAllegence = i.MyAllegence
                pop.Add(i)
            End If
        Next
        If pop.Count > 3 Then 'The Station is overpopulated
            For i As Integer = 1 To pop.Count - 3
                pop(Int(Rnd() * pop.Count)).MovementState = Fleet.FleetState.Target 'Send this random station to fight
            Next
        End If
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
