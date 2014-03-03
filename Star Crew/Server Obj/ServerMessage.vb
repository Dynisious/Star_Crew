<Serializable()>
Public Class ServerMessage

    Public bmp As Bitmap
    Public ship As PlayerShip

    Public Sub New()
        bmp = Galaxy.Bmp
        ship = Galaxy.clientShip
    End Sub

    Public Shared Function NewMessage() As Byte()
        Using fs As New IO.MemoryStream
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(fs, New ServerMessage)
            Dim buff() As Byte = fs.ToArray
            Return buff
        End Using
    End Function

End Class