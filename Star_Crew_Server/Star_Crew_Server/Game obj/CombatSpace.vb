Public Class CombatSpace 'Object that encompasses the Ship to Ship combat
    Public ClientShip As Ship 'The Ship object that Clients man stations on
    Public EngagedFleets As New List(Of Fleet) 'A list of all Fleet objects in this combat scenario
    Public Combatants As New List(Of Ship) With {.Capacity = 12} 'The list of all Ship objects in this combat scenario
    Public ReadOnly Spawnbox As Integer = 6000 'The area that the Ship's spawn in

    Public Sub Update_Combat() 'Updates the combat scenario
        Dim allies As Integer = 0 'The count of how many allies are remaining
        Dim enemies As Integer = 0 'The count of how many enemies are remaining
        For Each i As Ship In Combatants 'Loop through all Ships
            If i.ParentFleet.myAllegiance = ClientShip.ParentFleet.myAllegiance Then 'Add to the count of Allies
                allies = allies + 1
            Else 'Add to the count of enemies
                enemies = enemies + 1
            End If
        Next
        If allies < 3 Then 'Add renforcements
            For i As Integer = 0 To 5 - allies 'Loop up to 6 times
                If i = ClientShip.ParentFleet.shipList.Count Then Exit For 'There are no other Ships in the Fleet to add
                Dim index As Integer = Combatants.Count 'The new index of the Ship in the Combatants list
                Combatants.Add(ClientShip.ParentFleet.shipList(i)) 'Add the Ship to the list
                Combatants(index).CombatIndex = index 'Set the Ship's combat index
                Combatants(index).X = Int(Rnd() * Spawnbox) 'Set the Ship's X coord
                Combatants(index).Y = Int(Rnd() * Spawnbox) 'Set the Ship's Y coord
                allies = allies + 1 'Add one to the allies count
            Next
        End If
        If enemies < 3 Then 'Add renforcements
            Dim fleet As Integer = Int(Rnd() * EngagedFleets.Count)
            For i As Integer = 0 To 5 - enemies 'Loop up to 6 times
                If i = EngagedFleets(fleet).shipList.Count Then Exit For 'There are no other Ships in the Fleet to add
                Dim index As Integer = Combatants.Count 'The new index of the Ship in the Combatants list
                Combatants.Add(EngagedFleets(fleet).shipList(i)) 'Add the Ship to the list
                Combatants(index).CombatIndex = index 'Set the Ships combat index
                Combatants(index).X = Int(Rnd() * Spawnbox) 'Set the Ship's X coord
                Combatants(index).Y = Int(Rnd() * Spawnbox) 'Set the Ship's Y coord
                enemies = enemies + 1 'Add one to the enemies count
            Next
        End If
        If allies <> 0 And enemies <> 0 Then 'Update the combat
            For i As Integer = 0 To Combatants.Count - 1
                Combatants(i).Update()
            Next
            For i As Integer = 0 To Combatants.Count - 1
                Combatants(i).Batteries.Update()
            Next
            For i As Integer = 0 To Combatants.Count - 1
                Combatants(i).Bridge.Update()
            Next
            For i As Integer = 0 To Combatants.Count - 1
                Combatants(i).Shielding.Update()
            Next
            For i As Integer = 0 To Combatants.Count - 1
                Combatants(i).Engineering.Update()
            Next
        Else
            For Each i As Ship In Combatants 'Loop through all Ships
                i.CombatIndex = -1 'Clear the combat index
            Next
            Server.GameWorld.State = Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Sector_Transit 'Go to Sector Transit state
            For Each i As Fleet In EngagedFleets 'Loop through all Fleets
                i.Set_Stats() 'Set the Fleet's stats
            Next
        End If
    End Sub

    Public Sub Remove_Ship(ByVal nIndex As Integer) 'Removes the Ship object at the specified index
        Combatants.RemoveAt(nIndex) 'Remove the Ship
        Combatants.TrimExcess() 'Removes the blank spaces
        Dim friendlies As Integer = 0 'The count of how many friendly Ship's their are
        Dim enemies As Integer = 0 'The count of how many enemy Ships their are
        For i As Integer = 0 To Combatants.Count - 1
            Combatants(i).CombatIndex = i
            If Combatants(i).ParentFleet.myAllegiance = ClientShip.ParentFleet.myAllegiance Then 'They're friendly
                friendlies = friendlies + 1 'Add to the count
            Else 'They're enemies
                enemies = enemies + 1 'Add to the count
            End If
        Next
        If friendlies < 3 Then 'Renforce the Clients
            For Each i As Fleet In EngagedFleets
                If i.myAllegiance = ClientShip.ParentFleet.myAllegiance Then 'The Fleet is allied with the Client
                    For Each e As Ship In i.shipList
                        If e.InCombat = False Then 'Add this Ship to the fight
                            Combatants.Add(i.shipList(e.FleetIndex)) 'Add the Ship
                            Combatants(Combatants.Count - 1).InCombat = True 'Set the Ship to know it's in combat
                            Combatants(Combatants.Count - 1).CombatIndex = (Combatants.Count - 1) 'Set the Ship's combat index to the new index
                            friendlies = friendlies + 1
                            If friendlies = 6 Then
                                Exit For
                            End If
                        End If
                    Next
                    Exit For
                End If
            Next
        End If
        If enemies < 3 Then
            For Each i As Fleet In EngagedFleets
                If i.myAllegiance <> ClientShip.ParentFleet.myAllegiance Then 'The Fleet is allied with the Client
                    For Each e As Ship In i.shipList
                        If e.InCombat = False Then 'Add this Ship to the fight
                            Combatants.Add(i.shipList(e.FleetIndex)) 'Add the Ship
                            Combatants(Combatants.Count - 1).InCombat = True 'Set the Ship to know it's in combat
                            Combatants(Combatants.Count - 1).CombatIndex = (Combatants.Count - 1) 'Set the Ship's combat index to the new index
                            enemies = enemies + 1
                            If enemies = 6 Then
                                Exit For
                            End If
                        End If
                    Next
                    Exit For
                End If
            Next
        End If
    End Sub

    Public Sub AI_Combat(ByRef Attacker As Fleet, ByRef Defender As Fleet) 'Pits two AI Fleets against each other
        Dim attackingShip As Ship = Attacker.shipList(Int(Rnd() * Attacker.shipList.Count)) 'The selected Ship
        Dim selectedWeapon As Weapon = Nothing 'The selected Weapon
        Do Until selectedWeapon IsNot Nothing
            selectedWeapon = attackingShip.Mounts(Int(Rnd() * attackingShip.Mounts.Length)).MountedWeapon 'Select Weapon
        Loop
        Dim damage As Double = selectedWeapon.Damage.Current 'The damage being done
        Defender.shipList(Int(Rnd() * Defender.shipList.Count)).Take_Damage((Rnd() * 2 * Math.PI), selectedWeapon.Damage.Current, selectedWeapon.Type) 'Deal damage to the enemy
    End Sub

    Public Sub Recentre() 'Recenters which Ship object the Clients are controling
        For i As Integer = 0 To Combatants.Count - 1 'Loop through all the Ships
            If Combatants(i).ParentFleet.myAllegiance = ClientShip.ParentFleet.myAllegiance Then 'There are allied Ships left
                ClientShip = Combatants(i) 'Set the new ClientShip
                Exit Sub
            End If
        Next
        Server.Game_Over()
    End Sub

    Public Sub Generate_Scenario(ByRef Fleets As List(Of Fleet)) 'Passes in a List of Fleet objects for the Clients to combat Ship to Ship and sets the game's Galaxy object to update the combat scenario instead of the Sectors
        EngagedFleets.Clear() 'Clear the list
        EngagedFleets.AddRange(Fleets) 'Adds the Fleets to the scenario
        EngagedFleets.RemoveAt(Server.GameWorld.ClientFleet.index) 'Remove the Client's Fleet
        EngagedFleets.TrimExcess() 'Removes any blank spaces in the List
        Server.GameWorld.State = Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Ship_To_Ship 'Set the Galaxy to update Ship to Ship combat
        Combatants.Clear() 'Clear the list
        If ClientShip Is Nothing Then ClientShip = Server.GameWorld.ClientFleet.shipList(0) 'Make sure the Client Ship is not nothing
        For i As Integer = 0 To 5
            If ClientShip.ParentFleet.shipList.Count = i Then Exit For 'Stop looping
            Combatants.Add(ClientShip.ParentFleet.shipList(i)) 'Add the Ship to the list
            Combatants(i).InCombat = True 'Set the InCombat value to true
            Combatants(i).CombatIndex = i 'Set the CombatIndex value to the new index
            Combatants(i).X = Int(Rnd() * Spawnbox) 'Set the X coord
            Combatants(i).Y = Int(Rnd() * Spawnbox) 'Set the Y coord
        Next
        For Each i As Fleet In EngagedFleets
            If i.myAllegiance <> ClientShip.ParentFleet.myAllegiance Then
                For Each e As Ship In i.shipList
                    Combatants.Add(i.shipList(e.FleetIndex)) 'Add the Ship to the list
                    Combatants(Combatants.Count - 1).InCombat = True 'Set the InCombat value to true
                    Combatants(Combatants.Count - 1).CombatIndex = (Combatants.Count - 1) 'Set the CombatIndex value to the new index
                    Combatants(Combatants.Count - 1).X = Int(Rnd() * Spawnbox) 'Set the X coord
                    Combatants(Combatants.Count - 1).Y = Int(Rnd() * Spawnbox) 'Set the Y coord
                    If Combatants.Count = 12 Then 'Their are 12 Ships in combat
                        Exit Sub
                    End If
                Next
            End If
        Next
    End Sub

End Class
