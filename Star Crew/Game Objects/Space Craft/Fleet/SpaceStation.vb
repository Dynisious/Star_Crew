<Serializable()>
Public Class SpaceStation 'A Fleet with no alignment
    Inherits SpaceCraft 'The base Class of Ships, Fleets and Stations
    Public currentSector As Sector 'The Sector object the Space Station is currently inside
    Public Population As New List(Of Fleet) 'The Fleets nearby the SpaceStation
    Public Shared PopCap As Integer = 3 'The maximum number of Fleets allowed to stay at the station

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
        Population.Clear() 'Clear the population of the Station
        Population.TrimExcess()
        For Each i As Fleet In currentSector.fleetList 'Loop through all Fleets in the Sector
            Dim distance As Integer = Math.Sqrt(((Position.X - i.Position.X) ^ 2) + ((Position.Y - i.Position.Y) ^ 2)) 'Calculate the distance
            'of the Fleet
            If distance < Fleet.DetectRange Then 'They are part of the population of the Space Station
                If i.MyAllegence = MyAllegence And distance < Fleet.InteractRange Then 'Heal the Fleet
                    SpaceStation.Heal(i)
                End If
                Population.Add(i) 'Add them to the population of the Space Station
            End If
        Next

        If MyAllegence <> Galaxy.Allegence.Neutral Then
            If 0 = Int(Rnd() * 70) Then 'Spawn in a new Fleet every 7 seconds roughly
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
        For Each i As Fleet In Population 'Loop through the population of the Fleet
            popDivision(i.MyAllegence) = popDivision(i.MyAllegence) + 1 'Add 1 to the allegience
        Next
        For i As Integer = 0 To Galaxy.Allegence.max - 1
            If popDivision(i) > popDivision(MyAllegence) Then 'This allegience is the majority
                MyAllegence = i 'Set the allegience to the majority
            End If
        Next
        For i As Integer = 0 To Population.Count - 1 'Loop through the population of the Fleet
            If i < Population.Count Then 'Continue
                If Population(i).MyAllegence <> MyAllegence Then 'It's not allied with the Space Station
                    Population.Remove(Population(i)) 'Remove the Fleet from the population
                End If
            Else 'Exit the Loop
                Exit For
            End If
        Next
        For i As Integer = 0 To Population.Count - 1 'Loop through the remaining population
            If i < PopCap Then 'The Fleet is allowed to remain at the Station
                currentSector.fleetList(i).TargetLock = False 'The Fleet can choose it's own target
            Else 'Exit the Loop
                Exit For
            End If
        Next
    End Sub

End Class
