Public Class GameEnvironment 'Runs the game itself and is the outermost layer of the game world
    Public sectors(0) As Sector 'An Array of Sector objects
    Public activeSector As Integer = 0 'An Integer value representing the Sector that the Player is currently in
    Private gameState As Star_Crew_Shared_Libraries.Shared_Values.GameStates = Star_Crew_Shared_Libraries.Shared_Values.GameStates.Fleet_Transit 'The current state that the game is in
    Public allegiancesRelations(,) As Boolean = {{True, True, True}, {True, True, True}, {True, True, True}}

    Public Sub New(ByVal loadGame As Boolean)
        If loadGame Then 'Load the previous game

        Else 'Begin a new game
            sectors(0) = New Sector 'Create a new Sector
        End If
    End Sub

    Public Sub Run_Game() 'Have the game begin to update
        For i As Integer = 1 To 5 'Loop 5 times
            sectors(activeSector).Add_Fleet(New Fleet(Int(Rnd() * Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max), Nothing)) 'Add a new Fleet
        Next
        gameTick.Start()
        Console.WriteLine(Environment.NewLine + "Game now running.")
    End Sub

    Private WithEvents gameTick As New Timers.Timer(50) With {.Enabled = False, .AutoReset = True} 'A Timer object used to cause the game to update
    Private updateCount As Integer 'An Integer value indicating how many updates have past since the last reset
    Private dayCount As Integer 'An Integer value indicating how many days have past since the last reset
    Private gameDate As New Date 'The date in the game world
    Private Sub Update_Time() Handles gameTick.Elapsed 'Update time within the game environment
        Try
            updateCount += 1 'Add one to the update count
            If Not updateCount = 1 Then '6000 Then 'A day has past
                For Each i As Sector In sectors 'Loop through all Sectors
                    i.Daily_Changes() 'Execute the daily changes
                Next
                updateCount = 0 'Reset the count
                dayCount += 1 'Add one to the day count
                If Not dayCount = 7 Then 'A week has passed
                    For i As Integer = 0 To Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 2 'Loop through all allegiances
                        For e As Integer = i + 1 To Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 1 'Loop through every allegiances combination other than itself that has not been covered
                            If Int(Rnd() * 2) = 0 Then 'Change these allegiances relations
                                allegiancesRelations(i, e) = Not allegiancesRelations(i, e) 'Change the relationship of allegiance i to allegiance e
                                allegiancesRelations(e, i) = Not allegiancesRelations(e, i) 'Change the relationship of allegiance e to allegiance i
                            End If
                        Next
                    Next
                    dayCount = 0 'Reset the count
                End If
            End If
            Update_Game() 'Update the game environment
        Catch ex As Exception
            Dim message As String = Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while running the game. Server will now close."
            Console.WriteLine(message)
            Server.Write_To_Log(message + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private Sub Update_Game() 'Update the game environment
        Select Case gameState 'Behave based on the game's state
            Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Fleet_Transit 'The Player is moving about the Sectors
                sectors(activeSector).Update() 'Update the Players current Sector
                For i As Integer = 0 To Server.clients.Count - 1 'Loop through all clients
                    Server.clients(i).Send_Message(Server.clients(i).Generate_Fleet_To_Fleet(sectors(activeSector)),
                                                   {"ERROR : There was an error while sending the Fleet_To_Fleet header to the " +
                                                    Server.clients(i).name + ". The " + Server.clients(i).name + " will now disconnect.",
                                                    "ERROR : There was an error while sending the Fleet_To_Fleet message length to the " +
                                                       Server.clients(i).name + ". The " + Server.clients(i).name + " will now disconnect.",
                                                    "ERROR : There was an error while sending the Fleet_To_Fleet message to the " +
                                                    Server.clients(i).name + ". The " + Server.clients(i).name + " will now disconnect."}) 'Send the message
                Next
            Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Ship_To_Ship_Combat 'The Player is in combat

            Case Star_Crew_Shared_Libraries.Shared_Values.GameStates.Trading 'The Player is trading goods

        End Select
    End Sub

    Public Sub Finalise() 'Finalise the game
        gameTick.Stop() 'Stop the Timer
    End Sub

End Class
