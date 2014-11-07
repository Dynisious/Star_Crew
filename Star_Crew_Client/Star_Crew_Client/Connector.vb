Imports System.Drawing
Public Class Connector 'The object used to connect to and communicate with a Server
    Inherits Game_Library.Networking.Client
    Private ReadOnly displayBoxSideLength As Integer = My.Resources.NormalSpace.Height 'The length of one side of the display box
    Private ReadOnly displayBoxCenter As Integer = displayBoxSideLength / 2 'The coord of the center of the display box
    Private ReadOnly displayBoxMargin As Integer = (Client_Console.OutputScreen.Height - displayBoxSideLength) / 2 'The margin from the left and top sides of the display form that displayBox is located
    Private receivedElements() As Object 'An Array of objects to store the received message elements
    Private waitingForMessage As Boolean = True 'A Boolean value indecating whether the Client is waiting for an updated message
    Public sendHeader As Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header 'A Header to send to the Server specifying what control is being changed
    Public sendBoolean As Boolean 'A Boolean value to send to the Server specifying the state of the control
    Public messageToSend As Boolean = False 'A Boolean value indicating whether the Client is waiting to send a message
    Private WithEvents ticker As New Timers.Timer With {.Interval = 25, .AutoReset = True, .Enabled = True} 'Create a timer object to update the Screen
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
        MyBase.New(nIP, nPort, 300, 300, Client_Console.settingElements(0)) 'Attempts to connect to a Server and Set's the name etc of the Client
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(Name) 'Get an array of Bytes to represent the Client's name
        Send(BitConverter.GetBytes(buff.Length), Net.Sockets.SocketFlags.None) 'Send an Integer representing the number of Bytes to be received
        Send(buff, Net.Sockets.SocketFlags.None) 'Send the array of Bytes representing the Clients name
        Client_Console.CommsThread = New System.Threading.Thread(AddressOf Run_Comms)
        Client_Console.CommsThread.Start()
        Console.WriteLine(Environment.NewLine + "Client : Connected to {0}:{1}", nIP, nPort)
        Screen.GameScreen.Layout(Client_Console.OutputScreen, hosting)
    End Sub

    Public Sub Run_Comms() 'Handles the sending and receiving of messages
        Try
            Dim emptyIterations As Integer 'An Integer value that counts the iterations without a message from the client
            Dim iterationThreshold As Integer = 1000 'An Integer value that defines how many iterations can pass with no data before disconnect
            Do Until loopComms = False Or Connected = False
                If emptyIterations = iterationThreshold Then 'The Client has been waiting too long
                    loopComms = False 'Close the comms
                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : The Client passed the iteration threshold without data being received.")
                    If disconnectMessage <> "" Then 'Add another reason to the list
                        disconnectMessage = ("The Client passed the iteration threshold without data being received, " + disconnectMessage)
                    Else 'Set the reason
                        disconnectMessage = ("The Client passed the iteration threshold without data being received")
                    End If
                End If
                emptyIterations += 1 'Add one iteration

                '-----Receive Message from Server-----
                Try
                    If Available <> 0 Then 'There is data to receive
                        emptyIterations = 0 'Clear the empty iterations
                        Dim receivedHeader As Integer = Receive_Header(Net.Sockets.SocketFlags.None) 'Receive a message header from the Server
                        If receivedHeader = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception Then 'The Server received a bad message from the Client
                            Disconnecting = True
                            disconnectMessage = "The Server received a bad message from the Client"
                        ElseIf receivedHeader = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception Then 'The Client has been kicked from the Server
                            Disconnecting = True
                            disconnectMessage = "The Server kicked the Client"
                        ElseIf receivedHeader = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception Then 'The Server Closed
                            Disconnecting = True
                            Disconnecting = "The Server closed while the Client was still connected"
                        ElseIf receivedHeader = Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship Then 'The Server is sending a Ship_To_Ship message
                            If waitingForMessage = True Then 'The Client is waiting for a new message from the Client
                                receivedElements = Game_Library.Serialisation.FromBytes(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message
                                waitingForMessage = False
                            Else 'Clear the receive buffer
                                Receive_ByteArray(Net.Sockets.SocketFlags.None) 'Clear the buffer by receiving the message
                            End If

                            '-----Disconnect or Send a message back if necessary-----
                            Try
                                If Disconnecting = True Then 'Disconnect
                                    loopComms = False
                                    '-----Send the disconnect reason to the Server if there is one-----
                                    If disconnectReason <> -1 Then 'There's a reason to send
                                        Try
                                            Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send true to the Server
                                            Send(BitConverter.GetBytes(disconnectReason), Net.Sockets.SocketFlags.None) 'Send the disconnect reason to the Server
                                        Catch ex As Net.Sockets.SocketException
                                            Client_Console.Write_To_Error_Log(Environment.NewLine + "There was an error while sending the disconnect reason to the Server." +
                                                                              Environment.NewLine + ex.ToString())
                                        Catch ex As Exception
                                            Client_Console.Write_To_Error_Log(Environment.NewLine + "There was an unexpected and unhandled error while sending the disconnect reason to the Server. Client will now close." +
                                                                              Environment.NewLine + ex.ToString())
                                            End
                                        End Try
                                    Else
                                        Send(BitConverter.GetBytes(False), Net.Sockets.SocketFlags.None) 'Send false to the Server
                                    End If
                                    '------------------------------------------------------------------
                                ElseIf messageToSend = True Then 'There is a message pending to send
                                    Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Send true to the Server
                                    Send(BitConverter.GetBytes(sendHeader), Net.Sockets.SocketFlags.None) 'Send the message header to the Server
                                    Send(BitConverter.GetBytes(sendBoolean), Net.Sockets.SocketFlags.None) 'Send the boolean to the Server
                                    messageToSend = False
                                Else 'There is nothing to send
                                    Send(BitConverter.GetBytes(False), Net.Sockets.SocketFlags.None) 'Send false to the Server
                                End If
                            Catch ex As Net.Sockets.SocketException
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while sending a message to the Server. Client will now disconnect." +
                                                                  Environment.NewLine + ex.ToString())
                                Disconnecting = True
                                disconnectMessage = "The Client encountered an error while sending to the Server"
                            Catch ex As Exception
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while sending a message to the Server. Client will now close." +
                                                                  Environment.NewLine + ex.ToString())
                                End
                            End Try
                            '--------------------------------------------------------
                        Else 'The Client received an unknown header from the Server
                            Disconnecting = True
                            disconnectReason = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                            disconnectMessage = "The Server sent a bad message header"
                        End If
                    End If
                Catch ex As Net.Sockets.SocketException
                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while receiving a message from the Server. Client will now disconnect." +
                                                      Environment.NewLine + ex.ToString())
                    disconnectMessage = If((disconnectMessage = ""),
                                           ("There was an error while receiving a message from the Server, " + disconnectMessage),
                                           "There was an error while receiving a message from the Server")
                Catch ex As Exception
                    Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while receiving a message from the Server. Client will now close." +
                                                      Environment.NewLine + ex.ToString())
                    End
                End Try
                '-------------------------------------

                System.Threading.Thread.Sleep(10) 'Wait for 10 milliseconds
            Loop

            Finalise_Client()
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while executing the Client's comms. Client will now close." +
                                              Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private Delegate Sub Cross_Thread_Text_Setting(ByVal text As String)
    Private Sub Display_Ship_To_Ship() Handles ticker.Elapsed 'Used to display Ship to Ship combat
        Try
            If waitingForMessage = False Then 'Check that the message is up to date
                Dim positions() As Point = receivedElements(0) 'Get the Array of Point objects representing the objects' locations
                Dim directions() As Double = receivedElements(1) 'Get the Array of Double values representing the objects' directions
                Dim allegiances() As Star_Crew_Shared_Libraries.Shared_Values.Allegiances = receivedElements(2) 'Get the Array of Allegiances values representing the objects' allegiances
                Dim types() As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes = receivedElements(3) 'Get the array of ObjectTypes values representing the objects' types
                Dim hits() As Boolean = receivedElements(4) 'Get the Array of Boolean values representing the objects' hit states
                Dim clientSpeed As Double = receivedElements(5) 'Get the Speed of the Client
                Dim clientIndex As Integer = receivedElements(6) 'Get the index of the Client
                Dim clientFiring As Boolean = receivedElements(7) 'Get the Client's firing state
                Dim clientHull As New Point(receivedElements(8), receivedElements(9)) 'Get the Client's hull
                Dim clientAmmunition As New Point(receivedElements(10), receivedElements(11)) 'Get the Client's ammunition
                waitingForMessage = True 'The message can now be updated

                '-----Render Objects-----
                Dim d As New Cross_Thread_Text_Setting(AddressOf Screen.GameScreen.lblHull_Set_Text) 'Create a delegate
                Screen.GameScreen.lblHull.Invoke(d, {"HULL: " + (clientHull.X).ToString() + "/" + (clientHull.Y).ToString()}) 'Write the text to lblHull
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
                For i As Integer = 0 To positions.Length - 1 'Loop through all values
                    Dim distance As Integer = Math.Sqrt((positions(i).X ^ 2) + (positions(i).Y ^ 2)) 'Get the distance of the object from the Client's craft
                    If distance > 200 And types(i) <> Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile Then 'Draw them on the edge of the circle
                        Dim scale As Double = 200 / distance 'Calculate the scale of the distance against 200
                        positions(i).X = (scale * positions(i).X) + displayBoxCenter 'Calculate the scaled x coord
                        positions(i).Y = (scale * positions(i).Y) + displayBoxCenter 'Calculate the scaled y coord
                        imgG.FillEllipse(Drawing.Brushes.Red, New Rectangle(New Point((positions(i).X - 5), (positions(i).Y - 5)),
                                                                         New Size(10, 10))) 'Draw a circle 200 pixels away from the center of the object
                    Else 'Draw the actual image of the object
                        Select Case types(i) 'Choose the type to draw
                            Case Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile
                                If Math.Sqrt((positions(i).X ^ 2) + (positions(i).Y ^ 2)) < 200 Then 'Render projectile
                                    Dim xCoord As Integer = (displayBoxCenter + positions(i).X) 'The x coord of the projectile on the screen
                                    Dim yCoord As Integer = (displayBoxCenter + positions(i).Y) 'The y coord of the projectile on the screen
                                    imgG.DrawLine(Pens.Yellow, xCoord, yCoord,
                                                  CInt(xCoord + (5 * Math.Cos(directions(i)))),
                                                  CInt(yCoord + (5 * Math.Sin(directions(i))))) 'Draw the projectile
                                End If
                            Case Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Ship
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
                                If hits(i) Then bmpG.FillEllipse(Brushes.Orange, CInt(Int(Rnd() * craftBmp.Width) - 5 - (craftBmp.Width / 2) + hlfCrossLength),
                                    CInt(Int(Rnd() * craftBmp.Height) - 5 - (craftBmp.Height / 2) + hlfCrossLength), 10, 10) 'Draw an ellipse for the hit
                                imgG.DrawImage(bmp, New Point((positions(i).X + displayBoxCenter - hlfCrossLength),
                                                              (positions(i).Y + displayBoxCenter - hlfCrossLength))) 'Draw the image onto img
                        End Select
                    End If
                Next
                imgG.DrawRectangle(New Pen(Brushes.White, 2), 0, 0, displayBoxSideLength, displayBoxSideLength) 'Draw a border
                Client_Console.OutputScreen.CreateGraphics.DrawImage(img, New Point(displayBoxMargin, displayBoxMargin)) 'Draw the image onto the Screen
                '------------------------

                waitingForMessage = True
            End If
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while drawing the Ship_To_Ship environment. Client will now close." +
                                              Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Public Sub Disconnect_Client(ByVal reason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers, ByVal message As String)
        disconnectReason = reason
        disconnectMessage = message
        Disconnecting = True
        While Disconnecting = True
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
