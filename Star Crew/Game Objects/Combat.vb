Public Class Combat
    Public Shared centerShip As Ship
    Public Shared shipList(-1) As Ship
    Public Shared EnemyFleet As Fleet

    Public Shared Sub Generate(ByRef Enemies As Fleet)
        EnemyFleet = Enemies
        ReDim shipList(UBound(Sector.centerFleet.ShipList) + Enemies.ShipList.Length)
        Sector.centerFleet.ShipList.CopyTo(shipList, 0)
        Enemies.ShipList.CopyTo(shipList, Sector.centerFleet.ShipList.Length)
        Recenter()
        For i As Integer = 0 To UBound(shipList)
            shipList(i).InCombat = True
            shipList(i).Index = i
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
        For i As Integer = nShip.Index To UBound(shipList)
            If i <> UBound(shipList) Then
                shipList(i) = shipList(i + 1)
                shipList(i).Index = i
            End If
        Next
        ReDim Preserve shipList(UBound(shipList) - 1)
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

End Class
