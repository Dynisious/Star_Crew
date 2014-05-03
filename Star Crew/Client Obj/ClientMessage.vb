<Serializable()>
Public Class ClientMessage 'A Message representing key strokes sent to the Server by Clients
    Public Command As Integer 'The action the Client wants to complete
    Public Value As Integer '-1 means nothing, 0 means the key is being release or 1 means the key is being pressed
    Public Station As Station.StationTypes 'The station of type Station.StationTypes that the Client is controling

End Class
