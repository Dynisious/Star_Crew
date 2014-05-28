<Serializable()>
Public Class FriendlyFleet 'A Fleet aligned to the Player
    Inherits Fleet 'The base Class for all Fleets

    Public Sub New(ByVal nIndex As Integer, ByRef nSpaceStation As SpaceStation)
        MyBase.New(nIndex, Galaxy.Allegence.Friendly, ShipLayout.Formats.Fleet, nSpaceStation)
    End Sub

    Public Overrides Sub UpdateFleet()
        If ReferenceEquals(Me, ConsoleWindow.GameServer.GameWorld.centerFleet) = True Then 'It's the players Fleet so it needs to use this code
            For Each i As Fleet In currentSector.fleetList 'Check for interactions
                Dim distance As Integer = Math.Sqrt(((i.Position.X - Position.X) ^ 2) + ((i.Position.Y - Position.Y) ^ 2)) 'The distance between
                'the two Fleets
                If distance <= InteractRange And ReferenceEquals(i, Me) = False Then 'The Fleets can interact
                    If i.MyAllegence = Galaxy.Allegence.Pirate Then 'Enter a combat cenario
                        ConsoleWindow.GameServer.GameWorld.CombatSpace.Generate(i)
                        Speed.current = 0
                    ElseIf i.MyAllegence = Galaxy.Allegence.Friendly And ShipList.Count < PopulationCap And
                        i.Format <> ShipLayout.Formats.Station Then 'Combine the two Fleets
                        Dim temp As Integer = ShipList.Count 'The number of Ships in this Fleet
                        For e As Integer = 0 To PopulationCap - 1 - temp 'Loop until the Fleet is full
                            If i.ShipList.Count > 0 Then 'There's still Ships to move
                                Dim nShip As Ship = i.ShipList(0) 'The Ship that is being moved
                                ShipList.Add(nShip) 'Add the Ship to this Fleet
                                i.RemoveShip(nShip) 'Remove the Ship from the other Fleet
                            Else 'The Fleet needs to be removed
                                currentSector.RemoveFleet(i, True, False) 'Kill the Fleet
                                Exit For
                            End If
                        Next
                        Exit Sub
                    End If
                End If
            Next

            Position = New Point(Position.X + (Math.Cos(Direction) * Speed.current),
                                 Position.Y + (Math.Sin(Direction) * Speed.current))
            'Update the Fleets position
        Else
            MyBase.UpdateFleet()
        End If
    End Sub

End Class
