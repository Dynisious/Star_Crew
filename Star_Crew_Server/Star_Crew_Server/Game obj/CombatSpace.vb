Public Class CombatSpace
    Public WithEvents Ticker As New Timers.Timer With {.Interval = 100, .AutoReset = True, .Enabled = True}
    Public ShipList As New List(Of Ship) 'A List of Ship objects
    Public adding As New List(Of Ship) 'A List of Ship objects

    Public Sub Update() Handles Ticker.Elapsed
        Try
            If adding.Count <> 0 Then 'There are Ships to add
                Dim start As Integer = ShipList.Count 'Get the starting index for the new Ships
                ShipList.AddRange(adding) 'Add the new Ships
                adding.Clear() 'Clear the list
                For i As Integer = start To ShipList.Count - 1 'Loop through the new Ship's
                    ShipList(i).CombatIndex = i 'Set the Ship's combat index
                Next
            End If

            For Each i As Ship In ShipList 'Loop through all Ships
                i.Update() 'Update the Ship
            Next

            '-----Destroy Dead-----
            Dim destroying As New List(Of Ship) With {.Capacity = ShipList.Count}
            For i As Integer = 0 To ShipList.Count - 1 'Loop through all the Ships that have been destroyed
                If ShipList(i).Dead = True Then destroying.Add(ShipList(i))
            Next
            For Each i As Ship In destroying 'Loop through all the Ship's
                If ReferenceEquals((i.CombatIndex), Server.Comms.clientList(0).Craft) Then
                    Dim a = 1
                End If
                ShipList.RemoveAt(i.CombatIndex) 'Remove the Ship
                If i.CombatIndex <> ShipList.Count Then
                    For index As Integer = i.CombatIndex To ShipList.Count - 1 'Loops through to the end of the list
                        ShipList(index).CombatIndex = index 'Set the new Combat index
                    Next
                End If
                If ReferenceEquals(i, Server.Comms.clientList(0).Craft) Then
                    Dim a = 1
                End If
                i.CombatIndex = -1 'Clear the index
                If i.Dead = False Then
                    Dim a = 1
                End If
            Next
            For Each i As Ship In destroying 'Loop through all the Ship's that are destroying
                i.Destroy() 'Destroy the Ship
            Next
            '----------------------

            Server.Comms.InteractWithClients.WaitOne() 'Wait until the game has control of clientList
            For Each i As ServerClient In Server.Comms.clientList 'Loop through all ServerClients
                i.Generate_Message(ShipList) 'Generate a message for the Server to send
            Next
            Server.Comms.InteractWithClients.ReleaseMutex()
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while executing the game. Server will now close." +
                                      Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

End Class
