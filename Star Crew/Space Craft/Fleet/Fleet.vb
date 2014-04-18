Public MustInherit Class Fleet
    Inherits SpaceCraft
    Public ShipList(-1) As Ship

    Public Sub New(ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence)
        MyAllegence = nAllegence
        Index = nIndex
        ReDim ShipList(Int(Rnd() * 25))
        For i As Integer = 0 To UBound(ShipList)
            Select Case nAllegence
                Case Galaxy.Allegence.Player
                    ShipList(i) = New FriendlyShip(New Clunker, i)
                Case Galaxy.Allegence.Pirate
                    ShipList(i) = New PirateShip(New Clunker, i)
            End Select
        Next
    End Sub

    Public Shared Event FleetUpdate()
    Public Shared Sub UpdateFleet_Call()
        RaiseEvent FleetUpdate()
    End Sub
    Public Overridable Sub UpdateShip_Handle() Handles MyClass.FleetUpdate
        'Change Position
    End Sub

End Class
