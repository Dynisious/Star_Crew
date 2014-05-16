Public Class Combat 'Encloses the Ships that are fighting
    Public centerShip As Ship = New Ship(New Screamer, -1,Galaxy.Allegence.Friendly ) 'The Ship that the Players control
    Public shipList As New List(Of Ship) 'A List object of Ship objects that are in combat
    Public EnemyFleet As Fleet 'The Fleet fighting the Players Ships

    Public Sub Generate(ByRef Enemies As Fleet)
        EnemyFleet = Enemies 'Sets a new enemy Fleet
        shipList.Clear() 'Clear the List of Ships
        shipList.AddRange(Sector.centerFleet.ShipList) 'Add the Players Ships
        shipList.AddRange(Enemies.ShipList) 'Add the Enemies Ships
        shipList.TrimExcess() 'Remove any empty spaces from the List
        Recenter() 'Set the Ship the Player is controling
        For i As Integer = 0 To shipList.Count - 1 'Set the initial Stats of the Ships
            shipList(i).InCombat = True 'Set the Ship to be in combat
            shipList(i).Index = i 'Set the index of the Ship in the List
            shipList(i).Speed.current = Helm.MinimumSpeed 'Set the Speed of the Ship
            shipList(i).Position = New Point(Rnd() * SpaceCraft.SpawnBox, Rnd() * SpaceCraft.SpawnBox) 'Set the position of the Ship
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
        shipList.RemoveAt(nShip.Index) 'Remove the Ship
        For i As Integer = 0 To shipList.Count - 1 'Set the index of all remaining Ships
            shipList(i).Index = i
        Next
        shipList.TrimExcess() 'Remove the Blank Space
        If nShip.MyAllegence = Galaxy.Allegence.Friendly Then 'Remove the Ship from it's Fleet
            Sector.centerFleet.RemoveShip(nShip)
        Else
            EnemyFleet.RemoveShip(nShip)
        End If
    End Sub

    Public Sub UpdateCombatSenario() 'Updates the Ship
        ConsoleWindow.GameServer.GameWorld.RunPlayerControls() 'Run the Player controls
        For i As Integer = 0 To shipList.Count - 1
            If i < shipList.Count Then
                shipList(i).UpdateShip() 'Update Ships
            Else
                Exit For
            End If
        Next
    End Sub

    Public Sub AutoFight(ByRef fleet1 As Fleet, ByRef fleet2 As Fleet) 'Battles 2 AI Fleets with the combined values of all their Ships
        Dim damage1 As Double 'The collective damage of Fleet1
        Dim health1 As Double 'The collective hull of Fleet1
        Dim shield1 As Double 'The collective fore shield of Fleet1
        For Each i As Ship In fleet1.ShipList 'Set the values of Fleet1
            damage1 = damage1 + i.Batteries.Primary.Damage.current + i.Batteries.Secondary.Damage.current
            health1 = health1 + i.Hull.current
            shield1 = shield1 + i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current
        Next
        damage1 = damage1 / 4

        Dim damage2 As Double 'The collective damage of Fleet2
        Dim health2 As Double 'The collective hull of Fleet2
        Dim shield2 As Double 'The collective fore shield of Fleet2
        For Each i As Ship In fleet2.ShipList 'Set the values of Fleet2
            damage2 = damage1 + i.Batteries.Primary.Damage.current + i.Batteries.Secondary.Damage.current
            health2 = health1 + i.Hull.current
            shield2 = shield1 + i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current
        Next
        damage2 = damage2 / 4

        Dim overallDamage As Double = damage1 - shield2 'The damage that makes it through the shield
        If overallDamage < 0 Then 'There was more shield than damage
            shield2 = -overallDamage 'Set shield2 to the remaining shield
        Else 'There's no shield left
            shield2 = 0
        End If
        health2 = health2 - overallDamage 'Remove the overallDamage from health2
        If health2 <= 0 Then 'Destroy Fleet2
            fleet2.currentSector.RemoveFleet(fleet2, True, True)
            Exit Sub
        Else 'Distribute the remaining health and shields among Fleet2
            For Each i As Ship In fleet2.ShipList
                i.Hull.current = health2 / fleet2.ShipList.Count
                If i.Hull.current > i.Hull.max Then
                    i.Hull.current = i.Hull.max
                End If
                i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current = shield2 / fleet2.ShipList.Count
                If i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current > i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).max Then
                    i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current = i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).max
                End If
            Next
        End If

        overallDamage = damage2 - shield1 'The damage that makes it through the shield
        If overallDamage < 0 Then 'There was more shield than damage
            shield1 = -overallDamage
        Else 'There's no more shield
            shield1 = 0
        End If
        health1 = health1 - overallDamage 'remove the overallDamage from health1
        If health1 <= 0 Then 'Destroy Fleet1
            fleet1.currentSector.RemoveFleet(fleet1, True, True)
            Exit Sub
        Else 'Distribute the remaining shield and health
            For Each i As Ship In fleet1.ShipList
                i.Hull.current = health1 / fleet1.ShipList.Count
                If i.Hull.current > i.Hull.max Then
                    i.Hull.current = i.Hull.max
                End If
                i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current = shield1 / fleet1.ShipList.Count
                If i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current > i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).max Then
                    i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).current = i.Shielding.SubSystem.Defences(Shields.Sides.FrontShield).max
                End If
            Next
        End If
    End Sub

End Class
