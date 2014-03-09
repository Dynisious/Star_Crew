<Serializable()>
Public Class ServerMessage
    Public bmp As Bitmap
    Public ship As PlayerShip

    Public Sub New(ByVal nClientShip As PlayerShip, ByVal nImage As Bitmap)
        bmp = nImage
        ship = nClientShip
    End Sub

    Public Function ConstructMessage() As Byte()
        Using fs As New IO.MemoryStream
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(fs, Server.Communications.MessageToSend)
            Dim buff() As Byte = fs.ToArray
            Return buff
        End Using
    End Function

End Class