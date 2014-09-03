Public Class Communications 'object that runs the networking for the Server
    Public ClientList As New List(Of ServerClient) 'A List of ServerClient objects that communicate with the Server
    Public LoopComms As Boolean = True 'A Boolean value to keep the Communications object looping
    Private ServiceSocket As Net.Sockets.TcpListener 'A Net.Sockets.TcpListener object used to receive connection requests

    Public Sub Initialise_Communications() 'Initialises the Server's network
        Console.WriteLine("Initialising Network...")
        Server.CommsThread = New System.Threading.Thread(AddressOf Comms.Run_Comms) 'Create a new Thread to run the comms
        ServiceSocket = New Net.Sockets.TcpListener(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) 'Create a new TcpListener object
        Server.CommsThread.Start() 'Start running the comms
    End Sub

    Public Sub Finalise() 'Finalises the network
        LoopComms = False 'Lets the comms Close
        Server.CommsThread.Join() 'Waits until the comms stop
    End Sub

    Public Sub Run_Comms() 'handles the communications of the Server
        ServiceSocket.Start() 'Begin listening for requests
        Console.WriteLine("Server is now listening on {0} for connection requests", ServiceSocket.LocalEndpoint.ToString())
        Console.WriteLine("Network has been initialised")

        Dim hasNetwork As Boolean = False 'A Boolean value indecating whether this thread has control over the network
        While LoopComms = True 'The Comms are running
            If ServiceSocket.Pending() = True Then 'Their pending connection requests
                Server.UseNetwork.WaitOne() 'Wait until the comms have control of the network
                hasNetwork = True 'The comms have the network
                ClientList.Add(New ServerClient(ServiceSocket.AcceptSocket(), ClientList.Count)) 'Add a new Client to the list
            End If
            If ClientList.Count <> 0 Then 'There're connected Clients
                '-----Receive Messages-----
                Dim index As Integer 'The current index in the List of Clients
                Do Until index = ClientList.Count 'Loop through all Clients pending data
                    Try
                        If ClientList(index).Available <> 0 Then 'There're Bytes ready to receive
                            If hasNetwork = False Then 'Get the network
                                Server.UseNetwork.WaitOne() 'Wait until the comms have control of the network
                                hasNetwork = True 'This thread now has control of the network
                            End If
                            Dim buff(3) As Byte 'A buffer of 4 Bytes used to receive message headers
                            Dim receivedBytes As Integer = 0 'An Integer value counting how many Bytes have been received
                            While receivedBytes < 4 'Loop until 4 Bytes are received
                                receivedBytes = receivedBytes + ClientList(index).Receive(buff, receivedBytes, 4 - receivedBytes, Net.Sockets.SocketFlags.None) 'Receive up to enough Bytes to fill the buffer
                            End While
                            Dim messageType As Integer = BitConverter.ToInt32(buff, 0) 'Get the type of message being sent
                            Select Case messageType
                                Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception 'The Client suffered a bad connection
                                    ClientList(index).Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Tell the Client that the message was received
                                    Remove_Client(index, -1, "ERROR : The Client suffered a bad connection and is disconnecting.")
                                Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Disconnecting 'The Client is disconnecting from the Server
                                    ClientList(index).Send(BitConverter.GetBytes(True), Net.Sockets.SocketFlags.None) 'Tell the Client that the message was received
                                    Remove_Client(index, -1, "ERROR : The Client is disconnecting from the Server.")
                                Case Else 'The message is not an error
                                    Select Case ClientList(index).Station 'Select which station is sending a message
                                        Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Helm 'The Helm of the Ship
                                            ReDim buff(0) 'Redim the array to receive one Byte
                                            ClientList(index).Receive(buff, 0, 1, Net.Sockets.SocketFlags.None) 'Receive one Byte
                                            Dim val As Boolean = BitConverter.ToBoolean(buff, 0) 'Convert the Byte into a boolean
                                            Select Case messageType
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Up 'Throttle Up is changing
                                                    Server.GameWorld.ClientInteractions.ThrottleUp = val 'Set ThrottleUp to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ThrottleDown = False 'Set ThrottleDown to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Throttle_Down 'Throttle Down is changing
                                                    Server.GameWorld.ClientInteractions.ThrottleDown = val 'Set ThrottleDown to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ThrottleUp = False 'Set ThrottleDown to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Right 'Turn Right is changing
                                                    Server.GameWorld.ClientInteractions.ShieldRight = val 'Set ShipRight to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ShipLeft = False 'Set ShipLeft to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Turn_Left 'Turn Left is changing
                                                    Server.GameWorld.ClientInteractions.ShipLeft = val 'Set ShipLeft to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ShieldRight = False 'Set ShipRight to false
                                                    End If
                                            End Select
                                        Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Battery 'The Batteries of the Ship
                                            ReDim buff(0) 'Redim the array to receive one Byte
                                            ClientList(index).Receive(buff, 0, 1, Net.Sockets.SocketFlags.None) 'Receive one Byte
                                            Dim val As Boolean = BitConverter.ToBoolean(buff, 0) 'Convert the Byte into a boolean
                                            Select Case messageType
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Weapon_Right 'Turn Right is changing
                                                    Server.GameWorld.ClientInteractions.WeaponRight = val 'Set WeaponRight to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.WeaponLeft = False 'Set WeaponLeft to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Weapon_Left 'Turn Left is changing
                                                    Server.GameWorld.ClientInteractions.WeaponLeft = val 'Set WeaponLeft to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.WeaponRight = False 'Set WeaponRight to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Fire_Weapon 'Fire Weapon is changing
                                                    Server.GameWorld.ClientInteractions.FireWeapon = val 'Set FireWeapon to the val
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Next_Weapon 'Next Weapon is changing
                                                    Server.GameWorld.ClientInteractions.NextWeapon = val 'Set NextWeapon to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.PreviousWeapon = False 'Set PreviousWeapon to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Previous_Weapon 'Previous Weapon is changing
                                                    Server.GameWorld.ClientInteractions.PreviousWeapon = val 'Set PreviousWeapon to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.NextWeapon = False 'Set NextWeapon to false
                                                    End If
                                            End Select
                                        Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Shields 'The Shields of the Ship
                                            ReDim buff(0) 'Redim the array to receive one Byte
                                            ClientList(index).Receive(buff, 0, 1, Net.Sockets.SocketFlags.None) 'Receive one Byte
                                            Dim val As Boolean = BitConverter.ToBoolean(buff, 0) 'Convert the Byte into a boolean
                                            Select Case messageType
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Shield_Right 'Rotate Right is changing
                                                    Server.GameWorld.ClientInteractions.ShieldRight = val 'Set ShieldRight to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ShieldLeft = False 'Set ShieldLeft to false
                                                    End If
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Shield_Left 'Rotate Left is changing
                                                    Server.GameWorld.ClientInteractions.ShipLeft = val 'Set ShieldLeft to the val
                                                    If val = True Then 'The value is true
                                                        Server.GameWorld.ClientInteractions.ShieldRight = False 'Set ShieldRight to false
                                                    End If
                                            End Select
                                        Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Engines 'The Engines on the Ship
                                            Select Case messageType
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Repair_Station 'Repair Station is changing
                                                    ReDim buff(3) 'Redim the array to receive 4 Bytes
                                                    receivedBytes = 0 'Clear the count of received Bytes
                                                    While receivedBytes < 4 'Loop until 4 Bytes have been received
                                                        receivedBytes = receivedBytes + ClientList(index).Receive(buff, receivedBytes, 4 - receivedBytes, Net.Sockets.SocketFlags.None) 'Receive up to as many Bytes as are needed to fill buff
                                                    End While
                                                    Dim val As Star_Crew_Shared_Libraries.Shared_Values.StationTypes = BitConverter.ToInt32(buff, 0) 'Convert the Bytes into an Integer
                                                    Server.GameWorld.ClientInteractions.StationToRepair = val 'Set the new Station to be repaired
                                                Case Star_Crew_Shared_Libraries.Networking_Messages.Ship_Control_Header.Change_Power 'Change Power is changing
                                                    ClientList(index).Receive_ByteArray(buff) 'Receive the array of Bytes
                                                    Engines.Client_Power_Distribution(buff) 'Set the Client's power
                                            End Select
                                        Case Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception
                                            Remove_Client(index, -1, "ERROR : The Client suffered a bad connection and is disconnecting.")
                                    End Select
                            End Select
                        End If
                        index = index + 1 'Add one to the index
                    Catch ex As Exception
                        Dim message As String = ("ERROR : The Server was unable to receive message from '" +
                                                 ClientList(index).RemoteEndPoint.ToString() + "'. " +
                                                 ClientList(index).Station.ToString() + " will be disconnected" +
                                                 Environment.NewLine + ex.ToString())
                        Remove_Client(index, -1, message)
                    End Try
                Loop
                '--------------------------
            End If
            If hasNetwork = True Then 'The comms have control of the network
                Server.UseNetwork.ReleaseMutex() 'Release the mutex
                hasNetwork = False 'This thread no longer has control of the network
            End If
        End While

        '-----Finalise the comms-----
        ServiceSocket.Stop()
        For i As Integer = 0 To ClientList.Count - 1 'Loop through all Clients
            Dim message As String = ("Server : '" + ClientList(0).Station.ToString() + "' has been disconnected.")
            Remove_Client(0, Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception, message) 'Remove the Client
        Next
        ClientList.Clear() 'Clear the list of Clients
        If hasNetwork = True Then Server.UseNetwork.ReleaseMutex() 'Realease the Mutex
        Server.UseNetwork.Close() 'Close the Mutex
        '----------------------------
    End Sub

    Public Sub Remove_Client(ByVal index As Integer, ByVal reason As Star_Crew_Shared_Libraries.Networking_Messages.General_Headers, ByVal Message As String) 'Removes the Client at the specified index
        Dim endPoint As String = ClientList(index).RemoteEndPoint.ToString() 'Gets a string representing the remote end point of the Socket
        Dim station As String = ClientList(index).Station.ToString() 'Gets a string representing the ShipStation the Client is connected to
        If reason <> -1 Then 'There is a cause to be sent
            If ClientList(index).Connected = True Then 'The Client is still connected
                Try
                    ClientList(index).Send(BitConverter.GetBytes(reason), Net.Sockets.SocketFlags.None) 'Send the reason for the disconnect to the Client
                    Dim buff(0) As Byte
                    ClientList(index).Receive(buff, 0, 1, Net.Sockets.SocketFlags.None) 'Receive a Byte from the Client to say that they got the message
                Catch ex As Exception
                    Console.WriteLine("ERROR : There was an error trying to send to the Client '" + reason.ToString() + "'.")
                End Try
            Else 'The Client is disconnected
                Console.WriteLine("ERROR : The Client disconnected before the Server could send '" + reason.ToString() + "' to the Client.")
            End If
        End If
        ClientList(index).Disconnect(False) 'Disconnect the socket
        ClientList(index).Close() 'Close the socket
        ClientList.RemoveAt(index) 'Remove the Client
        If ClientList.Count <> 0 Then 'There are other Clients
            For i As Integer = 0 To ClientList.Count - 1 'Loop through the remaining Clients
                ClientList(i).Index = i 'Set the new index
            Next
        End If
        ClientList.TrimExcess() 'Remove spare spots
        If Message <> "" Then 'Theres a message to write
            Console.WriteLine(Message) 'Write the message to the console
        End If
        Console.WriteLine("Server : " + endPoint + " was disconnected from '" + station + "'" + Environment.NewLine) 'Write to the Console
    End Sub

    Public Sub Send_To(ByVal client As Star_Crew_Shared_Libraries.Shared_Values.StationTypes, ByVal nHeader As Byte(), ByVal nMessage As Byte()) 'Sends a message to a particular Client
        For Each i As ServerClient In ClientList 'Loop through all Clients
            If i.Station = client Then 'This is the correct Station
                Try
                    i.Send(nHeader, Net.Sockets.SocketFlags.None) 'Send the message header
                    i.Send_ByteArray(nMessage) 'Send the array of Bytes
                Catch ex As Exception
                    Remove_Client(i.Index, -1, ("Error : An exception was encountered trying to send to '" + client.ToString() +
                                                "'. " + client.ToString() + " will be disconnected." +
                                                Environment.NewLine + ex.ToString()))
                End Try
                Exit Sub
            End If
        Next
    End Sub

    Public Sub Send_All(ByVal nHeader As Byte(), ByVal nMessage As Byte()) 'Sends a message to all Clients
        Dim index As Integer 'The current index in the list of Clients
        Do Until index = ClientList.Count 'Loop through all Clients
            Try
                ClientList(index).Send(nHeader, Net.Sockets.SocketFlags.None) 'Send the message header
                ClientList(index).Send_ByteArray(nMessage) 'Send the array of Bytes
                index = index + 1 'Add one to the index
            Catch ex As Exception
                Remove_Client(index, -1, ("Error : An exception was encountered trying to send to '" +
                                           ClientList(index).Station.ToString() + "'. " + ClientList(index).Station.ToString() +
                                           " will be disconnected." + Environment.NewLine + ex.ToString()))
            End Try
        Loop
    End Sub

End Class
