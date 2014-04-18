Public Class Combat
    Public Shared centerShip As Ship
    Public Shared shipList() As Ship

    Public Shared Sub Generate(ByRef Enemies As Fleet)
        ReDim shipList(UBound(Sector.centerFleet.ShipList) + Enemies.ShipList.Length)
        Randomize()

        centerShip = New FriendlyShip(New Clunker, 0)
        shipList(0) = centerShip
        shipList(0).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        shipList(0).InCombat = True
        shipList(1) = New PirateShip(New Clunker, 1)
        shipList(1).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        shipList(1).InCombat = True
        For i As Integer = 2 To UBound(shipList)
            If Int(2 * Rnd()) = 0 Then
                shipList(i) = New FriendlyShip(New Clunker, i)
            Else
                shipList(i) = New PirateShip(New Clunker, i)
            End If
            shipList(i).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
            shipList(i).InCombat = True
        Next
    End Sub

    Public Shared Sub Recenter()
        For Each i As Ship In shipList
            If i.MyAllegence = Galaxy.Allegence.Player Then
                centerShip = i
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
        Next
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
    End Sub

    Public Shared Sub UpdateCombatSenario()
        Galaxy.PlayerControl.RunPlayerControls()
        Ship.UpdateShip_Call()
    End Sub

End Class
