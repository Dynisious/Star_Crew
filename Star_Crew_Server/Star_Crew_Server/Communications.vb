Public Class Communications 'object that runs the networking for the Server
    Private CommsThread As System.Threading.Thread 'A System.Threading.Mutex object that allows either the game or the communications to be in control during an interaction
    Public clientList As New List(Of ServerClient) 'A List of ServerClient objects that communicate with the Server
    Public loopComms As Boolean = True 'A Boolean value to keep the Communications object looping
    Private ServiceSocket As Net.Sockets.TcpListener 'A Net.Sockets.TcpListener object used to receive connection requests
    Public interactWithClients As System.Threading.Mutex 'A Mutex object used to allow only one thread to interact with clientlist at any time
    Private ClosingComms As Boolean = False 'The actual value of CommsRunning

    Public Sub Initialise_Communications() 'Initialises the Server's network
        Console.WriteLine("Initialising Communications...")
        loopComms = True
        CommsThread = New System.Threading.Thread(AddressOf Run_Comms) 'Create a new Thread to run the comms
        ServiceSocket = New Net.Sockets.TcpListener(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) 'Create a new TcpListener object
        CommsThread.Start() 'Start running the comms
    End Sub

    Private Sub Run_Comms() 'Handles the adding of new Client's to the list
        Try
            ServiceSocket.Start() 'Start Sevice
        Catch ex As System.Net.Sockets.SocketException
            Console.WriteLine(Environment.NewLine +
                              "The ServicePort for the server is already bound to by something else. The comms cannot open." +
                              Environment.NewLine)
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : A process was already bound to '0.0.0.0:" +
                                      CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) +
                                      "' so the Server's comms could not open." +
                                      Environment.NewLine + ex.ToString())
            Exit Sub
        End Try
        Console.WriteLine("Server : The Server is listening for connection requests at '" +
                          ServiceSocket.LocalEndpoint.ToString() + "'")
        Console.WriteLine("Communications Initialised")

        '-----Connect Clients-----
        Do Until loopComms = False 'Loop until the comms are closing
            If ServiceSocket.Pending() = True Then 'There are pending connections
                interactWithClients.WaitOne() 'Wait till the comms has control of clientList
                Dim clientCreated As Boolean 'A Boolean value indecationg whether the ServerClient was created successfully
                Dim client As New ServerClient(ServiceSocket.AcceptSocket(), clientList.Count, clientCreated) 'Create a new ServerClient
                If clientCreated = True Then clientList.Add(client) 'Add the ServerClient to the list
                interactWithClients.ReleaseMutex() 'Realease the mutex
            End If
        Loop
        '-------------------------

        Console.WriteLine(Environment.NewLine + "Server : Communications are finalising...")
        For i As Integer = 0 To clientList.Count - 1 'Loop through all Clients
            clientList(0).Disconnect_Client(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception, "") 'Disconnect the Client
        Next
        ClosingComms = False
        Console.WriteLine("Server : Communications are closed")
    End Sub

    Public Sub Close_Communications()
        loopComms = False
        ClosingComms = True
        While ClosingComms = True 'Loop
        End While
    End Sub

End Class
