Module ConsoleWindow
    Public OutputScreen As New Screen 'A Screen object for the GUI
    Public GameServer As New Server 'A Server object to run a Server
    Public clientThread As New Threading.Thread(AddressOf OutputScreen.Open) 'A Thread object for OutputScreen
    Public ServerThread As Threading.Thread 'A Thread object for GameServer

    Public Sub Main() 'Starts the application
        clientThread.Start() 'Opens the GUI
        Console.WriteLine("-----Star Crew-----")
        Console.WriteLine("for help with commands type '/help'" + Environment.NewLine +
                          Environment.NewLine +
                          "-----Station Controls-----" + Environment.NewLine +
                          "Helm (Must be maned at all times): Steer the helm with the" + Environment.NewLine +
                          "'Left' and 'Right' arrow keys." + Environment.NewLine +
                          "Speed Up and Down the the 'Up' and 'Down' arrow keys." + Environment.NewLine +
                          "Have the computer Match your targets speed automatically with 'M'." + Environment.NewLine +
                          "Press 'J' to exit combat to Warp Speed." + Environment.NewLine +
                          "Steer the Fleet while flying in the sector as you steer the Helm" + Environment.NewLine +
                          Environment.NewLine +
                          "Batteries: Rotate both Guns with the 'Left' and 'Right' arrow keys." + Environment.NewLine +
                          "Fire the Primary Gun with 'A'." + Environment.NewLine +
                          "Fire the Secondary Gun with 'D'." + Environment.NewLine +
                          "Select a New Target with 'M'." + Environment.NewLine +
                          Environment.NewLine +
                          "Shielding: Set the Shield to Prioritise with the arrow keys." + Environment.NewLine +
                          Environment.NewLine +
                          "Engineering: Control the Power Core's Temperature with the 'Up' and 'Down'" + Environment.NewLine +
                          "Arrow Keys; keep the temperature as close to 50*e^5 as possible or the Power" + Environment.NewLine +
                          "Core will not be at full efficiency." + Environment.NewLine +
                          Environment.NewLine +
                          "-----Sector Mechanics-----" + Environment.NewLine +
                          "Green: Green Fleets are Friendly and if you come close enough to them they" + Environment.NewLine +
                          "will add their forces to yours." + Environment.NewLine +
                          "Red: Red Fleets are the enemy and they will also combine forces to combat" + Environment.NewLine +
                          "yours." + Environment.NewLine +
                          "Yellow: Yellow indecates a station which will Repair all ships in a Fleet" + Environment.NewLine +
                          "up to 2/3 hull." + Environment.NewLine +
                          "Interaction: To interact with a Fleet collide your Fleet into it." + Environment.NewLine +
                          "Objective: The Objective of this game is to eleminate all Enemy Fleets." + Environment.NewLine +
                          Environment.NewLine +
                          "-----Genral-----" + Environment.NewLine +
                          "Press 'Z' to toggle Zoom" + Environment.NewLine)
    End Sub

End Module
