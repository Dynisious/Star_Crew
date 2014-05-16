<Serializable()>
Public Class FriendlyFleet 'A Fleet aligned to the Player
    Inherits Fleet 'The base Class for all Fleets

    Public Sub New(ByVal nIndex As Integer)
        MyBase.New(nIndex, Galaxy.Allegence.Friendly, ShipLayout.Formats.Fleet)
    End Sub

    Public Overrides Sub UpdateFleet()
        If ReferenceEquals(Me, Sector.centerFleet) = True Then
            For Each i As Fleet In currentSector.fleetList 'Check for interactions
                Dim distance As Integer = Math.Sqrt(((i.Position.X - Position.X) ^ 2) + ((i.Position.Y - Position.Y) ^ 2))
                If distance <= InteractRange And ReferenceEquals(i, Me) = False Then
                    If i.MyAllegence = Galaxy.Allegence.Pirate Then 'Enter a combat cenario
                        ConsoleWindow.GameServer.GameWorld.CombatSpace.Generate(i)
                    ElseIf i.MyAllegence = Galaxy.Allegence.Friendly And ShipList.Count < 50 And i.Format <> ShipLayout.Formats.Station Then
                        'Add the ships to this Fleet
                        Dim temp As Integer = ShipList.Count
                        For e As Integer = 0 To 49 - temp
                            If i.ShipList.Count > 0 Then
                                ShipList.Add(i.ShipList(0))
                                Dim nShip As Ship = i.ShipList(0)
                                i.RemoveShip(nShip)
                            Else
                                currentSector.RemoveFleet(i, True, False)
                                Exit For
                            End If
                        Next
                        Exit Sub
                    End If
                End If
            Next
        End If

        MyBase.UpdateFleet()
    End Sub

End Class
