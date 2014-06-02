﻿Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Class Client
    Public ClientLoop As Boolean = False 'A Boolean value indicating whether the Client is connected to a Server
    Public MyConnector As Net.Sockets.TcpClient 'A TcpClient object used to connect to a host server and communicate with it
    Public myMessage As New ClientMessage 'A ClientMessage object that gets sent to the Server indicating what keys
    'are being pressed/released
    Public IncomingMessage As ServerMessage 'A ServerMessage object that is sent to the Client from the Server
    Public Comms As Threading.Thread 'A Thread responsible for sending and receiving messages to and from the Server
    Private stars(150) As Star 'An Array of Star objects that assist visualizing motion
    Private ReadOnly BlankShipSpace As New Bitmap(40, 40) 'A blank Bitmap object to draw Ship Models onto while rendering
    Private BytesToReceive As Integer 'An Integer representing how many bytes the Client should wait to receive from the Server
    Private BytesReceived As Integer 'An Integer representing how many bytes of data the Client has Received from the Server
    Private MessageBuff(24000) As Byte 'An Array of Bytes to store the bytes of data received from the Server
    Private ByteBuff(3) As Byte 'An Array of 4 Bytes to convert into the BytesToReceive Integer
    Private BinarySerializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter 'A Serialiser Object to serialise/deserialise
    'bytes for sending/receiving
    Private SendBuff() As Byte 'An Array of Bytes to send to the Server
    Private IncommingMessageMutex As Threading.Mutex 'A Mutex object to prevent cross threading errors between the Comms' and the
    Public MyMessageMutex As Threading.Mutex 'A Mutex object to preven cross threading errors concerning keystrokes
    'Client's threads around IncommingMessage
    Private MutexCreated As Boolean 'A Boolean value indecating whether a Mutex object was created succesfully
    Private ModelDirectory(Galaxy.Allegence.max - 1)() As Bitmap 'A staggered Array object which stores all the images used by two enumerators
    Private LastState As Galaxy.Scenario = -1 'The state the Server's Galaxy was last in to compare to what it is currently in so as to Start
    'playing audio files
    Public Zoom As Integer = 100 'An integer representing how far in/out the player is zooming on the image so as to scale the image correctly

    Public Class Star
        Public Position As Point 'A Point object indecating the Star's position on the Bitmap that gets displayed
        Public Diameter As Integer = 8 'An Integer representing the Star's diameter in pixels
        Private Flash As Boolean = False 'A Boolean value indecating whether the Star is 'flashing'
        Private count As Integer 'An Integer representing how long the Star will 'flash' for
        Public Shared Speed As Double = 1 'An Integer representing to what factor the stars speed is multiplied
        Public Shared ReadOnly WarpSpeed As Integer = 20 'An Integer representing the value of Speed during 'Warp Speed'
        Private Shared WarpCount As Integer = 0 'The Count while 'warping' for Star speed

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Private Shared Event Update(ByRef nData As ServerMessage, ByVal nZoom As Double) 'A shared Event that updates all Star objects
        Public Shared Sub Update_Call(ByRef nData As ServerMessage, ByVal nZoom As Double) 'Raises the Update Event
            RaiseEvent Update(nData, nZoom)

            '-----Change the speed of Stars to simulate Warp-----
            If ConsoleWindow.OutputScreen.MyClient.IncomingMessage.Warping = Galaxy.Warp.Warping And
                WarpCount = -30 Then 'Begin the counter
                WarpCount = 69
                My.Computer.Audio.Play(My.Resources.Warp_Audio, AudioPlayMode.Background) 'Play the Audio for warping
            ElseIf WarpCount <= 20 And WarpCount > 0 Then 'Decelerate
                Speed = Speed - (WarpSpeed / 20)
                WarpCount = WarpCount - 1
            ElseIf WarpCount >= 50 Then 'Accelerate
                Speed = Speed + (WarpSpeed / 20)
                WarpCount = WarpCount - 1
            ElseIf WarpCount > -30 Then
                WarpCount = WarpCount - 1
            End If
            '----------------------------------------------------
        End Sub
        Private Sub Update_Handle(ByRef nData As ServerMessage, ByVal nZoom As Double) Handles MyClass.Update 'Handles the Update Event
            If Flash = False Then 'Random chance to 'flash'
                If Int(40 * Rnd()) = 0 Then 'Flash'
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


            Dim direction As Double = nData.Direction + Math.PI 'Set direction to be opposite
            'to the Player Ship's direction
            Position = New Point(Position.X + (nData.Speed.current * Math.Cos(direction) * Speed * nZoom),
                                 Position.Y + (nData.Speed.current * Math.Sin(direction) * Speed * nZoom))
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
        ReDim ModelDirectory(Galaxy.Allegence.Neutral)(ShipLayout.Formats.SectorMax - 1) 'Dimentionalise the directory for Neutral objects
        ReDim ModelDirectory(Galaxy.Allegence.Friendly)(ShipLayout.Formats.ShipsMax - 1) 'Dimentionalse the directory for Friendly objects
        ReDim ModelDirectory(Galaxy.Allegence.Pirate)(ShipLayout.Formats.ShipsMax - 1) 'Dimentionalise the directior for Enemy objects

        '-----Fill out the Directory-----
        For i As Integer = 0 To ShipLayout.Formats.SectorMax - 1
            Select Case i
                Case ShipLayout.Formats.Station
                    ModelDirectory(Galaxy.Allegence.Neutral)(i) = My.Resources.NeutralStation
            End Select
        Next
        For i As Integer = 0 To ShipLayout.Formats.ShipsMax - 1
            Select Case i
                Case ShipLayout.Formats.Station
                    ModelDirectory(Galaxy.Allegence.Friendly)(i) = My.Resources.FriendlyStation
                Case ShipLayout.Formats.Fleet
                    ModelDirectory(Galaxy.Allegence.Friendly)(i) = My.Resources.FriendlyFleet
                Case ShipLayout.Formats.Screamer
                    ModelDirectory(Galaxy.Allegence.Friendly)(i) = My.Resources.FriendlyScreamer
                Case ShipLayout.Formats.Thunder
                    ModelDirectory(Galaxy.Allegence.Friendly)(i) = My.Resources.FriendlyThunder
            End Select
        Next
        For i As Integer = 0 To ShipLayout.Formats.ShipsMax - 1
            Select Case i
                Case ShipLayout.Formats.Station
                    ModelDirectory(Galaxy.Allegence.Pirate)(i) = My.Resources.PirateStation
                Case ShipLayout.Formats.Fleet
                    ModelDirectory(Galaxy.Allegence.Pirate)(i) = My.Resources.PirateFleet
                Case ShipLayout.Formats.Screamer
                    ModelDirectory(Galaxy.Allegence.Pirate)(i) = My.Resources.PirateScreamer
                Case ShipLayout.Formats.Thunder
                    ModelDirectory(Galaxy.Allegence.Pirate)(i) = My.Resources.PirateThunder
            End Select
        Next
        '--------------------------------

        myMessage.Station = nStation 'Set the Station value of myStation to the player's selected Station.StationTypes enumerator

        '-----Attempt to connect to a remote Server-----
        Try
            MyConnector = New Net.Sockets.TcpClient(nIP, 1225) 'Create a new TcpClient object and connect it to a Server
            MyConnector.Client.ReceiveTimeout = 10000 'Wait 10 Seconds for data to be received before timing out
            MyConnector.Client.SendTimeout = 10000 'Wait 10 seconds for data to be sent before timing out
            ClientLoop = True 'Set ClientLoop to True so that the Client's communications will not close
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
            ClientLoop = False 'Let the Client's communications close
        End Try
        '-----------------------------------------------
    End Sub

    Public Sub SendCommand(ByVal command As Integer, ByVal value As Integer) 'Send key strokes to the Server
        If ClientLoop = True Then 'The Client is connected to a Server
            MyMessageMutex.WaitOne() 'Wait till the Mutex is free
            myMessage.Command = command 'The Action the Client is attempting
            myMessage.Value = value '-1 if Nothing, 0 if the key is being released and 1 if the key is being pressed
            MyMessageMutex.ReleaseMutex() 'Release the Mutex
        End If
    End Sub

    Private Sub StartComms()
        MyConnector.Client.Blocking = True 'Wait till the Message is sent
        MyConnector.Client.Send(BitConverter.GetBytes(myMessage.Station)) 'Send 4 bytes specifying which Station the Client wants to control

        While ClientLoop = True
            RunComms() 'Sends and Receives messages to and from the Server
        End While

        '-----Finalise the Client object-----
        Tick.Enabled = False 'Stop the Client object from drawing on the screen
        IncommingMessageMutex.Close() 'Close the Mutex
        MyMessageMutex.Close() 'Close the Mutex
        If MyConnector.Connected = True Then 'The Client is still connected to a Server
            MyConnector.Client.Disconnect(False) 'Disconnects the Socket from the Server
        End If
        MyConnector.Close() 'Close the TCPClient object
        '------------------------------------
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
            '-----------------------------
        Catch ex As Net.Sockets.SocketException 'The Client was disconnected from the Server
            Console.WriteLine()
            Console.WriteLine("Error : Connection to the Server was lost.")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            ClientLoop = False 'Let the Loop finish
            Beep()
            Exit Sub 'Leave the Subroutine emmidiately
        Catch ex As ArgumentOutOfRangeException 'The Server sent a message that was to big for the Client to receive
            Console.WriteLine()
            Console.WriteLine("Error : The Client was unable to receive the full message.")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine("Buffer Size: {0}, Message Size: {1}", MessageBuff.Length, BytesToReceive)
            Console.WriteLine()
            ClientLoop = False 'Let the Loop finish
            Beep()
            Exit Sub 'Leave the Subroutine emmidiately
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("Error : There was an unexpected and unhandled exception.")
            Console.WriteLine("please submit it as an issue at the URL bellow")
            Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            ClientLoop = False 'Let the Loop finish
            Beep()
            Exit Sub 'Leave the Subroutine emmidiately
        End Try
        IncommingMessageMutex.WaitOne() 'Block until the Mutex is free
        IncomingMessage = BinarySerializer.Deserialize(New IO.MemoryStream(MessageBuff, 0, BytesToReceive)) 'Deserialise the received Bytes
        'into a ServerMessage object
        IncommingMessageMutex.ReleaseMutex() 'Release the Mutex
        '-------------------------

        '-----Send Message-----
        MyMessageMutex.WaitOne() 'Wait for the Mutex to be free
        Try
            Dim byteStream As New IO.MemoryStream() 'A MemoryStream object to use with the BinarySerialiser object
            BinarySerializer.Serialize(byteStream, myMessage) 'Serialise the myMessage object into byteStream
            SendBuff = byteStream.ToArray 'Create an array of Bytes from byteStream
            MyConnector.Client.Blocking = True 'Set the MyConnector object to block until all Bytes are sent
            MyConnector.Client.Send(BitConverter.GetBytes(SendBuff.Length)) 'Send 4 Bytes representing the number of Bytes to be sent
            MyConnector.Client.Blocking = True 'Set the MyConnector object to block until all Bytes are sent
            MyConnector.Client.Send(SendBuff) 'Send the message to the Server
            byteStream.Close() 'Close the MemoryStream object
            myMessage.Command = -1 'Set the myMessage.Command value to -1 meaning their is nothing to send
            myMessage.Value = -1 'Set the myMessage.Value value to -1 meaning their is nothing to send
        Catch ex As Net.Sockets.SocketException
            Console.WriteLine()
            Console.WriteLine("Error : Connection to the Server was lost.")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            ClientLoop = False 'Let the Loop finish
            Beep()
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("Error : There was an unexpected and unhandled exception.")
            Console.WriteLine("please submit it as an issue at the URL bellow")
            Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
            Console.WriteLine()
            Console.WriteLine(ex.ToString)
            Console.WriteLine()
            ClientLoop = False 'Let the Loop finish
            Beep()
        End Try
        MyMessageMutex.ReleaseMutex() 'Release the Mutex
        '----------------------
    End Sub

    Public WithEvents Tick As New Timer With {.Interval = 100, .Enabled = True} 'A Timer object that 'ticks' 10 times a second
    Public primaryDirection As Double 'A Double representing the direction the Primary Weapon is facing
    Public primaryRec As Rectangle 'A Rectangle object representing the space that the Primary Weapon's arc gets drawn in
    Public secondaryDirection As Double 'A Double representing the direction the Secondary Weapon is facing
    Public secondaryRec As Rectangle 'A Rectangle object representing the space that the Secondary Weapon's arc gets drawn in

    Private Sub UpdateGraphics() Handles Tick.Tick 'Update the Image to display on the screen
        If IncomingMessage IsNot Nothing Then 'There is a message to read
            Dim zoomScale As Double = Zoom / 100 'The scale of the image
            IncommingMessageMutex.WaitOne() 'Block until the Mutex is free
            Dim MessageData As ServerMessage = IncomingMessage 'Create a copy of the IncommingMessage object
            IncommingMessageMutex.ReleaseMutex() 'Release the Mutex
            If LastState <> MessageData.State Then 'The Server's Galaxy has changed state
                LastState = MessageData.State 'Set the new LastState
                Select Case MessageData.State
                    Case Galaxy.Scenario.Battle
                        If MessageData.Warping = Galaxy.Warp.None Then 'Play the Battle Music
                            My.Computer.Audio.Play(My.Resources.The_Pounce_Extended, AudioPlayMode.BackgroundLoop)
                        End If
                    Case Galaxy.Scenario.Transit
                        My.Computer.Audio.Play(My.Resources.Timeless_Voyage_Extended, AudioPlayMode.BackgroundLoop) 'Play the Sector Music
                End Select
            End If
            If MessageData.State <> -1 Then
                Star.Update_Call(MessageData, zoomScale) 'Update all of the Star objects
                If MessageData.State = Galaxy.Scenario.Battle Then 'Set the Weapon directions etc
                    primaryDirection = Helm.NormaliseDirection(MessageData.Direction + MessageData.Primary.TurnDistance.current)
                    'Set the direction of the Primary Weapon
                    Dim Width As Integer = MessageData.Primary.Range.current * 2 * zoomScale 'Set the Width of primaryRec
                    Dim Height As Integer = MessageData.Primary.Range.current * 2 * zoomScale 'Set the Height of primaryRec
                    Dim startX As Integer = (Screen.ImageSize.X / 2) - MessageData.Primary.Range.current * zoomScale  'Set the X coordinate
                    'of primaryRec
                    Dim startY As Integer = (Screen.ImageSize.Y / 2) - MessageData.Primary.Range.current * zoomScale  'Se the Y coordinate
                    'of the primaryRec
                    primaryRec = New Rectangle(startX, startY, Width, Height) 'Create the primaryRec Rectangle object

                    secondaryDirection = Helm.NormaliseDirection(MessageData.Direction + MessageData.Secondary.TurnDistance.current)
                    'Set the direction of the Secodary Weapon
                    Width = MessageData.Secondary.Range.current * 2 * zoomScale 'Set the Width of the secondaryRec
                    Height = MessageData.Secondary.Range.current * 2 * zoomScale 'Set the Height of the secondaryRec
                    startX = (Screen.ImageSize.X / 2) - MessageData.Secondary.Range.current * zoomScale 'Set the X coordinate of
                    'secondaryRec
                    startY = (Screen.ImageSize.Y / 2) - MessageData.Secondary.Range.current * zoomScale 'Set the Y coordinate of the
                    'secondaryRec
                    secondaryRec = New Rectangle(startX, startY, Width, Height) 'Create the secondaryRec Rectangle object
                End If

                '-----Set Background-----
                Dim bmp As New Bitmap(Screen.ImageSize.X, Screen.ImageSize.Y) 'Create a new Bitmap object
                Dim gDisplay As Graphics = Graphics.FromImage(bmp) 'Create a Graphics object for bmp
                If MessageData.Warping = Galaxy.Warp.None Then  'The Player Ship is not 'warping'
                    gDisplay.DrawImage(My.Resources.NormalSpace, New Point(0, 0)) 'Draw the normal background
                ElseIf MessageData.Warping = Galaxy.Warp.Warping Or MessageData.Warping = Galaxy.Warp.Exiting Then 'The Player Ship is 'warping'
                    gDisplay.DrawImage(My.Resources.Warping, New Point(0, 0)) 'Draw the warping background
                End If
                '------------------------

                '-----Put on Radar----- 'Scale the positions of the Ships back so that they appear on the edge of a
                'circle if they are off screen; the Player Ship is always at (0,0)
                For i As Integer = 0 To UBound(MessageData.Positions)
                    MessageData.Positions(i).X = MessageData.Positions(i).X * zoomScale 'Scale the X Position
                    MessageData.Positions(i).Y = MessageData.Positions(i).Y * zoomScale 'Scale the Y Position
                    Dim distance As Integer = Math.Sqrt((MessageData.Positions(i).X ^ 2) + (MessageData.Positions(i).Y ^ 2)) 'Calculate the
                    'Ship's distance from the center of the image
                    If distance > 200 Then 'The (X,Y) coordinates need to be scaled back to put it on the edge of the radar
                        Dim scale As Double = distance / 200 'The scale of the distance relative to 200
                        MessageData.Positions(i).X = (MessageData.Positions(i).X / scale) 'Scale X
                        MessageData.Positions(i).Y = (MessageData.Positions(i).Y / scale) 'Scale Y
                    End If
                    MessageData.Positions(i).X = MessageData.Positions(i).X + ((Screen.ImageSize.X / 2) - 1) 'Add the offset so that 0
                    'on the X is in the center of the screen
                    MessageData.Positions(i).Y = MessageData.Positions(i).Y + ((Screen.ImageSize.Y / 2) - 1) 'Add the offset so that 0
                    'on the Y is in the center of the screen
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

                If MessageData.Warping <> Galaxy.Warp.Warping And
                    MessageData.Warping <> Galaxy.Warp.Exiting And
                    MessageData.State = Galaxy.Scenario.Battle And
                    MessageData.Primary IsNot Nothing Then 'The Player is in comabat so draw the Weapon arcs
                    '-----Batteries Arc-----
                    gDisplay.DrawPie(Pens.Yellow, primaryRec, CSng(180 * (
                                     primaryDirection + MessageData.PrimaryMount - (Battery.PlayerArc / 2)) / Math.PI),
                                     CSng(180 * Battery.PlayerArc / Math.PI)) 'Draw the yellow outline of a section of a circle
                    'that has the diameter of the Weapons Range value and the sweep of the Players 'hit arc'

                    gDisplay.DrawPie(Pens.Yellow, secondaryRec, CSng(180 * (
                                     secondaryDirection + MessageData.SecondaryMount - (Battery.PlayerArc / 2)) / Math.PI),
                                     CSng(180 * Battery.PlayerArc / Math.PI)) 'Draw the yellow outline of a section of a circle
                    'that has the diameter of the Weapons Range value and the sweep of the Players 'hit arc'
                    '-----------------------
                End If

                '-----Draw Ships----- 'Draw the Ship models onto bmp
                For i As Integer = 0 To UBound(MessageData.Positions)
                    If MessageData.Warping = Galaxy.Warp.None Or i = 0 Then 'The Player is not in 'warp' or it's the Players Ship
                        'or it is the Player's Ship
                        Dim model As Bitmap = ModelDirectory(MessageData.Positions(i).Allegience)(MessageData.Positions(i).Format) 'A Bitmap of a ship
                        model.MakeTransparent() 'Make the white space of the model Bitmap object transparent
                        Dim g As Graphics = Graphics.FromImage(BlankShipSpace) 'Create a Graphics object of BlankShipSpace
                        g.Clear(Color.Transparent) 'Clear BlankShipSpace
                        g.ScaleTransform(zoomScale, zoomScale) 'Scale the Image to the Zoom level
                        g.TranslateTransform(BlankShipSpace.Width / 2, BlankShipSpace.Height / 2) 'Move the turning point into the center of the Graphic
                        g.RotateTransform(180 * MessageData.Positions(i).Direction / Math.PI) 'Rotate the Graphic by the direction of the Ship
                        g.TranslateTransform(-(BlankShipSpace.Width / 2), -(BlankShipSpace.Height / 2)) 'Return the turning point to the edge of the Graphic
                        g.DrawImage(model, New PointF((BlankShipSpace.Width - model.Width) / 2, (BlankShipSpace.Height - model.Height) / 2))
                        'Draw model onto BlankShipSpace
                        If MessageData.Positions(i).Hull.current <> -1 Then 'There is a need for an indecator for the model
                            Dim nBrush As Brush 'The color of the indecator
                            Dim hullFraction As Double = MessageData.Positions(i).Hull.current / MessageData.Positions(i).Hull.max
                            If MessageData.Positions(i).Hit = True Then
                                nBrush = Brushes.Orange
                            ElseIf hullFraction > 2 / 3 Then
                                nBrush = Brushes.Blue
                            ElseIf hullFraction > 1 / 3 Then
                                nBrush = Brushes.GreenYellow
                            Else
                                nBrush = Brushes.DarkRed
                            End If
                            g.FillEllipse(nBrush, New Rectangle((BlankShipSpace.Width / 2) - 4, (BlankShipSpace.Height / 2) - 4, 8, 8))
                            'Draw a circle onto the center of the model indecating it's hull
                        End If
                        gDisplay.DrawImage(BlankShipSpace, New Point(MessageData.Positions(i).X - (BlankShipSpace.Width / 2 * zoomScale),
                                                                     MessageData.Positions(i).Y - (BlankShipSpace.Height / 2 * zoomScale)))
                        'Draw the model onto bmp
                    End If
                Next
                '--------------------

                If MessageData.State = Galaxy.Scenario.Battle And
                    MessageData.Warping = Galaxy.Warp.None Then 'Check for a target to highlight
                    '-----Batteries Target-----
                    If MessageData.TargetIndex > -1 And MessageData.Warping <> Galaxy.Warp.Warping And
                        MessageData.TargetIndex < MessageData.Positions.Length Then 'There is a target to highlight
                        gDisplay.DrawRectangle(Pens.Blue,
                                          MessageData.Positions(MessageData.TargetIndex).X - CInt((BlankShipSpace.Width / 2 * zoomScale)),
                                          MessageData.Positions(MessageData.TargetIndex).Y - CInt((BlankShipSpace.Height / 2 * zoomScale)),
                                          CInt(BlankShipSpace.Width * zoomScale), CInt(BlankShipSpace.Height * zoomScale))
                        'Draw a blue square around the targeted ship
                    End If
                    '--------------------------
                End If
                Screen.GamePlayLayout.picDisplayGraphics.Image = bmp 'Display the new Image
                '-------------------
            End If
        End If
    End Sub

End Class
