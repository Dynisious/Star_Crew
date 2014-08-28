Public Class ServerClient
    Inherits Game_Library.Networking.Client
    Private _Station As Star_Crew_Shared_Libraries.Shared_Values.StationTypes 'The actual value of Station
    Public ReadOnly Property Station As Star_Crew_Shared_Libraries.Shared_Values.StationTypes 'A StationTypes value representing which ShipStation this ServerClient is connected to
        Get
            Return _Station
        End Get
    End Property

    Public Sub New(ByRef nSocket As System.Net.Sockets.Socket, ByVal nIndex As Integer) 'Create a new ServerClient connected to the passed Socket
        MyBase.New(nSocket, "Unnamed Client", nIndex) 'Create a new Client connected to the passed Socket

        Dim buff(3) As Byte 'The array of Bytes used to send/receive an Integer to/from the new connection'
        Dim receivedBytes As Integer 'How many Bytes have been received so far
        Try
            While receivedBytes < 4 'Loop until the buffer is full
                receivedBytes = receivedBytes + Receive(buff, Net.Sockets.SocketFlags.None)
            End While
        Catch ex As Exception 'There was an unhandled exception
            Server.Comms.Remove_Client(nIndex, Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Bad_Connection_Exception,
                                       ("Error : The Client at " + RemoteEndPoint.ToString() +
                                        " did not receive the necessary connection information. The Socket will now disconnect." +
                                        Environment.NewLine + ex.ToString())) 'Disconnect the client
            Exit Sub 'Exit the Sub
        End Try

        _Station = BitConverter.ToInt32(buff, 0) 'Create an Integer value representing the ShipStation the Client wants to connect to
        Name = Station.ToString() 'Set the name of the ServerClient
        For Each i As ServerClient In Server.Comms.ClientList
            If i.Station = Station And i.Index <> Index Then 'The Station is already manned
                Send(BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Station_Already_Manned_Exception), Net.Sockets.SocketFlags.None) 'Send a message to the Client saying that the Station is already manned
                Server.Comms.Remove_Client(nIndex, Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Station_Already_Manned_Exception,
                                           ("ERROR : " + RemoteEndPoint.ToString() + " attempted to connect to '" + Name +
                                            "' which was already manned")) 'Disconnect the client
                Exit Sub
            End If
        Next
        Send(BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Client_Connection_Successful), Net.Sockets.SocketFlags.None) 'Send a messsage to the Client saying that the connection was successful
        Console.WriteLine("Server : A Client at {0} connected to {1}." + Environment.NewLine, RemoteEndPoint.ToString(), Name)
    End Sub

End Class
