Public Class Planet
    Public ParentSector As Sector 'A reference to the Sector object the Planet is inside
    Public Name As String 'A String value representing the name of the Planet
    Public Stores As New List(Of Item) 'A list of Item objects that the Planet has to trade with

End Class
