Imports System.Drawing
Public Class Connector 'The object used to connect to and communicate with a Server
    Inherits Game_Library.Networking.Client
    Private ReadOnly displayBoxSideLength As Integer = My.Resources.NormalSpace.Height 'The length of one side of the display box
    Private ReadOnly displayBoxCenter As Integer = displayBoxSideLength / 2 'The coord of the center of the display box
    Private ReadOnly displayBoxMargin As Integer = (Client_Console.OutputScreen.Height - displayBoxSideLength) / 2 'The margin from the left and top sides of the display form that displayBox is located
    Private receivedElements() As Object 'An Array of objects to store the received message elements
    Private waitingForMessage As Boolean = True 'A Boolean value indicating whether the Client is waiting for a new message
    Public receivingAlive As Boolean = True 'Keeps the receiving thread open
    Public sendingAlive As Boolean = True 'Keeps the sending thread open
    Private receiveThread As System.Threading.Thread 'A Thread object used to receive messages from the Client
    Private sendThread As System.Threading.Thread 'A Thread object used to send messages to the Client
    Private sendingSemaphore As System.Threading.Semaphore 'A Semaphore object used to block the sendThread until data is ready
    Private accessSendList As System.Threading.Mutex 'A Mutex object used to synchronise access to SendList
    Private sendList As New List(Of Byte()) 'A List object of Bytes to be sent
    Private errorList As New List(Of String) 'A List of strings that represent the error messages for the messages to be sent
    Private waitToClose As System.Threading.Mutex 'A Semaphore object used to block the receiveThread from closing until the SendThread is closed
    Private disconnecting As Boolean = False 'A Boolean value indicating whether the Client should continue sending or close for a disconnect
    Private WithEvents ticker As New Timers.Timer With {.Interval = 25, .AutoReset = True, .Enabled = True} 'Create a timer object to update the Screen
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
    Public values(4) As Boolean 'An array of Boolean values indicating which keys are down and which aren't
    Public Scaler As Double = 1 'Scales what is displayed on the screen
    Private Images() As Bitmap = {My.Resources.FriendlyScreamer, My.Resources.PirateThunder}

    Public Sub New(ByVal nIP As String, ByVal nPort As Integer)
        MyBase.New(nIP, nPort, 3000, -1, Client_Console.settingElements(0)) 'Attempts to connect to a Server and Set's the name etc of the Client
        Dim buff() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(Name) 'Get an array of Bytes to represent the Client's name
        Send(BitConverter.GetBytes(buff.Length), Net.Sockets.SocketFlags.None) 'Send an Integer representing the number of Bytes to be received
        Send(buff, Net.Sockets.SocketFlags.None) 'Send the array of Bytes representing the Clients name
        sendingSemaphore = New System.Threading.Semaphore(0, 1) 'Create a new Semaphore for the sending thread
        accessSendList = New System.Threading.Mutex(False) 'Create a new Mutex for synchronising access to SendList
        waitToClose = New System.Threading.Mutex(False) 'Create a new Mutex for stopping the Socket from closing until all sends are completed
        sendThread = New System.Threading.Thread(AddressOf Sending) 'Create a new Thread for sending messages
        sendThread.Start()
        receiveThread = New System.Threading.Thread(AddressOf Receiving) 'Create a new Thread for receiving messages
        receiveThread.Start() 'Start the thread
        Dim temp As String = (Environment.NewLine + "Client : Connected to " + CStr(nIP) + ":" + CStr(nPort))
        Console.WriteLine(temp) 'Write that the Client has connected and the ping
        Client_Console.Write_To_Error_Log(temp) 'Write that the Client has connected and the ping
        Screen.GameScreen.Layout(Client_Console.OutputScreen)
    End Sub

    Public Sub Send_Message(ByVal messages()() As Byte, ByVal errorMessages() As String)
        Try
            accessSendList.WaitOne()
            If sendList.Count = 0 Then sendingSemaphore.Release() 'Release the semaphore so that the sendThread starts sending
            sendList.AddRange(messages) 'Add the messages to the list
            errorList.AddRange(errorMessages) 'Add the error messages to the List
            accessSendList.ReleaseMutex()
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while calling Send_Message() in the " +
                                      Name + ". Server will now close." + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub
    Private Sub Sending() 'Sends each of the elements in data
        waitToClose.WaitOne() 'Hold the mutex until the Send thread is closed
        Try
            Do
                sendingSemaphore.WaitOne()
                Do
                    accessSendList.WaitOne()
                    Try
                        Send(sendList(0), Net.Sockets.SocketFlags.None) 'Send the message
                        sendList.RemoveAt(0)
                        errorList.RemoveAt(0)
                    Catch ex As Net.Sockets.SocketException
                        Client_Console.Write_To_Error_Log(Environment.NewLine + errorList(0) + Environment.NewLine + ex.ToString())
                        sendingAlive = False
                        receivingAlive = False
                        disconnecting = True
                    Finally
                        accessSendList.ReleaseMutex()
                    End Try
                Loop Until sendList.Count = 0 Or disconnecting 'Loop while there's messages to send
            Loop While sendingAlive 'Loop while the sending is alive
        Catch ex As Net.Sockets.SocketException
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while sending a message to the " +
                                      Name + ". The " + Name + " will now disconnect." + Environment.NewLine + ex.ToString())
            sendingAlive = False
            receivingAlive = False
            disconnecting = True
        Catch ex As Exception
            Client_Console.Write_To_Error_Log("ERROR : There was an unexpected and unhandled error while sending a message to the " + Name + ". Server will now close.")
            End
        Finally
            waitToClose.ReleaseMutex() 'The Send thread is closed
        End Try
    End Sub

    Private Sub Receiving() 'Receives messages
        Dim dropped As Integer
        Dim started As DateTime
        Try
            While receivingAlive 'Loop while comms are open
                If Available <> 0 Then 'Theres data to receive
                    Select Case Receive_Header(Net.Sockets.SocketFlags.None) 'Decide what to do with the received header
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship
                            If waitingForMessage Then
                                receivedElements = Game_Library.Serialisation.FromBytes(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message
                                waitingForMessage = False
                                If dropped <> 0 Then
                                    Dim message As String = ("NETWORKING : " + CStr(dropped) + " messages where dropped between " +
                                                             started.ToString() + " and " + Now.ToString())
                                    Client_Console.Write_To_Error_Log(message)
                                    Console.WriteLine(message)
                                    started = Nothing
                                    dropped = 0
                                End If
                            Else
                                If started = Nothing Then started = Now
                                dropped += 1
                                Receive_ByteArray(Net.Sockets.SocketFlags.None) 'Receive to clear the buffer
                            End If
                        Case Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Client_Lost 'The Client has lost
                            sendingAlive = False
                            receivingAlive = False
                            disconnecting = True
                            Console.WriteLine(Environment.NewLine + "Client : The Client has died and has disconnected.")
                            Dim d As New Screen.Death(AddressOf Screen.DeathScreen.Layout)
                            Client_Console.OutputScreen.Invoke(d, {Client_Console.OutputScreen})
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                            receivingAlive = False 'Close the Client
                            sendingAlive = False 'Close the Client
                            disconnecting = True
                            Client_Console.Write_To_Error_Log(Environment.NewLine + "Client : The Server receieved a bad message from the Client.")
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception
                                receivingAlive = False 'Close the Client
                                sendingAlive = False 'Close the Client
                                disconnecting = True
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "Client : The Server closed before disconnecting from the Client.")
                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception
                                receivingAlive = False 'Close the Client
                                sendingAlive = False 'Close the Client
                                disconnecting = True
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "Client : The the Client was kicked from the Server.")
                        Case Else
                                Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : The Server sent an unknown message header. Client will now disconnect.")
                                Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception)},
                                             {"ERROR : There was an error sending the Bad_Message_Exception to the Server. Client will now close."})
                                sendingAlive = False
                                receivingAlive = False
                    End Select
                End If

                System.Threading.Thread.Sleep(10)
            End While
        Catch ex As Game_Library.Networking.Client.ReceiveException
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while receiving a message from the Server. Client will now disconnect." +
                                              Environment.NewLine + ex.ToString())
            sendingAlive = False
            receivingAlive = False
            disconnecting = True
        Catch ex As Net.Sockets.SocketException
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while receiving a message from the Server. Client will now disconnect." +
                                              Environment.NewLine + ex.ToString())
            sendingAlive = False
            receivingAlive = False
            disconnecting = True
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while receiving a message from the Server. The Client will now close." +
                                              Environment.NewLine + ex.ToString())
            End
        End Try

        Finalise_Client()
    End Sub

    Private Sub Finalise_Client()
        Try
            waitToClose.WaitOne() 'Wait till the send thread is done before closing
            Client_Console.Client = Nothing
            If Client_Console.OutputScreen.Server IsNot Nothing Then
                If Client_Console.OutputScreen.Server.HasExited = False Then Client_Console.OutputScreen.Server.CloseMainWindow() 'Make Sure the Server is not left open
            End If
            Close()
            waitToClose.ReleaseMutex()
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while closing the connection to the Server. The Server will now close." +
                                              Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private Delegate Sub Text_Setting(ByVal text As String)
    Private Sub Display_Ship_To_Ship() Handles ticker.Elapsed 'Used to display Ship to Ship combat
        Try
            If Not waitingForMessage Then 'Check that the message is up to date
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
                Dim targetIndex As Integer = receivedElements(12) 'Get the targeted index
                Dim targetDistance As Integer = receivedElements(13) 'Get the targets range
                Dim shields() As Integer = receivedElements(14) 'Get the Shields
                waitingForMessage = True 'The message can now be updated

                '-----Scale and Translate Positions-----
                Dim xOffset As Integer = (75 * Math.Cos(directions(clientIndex) + Math.PI)) 'Get the x offset of the client
                Dim yOffset As Integer = (75 * Math.Sin(directions(clientIndex) + Math.PI)) 'Get the y offset of the client
                For i As Integer = 0 To UBound(positions) 'Loop through all positions
                    positions(i).X *= Scaler 'Scale the x coord
                    positions(i).Y *= Scaler 'Scale the y coord
                    positions(i).X += xOffset 'Translate the x coord
                    positions(i).Y += yOffset 'Translate the y coord
                Next
                '-----------------------------

                '-----Render Objects-----
                '-----Render Text-----
                Dim d As New Text_Setting(AddressOf Screen.GameScreen.lblHull_Set_Text) 'Create a delegate
                Screen.GameScreen.lblHull.Invoke(d, {"HULL: " + CStr(clientHull.X) + "/" + CStr(clientHull.Y)}) 'Write the text to lblHull
                d = New Text_Setting(AddressOf Screen.GameScreen.lblShield_Set_Text) 'Create a delegate
                Screen.GameScreen.lblHull.Invoke(d, {"SHIELD: " + CStr(shields(clientIndex)) + "% CAPACITY"}) 'Write the text to lblShield
                d = New Text_Setting(AddressOf Screen.GameScreen.lblThrottle_Set_Text) 'Create a delegate
                Screen.GameScreen.lblThrottle.Invoke(d, {"THROTTLE: " + CStr(FormatNumber((2 * clientSpeed), 2)) + "m/sec"}) 'Write the text to lblThrottle
                d = New Text_Setting(AddressOf Screen.GameScreen.lblAmmunition_Set_Text) 'Create a delegate
                Screen.GameScreen.lblAmmunition.Invoke(d, {"AMMUNITION: " + If((clientAmmunition.Y = -1), "INF",
                                                                               ((clientAmmunition.X).ToString() + "/" + (clientAmmunition.Y).ToString()))}) 'Write the text to lblAmmunition
                d = New Text_Setting(AddressOf Screen.GameScreen.lblTargetDistance_Set_Text) 'Create a delegate
                Screen.GameScreen.lblTargetDistance.Invoke(d, {"TARGET DISTANCE: " + If((targetIndex = -1), "N/A", (targetDistance.ToString() + "m"))}) 'Write the text to lblTargetDistance
                '---------------------

                Dim img As Bitmap = My.Resources.NormalSpace.Clone() 'Create a Bitmap to be drawn on
                Dim imgG As Drawing.Graphics = Graphics.FromImage(img) 'Create a graphics object from img

                '-----Draw Stars-----
                For i As Integer = 0 To UBound(Stars) 'Loop through every star
                    Stars(i).X += (Math.Cos(directions(clientIndex) + Math.PI) * clientSpeed * Scaler) 'Move the star
                    If Stars(i).X < 0 Then
                        Stars(i).X = displayBoxSideLength
                    ElseIf Stars(i).X > displayBoxSideLength Then
                        Stars(i).X = 0
                    End If

                    Stars(i).Y += (Math.Sin(directions(clientIndex) + Math.PI) * clientSpeed * Scaler) 'Move the star
                    If Stars(i).Y < 0 Then
                        Stars(i).Y = displayBoxSideLength
                    ElseIf Stars(i).Y > displayBoxSideLength Then
                        Stars(i).Y = 0
                    End If

                    imgG.FillEllipse(Brushes.White, Stars(i).X + xOffset, Stars(i).Y + yOffset, 7, 7) 'Draw Star
                Next
                '--------------------
                '-----Draw Objects-----
                For i As Integer = 0 To positions.Length - 1 'Loop through all values
                    Dim distance As Integer = Math.Sqrt((positions(i).X ^ 2) + (positions(i).Y ^ 2)) 'Get the distance of the object from the Client's craft
                    If distance > 200 And types(i) <> Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile Then 'Draw them on the edge of the circle
                        Dim scale As Double = 200 / distance 'Calculate the scale of the distance against 200
                        positions(i).X = (scale * positions(i).X) + displayBoxCenter 'Calculate the scaled x coord
                        positions(i).Y = (scale * positions(i).Y) + displayBoxCenter 'Calculate the scaled y coord
                        imgG.FillEllipse(If((allegiances(i) = allegiances(clientIndex)),
                                            Brushes.Green,
                                            If((i = targetIndex),
                                               Drawing.Brushes.Blue,
                                               Drawing.Brushes.Red)), New Rectangle(New Point((positions(i).X - 5), (positions(i).Y - 5)),
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
                            Case Else
                                Dim craftBmp As Bitmap = Images(types(i) - Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.ShipsStart).Clone() 'Create a Bitmap to draw
                                craftBmp.MakeTransparent() 'Clear the white space from the image
                                Dim crossLength As Integer = (Scaler * Math.Sqrt((craftBmp.Width ^ 2) + (craftBmp.Height ^ 2))) 'Calculate the cross length of the bitmap to draw
                                Dim hlfCrossLength As Integer = crossLength / 2 'Get half of crossLengthreceiving
                                Dim bmp As New Bitmap(crossLength, crossLength) 'Create a bitmap to draw on that will always fit the image to draw
                                Dim bmpG As Graphics = Graphics.FromImage(bmp) 'Create a graphics object from bmp
                                If targetIndex = i Then bmpG.DrawRectangle(Pens.Blue, 1, 1, crossLength - 2, crossLength - 2)
                                bmpG.DrawArc(New Drawing.Pen(Drawing.Color.FromArgb((shields(i) * 255 / 100), Color.Red), 1), 1, 1, crossLength - 2, crossLength - 2, 0, 360)
                                bmpG.TranslateTransform(hlfCrossLength, hlfCrossLength) 'Move the center of the image onto the pivot
                                bmpG.ScaleTransform(Scaler, Scaler) 'Scale the image
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
                '----------------------
                '-----Draw Images-----
                imgG.DrawRectangle(New Pen(Brushes.White, 2), 0, 0, displayBoxSideLength, displayBoxSideLength) 'Draw a border
                Client_Console.OutputScreen.CreateGraphics.DrawImage(img, New Point(displayBoxMargin, displayBoxMargin)) 'Draw the image onto the Screen
                '---------------------
                '------------------------

                waitingForMessage = True
            End If
        Catch ex As Exception
            Client_Console.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while drawing the Ship_To_Ship environment. Client will now close." +
                                              Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

End Class
