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
                i.Hull.current = i.Hull.max * 2 / 3 'Repair the Hull
            End If
            i.Engineering.SubSystem.PowerCore.current = i.Engineering.SubSystem.PowerCore.max 'Repair the Power core
            i.Engineering.SubSystem.Engines.current = i.Engineering.SubSystem.Engines.max 'Repair the Engines
        Next
    End Sub

    Public Sub UpdateStation()
        Dim pop As New List(Of Fleet) 'The Fleets nearby the SpaceStation
        For Each i As Fleet In currentSector.fleetList
            Dim distance As Integer = Math.Sqrt(((Position.X - i.Position.X) ^ 2) + ((Position.Y - i.Position.Y) ^ 2)) 'Calculate the distance
            'of the Fleet
            If distance < Fleet.InteractRange * 5 Then 'They are part of the population of the Space Station
                If i.MyAllegence = MyAllegence Then 'Heal the Fleet
                    SpaceStation.Heal(i)
                End If
                pop.Add(i) 'Add them to the population of the Space Station
            End If
        Next
        If pop.Count > 3 Then 'The Station is overpopulated
            For i As Integer = 1 To pop.Count - 3 'Loop through the excess population
                pop(Int(Rnd() * pop.Count)).MovementState = Fleet.FleetState.Target 'Send this random Fleet to fight
            Next
        End If
        If MyAllegence <> Galaxy.Allegence.Neutral Then
            If 0 = Int(Rnd() * 60) Then 'Spawn in a new Fleet every 6 seconds roughly
                Select Case MyAllegence
                    Case Galaxy.Allegence.Friendly
                        currentSector.AddFleet(New FriendlyFleet(currentSector.fleetList.Count, Me))
                    Case Galaxy.Allegence.Pirate
                        currentSector.AddFleet(New PirateFleet(currentSector.fleetList.Count, Me))
                End Select
                currentSector.fleetList(currentSector.fleetList.Count - 1).Position =
                    New Point(Position.X + Int((Rnd() * 2 * Fleet.InteractRange) - Fleet.InteractRange),
                              Position.Y + Int((Rnd() * 2 * Fleet.InteractRange) - Fleet.InteractRange))
                'Set the Fleet's position
            End If
        End If
        Dim popDivision(Galaxy.Allegence.max - 1) As Integer 'The number of Fleets sorted by allegience
        For Each i As Fleet In pop
            popDivision(i.MyAllegence) = popDivision(i.MyAllegence) + 1 'Add 1 to the allegience
        Next
        For i As Integer = 0 To Galaxy.Allegence.max - 1
            If popDivision(i) > popDivision(MyAllegence) Then 'This allegience is the majority
                MyAllegence = i 'Set the allegience to the majority
            End If
        Next
    End Sub

End Class
