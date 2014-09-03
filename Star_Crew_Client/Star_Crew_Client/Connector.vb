Public Class Connector 'The object used to connect to and communicate with a Server
    Inherits Game_Library.Networking.Client
    Public LoopComms As Boolean = True 'A Boolean value to control when the communications stop looping
    Private ReadOnly displayBoxSideLength As Integer = My.Resources.NormalSpace.Height 'The length of one side of the display box
    Private ReadOnly displayBoxMargin As Integer = (Client_Console.OutputScreen.Height - displayBoxSideLength) / 2 'The margin from the left side and top of the display form that displayBox is located

    Public Sub New(ByVal nStation As Star_Crew_Shared_Libraries.Shared_Values.StationTypes, ByVal nIP As String, ByVal nPort As Integer, ByVal nName As String, ByVal hosting As Boolean)
        MyBase.New(nIP, nPort, nName) 'Attempts to connect to a Server and Set's the name etc of the Client
        Console.WriteLine("Client : Connecting to {0} at {1}:{2}", nStation.ToString(), nIP, nPort)
        Dim successful As Boolean = True 'The Client was successful in connecting to the Server
        Send(BitConverter.GetBytes(nStation), Net.Sockets.SocketFlags.None) 'Send which station you want to connect to
        Dim buff(3) As Byte 'A buffer of 4 Bytes that gets serialised into a message from the Client
        Dim received As Integer 'A count of how many Bytes have been received
        Try
            While received <> 4 'Loop until a full Integer is received
                received = received + Receive(buff, received, 4 - received, Net.Sockets.SocketFlags.None) 'Receive up to enough Bytes to fill the buffer
            End While
        Catch ex As Exception 'An exception occoured while receiving from the Server
            Console.WriteLine("ERROR : There was an error while trying to receive a confirmation message from the Server at {0}:{1}", nIP, nPort) 'Tell the user that there was an error while receiving a message from the Server
            successful = False 'The Client was unsuccessful in connecting to the Server
        End Try
        Select Case BitConverter.ToInt32(buff, 0) 'Convert the buffer to an Integer and see what the message is
            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Connection_Successful 'The connection was successful
                Console.WriteLine("Client : Connection to {0} at {1}:{2} was successful.", nStation.ToString(), nIP, nPort) 'Tell the user the connection succeded
            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Station_Already_Manned_Exception 'Someone is already connected to the ShipStation
                Console.WriteLine("ERROR : The requested station '{0}' was already connected at {1}:{2}", nStation.ToString(), nIP, nPort) 'Tell the user there was already someone connected to the station
                successful = False 'The Client was unsuccessful in connecting to the Server
            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception 'There was a bad connection
                Console.WriteLine("ERROR : There was a bad connection while attempting to connect to the Server at {0}:{1}", nIP, nPort) 'Tell the user that there was a bad connection
                successful = False 'The Client was unsuccessful in connecting to the Server
            Case Else 'There was an unknown error and so the Client was disconnected
                Console.WriteLine("ERROR : There was an unknown error received from the Server at {0}:{1}", nIP, nPort) 'Tell the user that there was an unknown error
        End Select
        If successful = False Then 'There was an error trying to connect
            Disconnect_Client("", Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting) 'Disconnect the Client
        Else 'There was no error and the Client is connected properly
            Screen.GameScreen.Layout(Client_Console.OutputScreen, hosting) 'Set the layout on the Screen to the GameScreen layout
            Client_Console.OutputScreen.CreateGraphics.DrawRectangle(New System.Drawing.Pen(Drawing.Brushes.DarkTurquoise, 2), displayBoxMargin, displayBoxMargin, displayBoxSideLength, displayBoxSideLength) 'Draw a border
            Client_Console.CommsThread = New System.Threading.Thread(AddressOf Run_Comms) 'Create a new thread to run the comms
            Client_Console.CommsThread.Start() 'Start the thread
        End If
    End Sub

    Public Sub Run_Comms() 'Handles the sending and receiving of messages
        Dim buffer(3) As Byte 'A Buffer to store the Bytes sent/received to/from the Client
        Dim received As Integer 'A Count of how many Bytes have been received
        Dim header As Integer 'An Integer value representing the state of the Server's Galaxy at the time of the message
        Dim hasNetwork As Boolean = False 'A Boolean value representing whether or not the Client currently has control of the network
        Do While LoopComms = True 'Keep looping the communications
            If Available <> 0 Then 'There's data to be received
                Client_Console.UseNetwork.WaitOne() 'Wait until the Client has control of the network
                hasNetwork = True 'The Client now has control of the network
                Try
                    While received <> 4 'Loop until 4 Bytes have been received
                        received = received + Receive(buffer, received, 4 - received, Net.Sockets.SocketFlags.None) 'Receive as many Bytes as is necessary to fill the buffer
                    End While
                    header = BitConverter.ToInt32(buffer, 0) 'Get the header of the message
                    Receive_ByteArray(buffer) 'Receive an array of Byte's representing the rest of the message

                    Select Case header 'See what type of state the Server's Galaxy object is in
                        Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Sector_Transit 'The Galaxy is in the Sector_Transit state
                            Dim message As Star_Crew_Shared_Libraries.Networking_Messages.SectorView = Game_Library.Serialisation.FromBytes(buffer) 'Serialise the message into a SectorView object
                        Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Ship_To_Ship 'The Galaxy is in the Ship_To_Ship state
                            Dim message As Star_Crew_Shared_Libraries.Networking_Messages.ShipView = Game_Library.Serialisation.FromBytes(buffer) 'Serialise the message into a ShipView object

                            '-----Draw Graphics-----
                            Dim displayBox As System.Drawing.Bitmap = My.Resources.NormalSpace.Clone() 'The image to draw onto
                            Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(displayBox) 'Create a graphics object from the NormalSpace bitmap
                            For i As Integer = 0 To message.Allegiancies.Length - 1 'Loop through every object
                                Dim distance As Integer = Math.Sqrt((message.Positions(i).X ^ 2) + (message.Positions(i).Y ^ 2)) 'The distance of the object from the ClientShip
                                Dim colour As System.Drawing.Brush
                                If message.Allegiancies(i) = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Emperial_Forces Then
                                    colour = Drawing.Brushes.Blue
                                Else
                                    colour = Drawing.Brushes.Red
                                End If
                                If distance > 200 Then 'Draw a dot on the border
                                    Dim ratio As Double = 200 / distance
                                    g.FillEllipse(colour, CInt((displayBoxSideLength / 2) - 5 + (message.Positions(i).X * ratio)),
                                                  CInt((displayBoxSideLength / 2) - 5 + (message.Positions(i).Y * ratio)), 10, 10) 'Draw a circle on the border of the circle
                                Else 'Draw the Ship
                                    Dim bmp As System.Drawing.Bitmap 'The Bitmap that gets drawn onto the screen
                                    Dim sideLength As Integer 'The length of the image's cross section
                                    Select Case message.Types(i) 'Select the type of object this one is
                                        Case Star_Crew_Shared_Libraries.Shared_Values.ShipTypes.Screamer
                                            sideLength = Math.Sqrt((My.Resources.FriendlyScreamer.Width ^ 2) + (My.Resources.FriendlyScreamer.Height ^ 2))
                                            bmp = New System.Drawing.Bitmap(sideLength, sideLength)
                                    End Select
                                    Dim drawer As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp) 'Create a graphics object from bmp
                                    drawer.TranslateTransform(sideLength / 2, sideLength / 2) 'Translate the origin into the center of the image
                                    drawer.RotateTransform(180 * message.Directions(i) / Math.PI) 'Rotate the image
                                    drawer.TranslateTransform(-sideLength / 2, -sideLength / 2) 'Translate the origin back to the upper left corner
                                    Dim xCoord As Integer = (sideLength - My.Resources.FriendlyScreamer.Width) / 2 'The X coord for the upper left corner of the image inside bmp
                                    Dim yCoord As Integer = (sideLength - My.Resources.FriendlyScreamer.Height) / 2 'The Y coord for the upper left corner of the image inside bmp
                                    drawer.DrawImage(My.Resources.FriendlyScreamer, xCoord, yCoord) 'Draw the image onto bmp
                                    drawer.FillEllipse(colour, CInt((sideLength / 2) - 5), CInt((sideLength / 2) - 5), 10, 10)
                                    bmp.MakeTransparent(Drawing.Color.White) 'Set all white pixels to transparent
                                    xCoord = (message.Positions(i).X + (displayBox.Width / 2) - (sideLength / 2)) 'The X coord for the upper left corner of bmp inside displayBox
                                    yCoord = (message.Positions(i).Y + (displayBox.Height / 2) - (sideLength / 2)) 'The Y coord for the upper left corner of bmp inside displayBox
                                    g.DrawImage(bmp, xCoord, yCoord) 'Draw bmp onto displayBox
                                End If
                            Next
                            Client_Console.OutputScreen.CreateGraphics.DrawImage(displayBox, displayBoxMargin, displayBoxMargin) 'Draw displayBox onto the Screen
                            '-----------------------
                        Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Shop_Interface 'The Galaxy is in the Shop_Interface state

                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception 'The Server received a bad message from the Client
                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send a Byte to the Server to signify the message went through
                            Disconnect_Client("ERROR : The Server suffered a connection error and disconnected the Client.", -1) 'Disconnect the Client
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception 'The Client has been kicked by the Server
                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send a Byte to the Server to signify the message went through
                            Disconnect_Client("ERROR : The Client has been kicked from the Server.", -1) 'Disconnect the Client
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception 'The Server Closed while the Client was connected
                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send a Byte to the Server to signify the message went through
                            Disconnect_Client("ERROR : The Server closed while the Client was still connected.", -1) 'Disconnect the Client
                    End Select
                Catch ex As Exception
                    Disconnect_Client(("ERROR : There was an error while receiving a message from the Server. Client will now disconnect." +
                                       Environment.NewLine + ex.ToString()),
                                   Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception) 'Disconnect from the Server saying that the Client is disconnecting due to a bad connection
                Finally
                    received = 0 'Clear the receive count
                    If hasNetwork = True Then 'The Client currently has control of the network
                        Client_Console.UseNetwork.ReleaseMutex() 'Release the mutex
                    End If
                End Try
            End If
        Loop

        If Connected = True Then 'The Client is still currently connected
            Disconnect_Client("Client : Communications are finalising",
                              Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting) 'Disconnect the Client from the Server
        End If
    End Sub

    Public Sub Disconnect_Client(ByVal message As String, ByVal reason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers) 'Disconnects the Client from the Server
        If reason <> -1 Then 'There's a reason to be sent
            Try
                If Connected = True Then 'The Client is still connected to the Server
                    Try
                        Send(BitConverter.GetBytes(reason), Net.Sockets.SocketFlags.None) 'Send the reason to the Server
                        Dim buff(0) As Byte
                        Receive(buff, 0, 1, Net.Sockets.SocketFlags.None) 'Receive a Byte from the Server to signify the message went through
                    Catch ex As Exception
                        Console.WriteLine("ERROR : There was an error trying to send to the Server '" + reason.ToString() + "'.")
                    End Try
                Else 'The Client is already disconnected from the Server
                    Console.WriteLine("ERROR : Client was already disconnected before it could send '" + reason.ToString() + "' to the Server.")
                End If
            Catch ex As Exception
                Console.WriteLine("ERROR : There was an error while trying to send '" + reason.ToString() +
                                  "' to the Server." + Environment.NewLine + ex.ToString())
            End Try
        End If
        LoopComms = False 'Stop Looping the communications
        If Connected = True Then 'Disconnect the Client
            Console.WriteLine("Client : The Client has disconnected from " + RemoteEndPoint.ToString()) 'Write to the Console
            Disconnect(False) 'Disconnect the socket
            Close() 'Close the socket
            Client_Console.Client = Nothing 'Clear the Client
        End If
        If message <> "" Then 'There is a message to write
            Console.WriteLine(message) 'Write to the Console
        End If
    End Sub

End Class
