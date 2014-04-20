<Serializable()>
Public Class ServerMessage
    Public Craft As SpaceCraft
    Public Positions() As GraphicPosition
    Public Warping As Galaxy.Warp = -1
    Public State As Galaxy.Scenario = -1

    Public Sub New(ByVal nCraft As SpaceCraft, ByVal nPositions() As GraphicPosition,
                   ByVal nWarping As Galaxy.Warp, ByVal nState As Galaxy.Scenario)
        Craft = nCraft
        Positions = nPositions
        Warping = nWarping
        State = nState
    End Sub

    Public Shared Function ConstructMessage() As Byte()
        Using fs As New IO.MemoryStream
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(fs, Server.ServerComms.MessageToSend)
            Dim buff() As Byte = fs.ToArray
            Return buff
        End Using
    End Function

End Class