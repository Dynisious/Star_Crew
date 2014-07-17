Public Class CombatSpace 'Object that encompasses the Ship to Ship combat
    Public ClientShip As Ship 'The Ship object that Clients man stations on
    Public Combatants As New List(Of Ship) 'The list of all Ship objects in this combat scenario

    Public Sub Update_Combat() 'Updates the combat scenario

    End Sub

    Public Sub Remove_Ship(ByVal nIndex As Integer) 'Removes the Ship object at the specified index

    End Sub

    Public Sub AI_Combat() 'Pits two AI Fleets against each other

    End Sub

    Public Sub Recentre() 'Recenters which Ship object the Clients are controling

    End Sub

    Public Sub Generate_Scenario(ByRef nEnemy As Fleet) 'Generates a new combat scenario for using the Client's Fleet and the AI Fleet that is passed in

    End Sub

End Class
