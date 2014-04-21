Public Class Combat
    Public Shared centerShip As Ship = New FriendlyShip(New Clunker, -1)
    Public Shared shipList As New List(Of Ship)
    Public Shared EnemyFleet As Fleet

    Public Shared Sub Generate(ByRef Enemies As Fleet)
        EnemyFleet = Enemies
        shipList.Clear()
        shipList.AddRange(Sector.centerFleet.ShipList)
        shipList.AddRange(Enemies.ShipList)
        shipList.TrimExcess()
        Recenter()
        For i As Integer = 0 To shipList.Count - 1
            shipList(i).InCombat = True
            shipList(i).Index = i
            shipList(i).Speed.current = Helm.MinimumSpeed
            shipList(i).Position = New Point(Rnd() * SpaceCraft.SpawnBox, Rnd() * SpaceCraft.SpawnBox)
        Next
        Galaxy.State = Galaxy.Scenario.Battle
    End Sub

    Public Shared Sub Recenter()
        If shipList(0).MyAllegence = Galaxy.Allegence.Player Then
            centerShip = shipList(0)
            For Each e As Net.Sockets.Socket In ServerComms.Ports
                If e.GetType Is GetType(ServerSideClient) Then
                    Select Case CType(e, Server.ServerSideClient).MyStation
                        Case Station.StationTypes.Helm
                            CType(centerShip, Ship).Helm.PlayerControled = True
                        Case Station.StationTypes.Batteries
                            CType(centerShip, Ship).Batteries.PlayerControled = True
                        Case Station.StationTypes.Shielding
                            CType(centerShip, Ship).Batteries.PlayerControled = True
                        Case Station.StationTypes.Engineering
                            CType(centerShip, Ship).Engineering.PlayerControled = True
                    End Select
                End If
            Next
            Exit Sub
        End If
        Galaxy.GalaxyTimer.Enabled = False
        Console.WriteLine("Player is Defeated")
    End Sub

    Public Shared Sub RemoveShip(ByRef nShip As Ship)
        shipList.RemoveAt(nShip.Index)
        For i As Integer = nShip.Index To shipList.Count - 1
            shipList(i).Index = i
        Next
        shipList.TrimExcess()
        If nShip.MyAllegence = Galaxy.Allegence.Player Then
            Sector.centerFleet.RemoveShip(nShip)
        Else
            EnemyFleet.RemoveShip(nShip)
        End If
    End Sub

    Public Shared Sub UpdateCombatSenario()
        Galaxy.PlayerControl.RunPlayerControls()
        Ship.UpdateShip_Call()
    End Sub

    Public Shared Sub AutoFight(ByRef fleet1 As Fleet, ByRef fleet2 As Fleet)
        Dim damage1 As Double
        Dim health1 As Double
        Dim shield1 As Double
        For Each i As Ship In fleet1.ShipList
            damage1 = damage1 + i.Batteries.Primary.Damage.current + i.Batteries.Secondary.Damage.current
            health1 = health1 + i.Hull.current
            shield1 = shield1 + i.Shielding.ShipShields(Shields.Sides.FrontShield).current
        Next

        Dim damage2 As Double
        Dim health2 As Double
        Dim shield2 As Double
        For Each i As Ship In fleet2.ShipList
            damage2 = damage1 + i.Batteries.Primary.Damage.current + i.Batteries.Secondary.Damage.current
            health2 = health1 + i.Hull.current
            shield2 = shield1 + i.Shielding.ShipShields(Shields.Sides.FrontShield).current
        Next

        Dim overallDamage As Double = damage1 - shield2
        If overallDamage < 0 Then
            shield2 = -overallDamage
        Else
            shield2 = 0
        End If
        health2 = health2 - overallDamage
        If health2 <= 0 Then
            For i As Integer = 0 To fleet2.ShipList.Count - 1
                fleet2.ShipList(0).DestroyShip()
            Next
            fleet2.ShipList.TrimExcess()
            fleet2.currentSector.RemoveFleet(fleet2, True)
            Exit Sub
        Else
            For Each i As Ship In fleet2.ShipList
                i.Hull.current = health2 / fleet2.ShipList.Count
                If i.Hull.current > i.Hull.max Then
                    i.Hull.current = i.Hull.max
                End If
                i.Shielding.ShipShields(Shields.Sides.FrontShield).current = shield2 / fleet2.ShipList.Count
                If i.Shielding.ShipShields(Shields.Sides.FrontShield).current > i.Shielding.ShipShields(Shields.Sides.FrontShield).max Then
                    i.Shielding.ShipShields(Shields.Sides.FrontShield).current = i.Shielding.ShipShields(Shields.Sides.FrontShield).max
                End If
            Next
        End If

        overallDamage = damage2 - shield1
        If overallDamage < 0 Then
            shield1 = -overallDamage
        Else
            shield1 = 0
        End If
        health1 = health1 - overallDamage
        If health1 <= 0 Then
            For i As Integer = 0 To fleet1.ShipList.Count - 1
                fleet1.ShipList(0).DestroyShip()
            Next
            fleet1.ShipList.TrimExcess()
            fleet1.currentSector.RemoveFleet(fleet1, True)
            Exit Sub
        Else
            For Each i As Ship In fleet1.ShipList
                i.Hull.current = health1 / fleet1.ShipList.Count
                If i.Hull.current > i.Hull.max Then
                    i.Hull.current = i.Hull.max
                End If
                i.Shielding.ShipShields(Shields.Sides.FrontShield).current = shield1 / fleet1.ShipList.Count
                If i.Shielding.ShipShields(Shields.Sides.FrontShield).current > i.Shielding.ShipShields(Shields.Sides.FrontShield).max Then
                    i.Shielding.ShipShields(Shields.Sides.FrontShield).current = i.Shielding.ShipShields(Shields.Sides.FrontShield).max
                End If
            Next
        End If
    End Sub

End Class
