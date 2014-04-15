Public Class Client
    Public Shared Connected As Boolean = False
    Public Shared MyConnector As Net.Sockets.TcpClient
    Public Shared myMessage As New ClientMessage
    Public Shared serversMessage As ServerMessage
    Public Shared comms As Threading.Thread
    Private Shared stars(200) As Star

    Public Class Star
        Public Position As Point
        Public Flash As Boolean = False
        Private count As Integer
        Public Shared Speed As Double = 1
        Public Shared ReadOnly WarpSpeed As Integer = 20

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Public Sub Update()
            If Flash = False Then
                If Int(80 * Rnd()) = 0 Then
                    Flash = True
                End If
                count = 8
            ElseIf count = 1 Then
                count = 0
                Flash = False
            Else
                count = count - 1
            End If


            Dim direction As Double = serversMessage.Ship.Helm.Direction + Math.PI
            Position = New Point(Position.X + (serversMessage.Ship.Helm.Throttle.current * Math.Cos(direction) * Speed),
                                 Position.Y + (serversMessage.Ship.Helm.Throttle.current * Math.Sin(direction) * Speed))
            If Position.X <= 2 Then
                Position = New Point(Position.X + Screen.ImageSize.X - 5, Position.Y)
            ElseIf Position.X >= Screen.GamePlayLayout.picDisplayGraphics.Width - 3 Then
                Position = New Point(Position.X - Screen.ImageSize.X + 5, Position.Y)
            End If
            If Position.Y <= 2 Then
                Position = New Point(Position.X, Position.Y + Screen.ImageSize.Y - 5)
            ElseIf Position.Y >= Screen.GamePlayLayout.picDisplayGraphics.Height - 3 Then
                Position = New Point(Position.X, Position.Y - Screen.ImageSize.Y + 5)
            End If

            If serversMessage.Warping = Galaxy.Warp.Warping Then
                Speed = WarpSpeed
            Else
                Speed = 1
            End If
        End Sub
    End Class

    Public Sub New(ByVal nIP As String, ByVal nStation As Integer)
        myMessage.Station = nStation
        Try
            MyConnector = New Net.Sockets.TcpClient(nIP, 1225)
            Connected = True
            For i As Integer = 0 To UBound(stars)
                stars(i) = New Star(New Point(Int((594 * Rnd()) + 3), Int((594 * Rnd())) + 3))
            Next
            Tick.Enabled = True
            comms = New Threading.Thread(AddressOf RunComms_Call)
            comms.Start()
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error: Could not connect to server")
            Console.WriteLine("Check address and make sure the server exists")
            Console.WriteLine(ex.ToString)
            Console.Beep()
            Connected = False
        End Try
    End Sub

    Private Shared Event SendCommand(ByVal command As Integer, ByVal value As Integer)
    Public Shared Sub SendCommand_Call(ByVal command As Integer, ByVal value As Integer)
        RaiseEvent SendCommand(command, value)
    End Sub
    Private Shared Sub SendCommand_Handle(ByVal command As Integer, ByVal value As Integer) Handles Me.SendCommand
        myMessage.Command = command
        myMessage.Value = value
    End Sub

    Private Shared Event RunComms()
    Private Shared Sub RunComms_Call()
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(CInt(myMessage.Station))
        MyConnector.Client.Send(buff)

        While True
            RaiseEvent RunComms()
        End While
    End Sub
    Private Shared Sub RunComms_Handle() Handles Me.RunComms
        '-----Make an ArrayList for the socket-----
        Dim socket As New ArrayList
        socket.Add(MyConnector.Client)
        '------------------------------------------

        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        '-----Recieve Message-----
        Net.Sockets.Socket.Select(socket, Nothing, Nothing, -1)
        Try
            serversMessage = bf.Deserialize(MyConnector.GetStream())
            primaryDirection = serversMessage.Ship.Helm.Direction + serversMessage.Ship.Batteries.Primary.TurnDistance.current
            primaryRadius = serversMessage.Ship.Batteries.Primary.Range.current
            secondaryDirection = serversMessage.Ship.Helm.Direction + serversMessage.Ship.Batteries.Secondary.TurnDistance.current
            secondaryRadius = serversMessage.Ship.Batteries.Secondary.Range.current
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error : The Client was disconnected unexpectedly")
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            MyConnector.Close()
            Connected = False
            comms.Abort()
        Catch ex As InvalidOperationException
            Console.WriteLine()
            Console.WriteLine("Error : That Position is already filled")
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            MyConnector.Close()
            Connected = False
            comms.Abort()
        Catch ex As Runtime.Serialization.SerializationException
            Console.WriteLine()
            Console.WriteLine("Error : That Position is already filled")
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            MyConnector.Close()
            Connected = False
            comms.Abort()
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("Error : There was an unexpected and unhandled exception.")
            Console.WriteLine("The error message has now been copied to your clipboard")
            Console.WriteLine("please submit it as an issue at the URL bellow")
            Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            'My.Computer.Clipboard.SetText(ex.ToString)***
            MyConnector.Close()
            Connected = False
            comms.Abort()
        End Try
        '-------------------------

        '-----Send Message-----
        Net.Sockets.Socket.Select(Nothing, socket, Nothing, -1)
        Using fs As New IO.MemoryStream
            bf.Serialize(fs, myMessage)
            Try
                MyConnector.Client.Send(fs.ToArray())
            Catch ex As Net.Sockets.SocketException
                Console.WriteLine()
                Console.WriteLine("Error : The Client was disconnected unexpectedly")
                Console.WriteLine(ex.ToString)
                Console.WriteLine()
                MyConnector.Close()
                Connected = False
                comms.Abort()
            Catch ex As Exception
                Console.WriteLine()
                Console.WriteLine("Error : There was an unexpected and unhandled exception.")
                Console.WriteLine("The error message has now been copied to your clipboard")
                Console.WriteLine("please submit it as an issue at the URL bellow")
                Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
                Console.WriteLine()
                Console.WriteLine(ex.ToString)
                Console.WriteLine()
                My.Computer.Clipboard.SetText(ex.ToString)
                MyConnector.Close()
                Connected = False
                comms.Abort()
            End Try
            myMessage.Command = -1
            myMessage.Value = -1
        End Using
        '----------------------
    End Sub

    Private Shared WithEvents Tick As New Timer With {.Interval = 100, .Enabled = False}
    Private Shared primaryDirection As Double
    Private Shared primaryRadius As Integer
    Private Shared secondaryDirection As Double
    Private Shared secondaryRadius As Integer
    Private Shared Sub UpdateGraphics() Handles Tick.Tick
        If serversMessage IsNot Nothing Then
            Dim messageData As New ServerMessage(serversMessage)
            If messageData.Ship IsNot Nothing Then
                For Each i As Star In stars
                    i.Update()
                Next

                '-----Clear old-----
                Dim bmp As Bitmap
                Select Case messageData.Warping
                    Case Galaxy.Warp.None
                        bmp = My.Resources.NormalSpace.Clone
                    Case Galaxy.Warp.Warping
                        bmp = My.Resources.Warping.Clone
                End Select
                '-------------------

                '-----Put on Radar-----
                For i As Integer = 0 To UBound(messageData.Positions)
                    Dim distance As Integer = Math.Sqrt((messageData.Positions(i).X ^ 2) + (messageData.Positions(i).Y ^ 2))
                    If messageData.Positions(i).X <= 0 Or messageData.Positions(i).X >= Screen.ImageSize.X - 1 Or messageData.Positions(i).Y <= 0 Or messageData.Positions(i).Y >= Screen.ImageSize.Y - 1 Then
                        Dim a = 1
                    End If
                    If distance > 200 Then
                        Dim scale As Double = distance / 200
                        messageData.Positions(i).X = (messageData.Positions(i).X / scale)
                        messageData.Positions(i).Y = (messageData.Positions(i).Y / scale)
                    End If
                    messageData.Positions(i).X = messageData.Positions(i).X + ((Screen.ImageSize.X / 2) - 1)
                    messageData.Positions(i).Y = messageData.Positions(i).Y + ((Screen.ImageSize.Y / 2) - 1)
                Next
                '----------------------

                '-----Render-----
                For Each i As Star In stars
                    If i.Flash = True Then
                        For x As Integer = -2 To 2
                            For y As Integer = -2 To 2
                                bmp.SetPixel(i.Position.X + x, i.Position.Y + y, Color.White)
                            Next
                        Next
                    Else
                        bmp.SetPixel(i.Position.X, i.Position.Y, Color.White)
                    End If
                Next

                If messageData.Warping <> Galaxy.Warp.Warping Then
                    '-----Batteries Arc-----
                    '-----Primary Arc-----
                    primaryDirection = messageData.Ship.Helm.Direction + messageData.Ship.Batteries.Primary.TurnDistance.current
                    primaryRadius = messageData.Ship.Helm.Direction + messageData.Ship.Batteries.Primary.Range.current
                    For i As Integer = 1 To primaryRadius
                        Dim x As Integer = Math.Cos(primaryDirection + -(Battery.PlayerArc / 2)) * i
                        Dim y As Integer = Math.Sin(primaryDirection + -(Battery.PlayerArc / 2)) * i
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                        x = Math.Cos(primaryDirection + (Battery.PlayerArc / 2)) * i
                        y = Math.Sin(primaryDirection + (Battery.PlayerArc / 2)) * i
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                    Next
                    For i As Double = -(Battery.PlayerArc / 2) To (Battery.PlayerArc / 2) Step Battery.PlayerArc / (2 * Math.PI)
                        Dim x As Integer = Math.Cos(primaryDirection + i) * primaryRadius
                        Dim y As Integer = Math.Sin(primaryDirection + i) * primaryRadius
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                    Next
                    '---------------------
                    '-----Secondary Arc-----
                    secondaryDirection = messageData.Ship.Helm.Direction + messageData.Ship.Batteries.Secondary.TurnDistance.current
                    secondaryRadius = messageData.Ship.Helm.Direction + messageData.Ship.Batteries.Secondary.Range.current
                    For i As Integer = 1 To secondaryRadius
                        Dim x As Integer = Math.Cos(secondaryDirection + -(Battery.PlayerArc / 2)) * i
                        Dim y As Integer = Math.Sin(secondaryDirection + -(Battery.PlayerArc / 2)) * i
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                        x = Math.Cos(secondaryDirection + (Battery.PlayerArc / 2)) * i
                        y = Math.Sin(secondaryDirection + (Battery.PlayerArc / 2)) * i
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                    Next
                    For i As Double = -(Battery.PlayerArc / 2) To (Battery.PlayerArc / 2) Step Battery.PlayerArc / (2 * Math.PI)
                        Dim x As Integer = Math.Cos(secondaryDirection + i) * secondaryRadius
                        Dim y As Integer = Math.Sin(secondaryDirection + i) * secondaryRadius
                        bmp.SetPixel(x + ((Screen.ImageSize.X / 2) - 1), y + ((Screen.ImageSize.Y / 2) - 1), Color.Yellow)
                    Next
                    '-----------------------
                    '-----------------------
                End If

                Dim count As Integer = 0
                For i As Integer = 0 To UBound(messageData.Positions)
                    If (messageData.Warping = Galaxy.Warp.None) Or (messageData.Warping = Galaxy.Warp.Warping And count = messageData.Ship.Index) Then
                        Dim col As Color = serversMessage.Positions(i).Col
                        If serversMessage.Positions(i).Hit = True Then
                            col = Color.Orange
                        End If
                        For x As Integer = -3 To 3
                            For y As Integer = -3 To 3
                                bmp.SetPixel(messageData.Positions(i).X + x, messageData.Positions(i).Y + y, col)
                            Next
                        Next
                        col = messageData.Positions(i).Col
                        If messageData.Positions(i).Firing = True Then
                            col = Color.Orange
                        End If
                        For x As Integer = -1 To 1
                            For y As Integer = -1 To 1
                                Dim xStart As Integer = messageData.Positions(i).X + (Math.Cos(messageData.Positions(i).Direction) * 11)
                                Dim yStart As Integer = messageData.Positions(i).Y + (Math.Sin(messageData.Positions(i).Direction) * 11)
                                bmp.SetPixel(xStart + x, yStart + y, col)
                            Next
                        Next
                    End If
                    count = count + 1
                Next

                '-----Batteries Target-----
                If messageData.Ship.Helm.Target IsNot Nothing And messageData.Warping <> Galaxy.Warp.Warping Then
                    If messageData.Ship.Helm.Target.Hit = False Then
                        Dim col As Color = messageData.Positions(messageData.Ship.Helm.Target.Index).Col
                        If messageData.Positions(messageData.Ship.Helm.Target.Index).Hit = True Then
                            col = Color.Orange
                        End If
                        For x As Integer = -3 To 3
                            For y As Integer = -3 To 3
                                bmp.SetPixel(messageData.Positions(messageData.Ship.Helm.Target.Index).X + x,
                                             messageData.Positions(messageData.Ship.Helm.Target.Index).Y + y, Color.Blue)
                            Next
                        Next
                        col = messageData.Positions(messageData.Ship.Helm.Target.Index).Col
                        If messageData.Positions(messageData.Ship.Helm.Target.Index).Firing = True Then
                            col = Color.Orange
                        End If
                        For x As Integer = -1 To 1
                            For y As Integer = -1 To 1
                                Dim xStart As Integer = messageData.Positions(messageData.Ship.Helm.Target.Index).X + (Math.Cos(messageData.Positions(messageData.Ship.Helm.Target.Index).Direction) * 11)
                                Dim yStart As Integer = messageData.Positions(messageData.Ship.Helm.Target.Index).Y + (Math.Sin(messageData.Positions(messageData.Ship.Helm.Target.Index).Direction) * 11)
                                bmp.SetPixel(xStart + x, yStart + y, col)
                            Next
                        Next
                    End If
                End If
                '--------------------------
                '-------------------
                Screen.GamePlayLayout.picDisplayGraphics.Image = bmp
            End If
        End If
    End Sub

End Class
