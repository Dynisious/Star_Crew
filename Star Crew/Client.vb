Public Class Client
    Public canSend As Boolean = False
    Public connected As Boolean = False
    Public MyConnector As Net.Sockets.TcpClient
    Public myStation As Station.StationTypes = Station.StationTypes.Max
    Public Message As ServerMessage
    Public comms As New Threading.Thread(AddressOf Communications)

    Public Sub New(ByVal nIP As String, ByVal nStation As Integer)
        myStation = nStation
        Try
            MyConnector = New Net.Sockets.TcpClient(nIP, 1225)
            connected = True
            comms.Start()
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error: Could not connect to server")
            Console.WriteLine("Check address and make sure a station is selected")
            Console.WriteLine(ex)
        End Try
    End Sub

    Private Sub Communications()
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(CStr(myStation))
        MyConnector.Client.Send(buff)

        While True
            Dim socket As New ArrayList
            socket.Add(MyConnector.Client)
            Net.Sockets.Socket.Select(socket, Nothing, Nothing, -1)

            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Message = bf.Deserialize(MyConnector.GetStream())

            socket.Clear()
            socket.Add(MyConnector.Client)
            Net.Sockets.Socket.Select(Nothing, socket, Nothing, -1)

            Using fs As New IO.MemoryStream
                Select Case myStation
                    Case Station.StationTypes.Helm
                        bf.Serialize(fs, Message.ship.Helm)
                    Case Station.StationTypes.Batteries
                        bf.Serialize(fs, Message.ship.Batteries)
                    Case Station.StationTypes.Shielding
                        bf.Serialize(fs, Message.ship.Shielding)
                    Case Station.StationTypes.Engineering
                        bf.Serialize(fs, Message.ship.Engineering)
                End Select
                buff = fs.ToArray()
                MyConnector.Client.Send(buff)
            End Using
        End While
    End Sub

End Class
