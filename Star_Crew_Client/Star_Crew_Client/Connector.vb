Imports System.Drawing
Public Class Connector 'The object used to connect to and communicate with a Server
    Inherits Game_Library.Networking.Client
    Private ReadOnly displayBoxSideLength As Integer = My.Resources.NormalSpace.Height 'The length of one side of the display box
    Private ReadOnly displayBoxCenter As Integer = displayBoxSideLength / 2 'The coord of the center of the display box
    Private ReadOnly displayBoxMargin As Integer = (Client_Console.OutputScreen.Height - displayBoxSideLength) / 2 'The margin from the left and top sides of the display form that displayBox is located
    Private receivedElements() As Object 'An Array of objects to store the received message elements
    Private waitingForMessage As Boolean = True 'A Boolean value indecating whether the Client is waiting for an updated message
    Public sendBuff() As Byte 'An Array of Bytes to store the message to be sent
    Public messageToSend As Boolean = False 'A Boolean value indicating whether the Client is waiting to send a message
    Private WithEvents ticker As New Timers.Timer With {.Interval = 100, .AutoReset = True, .Enabled = True} 'Create a timer object to update the Screen
    Private disconnectReason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers = -1
    Private disconnectMessage As String = ""
    Public Disconnecting As Boolean = False
    Public loopComms As Boolean = True
    Private Stars() As Point = {New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength)),
                                New Point(Int(Rnd() * displayBoxSideLength), Int(Rnd() * displayBoxSideLength))}

    Public Sub New(ByVal nIP As String, ByVal nPort As Integer, ByVal hosting As Boolean)
        MyBase.New(nIP, nPort, Client_Console.settingElements(0)) 'Attempts to connect to a Server and Set's the name etc of the Client
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(Name) 'Get an array of Bytes to represent the Client's name
        Send(BitConverter.GetBytes(buff.Length), Net.Sockets.SocketFlags.None) 'Send an Integer representing the number of Bytes to be received
        Send(buff, Net.Sockets.SocketFlags.None) 'Send the array of Bytes representing the Clients name
        Client_Console.CommsThread = New System.Threading.Thread(AddressOf Run_Comms)
        Client_Console.CommsThread.Start()
        Console.WriteLine(Environment.NewLine + "Client : Connected to {0}:{1}", nIP, nPort)
        Screen.GameScreen.Layout(Client_Console.OutputScreen, hosting)
    End Sub

    Public Sub Run_Comms() 'Handles the sending and receiving of messages
        Do Until loopComms = False Or Connected = False
            Try
                If Available <> 0 Then 'There is data to receive
                    '-----Receive Message-----
                    Try
                        Select Case Receive_Header(Net.Sockets.SocketFlags.None)
                            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                                disconnectMessage = "The Server received a bad message from the Client"
                                Disconnecting = True
                            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception
                                disconnectMessage = "The Server Kicked the Client"
                                Disconnecting = True
                            Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception
                                disconnectMessage = "The Server Closed"
                                Disconnecting = True
                            Case Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship
                                If waitingForMessage = True Then
                                    receivedElements = Game_Library.Serialisation.FromBytes(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message elements
                                    waitingForMessage = False 'The message has been received
                                Else
                                    Receive_ByteArray(Net.Sockets.SocketFlags.None) 'Receive the data to clear the buffer
                                End If
                        End Select
                    Catch ex As Net.Sockets.SocketException
                        Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to receive a message from the Server. The Client will now disconnect." +
                                                          Environment.NewLine + ex.ToString())
                        Disconnecting = True
                    Catch ex As Exception
                        Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while trying to receive from the Server. The Client will now close." +
                                                          Environment.NewLine + ex.ToString())
                        Client_Console.Close_Client()
                    End Try
                    '-------------------------

                    '-----Send Message-----
                    If Disconnecting = True Then 'Disconnect Client
                        loopComms = False 'Close the comms
                        If disconnectReason <> -1 Then 'There is a cause to be sent
                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send true to the Server
                            If Connected = True Then 'The Client is still connected
                                Try
                                    Send(BitConverter.GetBytes(disconnectReason), Net.Sockets.SocketFlags.None) 'Send the reason for the disconnect to the Client
                                Catch ex As Net.Sockets.SocketException
                                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error trying to send to the Server '" +
                                                                      disconnectReason.ToString() + "'." + Environment.NewLine + ex.ToString())
                                Catch ex As Exception
                                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while trying to send to the Server '" +
                                                                      disconnectReason.ToString() + "'. The Client will now close" + Environment.NewLine + ex.ToString())
                                End Try
                            Else 'The Client is disconnected
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : The Server disconnected before the Client could send '" +
                                                                  disconnectReason.ToString() + "' to the Server.")
                            End If
                        Else
                            Send(BitConverter.GetBytes(False), Net.Sockets.SocketFlags.None) 'Send false to the Server
                        End If
                    ElseIf messageToSend = True Then 'The Client is ready to send a message
                        Try
                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send true to the Server
                            Send(BitConverter.GetBytes(sendBuff.Length), Net.Sockets.SocketFlags.None) 'Send the number of Bytes to be received to the Server
                            Send(sendBuff, Net.Sockets.SocketFlags.None) 'Send the message to the Server
                        Catch ex As Net.Sockets.SocketException
                            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to send a message to the Server. The Client will now disconnect." +
                                                              Environment.NewLine + ex.ToString())
                            Disconnecting = True
                        Catch ex As Exception
                            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while trying to send a message to the Server. The Client will now close." +
                                                              Environment.NewLine + ex.ToString())
                            Disconnecting = True
                        End Try
                        messageToSend = False
                    Else 'No message is to be sent
                        Send(BitConverter.GetBytes(False), Net.Sockets.SocketFlags.None) 'Send false to the Server
                    End If
                    '----------------------
                End If
            Catch ex As Net.Sockets.SocketException
                Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while executing the comms for the Client. They will now disconnect." +
                                                  Environment.NewLine + ex.ToString())
                Disconnecting = True
            End Try
        Loop

        Finalise_Client()
    End Sub

    Private Delegate Sub Cross_Thread_Text_Setting(ByVal text As String)
    Private Sub Display_Ship_To_Ship() Handles ticker.Elapsed 'Used to display Ship to Ship combat
        If waitingForMessage = False Then 'Check that the message is up to date
            Dim positions() As Point = receivedElements(0) 'Get the Array of Point objects representing the objects' locations
            Dim directions() As Double = receivedElements(1) 'Get the Array of Double values representing the objects' directions
            Dim allegiances() As Star_Crew_Shared_Libraries.Shared_Values.Allegiances = receivedElements(2) 'Get the Array of Allegiances values representing the objects' allegiances
            Dim clientSpeed As Double = receivedElements(3) 'Get the Speed of the Client
            Dim clientIndex As Integer = receivedElements(4) 'Get the index of the Client
            Dim clientRange As Integer = receivedElements(5) 'Get the Client's Weapon range
            Dim clientFiring As Boolean = receivedElements(6) 'Get the Client's firing state
            Dim clientHull As New System.Drawing.Point(receivedElements(7), receivedElements(8)) 'Get the Client's hull
            waitingForMessage = True 'The message can now be updated

            '-----Render Objects-----
            Dim d As New Cross_Thread_Text_Setting(AddressOf Screen.GameScreen.lblHull_Set_Text) 'Create a delegate
            Screen.GameScreen.lblHull.Invoke(d, {"HULL: " + CStr(clientHull.X) + "/" + CStr(clientHull.Y)}) 'Write the text to lblHull
            Dim img As Bitmap = My.Resources.NormalSpace.Clone() 'Create a Bitmap to be drawn on
            Dim imgG As Drawing.Graphics = Graphics.FromImage(img) 'Create a graphics object from img
            For i As Integer = 0 To UBound(Stars) 'Loop through every star
                Stars(i).X += (Math.Cos(directions(clientIndex) + Math.PI) * clientSpeed) 'Move the star
                If Stars(i).X < 0 Then
                    Stars(i).X = displayBoxSideLength
                ElseIf Stars(i).X > displayBoxSideLength Then
                    Stars(i).X = 0
                End If

                Stars(i).Y += (Math.Sin(directions(clientIndex) + Math.PI) * clientSpeed) 'Move the star
                If Stars(i).Y < 0 Then
                    Stars(i).Y = displayBoxSideLength
                ElseIf Stars(i).Y > displayBoxSideLength Then
                    Stars(i).Y = 0
                End If

                imgG.FillEllipse(Brushes.White, Stars(i).X, Stars(i).Y, 7, 7)
            Next
            imgG.DrawLine(If((clientFiring = True), Pens.Red, Pens.Yellow), displayBoxCenter, displayBoxCenter,
                          CInt(displayBoxCenter + (Math.Cos(directions(clientIndex)) * clientRange)),
                          CInt(displayBoxCenter + (Math.Sin(directions(clientIndex)) * clientRange))) 'Draw the Weapon Line
            For i As Integer = 0 To positions.Length - 1 'Loop through all values
                Dim distance As Integer = Math.Sqrt((positions(i).X ^ 2) + (positions(i).Y ^ 2)) 'Get the distance of the object from the Client's craft
                If distance > 200 Then 'Draw them on the edge of the circle
                    Dim scale As Double = 200 / distance 'Calculate the scale of the distance against 200
                    positions(i).X = (scale * positions(i).X) + displayBoxCenter 'Calculate the scaled x coord
                    positions(i).Y = (scale * positions(i).Y) + displayBoxCenter 'Calculate the scaled y coord
                    imgG.FillEllipse(Drawing.Brushes.Red, New Rectangle(New Point((positions(i).X - 5), (positions(i).Y - 5)),
                                                                     New Size(10, 10))) 'Draw a circle 200 pixels away from the center of the object
                Else 'Draw the actual image of the object
                    Dim craftBmp As New Bitmap(CType(If((allegiances(i) = 1), My.Resources.FriendlyScreamer.Clone(),
                                                        My.Resources.FriendlyFleet.Clone()), Image)) 'Create a Bitmap to draw
                    craftBmp.MakeTransparent() 'Clear the white space from the image
                    Dim crossLength As Integer = Math.Sqrt((craftBmp.Width ^ 2) + (craftBmp.Height ^ 2)) 'Calculate the cross length of the bitmap to draw
                    Dim hlfCrossLength As Integer = crossLength / 2 'Get half of crossLength
                    Dim bmp As New Bitmap(crossLength, crossLength) 'Create a bitmap to draw on that will always fit the image to draw
                    Dim bmpG As Graphics = Graphics.FromImage(bmp) 'Create a graphics object from bmp
                    bmpG.TranslateTransform(hlfCrossLength, hlfCrossLength) 'Move the center of the image onto the pivot
                    bmpG.RotateTransform(180 * directions(i) / Math.PI) 'Rotate the image
                    bmpG.TranslateTransform(-hlfCrossLength, -hlfCrossLength) 'Move the center of the image back into the center of the box
                    bmpG.DrawImage(craftBmp, CInt((crossLength - craftBmp.Width) / 2), CInt((crossLength - craftBmp.Height) / 2))
                    imgG.DrawImage(bmp, New Point((positions(i).X + displayBoxCenter - (crossLength / 2)),
                                                  (positions(i).Y + displayBoxCenter - (crossLength / 2)))) 'Draw the image onto img
                End If
            Next
            imgG.DrawRectangle(New Pen(Brushes.White, 2), 0, 0, displayBoxSideLength, displayBoxSideLength) 'Draw a border
            Client_Console.OutputScreen.CreateGraphics.DrawImage(img, New Point(displayBoxMargin, displayBoxMargin)) 'Draw the image onto the Screen
            '------------------------

            waitingForMessage = True
        End If
    End Sub

    Public Sub Disconnect_Client(ByVal reason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers, ByVal message As String)
        disconnectReason = reason
        disconnectMessage = message
        Disconnecting = True
        While Disconnecting = True 'Loop
        End While
    End Sub

    Private Sub Finalise_Client() 'Disconnects the Client from a Server
        Console.WriteLine(Environment.NewLine + "Client : Disconnecting from the '" + Name + "'" +
                          If((disconnectMessage <> ""), (" because " + disconnectMessage), "") + ".")
        messageToSend = False
        If Connected = True Then 'Disconnect
            Dim endPoint As String = RemoteEndPoint.ToString() 'Gets a string representing the remote end point of the Socket
            If disconnectReason <> -1 Then Disconnect(False) 'Disconnect the socket
            Close() 'Close the socket
            Client_Console.Client = Nothing 'Remove the Client
            Console.WriteLine("Client : " + endPoint + " was disconnected from the '" + Name + "'.") 'Write to the Console
        End If
        Client_Console.Client = Nothing 'Clear Client
        Client_Console.CommsThread = Nothing 'Clear the Thead object
        Disconnecting = False
    End Sub

End Class
