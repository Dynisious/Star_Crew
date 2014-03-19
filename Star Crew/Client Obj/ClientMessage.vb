Public Class ClientMessage
    Public Command As Integer
    Public Value As Double

    Public Sub New(ByVal nCommand As Integer, ByVal nValue As Double)
        Command = nCommand
        Value = nValue
    End Sub

    Public Function DecodeMessage(ByRef nSocket As Net.Sockets.Socket) As ClientMessage
        Try
            Using fs As New Net.Sockets.NetworkStream(nSocket)
                Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                Dim temp As ClientMessage = bf.Deserialize(fs)
                Return temp
            End Using
        Catch ex As Exception
            Console.WriteLine("Error : There was an error while decoding the message")
            Console.WriteLine(ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function SendMessage(ByRef nSocket As Net.Sockets.Socket) As Boolean
        Try
            Using fs As New IO.MemoryStream
                Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                bf.Serialize(fs, Me)
                Dim buff() As Byte = fs.ToArray
                nSocket.Send(buff, Net.Sockets.SocketFlags.None)
            End Using
        Catch ex As Exception
            Console.WriteLine("Error : There was an error while sending the message")
            Console.WriteLine(ex.ToString())
            Return False
        End Try
    End Function

End Class
