<Serializable()>
Public Class Combat 'Encloses the Ships that are fighting
    Public centerShip As Ship = New Ship(New Screamer, -1, Galaxy.Allegence.Friendly) 'The Ship that the Players control
    Public shipList As New List(Of Ship) 'A List object of Ship objects that are in combat
    Public EnemyFleet As Fleet 'The Fleet fighting the Players Ships

    Public Sub Generate(ByRef Enemies As Fleet)
        EnemyFleet = Enemies 'Sets a new enemy Fleet
        EnemyFleet.Speed.current = 0 'Set the Fleet to stop moving
        ConsoleWindow.GameServer.GameWorld.centerFleet.Speed.current = 0 'Set the Fleet to stop moving
        shipList.Clear() 'Clear the List of Ships
        shipList.AddRange(ConsoleWindow.GameServer.GameWorld.centerFleet.ShipList) 'Add the Players Ships
        shipList.AddRange(Enemies.ShipList) 'Add the Enemies Ships
        shipList.TrimExcess() 'Remove any empty spaces from the List
        Recenter() 'Set the Ship the Player is controling
        For i As Integer = 0 To shipList.Count - 1 'Set the initial Stats of the Ships
            shipList(i).InCombat = True 'Set the Ship to be in combat
            shipList(i).Index = i 'Set the index of the Ship in the List
            shipList(i).Speed.current = Helm.MinimumSpeed 'Set the Speed of the Ship
            shipList(i).Position = New Point(Rnd() * SpaceCraft.SpawnBox, Rnd() * SpaceCraft.SpawnBox) 'Set the position of the Ship
            For e As Integer = 0 To Shields.Sides.Max - 1
                shipList(i).Shielding.SubSystem.Defences(e).current = shipList(i).Shielding.SubSystem.Defences(e).max 'Reset the Shields
            Next
        Next
        ConsoleWindow.GameServer.GameWorld.State = Galaxy.Scenario.Battle 'Change the Galaxie's State to update the Combat scenario
    End Sub

    Public Sub Recenter() 'Sets the Ship that the Players control
        If shipList(0).MyAllegence = Galaxy.Allegence.Friendly Then
            centerShip = shipList(0) 'Set the Ship
            For Each e As ServerSideClient In GameServer.Clients 'Set the Stations with Clients connected to be controled by Players
                Select Case e.MyStation
                    Case Station.StationTypes.Helm 'Helm
                        centerShip.Helm.PlayerControled = True
                    Case Station.StationTypes.Batteries 'Batteries
                        centerShip.Batteries.PlayerControled = True
                    Case Station.StationTypes.Shielding 'Shielding
                        centerShip.Batteries.PlayerControled = True
                    Case Station.StationTypes.Engineering 'Engineering
                        centerShip.Engineering.PlayerControled = True
                End Select
            Next
        Else
            ConsoleWindow.GameServer.GameWorld.GalaxyTimer.Enabled = False 'Stop updating the game
            Console.WriteLine("Player is Defeated") 'Print out the defeat message
        End If
    End Sub

    Public Sub RemoveShip(ByRef nShip As Ship) 'Remove the specified ship from the List
        If nShip.Index < shipList.Count Then
            shipList.RemoveAt(nShip.Index) 'Remove the Ship
            For i As Integer = 0 To shipList.Count - 1 'Set the index of all remaining Ships
                shipList(i).Index = i
            Next
            shipList.TrimExcess() 'Remove the Blank Space

            '-----Remove the Ship from it's Fleet-----
            If nShip.MyAllegence = Galaxy.Allegence.Friendly Then 'It's part of the Players Ship
                ConsoleWindow.GameServer.GameWorld.centerFleet.RemoveShip(nShip)
            Else 'It's part of the enemies Ship
                EnemyFleet.RemoveShip(nShip)
            End If
            '-----------------------------------------
        End If
    End Sub

    Public Sub UpdateCombatSenario() 'Updates the Ship
        ConsoleWindow.GameServer.GameWorld.RunPlayerControls() 'Run the Player controls
        For i As Integer = 0 To shipList.Count - 1
            If i < shipList.Count Then 'The Loop has reached the end of the Loop
                shipList(i).UpdateShip() 'Update Ships
            Else
                Exit For
            End If
        Next
    End Sub

    Public Sub AutoFight(ByRef attacking As Fleet, ByRef defending As Fleet) 'Battles 2 AI Fleets
        attacking.Speed.current = 0 'Stop the Fleet
        defending.Speed.current = 0 'Stop the Fleet
        If defending.ShipList.Count <> 0 Then 'Their defending ships
            Dim attacker As Ship = attacking.ShipList(Int(Rnd() * attacking.ShipList.Count)) 'The Ship that is attacking
            Dim target As Ship = defending.ShipList(Int(Rnd() * defending.ShipList.Count)) 'The Ship that is defending
            Dim direction As Double = (Rnd() * 2 * Math.PI) 'The vector that the attacker is attacking from
            If target.TakeDamage(attacker.Batteries.Primary, attacker, direction) = False Then 'The target is still alive
                If target.TakeDamage(attacker.Batteries.Secondary, attacker, direction) = True Then 'The target was destroyed
                    defending.RemoveShip(target) 'Remove the Ship from the Fleet
                End If
            Else 'The target was destroyed
                defending.RemoveShip(target) 'Remove the Ship from the Fleet
            End If

            defending.SetStats_Handle() 'Update the stats of the Fleet
        Else
            defending.currentSector.RemoveFleet(defending, True, False) 'Destroy the Fleet
        End If
    End Sub

End Class
