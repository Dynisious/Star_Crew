Module Client_Console 'Used to output messages to the console for error handling etc for the Client
    Public OutputScreen As New Screen 'A Screen object used as the GUI for the Client
    Public Client As Connector 'A Connector object used to connect to a Server
    Public CommsThread As System.Threading.Thread 'A Thread object used to run the communications
    Public UseNetwork As System.Threading.Mutex 'A Mutex object used to synchronise the use of the network

    Sub Main()
        Console.Write("-----Star Crew Client-----")
        Console.WriteLine("Initialising Objects...")
        Randomize() 'Set the random sequense of numbers
        Dim newMutex As Boolean 'Necessary for creating a new Mutex
        Dim mutexSecurity As New System.Security.AccessControl.MutexSecurity 'The security object for the Mutex
        Dim user As String = Environment.UserDomainName + "\" + Environment.UserName 'The name of the current user
        Dim rule As New System.Security.AccessControl.MutexAccessRule(
            user, System.Security.AccessControl.MutexRights.Modify Or
            System.Security.AccessControl.MutexRights.Synchronize,
            System.Security.AccessControl.AccessControlType.Allow) 'The security rule for the Mutex
        mutexSecurity.AddAccessRule(rule) 'Add the rule to the Security
        UseNetwork = New System.Threading.Mutex(False, "UseNetwork", newMutex, mutexSecurity) 'Create a new Mutex to control access to the network
        Console.WriteLine("Objects have been Initialised")
        Console.WriteLine("Press the 'Enter' Key when ready to begin...")
        Console.Title = "Star Crew Client Console"
        Console.ReadLine()

        OutputScreen.Give_Control() 'Makes the Screen object visible and gives it control over this thread
    End Sub

End Module
