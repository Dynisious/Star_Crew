Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Class Client
    Public Connected As Boolean = False
    Public MyConnector As Net.Sockets.TcpClient
    Public myMessage As New ClientMessage
    Public IncomingMessage As ServerMessage
    Public Comms As Threading.Thread
    Private stars(200) As Star
    Private ReadOnly BlankShipSpace As New Bitmap(40, 40)
    Private BytesToReceive As Integer
    Private BytesReceived As Integer
    Private MessageBuff(11000) As Byte
    Private ByteBuff(3) As Byte
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
    Private SendBuff() As Byte

    Public Class Star
        Public Position As Point
        Public Diamiter As Integer = 8
        Private Flash As Boolean = False
        Private count As Integer
        Public Shared Speed As Double = 1
        Public Shared ReadOnly WarpSpeed As Integer = 20
        Private Shared Event Update()

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Public Shared Sub Update_Call()
            RaiseEvent Update()
        End Sub
        Private Sub Update_Handle() Handles MyClass.Update
            If Flash = False Then
                If Int(80 * Rnd()) = 0 Then
                    Flash = True
                End If
                count = 10
            ElseIf count = 1 Then
                count = 0
                Flash = False
                Diamiter = 8
            Else
                count = count - 1
                If Diamiter > 1 Then
                    Diamiter = Diamiter - 1
                Else
                    Diamiter = Diamiter + 1
                End If
            End If


            Dim direction As Double = ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Direction + Math.PI
            Position = New Point(Position.X + (ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Speed.current * Math.Cos(direction) * Speed),
                                 Position.Y + (ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Speed.current * Math.Sin(direction) * Speed))
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

            If ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Warping = Galaxy.Warp.Warping Then
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
            Comms = New Threading.Thread(AddressOf StartComms)
            Comms.Start()
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error: Could not connect to server")
            Console.WriteLine("Check address and make sure the server exists")
            Console.WriteLine(ex.ToString)
            Console.Beep()
            Connected = False
        End Try
    End Sub

    Public Sub SendCommand(ByVal command As Integer, ByVal value As Integer)
        myMessage.Command = command
        myMessage.Value = value
    End Sub

    Private Sub StartComms()
        MyConnector.Client.Send(BitConverter.GetBytes(myMessage.Station))

        While True
            RunComms()
        End While
    End Sub
    Private Sub RunComms()
        '-----Recieve Message-----
        BytesReceived = 0
        BytesToReceive = 0
        Try
            '-----Get Number of Bytes to Receive-----
            While BytesReceived < 4
                BytesReceived = BytesReceived +
                    MyConnector.Client.Receive(ByteBuff, BytesReceived, 4 - BytesReceived, Net.Sockets.SocketFlags.None)
            End While
            BytesToReceive = BitConverter.ToInt32(ByteBuff, 0)
            '----------------------------------------

            '-----Receive the Message-----
            BytesReceived = 0
            While BytesReceived < BytesToReceive
                BytesReceived = BytesReceived +
            MyConnector.Client.Receive(MessageBuff, BytesReceived, BytesToReceive - BytesReceived, Net.Sockets.SocketFlags.None)
            End While
            IncomingMessage = BinarySerializer.Deserialize(New IO.MemoryStream(MessageBuff, 0, BytesToReceive))
            '-----------------------------
        Catch ex As Net.Sockets.SocketException
            MyConnector.Close()
            Connected = False
            Comms.Abort()
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("Error : There was an unexpected and unhandled exception.")
            Console.WriteLine("please submit it as an issue at the URL bellow")
            Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            MyConnector.Close()
            Connected = False
            Comms.Abort()
        End Try
        '-------------------------

        '-----Send Message-----
        Try
            Dim byteStream As New IO.MemoryStream()
            BinarySerializer.Serialize(byteStream, myMessage)
            SendBuff = byteStream.ToArray
            MyConnector.Client.Blocking = True
            MyConnector.Client.Send(BitConverter.GetBytes(SendBuff.Length))
            MyConnector.Client.Blocking = True
            MyConnector.Client.Send(SendBuff)
            byteStream.Close()
        Catch ex As Net.Sockets.SocketException
            MyConnector.Close()
            Connected = False
            Comms.Abort()
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("Error : There was an unexpected and unhandled exception.")
            Console.WriteLine("please submit it as an issue at the URL bellow")
            Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            MyConnector.Close()
            Connected = False
            Comms.Abort()
        End Try
        myMessage.Command = -1
        myMessage.Value = -1
        '----------------------
    End Sub

    Private WithEvents Tick As New Timer With {.Interval = 100, .Enabled = True}
    Public primaryDirection As Double
    Public primaryRec As Rectangle
    Public secondaryDirection As Double
    Public secondaryRec As Rectangle

    Private Sub UpdateGraphics() Handles Tick.Tick
        If IncomingMessage IsNot Nothing Then
            Dim MessageData As ServerMessage = IncomingMessage
            Star.Update_Call()
            If MessageData.CenterShip IsNot Nothing Then
                primaryDirection = Helm.NormalizeDirection(MessageData.Direction + MessageData.Primary.TurnDistance.current)
                Dim Width As Integer = MessageData.Primary.Range.current * 2
                Dim Height As Integer = MessageData.Primary.Range.current * 2
                Dim startX As Integer = (Screen.ImageSize.X / 2) - MessageData.Primary.Range.current
                Dim startY As Integer = (Screen.ImageSize.Y / 2) - MessageData.Primary.Range.current
                primaryRec = New Rectangle(startX, startY, Width, Height)

                secondaryDirection = Helm.NormalizeDirection(MessageData.Direction + MessageData.Secondary.TurnDistance.current)
                Width = MessageData.Secondary.Range.current * 2
                Height = MessageData.Secondary.Range.current * 2
                startX = (Screen.ImageSize.X / 2) - (Width / 2)
                startY = (Screen.ImageSize.Y / 2) - (Height / 2)
                secondaryRec = New Rectangle(startX, startY, Width, Height)
            End If

            '-----Set Background-----
            Dim bmp As New Bitmap(Screen.ImageSize.X, Screen.ImageSize.Y)
            Dim gDisplay As Graphics = Graphics.FromImage(bmp)
            Select Case MessageData.Warping
                Case Galaxy.Warp.None
                    gDisplay.DrawImage(My.Resources.NormalSpace, New Point(0, 0))
                Case Galaxy.Warp.Warping
                    gDisplay.DrawImage(My.Resources.Warping, New Point(0, 0))
            End Select
            '------------------------

            '-----Put on Radar-----
            For i As Integer = 0 To UBound(MessageData.Positions)
                Dim distance As Integer = Math.Sqrt((MessageData.Positions(i).X ^ 2) + (MessageData.Positions(i).Y ^ 2))
                If distance > 200 Then
                    Dim scale As Double = distance / 200
                    MessageData.Positions(i).X = (MessageData.Positions(i).X / scale)
                    MessageData.Positions(i).Y = (MessageData.Positions(i).Y / scale)
                End If
                MessageData.Positions(i).X = MessageData.Positions(i).X + ((Screen.ImageSize.X / 2) - 1)
                MessageData.Positions(i).Y = MessageData.Positions(i).Y + ((Screen.ImageSize.Y / 2) - 1)
            Next
            '----------------------

            '-----Render-----
            For Each i As Star In stars
                gDisplay.FillEllipse(Brushes.White, CInt(i.Position.X - (i.Diamiter / 2)), CInt(i.Position.Y - (i.Diamiter / 2)), CInt(i.Diamiter), CInt(i.Diamiter))
            Next

            If MessageData.Warping <> Galaxy.Warp.Warping And MessageData.State = Galaxy.Scenario.Battle And MessageData.CenterShip IsNot Nothing Then
                '-----Batteries Arc-----
                gDisplay.DrawPie(Pens.Yellow, primaryRec, CSng(180 * (primaryDirection - (Battery.PlayerArc / 2)) / Math.PI), CSng(180 * Battery.PlayerArc / Math.PI))

                gDisplay.DrawPie(Pens.Yellow, secondaryRec, CSng(180 * (secondaryDirection - (Battery.PlayerArc / 2)) / Math.PI), CSng(180 * Battery.PlayerArc / Math.PI))
                '-----------------------
            End If

            For i As Integer = 0 To UBound(MessageData.Positions)
                If (MessageData.Warping = Galaxy.Warp.None) Or (MessageData.Warping = Galaxy.Warp.Warping And i = 0) Then
                    Dim model As Bitmap
                    Select Case MessageData.Positions(i).Allegience
                        Case Galaxy.Allegence.Player
                            model = My.Resources.Friendly
                        Case Galaxy.Allegence.Pirate
                            model = My.Resources.Pirate
                        Case Galaxy.Allegence.Neutral
                            model = My.Resources.Station
                    End Select
                    model.MakeTransparent()
                    Dim g As Graphics = Graphics.FromImage(BlankShipSpace)
                    g.Clear(Color.Transparent)
                    g.TranslateTransform(BlankShipSpace.Width / 2, BlankShipSpace.Height / 2)
                    g.RotateTransform(180 * MessageData.Positions(i).Direction / Math.PI)
                    g.TranslateTransform(-(BlankShipSpace.Width / 2), -(BlankShipSpace.Height / 2))
                    g.DrawImage(model, New Point(3, 3))
                    gDisplay.DrawImage(BlankShipSpace, New Point(MessageData.Positions(i).X - (model.Width / 2), MessageData.Positions(i).Y - (model.Height / 2)))
                End If
            Next

            If MessageData.CenterShip IsNot Nothing Then
                '-----Batteries Target-----
                If MessageData.TargetIndex > -1 And MessageData.Warping <> Galaxy.Warp.Warping And
                    MessageData.TargetIndex < MessageData.Positions.Length Then
                    Dim g As Graphics = Graphics.FromImage(BlankShipSpace)
                    g.Clear(Color.Transparent)
                    gDisplay.DrawRectangle(Pens.Blue,
                                      MessageData.Positions(MessageData.TargetIndex).X - CInt((BlankShipSpace.Width / 2)),
                                      MessageData.Positions(MessageData.TargetIndex).Y - CInt((BlankShipSpace.Height / 2)),
                                      BlankShipSpace.Width, BlankShipSpace.Height)
                End If
                '--------------------------
            End If
            Screen.GamePlayLayout.picDisplayGraphics.Image = bmp
            '-------------------
        End If
    End Sub

End Class
