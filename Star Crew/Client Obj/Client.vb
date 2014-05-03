Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Class Client
    Public Connected As Boolean = False 'A Boolean value indicating whether the Client is connected to a Server
    Public MyConnector As Net.Sockets.TcpClient 'A TcpClient object used to connect to a host server and communicate with it
    Public myMessage As New ClientMessage 'A ClientMessage object that gets sent to the Server indicating what keys
    'are being pressed/released
    Public IncomingMessage As ServerMessage 'A ServerMessage object that is sent to the Client from the Server
    Public Comms As Threading.Thread 'A Thread responsible for sending and receiving messages to and from the Server
    Private stars(200) As Star 'An Array of Star objects that assist visualizing motion
    Private ReadOnly BlankShipSpace As New Bitmap(40, 40) 'A blank Bitmap object to draw Ship Models onto while rendering
    Private BytesToReceive As Integer 'An Integer representing how many bytes the Client should wait to receive from the Server
    Private BytesReceived As Integer 'An Integer representing how many bytes of data the Client has Received from the Server
    Private MessageBuff(11000) As Byte 'An Array of Bytes to store the bytes of data received from the Server
    Private ByteBuff(3) As Byte 'An Array of 4 Bytes to convert into the BytesToReceive Integer
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter 'A Serialiser Object to serialise/deserialise
    'bytes for sending/receiving
    Private SendBuff() As Byte 'An Array of Bytes to send to the Server
    Private IncommingMessageMutex As Threading.Mutex 'A Mutex object to prevent cross threading errors between the Comms' and the
    Public MyMessageMutex As Threading.Mutex 'A Mutex object to preven cross threading errors concerning keystrokes
    'Client's threads around IncommingMessage
    Private MutexCreated As Boolean 'A Boolean value indecating whether a Mutex object was created succesfully

    Public Class Star
        Public Position As Point 'A Point object indecating the Star's position on the Bitmap that gets displayed
        Public Diameter As Integer = 8 'An Integer representing the Star's diameter in pixels
        Private Flash As Boolean = False 'A Boolean value indecating whether the Star is 'flashing'
        Private count As Integer 'An Integer representing how long the Star will 'flash' for
        Public Shared Speed As Double = 1 'An Integer representing to what factor the stars speed is multiplied
        Public Shared ReadOnly WarpSpeed As Integer = 20 'An Integer representing the value of Speed during 'Warp Speed'

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Private Shared Event Update(ByRef nData As ServerMessage) 'A shared Event that updates all Star objects
        Public Shared Sub Update_Call(ByRef nData As ServerMessage) 'Raises the Update Event
            RaiseEvent Update(nData)

            If ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Warping = Galaxy.Warp.Warping Then 'Set Speed to = WarpSpeed
                Speed = WarpSpeed
            Else 'Reset Speed to 1
                Speed = 1
            End If
        End Sub
        Private Sub Update_Handle(ByRef nData As ServerMessage) Handles MyClass.Update 'Handles the Update Event
            If Flash = False Then 'Random chance to 'flash'
                If Int(80 * Rnd()) = 0 Then 'Flash'
                    Flash = True
                    count = 10
                End If
            ElseIf count = 1 Then 'Return to normal
                count = 0
                Flash = False
                Diameter = 8
            Else 'Decrease or Increase Diameter
                count = count - 1
                If Diameter > 1 Then 'Decrease
                    Diameter = Diameter - 1
                Else 'Increase
                    Diameter = Diameter + 1
                End If
            End If


            Dim direction As Double = ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Direction + Math.PI 'Set direction to be opposite
            'to the Player Ship's direction
            Position = New Point(Position.X + (ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Speed.current * Math.Cos(direction) * Speed),
                                 Position.Y + (ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Speed.current * Math.Sin(direction) * Speed))
            'Change Position to new position relative to the Player Ship's speed * by Speed value

            '-----Keep Stars on screen-----
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
            '------------------------------
        End Sub
    End Class

    Public Sub New(ByVal nIP As String, ByVal nStation As Integer)
        myMessage.Station = nStation 'Set the Station value of myStation to the selected Station.StationTypes enumerator
        Try
            MyConnector = New Net.Sockets.TcpClient(nIP, 1225) 'Create a new TcpClient object
            Connected = True 'Set Connected to True
            For i As Integer = 0 To UBound(stars)
                stars(i) = New Star(New Point(Int((594 * Rnd()) + 3), Int((594 * Rnd())) + 3)) 'Create the Star objects
            Next
            Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'Set the name of the user
            Dim securityProtocols As New Security.AccessControl.MutexSecurity 'Create a new MutexSecurity object
            securityProtocols.AddAccessRule(
                New Security.AccessControl.MutexAccessRule(user,
                                                           Security.AccessControl.MutexRights.Modify Or
                                                           Security.AccessControl.MutexRights.Synchronize,
                                                           Security.AccessControl.AccessControlType.Allow))
            'Add a MutexAccessRule that allows any one Thread to Access, Release or Wait on this Mutex
            IncommingMessageMutex = New Threading.Mutex(False, "MessageMutex", MutexCreated, securityProtocols) 'Create a new Mutex without
            'giving initial ownership to this thread and with the security specified in securityProtocols
            MyMessageMutex = New Threading.Mutex(False, "MessageMutex", MutexCreated, securityProtocols) 'Create a new Mutex without
            'giving initial ownership to this thread and with the security specified in securityProtocols
            Tick.Enabled = True 'Start the Timer object that updates the screens Image
            Comms = New Threading.Thread(AddressOf StartComms) 'Create a new Comms thread
            Comms.Start() 'Start the Comms thread
        Catch ex As Net.Sockets.SocketException 'The TcpClient could not connect to the server
            Console.WriteLine()
            Console.WriteLine("Error: Could not connect to server")
            Console.WriteLine("Check address and make sure the server exists")
            Console.WriteLine(ex.ToString)
            Console.Beep()
            Connected = False
        End Try
    End Sub

    Public Sub SendCommand(ByVal command As Integer, ByVal value As Integer) 'Send key strokes to the Server
        MyMessageMutex.WaitOne() 'Wait till the Mutex is free
        myMessage.Command = command 'The Action the Client is attempting
        myMessage.Value = value '-1 if Nothing, 0 if the key is being released and 1 if the key is being pressed
        MyMessageMutex.ReleaseMutex() 'Release the Mutex
    End Sub

    Private Sub StartComms()
        MyConnector.Client.Send(BitConverter.GetBytes(myMessage.Station)) 'Send 4 bytes specifying which Station the Client wants to control

        While True
            RunComms() 'Sends and Receives messages to and from the Server
        End While
    End Sub
    Private Sub RunComms()
        '-----Recieve Message-----
        BytesReceived = 0 'Reset the number of Bytes that have been received
        BytesToReceive = 0 'Reset the number of Bytes received
        Try
            '-----Get Number of Bytes to Receive-----
            While BytesReceived < 4 'While the Client has received less than 4 Bytes
                BytesReceived = BytesReceived +
                    MyConnector.Client.Receive(ByteBuff, BytesReceived, 4 - BytesReceived, Net.Sockets.SocketFlags.None)
                'Receive up to the remaining bytes necessary
            End While
            BytesToReceive = BitConverter.ToInt32(ByteBuff, 0) 'Get an Integer representing the number of Bytes the Server is about to send
            '----------------------------------------

            '-----Receive the Message-----
            BytesReceived = 0 'Reset the number of bytes that have been received
            While BytesReceived < BytesToReceive 'While the Client has not received a full message
                BytesReceived = BytesReceived +
            MyConnector.Client.Receive(MessageBuff, BytesReceived, BytesToReceive - BytesReceived, Net.Sockets.SocketFlags.None)
                'Receive the up to the number of Bytes needed for a full message
            End While
            IncommingMessageMutex.WaitOne() 'Block until the Mutex is free
            IncomingMessage = BinarySerializer.Deserialize(New IO.MemoryStream(MessageBuff, 0, BytesToReceive)) 'Deserialise the received Bytes
            'into a ServerMessage object
            IncommingMessageMutex.ReleaseMutex() 'Release the Mutex
            '-----------------------------
        Catch ex As Net.Sockets.SocketException 'The Client was disconnected from the Server
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
            Dim byteStream As New IO.MemoryStream() 'A MemoryStream object to use with the BinarySerialiser object
            BinarySerializer.Serialize(byteStream, myMessage) 'Serialise the myMessage object into byteStream
            SendBuff = byteStream.ToArray 'Create an array of Bytes from byteStream
            MyConnector.Client.Blocking = True 'Set the MyConnector object to block until all Bytes are sent
            MyConnector.Client.Send(BitConverter.GetBytes(SendBuff.Length)) 'Send 4 Bytes representing the number of Bytes to be sent
            MyConnector.Client.Blocking = True 'Set the MyConnector object to block until all Bytes are sent
            MyConnector.Client.Send(SendBuff) 'Send the message to the Server
            byteStream.Close() 'Close the MemoryStream object
        Catch ex As System.NullReferenceException
            MyConnector.Close()
            Connected = False
            Comms.Abort()
        Catch ex As Net.Sockets.SocketException 'The Client was disconnected from the Server
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
        myMessage.Command = -1 'Set the myMessage.Command value to -1 meaning their is nothing to send
        myMessage.Value = -1 'Set the myMessage.Value value to -1 meaning their is nothing to send
        '----------------------
    End Sub

    Public WithEvents Tick As New Timer With {.Interval = 100, .Enabled = True} 'A Timer object that 'ticks' 10 times a second
    Public primaryDirection As Double 'A Double representing the direction the Primary Weapon is facing
    Public primaryRec As Rectangle 'A Rectangle object representing the space that the Primary Weapon's arc gets drawn in
    Public secondaryDirection As Double 'A Double representing the direction the Secondary Weapon is facing
    Public secondaryRec As Rectangle 'A Rectangle object representing the space that the Secondary Weapon's arc gets drawn in

    Private Sub UpdateGraphics() Handles Tick.Tick 'Update the Image to display on the screen
        If IncomingMessage IsNot Nothing Then 'There is a message to read
            IncommingMessageMutex.WaitOne() 'Block until the Mutex is free
            Dim MessageData As ServerMessage = IncomingMessage 'Create a copy of the IncommingMessage object
            IncommingMessageMutex.ReleaseMutex() 'Release the Mutex
            Star.Update_Call(MessageData) 'Update all of the Star objects
            If MessageData.CenterShip IsNot Nothing Then 'Set the Weapon directions etc
                primaryDirection = Helm.NormalizeDirection(MessageData.Direction + MessageData.Primary.TurnDistance.current)
                'Set the direction of the Primary Weapon
                Dim Width As Integer = MessageData.Primary.Range.current * 2 'Set the Width of primaryRec
                Dim Height As Integer = MessageData.Primary.Range.current * 2 'Set the Height of primaryRec
                Dim startX As Integer = (Screen.ImageSize.X / 2) - MessageData.Primary.Range.current 'Set the X coordinate of primaryRec
                Dim startY As Integer = (Screen.ImageSize.Y / 2) - MessageData.Primary.Range.current 'Se the Y coordinate of the primaryRec
                primaryRec = New Rectangle(startX, startY, Width, Height) 'Create the primaryRec Rectangle object

                secondaryDirection = Helm.NormalizeDirection(MessageData.Direction + MessageData.Secondary.TurnDistance.current)
                'Set the direction of the Secodary Weapon
                Width = MessageData.Secondary.Range.current * 2 'Set the Width of the secondaryRec
                Height = MessageData.Secondary.Range.current * 2 'Set the Height of the secondaryRec
                startX = (Screen.ImageSize.X / 2) - (Width / 2) 'Set the X coordinate of secondaryRec
                startY = (Screen.ImageSize.Y / 2) - (Height / 2) 'Set the Y coordinate of the secondaryRec
                secondaryRec = New Rectangle(startX, startY, Width, Height) 'Create the secondaryRec Rectangle object
            End If

            '-----Set Background-----
            Dim bmp As New Bitmap(Screen.ImageSize.X, Screen.ImageSize.Y) 'Create a new Bitmap object
            Dim gDisplay As Graphics = Graphics.FromImage(bmp) 'Create a Graphics object for bmp
            Select Case MessageData.Warping 'Select the initial background of bmp
                Case Galaxy.Warp.None 'The Player Ship is not 'warping'
                    gDisplay.DrawImage(My.Resources.NormalSpace, New Point(0, 0)) 'Draw the normal background
                Case Galaxy.Warp.Warping 'The Player Ship is 'warping'
                    gDisplay.DrawImage(My.Resources.Warping, New Point(0, 0)) 'Draw the warping background
            End Select
            '------------------------

            '-----Put on Radar----- 'Scale the positions of the Ships back so that they appear on the edge of a circle if they are off screen
            'The Player Ship is always at (0,0)
            For i As Integer = 0 To UBound(MessageData.Positions)
                Dim distance As Integer = Math.Sqrt((MessageData.Positions(i).X ^ 2) + (MessageData.Positions(i).Y ^ 2))
                'The distance of the Ship from the Player Ship
                If distance > 200 Then 'The (X,Y) coordinates need to be scaled back
                    Dim scale As Double = distance / 200 'The scale of the distance relative to 200
                    MessageData.Positions(i).X = (MessageData.Positions(i).X / scale) 'Scale X
                    MessageData.Positions(i).Y = (MessageData.Positions(i).Y / scale) 'Scale Y
                End If
                MessageData.Positions(i).X = MessageData.Positions(i).X + ((Screen.ImageSize.X / 2) - 1) 'Add the offset so that 0 on the X is
                'in the center of the screen
                MessageData.Positions(i).Y = MessageData.Positions(i).Y + ((Screen.ImageSize.Y / 2) - 1) 'Add the offset so that 0 on the Y is
                'in the center of the screen
            Next
            '----------------------

            '-----Render-----
            For Each i As Star In stars
                gDisplay.FillEllipse(Brushes.White,
                                     CInt(i.Position.X - (i.Diameter / 2)),
                                     CInt(i.Position.Y - (i.Diameter / 2)),
                                     CInt(i.Diameter), CInt(i.Diameter))
                'Draw a white circle on bmp as wide as the Star's Diameter value
            Next

            If MessageData.Warping <> Galaxy.Warp.Warping And MessageData.State = Galaxy.Scenario.Battle And
                MessageData.CenterShip IsNot Nothing Then 'The Player is in comabat so draw the Weapon arcs
                '-----Batteries Arc-----
                gDisplay.DrawPie(Pens.Yellow, primaryRec, CSng(180 * (primaryDirection - (Battery.PlayerArc / 2)) / Math.PI),
                                 CSng(180 * Battery.PlayerArc / Math.PI)) 'Draw the yellow outline of a section of a circle
                'that has the diameter of the Weapons Range value and the sweep of the Players 'hit arc'

                gDisplay.DrawPie(Pens.Yellow, secondaryRec, CSng(180 * (secondaryDirection - (Battery.PlayerArc / 2)) / Math.PI),
                                 CSng(180 * Battery.PlayerArc / Math.PI)) 'Draw the yellow outline of a section of a circle
                'that has the diameter of the Weapons Range value and the sweep of the Players 'hit arc'
                '-----------------------
            End If

            '-----Draw Ships----- 'Draw the Ship models onto bmp
            For i As Integer = 0 To UBound(MessageData.Positions)
                If (MessageData.Warping = Galaxy.Warp.None) Or (MessageData.Warping = Galaxy.Warp.Warping And i = 0) Then 'The Player is not in 'warp'
                    'or it is the Player's Ship
                    Dim model As Bitmap 'A Bitmap of a ship
                    Select Case MessageData.Positions(i).Allegience 'Select the Image to draw
                        Case Galaxy.Allegence.Player
                            model = My.Resources.Friendly
                        Case Galaxy.Allegence.Pirate
                            model = My.Resources.Pirate
                        Case Galaxy.Allegence.Neutral
                            model = My.Resources.Station
                    End Select
                    model.MakeTransparent() 'Make the white space of the model Bitmap object transparent
                    Dim g As Graphics = Graphics.FromImage(BlankShipSpace) 'Create a Graphics object of BlankShipSpace
                    g.Clear(Color.Transparent) 'Clear BlankShipSpace
                    g.TranslateTransform(BlankShipSpace.Width / 2, BlankShipSpace.Height / 2) 'Move the turning point into the center of the Graphic
                    g.RotateTransform(180 * MessageData.Positions(i).Direction / Math.PI) 'Rotate the Graphic by the direction of the Ship
                    g.TranslateTransform(-(BlankShipSpace.Width / 2), -(BlankShipSpace.Height / 2)) 'Return the turning point to the edge of the Graphic
                    g.DrawImage(model, New Point(3, 3)) 'Draw model onto BlankShipSpace
                    gDisplay.DrawImage(BlankShipSpace, New Point(MessageData.Positions(i).X - (model.Width / 2),
                                                                 MessageData.Positions(i).Y - (model.Height / 2)))
                    'Draw BlankShipSpace onto bmp
                End If
            Next
            '--------------------

            If MessageData.CenterShip IsNot Nothing Then 'Check for a target to highlight
                '-----Batteries Target-----
                If MessageData.TargetIndex > -1 And MessageData.Warping <> Galaxy.Warp.Warping And
                    MessageData.TargetIndex < MessageData.Positions.Length Then 'There is a target to highlight
                    Dim g As Graphics = Graphics.FromImage(BlankShipSpace) 'Create a Graphics object from BlankShipSpace
                    g.Clear(Color.Transparent) 'Clear BlankShipSpace
                    gDisplay.DrawRectangle(Pens.Blue,
                                      MessageData.Positions(MessageData.TargetIndex).X - CInt((BlankShipSpace.Width / 2)),
                                      MessageData.Positions(MessageData.TargetIndex).Y - CInt((BlankShipSpace.Height / 2)),
                                      BlankShipSpace.Width, BlankShipSpace.Height)
                    'Draw a blue square around the targeted ship
                End If
                '--------------------------
            End If
            Screen.GamePlayLayout.picDisplayGraphics.Image = bmp 'Display the new Image
            '-------------------
        End If
    End Sub

End Class
