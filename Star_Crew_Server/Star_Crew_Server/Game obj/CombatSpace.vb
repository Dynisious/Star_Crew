Public Class CombatSpace
    Public WithEvents Ticker As New Timers.Timer With {.Interval = 50, .AutoReset = True, .Enabled = True}
    Public gameThread As New System.Threading.Thread(AddressOf GameLoop) 'A Thread to run the gameLoop
    Private GameSemaphore As New System.Threading.Semaphore(0, 1) 'Create a semaphore to use for the game loop
    Public gameOpen As Boolean = True 'A Boolean value to keep the game loop open
    Public ShipList As New List(Of Ship) 'A List of Ship objects
    Public adding As New List(Of Ship) 'A List of Ship objects

    Private Sub Open_Semaphore() Handles Ticker.Elapsed
        GameSemaphore.Release()
    End Sub

    Private Sub GameLoop()
        While gameOpen 'Loop
            GameSemaphore.WaitOne() 'Wait to loop
            Try
                If adding.Count <> 0 Then 'There are Ships to add
                    Dim start As Integer = ShipList.Count 'Get the starting index for the new Ships
                    ShipList.AddRange(adding) 'Add the new Ships
                    adding.Clear() 'Clear the list
                    For i As Integer = start To ShipList.Count - 1 'Loop through the new Ship's
                        ShipList(i).CombatIndex = i 'Set the Ship's combat index
                    Next
                End If

                '-----Update Ships-----
                For Each i As Ship In ShipList 'Loop through all Ships
                    i.Update() 'Update the Ship
                Next
                '----------------------

                '-----Collisions-----
                Dim collisions As New List(Of Ship) 'a List of Ship objects that can collide
                For Each i As Ship In ShipList 'Loop through all Ships
                    If i.Physics Then collisions.Add(ShipList(i.CombatIndex)) 'Add the Ship to collisions
                Next
                For i As Integer = 0 To (collisions.Count - 2) 'Loop through all the collision indexes but the last one
                    For e As Integer = (i + 1) To (collisions.Count - 1) 'Loop through all the collision indexes that haven't collided with this one yet that aren't itself
                        If ShipList(i).Allegiance <> ShipList(e).Allegiance Then 'They can collide
                            Dim objX As Integer = (collisions(e).X - collisions(i).X) 'Get the x coord of the e index relative to the i index
                            Dim objY As Integer = (collisions(e).Y - collisions(i).Y) 'Get the y coord of the e index relative to the i index
                            Dim objDirection As Double = Math.Atan2(objY, objX) 'Get the direction in world space of the e index relative to the i index
                            If Math.Sqrt((objX ^ 2) + (objY ^ 2)) < (ShipList(i).Get_Collision_Radia(objDirection) + ShipList(e).Get_Collision_Radia(objDirection)) Then 'The two objects collide
                                ShipList(i).Collide(ShipList(e)) 'Collide i
                                ShipList(e).Collide(ShipList(i)) 'Collide e
                            End If
                        End If
                    Next
                Next
                '--------------------

                '-----Destroy Dead-----
                Dim destroying As New List(Of Ship) With {.Capacity = ShipList.Count}
                For i As Integer = 0 To ShipList.Count - 1 'Loop through all the Ships that have been destroyed
                    If ShipList(i).Dead = True Then destroying.Add(ShipList(i))
                Next
                For Each i As Ship In destroying 'Loop through all the Ship's
                    ShipList.RemoveAt(i.CombatIndex) 'Remove the Ship
                    If i.CombatIndex <> ShipList.Count Then
                        For index As Integer = i.CombatIndex To ShipList.Count - 1 'Loops through to the end of the list
                            ShipList(index).CombatIndex = index 'Set the new Combat index
                        Next
                    End If
                    i.CombatIndex = -1 'Clear the index
                Next
                For Each i As Ship In destroying 'Loop through all the Ship's that are destroying
                    i.Destroy() 'Destroy the Ship
                Next
                '----------------------

                For Each i As ServerClient In Server.Comms.clientList 'Loop through all ServerClients
                    i.Generate_Message(ShipList) 'Generate a message for the Server to send
                Next
            Catch ex As Exception
                Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while executing the game. Server will now close." +
                                          Environment.NewLine + ex.ToString())
                End
            End Try
        End While
    End Sub

End Class
