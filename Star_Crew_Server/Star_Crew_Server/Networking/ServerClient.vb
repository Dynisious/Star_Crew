Public Class ServerClient
    Inherits Game_Library.Networking.Client
    Public AsyncCompleted As Boolean 'A Boolean value indecating whether the Client is done doing an Async Send/Receive
    Public Craft As Ship 'The Ship that this Client controls
    Private messageToClient() As Byte 'An array of Bytes which represent the message to be sent to this Client
    Private messageReadyToSend As Boolean = False 'A Boolean value indicating whether the ServerClient's message is ready to send
    Private disconnectReason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers = -1 'The reason for the ServerClient to disconnect
    Private disconnectMessage As String = "" 'The message to display while the Server Client disconnects
    Public Disconnecting As Boolean = False 'A Boolean value indicating whether the Client is disconnecting
    Public runClient As Boolean = True 'Keeps the Client open
    Private ClientThread As New System.Threading.Thread(AddressOf Run_Comms) 'A Thread object to run the comms for the Client
    Private _throttleUp As Boolean = False 'The actual value of throttleUp
    Public ReadOnly Property throttleUp As Boolean 'A Boolean value indicating whether the throttle needs to increase
        Get
            Return _throttleUp
        End Get
    End Property
    Private _throttleDown As Boolean = False 'The actual value of throttleDown
    Public ReadOnly Property throttleDown As Boolean 'A Boolean value indicating whether the throttle needs to decrease
        Get
            Return _throttleDown
        End Get
    End Property
    Private _turnRight As Boolean = False 'The actual value of turnRight
    Public ReadOnly Property turnRight As Boolean 'A Boolean value indicating whether the Ship needs to turn right
        Get
            Return _turnRight
        End Get
    End Property
    Private _turnLeft As Boolean = False 'The actual value of turnLeft
    Public ReadOnly Property turnLeft As Boolean 'A Boolean value indicating whether the ship needs to turn left
        Get
            Return _turnLeft
        End Get
    End Property
    Private _fireWeapons As Boolean = False 'The actual value of fireWeapons
    Public ReadOnly Property fireWeapons As Boolean 'A Boolean value indicating whether the Ship needs to fire Weapons
        Get
            Return _fireWeapons
        End Get
    End Property

    Public Sub New(ByRef nSocket As System.Net.Sockets.Socket, ByVal index As Integer, ByRef successfulConnection As Boolean)
        MyBase.New(nSocket, 300, 300, "Unnamed Client", index)
        Console.WriteLine(Environment.NewLine + "Server : Connecting a new Client at '" + RemoteEndPoint.ToString() + "'...")
        successfulConnection = False 'The connection is not yet successful
        Try
            Name = Text.ASCIIEncoding.ASCII.GetString(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Get the name of the Ship
            successfulConnection = True 'The connection was successful
            ClientThread.Start()
            Craft = New PlayerShip(Me)
            Server.Combat.adding.Add(Craft)
            Console.WriteLine("Server : The '" + Name + "' has been connected.")
        Catch ex As Net.Sockets.SocketException
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while connecting a new Client." +
                                      Environment.NewLine + ex.ToString())
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while connecting a new Client. Communications will now close." +
                                      Environment.NewLine + ex.ToString())
        End Try
        If successfulConnection = False Then Close() 'Close the socket
    End Sub

    Private Sub Run_Comms() 'Handles the receiving and sending of messages for the ServerClient
        Dim emptyIterations As Integer 'An Integer value counting the number of iterations passed without a response
        Dim iterationThreshold As Integer = 1000 'An Integer value counting the number of iterations allowed to pass before exiting

        Try
            Do Until runClient = False Or Connected = False
                If emptyIterations = iterationThreshold Then 'Close the comms
                    If disconnectMessage <> "" Then 'Add another reason to the list
                        disconnectMessage = ("The ServerClient passed the iteration threshold without processing, " + disconnectMessage)
                    Else 'Set the reason
                        disconnectMessage = ("The ServerClient passed the iteration threshold without processing.")
                    End If
                End If
                emptyIterations += 1 'Add one to the iteration count

                '-----Send Message or Disconnect-----
                If Disconnecting = True Then 'The ServerClient is disconnecting
                    '-----Send a reason to the Client if needed and close the loop-----
                    If disconnectReason <> -1 Then
                        Try
                            Send(BitConverter.GetBytes(disconnectReason), Net.Sockets.SocketFlags.None) 'Send the reason for the disconnect to the Client
                        Catch ex As Net.Sockets.SocketException
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to send the disconnect reason to the '" +
                                                      Name + "'." + Environment.NewLine + ex.ToString())
                        Catch ex As Exception
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while trying to send the disconnect reason to the '" +
                                                      Name + "'. The Server will now close." + Environment.NewLine + ex.ToString())
                            End
                        End Try
                    End If
                    runClient = False
                    '------------------------------------------------------------------
                ElseIf messageReadyToSend = True Then 'There is a message ready to send
                    emptyIterations = 0
                    '-----Send the message to the Client-----
                    Try
                        Send(BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship), Net.Sockets.SocketFlags.None) 'Send the message header to the Client
                        Send(BitConverter.GetBytes(messageToClient.Length), Net.Sockets.SocketFlags.None) 'Send the message length to the Client
                        Send(messageToClient, Net.Sockets.SocketFlags.None) 'Send the message to the Client
                        messageReadyToSend = False 'Let the game know this ServerClient is ready to generate a new message

                        '-----Receive from the Client-----
                        Try
                            If BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) Then 'The Client is going to send a message
                                Dim messageHeader As Integer = Receive_Header(Net.Sockets.SocketFlags.None) 'Receive the message header from the Client
                                If messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception Then 'The Client received a bad message from the Server
                                    Server.Comms.closing.Add(Me)
                                    disconnectMessage = ("The '" + Name + "' received a bad message from the ServerClient")
                                ElseIf messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Weapons Then 'The Client is changing Fire_Weapons
                                    _fireWeapons = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Receive the Boolean
                                ElseIf messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down Then 'The Client is changing Throttle_Down
                                    _throttleDown = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Receive the Boolean
                                ElseIf messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up Then 'The Client is changing Throttle_Up
                                    _throttleUp = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Receive the Boolean
                                ElseIf messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left Then 'The Client is changing Turn_Left
                                    _turnLeft = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Receive the Boolean
                                ElseIf messageHeader = Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right Then 'The Client is changing Turn_Right
                                    _turnRight = BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) 'Receive the Boolean
                                Else 'The Client sent an unknown message header
                                    Server.Comms.closing.Add(Me)
                                    disconnectReason = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                                    disconnectMessage = ("The '" + Name + "' sent a bad message header to the Server")
                                End If
                            End If
                        Catch ex As Game_Library.Networking.Client.ReceiveException
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to receive a message from the '" + Name +
                                                      "'. The ServerClient will now disconnect." + Environment.NewLine + ex.ToString())
                            disconnectMessage = ("The ServerClient encountered an error while receiving from the '" + Name + "'")
                            Server.Comms.closing.Add(Me)
                        Catch ex As Net.Sockets.SocketException
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to receive a message from the '" + Name +
                                                      "'. The ServerClient will now disconnect." + Environment.NewLine + ex.ToString())
                            disconnectMessage = ("The ServerClient encountered an error while receiving from the '" + Name + "'")
                            Server.Comms.closing.Add(Me)
                        Catch ex As Exception
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while trying to receive a message from the '" +
                                                      Name + "'. The Server will now close." + Environment.NewLine + ex.ToString())
                            End
                        End Try
                        '---------------------------------
                    Catch ex As Net.Sockets.SocketException
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to send the message to the '" + Name +
                                                  "'. The ServerClient will now disconnect." + Environment.NewLine + ex.ToString())
                        disconnectMessage = ("The ServerClient encountered an error while sending to the '" + Name + "'")
                        Server.Comms.closing.Add(Me)
                    Catch ex As Exception
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled error while trying to send the message to the '" +
                                                  Name + "'. The Server will now close." + Environment.NewLine + ex.ToString())
                        End
                    End Try
                    '----------------------------------------
                End If
                '------------------------------------

                System.Threading.Thread.Sleep(10) 'Wait for 10 milliseconds
            Loop

            Finalise_Client()
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while executing the comms for the '" + Name +
                                      "'. Server will now close." + Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

    Private Sub Finalise_Client() 'Disconnects the ServerClient from the Client and finalises the ServerClient
        Craft.Dead = True 'Destroy the Craft
        Console.WriteLine(Environment.NewLine + "Server : Disconnecting the '" + Server.Comms.clientList(Index).Name + "'" +
                          If((disconnectMessage <> ""), (" because " + disconnectMessage), "") + ".")
        Dim endPoint As String = Server.Comms.clientList(Index).RemoteEndPoint.ToString() 'Gets a string representing the remote end point of the Socket
        If Index <> Server.Comms.clientList.Count Then 'There are Client's who's index has changed
            For i As Integer = Index To Server.Comms.clientList.Count - 1 'Loop through those Clients
                Server.Comms.clientList(i).Index = i 'Set the new index
            Next
        End If
        Index = -1 'Clear the index
        If disconnectReason <> -1 Then Disconnect(False) 'Disconnect the socket
        Close() 'Close the socket
        Disconnecting = False
        Console.WriteLine("Server : '" + endPoint + "' was disconnected.") 'Write to the Console
    End Sub

    Public Sub Generate_Message(ByVal objects As List(Of Ship)) 'Creates a message for the ServerClient to send
        If Craft.Dead = False Then
            If messageReadyToSend = False And Craft.CombatIndex <> -1 Then 'The craft is still alive and there is no message pending
                Dim positions(objects.Count - 1) As System.Drawing.Point 'An array of point objects to be sent to the Client
                Dim directions(objects.Count - 1) As Double 'An array of double values to be sent to the Client
                Dim allegiances(objects.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'An array of alleiances values to be sent to the Client
                Dim types(objects.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'An array of ObjectType values to be sent to the Client
                Dim hits(objects.Count - 1) As Boolean 'An array of Boolean values to be sent to the Client
                For i As Integer = 0 To objects.Count - 1 'Loop through all Indexs
                    positions(i) = New System.Drawing.Point((objects(i).X - Craft.X), (objects(i).Y - Craft.Y)) 'Create a point object relative to the object
                    directions(i) = objects(i).Direction 'Set the object's direction
                    allegiances(i) = -1 'Set the object's allegiance
                    types(i) = objects(i).Type
                    hits(i) = objects(i).hit 'Set the object's hit state
                Next
                allegiances(Craft.CombatIndex) = 1 'Set the craft's allegiance
                messageToClient = Game_Library.Serialisation.ToBytes({positions, directions, allegiances, types, hits, Craft.Throttle.Current,
                                                                      Craft.Throttle.Maximum, Craft.CombatIndex, Craft.firing, Craft.Hull.Current,
                                                                      Craft.Hull.Maximum, Craft.Primary.Ammunition.Current,
                                                                      Craft.Primary.Ammunition.Maximum}) 'Convert the message into Bytes
                messageReadyToSend = True
            End If
        ElseIf Disconnecting = False Then 'The craft is dead and the Client needs to disconnect
            Server.Comms.closing.Add(Me) 'Add this ServerClient to the list of closing clients
            disconnectReason = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception 'Set the disconnect reason for the message
            disconnectMessage = ("The Client died and was therefore kicked from the server") 'Set the disconnect message for the Client
        End If
    End Sub

End Class
