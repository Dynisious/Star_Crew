Public Class Planet
    Public ParentSector As Sector 'A reference to the Sector object the Planet is inside
    Public Location As System.Drawing.Point 'A Point representing the position of the Planet in the Sector
    Public name As String 'A String value representing the name of the Planet
    Public index As Integer 'An Integer representing the Index of the Planet in the galaxy
    Public inventory As New List(Of Item) 'A list of Item objects that the Planet has to trade with

    Public Sub Update() 'Interacts with any available Fleets in range

    End Sub

    Public Sub Receive_Shipments() 'Removes any Shipment objects from a Fleet

    End Sub

End Class
