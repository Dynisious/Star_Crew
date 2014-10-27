Public Class CombatSpace
    Public WithEvents Ticker As New Timers.Timer With {.Interval = 100, .AutoReset = True, .Enabled = True}
    Public ShipList As New List(Of Ship) 'A List of Ship objects

    Public Sub Update() Handles Ticker.Elapsed
        For Each i As Ship In ShipList 'Loop through all Ships
            i.Update() 'Update the Ship
        Next

        Dim destroying As New List(Of Ship) With {.Capacity = ShipList.Count}
        For i As Integer = 0 To ShipList.Count - 1 'Loop through all the Ships that have been destroyed
            If ShipList(i).Dead = True Then destroying.Add(ShipList(i))
        Next
        Remove_Ships(destroying.ToArray()) 'Remove the Ships
        For Each i As Ship In destroying 'Loop through all the Ship's that are destroying
            i.Destroy() 'Destroy the Ship
        Next

        Server.Comms.interactWithClients.WaitOne() 'Wait until the game has control of clientList
        For Each i As ServerClient In Server.Comms.clientList 'Loop through all ServerClients
            i.Generate_Message(ShipList) 'Generate a message for the Server to send
        Next
        Server.Comms.interactWithClients.ReleaseMutex()
    End Sub

    Public Sub Add_Ship(ByRef nShip As Ship) 'Adds a Ship to the Fight
        nShip.CombatIndex = ShipList.Count 'Set the Ship's combat index
        ShipList.Add(nShip) 'Adds the Ship to the list
    End Sub

    Public Sub Remove_Ships(ByVal ships() As Ship) 'Removes the Ship at the specified index
        For Each i As Ship In ships 'Loop through all the Ship's
            ShipList.RemoveAt(i.CombatIndex) 'Remove the Ship
            i.CombatIndex = -1
        Next
        If ShipList.Count <> 0 Then 'There's remaining Ship's
            For i As Integer = 0 To ShipList.Count - 1 'Loops through to the end of the list
                ShipList(i).CombatIndex = i 'Set the new Combat index
            Next
        End If
    End Sub

End Class
