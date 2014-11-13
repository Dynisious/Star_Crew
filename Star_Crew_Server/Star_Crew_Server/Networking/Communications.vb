Public Class Communications 'object that runs the networking for the Server
    Public CommsThread As System.Threading.Thread 'A System.Threading.Mutex object that allows either the game or the communications to be in control during an interaction
    Public clientList As New List(Of ServerClient) 'A List of ServerClient objects that communicate with the Server
    Public serverComms As Boolean = True 'A Boolean value to keep the Communications object looping
    Private ServiceSocket As Net.Sockets.TcpListener 'A Net.Sockets.TcpListener object used to receive connection requests
    Public InteractWithClients As System.Threading.Mutex 'A Mutex object used to allow only one thread to interact with clientlist at any time

    Public Sub Initialise_Communications() 'Initialises the Server's network
        Console.WriteLine("Initialising Communications...")
        serverComms = True
        CommsThread = New System.Threading.Thread(AddressOf Run_Comms) 'Create a new Thread to run the comms
        ServiceSocket = New Net.Sockets.TcpListener(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) 'Create a new TcpListener object
        CommsThread.Start() 'Start running the comms
    End Sub

    Private Sub Run_Comms() 'Handles the adding of new Client's to the list and the disconnecting clients
        Try
            Try
                ServiceSocket.Start() 'Start Sevice
            Catch ex As System.Net.Sockets.SocketException
                Console.WriteLine(Environment.NewLine + "The ServicePort for the server is already bound to by something else. The comms cannot open.")
                Server.Write_To_Error_Log(Environment.NewLine + "ERROR : A process was already bound to '0.0.0.0:" +
                                          CStr(Star_Crew_Shared_Libraries.Shared_Values.Values.ServicePort) +
                                          "' so the Server's comms could not open." +
                                          Environment.NewLine + ex.ToString())
                Exit Sub
            End Try
            Console.WriteLine("Server : The Server is listening for connection requests at '" +
                              ServiceSocket.LocalEndpoint.ToString() + "'")
            Console.WriteLine("Communications Initialised")

            '-----Run Comms-----
            Do While serverComms 'Loop until the comms are closing
                If ServiceSocket.Pending() Then 'There are pending connections
                    InteractWithClients.WaitOne() 'Wait till the comms has control of clientList
                    Dim clientCreated As Boolean 'A Boolean value indecationg whether the ServerClient was created successfully
                    Dim client As New ServerClient(ServiceSocket.AcceptSocket(), clientList.Count, clientCreated) 'Create a new ServerClient
                    If clientCreated Then clientList.Add(client) 'Add the ServerClient to the list
                    InteractWithClients.ReleaseMutex() 'Release the mutex
                End If

                System.Threading.Thread.Sleep(100)
            Loop
            '-------------------

            Console.WriteLine("InteractWithClients.WaitOne()")
            InteractWithClients.WaitOne()
            For Each i As ServerClient In clientList
                i.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.General_Headers.Server_Closed_Exception)},
                               {"ERROR : There was an error while sending the Server_Closed_Exception to the Client."})
                i.receivingAlive = False
                i.sendingAlive = False
            Next
            Console.WriteLine("InteractWithClients.ReleaseMutex()")
            InteractWithClients.ReleaseMutex()

            While clientList.Count <> 0
                System.Threading.Thread.Sleep(100)
            End While

            Console.WriteLine("Server : Communications are closed")
        Catch ex As Exception
            Server.Write_To_Error_Log(Environment.NewLine + "ERROR : There was an error while excecuting the comms for the Server. Server will now close." +
                                      Environment.NewLine + ex.ToString())
            End
        End Try
    End Sub

End Class
