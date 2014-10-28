Public Class ServerClient
    Inherits Game_Library.Networking.Client
    Public AsyncCompleted As Boolean 'A Boolean value indecating whether the Client is done doing an Async Send/Receive
    Public Craft As Ship 'The Ship that this Client controls
    Private messageToClient() As Byte 'An array of Bytes which represent the message to be sent to this Client
    Private messageReadyToSend As Boolean = False 'A Boolean value indicating whether the ServerClient's message is ready to send
    Public clientComms As Boolean = True 'A Boolean value that keeps the comms running as long as it is true
    Private ClientThread As New System.Threading.Thread(AddressOf Run_Comms)
    Private disconnectReason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers = -1 'The reason for the ServerClient to disconnect
    Private disconnectMessage As String = "" 'The message to display while the Server Client disconnects
    Private _Disconnecting As Boolean = False 'A Boolean value indicating whether the Client is disconnecting
    Public ReadOnly Property Disconnecting As Boolean
        Get
            Return _Disconnecting
        End Get
    End Property
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
        MyBase.New(nSocket, "Unnamed Client", index)
        Console.WriteLine(Environment.NewLine + "Server : Connecting a new Client at '" + RemoteEndPoint.ToString() + "'...")
        successfulConnection = False 'The connection is not yet successful
        Try
            Name = Text.ASCIIEncoding.ASCII.GetString(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Get the name of the Ship
            successfulConnection = True 'The connection was successful
            Craft = New PlayerShip(Me)
            Server.Combat.Add_Ship(Craft)
            ClientThread.Start()
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
        Do Until clientComms = False Or Connected = False 'Loop until the comms closes
            '-----Send a message to the Client-----
            If Disconnecting = False Then 'Proceed
                If messageReadyToSend = True Then 'Send the message
                    Try
                        Send(BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Ship_To_Ship), Net.Sockets.SocketFlags.None) 'Send the message header to the Client
                        Send(BitConverter.GetBytes(messageToClient.Length), Net.Sockets.SocketFlags.None) 'Send the number of Bytes to be sent to the Client
                        Send(messageToClient, Net.Sockets.SocketFlags.None) 'Send the message
                    Catch ex As Net.Sockets.SocketException
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to send to the '" + Name +
                                                  "'. They will now be disconnected." + Environment.NewLine + ex.ToString())
                        _Disconnecting = True 'Let the loop close
                    Catch ex As Exception
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while trying to send to the '" +
                                                  Name + "'. The Comms will now close" + Environment.NewLine + ex.ToString())
                        _Disconnecting = True  'Let the loop close
                        Server.Comms.ClosingComms = False 'Close the Server comms
                    End Try

                    '-----Receive a message from the Client-----
                    Try
                        If BitConverter.ToBoolean(Receive_ByteArray(Net.Sockets.SocketFlags.None, 1), 0) Then 'There is data to receive
                            Dim message() As Object = Game_Library.Serialisation.FromBytes(Receive_ByteArray(Net.Sockets.SocketFlags.None)) 'Receive the message
                            Select Case message(0)
                                Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Message_Exception
                                    disconnectMessage = "The Client received a bad message from the Server"
                                    _Disconnecting = True
                                Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting
                                    disconnectMessage = "The Client is disconnecting from the Server"
                                    _Disconnecting = True
                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up
                                    _throttleUp = message(1) 'Receive the boolean
                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down
                                    _throttleDown = message(1) 'Receive the boolean
                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right
                                    _turnRight = message(1) 'Receive the boolean
                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left
                                    _turnLeft = message(1) 'Receive the boolean
                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Weapons
                                    _fireWeapons = message(1) 'Receive the boolean
                            End Select
                        End If
                    Catch ex As Net.Sockets.SocketException
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while trying to receive a message from the '" +
                                                  Name + "'. They will now be disconnected." + Environment.NewLine + ex.ToString())
                        _Disconnecting = True 'Let the loop close
                    Catch ex As Exception
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while trying to receive a message from the '" +
                                                  Name + "'. The Comms will now close." + Environment.NewLine + ex.ToString())
                        _Disconnecting = True 'Let the loop close
                        Server.Comms.ClosingComms = True  'Close the communications
                    End Try
                End If
                '-------------------------------------------
            Else 'Disconnect
                clientComms = False
                If disconnectReason <> -1 Then 'There is a cause to be sent
                    If Connected = True Then 'The Client is still connected
                        Try
                            Send(BitConverter.GetBytes(disconnectReason), Net.Sockets.SocketFlags.None) 'Send the reason for the disconnect to the Client
                        Catch ex As Net.Sockets.SocketException
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error trying to send to the '" + Name + "', '" +
                                                      disconnectReason.ToString() + "'.")
                        Catch ex As Exception
                            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an unexpected and unhandled exception while trying to send the reason for the disconnect to the Client." +
                                                      Environment.NewLine + ex.ToString())
                            Server.Comms.ClosingComms = True
                        End Try
                    Else 'The Client is disconnected
                        Server.Write_To_Error_Log(Environment.NewLine + "ERROR : The Client disconnected before the Server could send '" +
                                                  disconnectReason.ToString() + "' to the Client.")
                    End If
                End If
            End If
            '--------------------------------------
        Loop

        Finalise_Client()
    End Sub

    Public Sub Disconnect_Client()
        _Disconnecting = True
    End Sub

    Private Sub Finalise_Client() 'Disconnects the ServerClient from the Client and finalises the ServerClient
        Craft.Dead = True 'Destroy the Craft
        Console.WriteLine(Environment.NewLine + "Server : Disconnecting the '" + Server.Comms.clientList(Index).Name + "'" +
                          If((disconnectMessage <> ""), (" because " + disconnectMessage), "") + ".")
        Dim endPoint As String = Server.Comms.clientList(Index).RemoteEndPoint.ToString() 'Gets a string representing the remote end point of the Socket
        Server.Comms.interactWithClients.WaitOne() 'Wait until the Client has control of clientList
        If Index <> Server.Comms.clientList.Count Then 'There are Client's who's index has changed
            For i As Integer = Index To Server.Comms.clientList.Count - 1 'Loop through those Clients
                Server.Comms.clientList(i).Index = i 'Set the new index
            Next
        End If
        Index = -1 'Clear the index
        If disconnectReason <> -1 Then Disconnect(False) 'Disconnect the socket
        Close() 'Close the socket
        _Disconnecting = False
        Console.WriteLine("Server : '" + endPoint + "' was disconnected.") 'Write to the Console
    End Sub

    Public Sub Generate_Message(ByVal objects As List(Of Ship)) 'Creates a message for the ServerClient to send
        If Craft.Dead = False Then 'The craft is still alive
            Dim positions(objects.Count - 1) As System.Drawing.Point 'An array of point objects to be sent to the Client
            Dim directions(objects.Count - 1) As Double 'An array of double values to be sent to the Client
            Dim allegiances(objects.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'An array of alleiances values to be sent to the Client
            For i As Integer = 0 To positions.Length - 1 'Loop through all Indexs
                positions(i) = New System.Drawing.Point((objects(i).X - Craft.X), (objects(i).Y - Craft.Y)) 'Create a point object relative to the object
                directions(i) = objects(i).Direction 'Set the object's direction
                allegiances(i) = -1 'Set the object's allegiance
            Next
            allegiances(Craft.CombatIndex) = 1 'Set the craft's allegiance
            messageToClient = Game_Library.Serialisation.ToBytes({positions, directions, allegiances, Craft.Speed, Craft.CombatIndex, Craft.Gun.Range, Craft.firing, Craft.Hull.Current, Craft.Hull.Maximum}) 'Convert the message into Bytes
            messageReadyToSend = True
        ElseIf Disconnecting = False Then 'The craft is dead and the Client needs to disconnect
            Server.Comms.closing.Add(Me) 'Add this ServerClient to the list of closing clients
            disconnectReason = Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Kicked_Exception 'Set the disconnect reason for the message
            disconnectMessage = ("The Client died and was therefore kicked from the server") 'Set the disconnect message for the Client
        End If
    End Sub

End Class
