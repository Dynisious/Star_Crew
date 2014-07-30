Public Class CombatSpace 'Object that encompasses the Ship to Ship combat
    Public ClientShip As Ship 'The Ship object that Clients man stations on
    Public EngagedFleets As New List(Of Fleet) 'A list of all Fleet objects in this combat scenario
    Public Combatants As New List(Of Ship) 'The list of all Ship objects in this combat scenario

    Public Sub Update_Combat() 'Updates the combat scenario
        For Each i As Ship In Combatants
            i.Update()
        Next
        For Each i As Ship In Combatants
            i.Bridge.Update()
        Next
        For Each i As Ship In Combatants
            i.Batteries.Update()
        Next
        For Each i As Ship In Combatants
            i.Shielding.Update()
        Next
        For Each i As Ship In Combatants
            i.Engineering.Update()
        Next
    End Sub

    Public Sub Remove_Ship(ByVal nIndex As Integer) 'Removes the Ship object at the specified index
        Combatants.RemoveAt(nIndex) 'Remove the Ship
        Combatants.TrimExcess() 'Removes the blank spaces
        Dim friendlies As Integer = 0 'The count of how many friendly Ship's their are
        Dim enemies As Integer = 0 'The count of how many enemy Ships their are
        For i As Integer = 0 To Combatants.Count - 1
            Combatants(i).CombatIndex = i
            If Combatants(i).myAllegiance = ClientShip.myAllegiance Then 'They're friendly
                friendlies = friendlies + 1 'Add to the count
            Else 'They're enemies
                enemies = enemies + 1 'Add to the count
            End If
        Next
        If friendlies < 3 Then 'Renforce the Clients
            For Each i As Fleet In EngagedFleets
                If i.myAllegiance = ClientShip.myAllegiance Then 'The Fleet is allied with the Client
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
                If i.myAllegiance <> ClientShip.myAllegiance Then 'The Fleet is allied with the Client
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
            If Combatants(i).myAllegiance = ClientShip.myAllegiance Then 'There are allied Ships left
                ClientShip = Combatants(i) 'Set the new ClientShip
                Exit Sub
            End If
        Next
        Server.Game_Over()
    End Sub

    Public Sub Generate_Scenario(ByRef nFleets() As Fleet) 'Passes in an array of Fleet objects for the Clients to combat ship to ship and sets the Game World to update the combat scenario instead of the sectors
        EngagedFleets.Clear() 'Clear the list
        EngagedFleets.AddRange(nFleets) 'Adds the Fleets to the scenario
        EngagedFleets.TrimExcess() 'Removes any blank spaces in the List
        Server.GameWorld.State = Galaxy.GalaxyStates.Ship_To_Ship 'Set the Galaxy to update Ship to Ship combat
        Combatants.Clear() 'Clear the list
        Combatants.TrimExcess()
        For i As Integer = 0 To 5
            Combatants.Add(ClientShip.ParentFleet.shipList(i)) 'Add the Ship to the list
            Combatants(i).InCombat = True 'Set the InCombat value to true
            Combatants(i).CombatIndex = i 'Set the CombatIndex value to the new index
        Next
        For Each i As Fleet In EngagedFleets
            If i.myAllegiance <> ClientShip.myAllegiance Then
                For Each e As Ship In i.shipList
                    Combatants.Add(i.shipList(e.FleetIndex)) 'Add the Ship to the list
                    Combatants(Combatants.Count - 1).InCombat = True 'Set the InCombat value to true
                    Combatants(Combatants.Count - 1).CombatIndex = (Combatants.Count - 1) 'Set the CombatIndex value to the new index
                    If Combatants.Count = 12 Then 'Their are 12 Ships in combat
                        Exit Sub
                    End If
                Next
            End If
        Next
    End Sub

End Class
