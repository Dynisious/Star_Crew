<Serializable()>
Public Class ServerMessage
    Public Ship As Ship
    Public Positions() As GraphicPosition
    Public Warping As Galaxy.Warp

    Public Sub New(ByVal nCenterShip As Ship, ByVal nPositions() As GraphicPosition, ByVal nWarping As Galaxy.Warp)
        Ship = nCenterShip
        Positions = nPositions
        Warping = nWarping
    End Sub
    Public Sub New(ByVal nData As ServerMessage)
        Ship = nData.Ship
        Positions = nData.Positions
        Warping = nData.Warping
    End Sub

    Public Shared Function ConstructMessage() As Byte()
        Using fs As New IO.MemoryStream
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(fs, Server.ServerComms.MessageToSend)
            Dim buff() As Byte = fs.ToArray
            Return buff
        End Using
    End Function

    Public Shared Function CopyMessage(ByVal message As ServerMessage) As ServerMessage
        Dim newMessage As New ServerMessage(message.Ship, message.Positions, message.Warping)
        Return newMessage
    End Function

End Class