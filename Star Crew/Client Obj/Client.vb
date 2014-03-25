Public Class Client
    Public Connected As Boolean = False
    Public MyConnector As Net.Sockets.TcpClient
    Public myMessage As New ClientMessage
    Public serversMessage As ServerMessage
    Public comms As New Threading.Thread(AddressOf RunComms_Call)

    Public Sub New(ByVal nIP As String, ByVal nStation As Integer)
        myMessage.Station = nStation
        Try
            MyConnector = New Net.Sockets.TcpClient(nIP, 1225)
            Connected = True
            comms.Start()
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error: Could not connect to server")
            Console.WriteLine("Check address and make sure the server exists")
            Console.WriteLine(ex)
        End Try
    End Sub

    Private Event SendCommand(ByVal command As Integer)
    Public Sub SendCommand_Call(ByVal command As Integer)
        RaiseEvent SendCommand(command)
    End Sub
    Private Sub SendCommand_Handle(ByVal command As Integer) Handles Me.SendCommand
        myMessage.Command = command
    End Sub

    Private Event RunComms()
    Private Sub RunComms_Call()
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(CInt(myMessage.Station))
        MyConnector.Client.Send(buff)

        While True
            RaiseEvent RunComms()
        End While
    End Sub
    Private Sub RunComms_Handle() Handles Me.RunComms
        '-----Make an ArrayList for the socket-----
        Dim socket As New ArrayList
        socket.Add(MyConnector.Client)
        '------------------------------------------

        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        '-----Recieve Message-----
        Net.Sockets.Socket.Select(socket, Nothing, Nothing, -1)
        serversMessage = bf.Deserialize(MyConnector.GetStream())
        '-------------------------

        '-----Send Message-----
        Net.Sockets.Socket.Select(Nothing, socket, Nothing, -1)
        Using fs As New IO.MemoryStream
            bf.Serialize(fs, myMessage)
            MyConnector.Client.Send(fs.ToArray())
            myMessage.Command = -1
        End Using
        '----------------------
    End Sub

End Class
