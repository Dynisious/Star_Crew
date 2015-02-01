Public Class Sector 'Contains a Planet object and has Fleets move around within the Sector
    Private myPlanet As New Planet 'A Planet object for the Sector
    Public fleets As New List(Of Fleet) 'A List of Fleet objects within the Sector
    Public allegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Neutral 'The current allignment of this Sector
    Private contested As Boolean = False 'A Boolean value indicating whether the Sector is currently contested

    Public Sub Add_Fleet(ByRef f As Fleet) 'Adds the specified Fleet to the Sector's List
        fleets.Add(f) 'Add the Fleet to the List

        If allegiance = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Neutral And Not contested Then 'The Sector has no current allegiance
            allegiance = f.allegiance 'Set the new allegiance of the Sector
        ElseIf allegiance <> f.allegiance Then 'There are different allegiances
            If Not Server.game.allegiancesRelations(allegiance, f.allegiance) Then 'The Sector is being contested
                allegiance = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Neutral 'Set the Sector's allegiance to Neutral
                contested = True 'Set the Sector's contested value to true
            End If
        End If
    End Sub

    Public Sub Remove_Fleet(ByRef f As Fleet) 'Removes the specified Fleet from the Sector's List
        fleets.Remove(f) 'Remove the Fleet from the List

        If contested Then 'Check to see whether the Sector is still contested
            contested = False 'Reset contested until a conflict is confirmed
            Dim inSector(Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 1) As Boolean 'An array of Boolean values representing what allegiances are currently in the Sector
            For i As Integer = 0 To Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 1 'Loop through all allegiaces
                inSector(i) = False 'Set the boolean to false
            Next
            For Each i As Fleet In fleets 'Loop through all Fleets
                inSector(i.allegiance) = True 'Set the Boolean to true
            Next
            For i As Integer = 0 To Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 2 'Loop through all allegiances
                If inSector(i) Then 'There is a presence for allegiance 'i'
                    For e As Integer = i + 1 To Star_Crew_Shared_Libraries.Shared_Values.Allegiances.max - 1 'Loop through all allegiances combinations other than to itself
                        If inSector(e) Then 'There is a presence for allegiance 'e'
                            If Not Server.game.allegiancesRelations(i, e) Then 'These two allegiances are against eachother
                                contested = True 'A conflict has been confirmed
                                Exit Sub 'There is no more need to search for conflicts
                            End If
                        End If
                    Next
                End If
            Next
        End If
    End Sub

    Public Sub Daily_Changes() 'Perform the changes that occour in a Sector each day
        myPlanet.Update() 'Update the Planet
    End Sub

    Public Sub Update() 'Update this Sector
        For Each i As Fleet In fleets 'Loop through all fleets
            i.Update() 'Update the Fleet
        Next
    End Sub

End Class
