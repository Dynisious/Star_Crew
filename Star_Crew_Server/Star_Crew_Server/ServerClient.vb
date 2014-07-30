Public Class ServerClient
    Inherits Game_Library.Networking.Client
    Public Station As ShipStation.StationTypes 'A StationTypes value representing which ShipStation this ServerClient is connected to
    Public Enum ServerClientMessages 'An enumerator of messages that can be sent/received to/from the Client
        Client_Connection_Successful 'This is sent to the Client when they succesfully connect to the Server
        Station_Already_Manned_Exception 'This is sent to the Client when they try to connect to a Station that is already manned
    End Enum

    Public Sub New(ByRef nSocket As System.Net.Sockets.Socket, ByVal nIndex As Integer) 'Create a new ServerClient connected to the passed Socket
        MyBase.New(nSocket, "Unnamed Client", nIndex) 'Create a new Client connected to the passed Socket

        Dim buff(3) As Byte 'The array of Bytes used to send/receive an Integer to/from the new connection'
        Dim receivedBytes As Integer 'How many Bytes have been received so far
        Try
            While receivedBytes < 4 'Loop until the buffer is full
                receivedBytes = receivedBytes + Receive(buff, Net.Sockets.SocketFlags.None)
            End While
        Catch ex As System.Net.Sockets.SocketException 'The Socket was unable to receive from the Client
            Console.WriteLine("Error : ServerClient{0} did not receive the necessary connection information. The Socket will now disconnect.", nIndex)
            Console.WriteLine()
            Console.WriteLine(ex.ToString())
            Disconnect(False)
            Server.ClientList.RemoveAt(nIndex) 'Remove the ServerClient
            Server.ClientList.TrimExcess() 'Remove blank spaces from the list
            If nIndex <> Server.ClientList.Count Then 'There are Clients higher in the list
                For i As Integer = nIndex To Server.ClientList.Count - 1 'Loop to the end of the list
                    Server.ClientList(i).Index = i 'Set the new index
                Next
            End If
            Exit Sub 'Exit the Sub
        Catch ex As Exception 'There was an unhandled exception
            Console.WriteLine("Error : ServerClient{0} did not receive the necessary connection information. The Socket will now disconnect.", nIndex)
            Console.WriteLine()
            Console.WriteLine(ex.ToString())
            Disconnect(False)
            Server.ClientList.RemoveAt(nIndex) 'Remove the ServerClient
            Server.ClientList.TrimExcess() 'Remove blank spaces from the list
            If nIndex <> Server.ClientList.Count Then 'There are Clients higher in the list
                For i As Integer = nIndex To Server.ClientList.Count - 1 'Loop to the end of the list
                    Server.ClientList(i).Index = i 'Set the new index
                Next
            End If
            Exit Sub 'Exit the Sub
        End Try

        Station = BitConverter.ToInt32(buff, 0) 'Create an Integer value representing the ShipStation the Client wants to connect to
        Name = Station.ToString() 'Set the name of the ServerClient
        For Each i As ServerClient In Server.ClientList
            If i.Station = Station And i.Index = Index Then 'The Station is already manned
                Send(BitConverter.GetBytes(ServerClientMessages.Station_Already_Manned_Exception), Net.Sockets.SocketFlags.None) 'Send a message to the Client saying that the Station is already manned
                Console.WriteLine("Server : {0} attempted to connect to '{1}' which was already manned", RemoteEndPoint.ToString(), Name) 'Write to the Console what happened
                Disconnect(False) 'Disconnect the Client
                Server.ClientList.RemoveAt(Index) 'Remove the Client
                Server.ClientList.TrimExcess() 'Remove any blank spaces
                If Index <> Server.ClientList.Count Then 'There are Clients higher in the list
                    For e As Integer = Index To Server.ClientList.Count - 1 'Loop to the end of the list
                        Server.ClientList(e).Index = e 'Set the new index
                    Next
                End If
                Exit Sub
            End If
        Next
        Send(BitConverter.GetBytes(ServerClientMessages.Client_Connection_Successful), Net.Sockets.SocketFlags.None) 'Send a messsage to the Client saying that the connection was successful
    End Sub

End Class
