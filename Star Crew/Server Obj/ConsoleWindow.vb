Module ConsoleWindow
    Public OutputScreen As New Screen 'A Screen object for the GUI
    Public GameServer As New Server 'A Server object to run a Server
    Public clientThread As New Threading.Thread(AddressOf OutputScreen.Open) 'A Thread object for OutputScreen
    Public ServerThread As Threading.Thread 'A Thread object for GameServer

    Public Sub Main() 'Starts the application
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
                          "Green: Green means Friendly. If it is a Friendly Fleet then interact with it" + Environment.NewLine +
                          "to Add it's Ships to your Fleet." + Environment.NewLine +
                          "Red: Red means Enemy. If it is a Enemy Fleet interact with it to engage your" + Environment.NewLine +
                          "Ships with theirs." + Environment.NewLine +
                          "Yellow: Yellow means neutral. Neutral objects will lay dormant until they are" +
                          "captured by a team." + Environment.NewLine +
                          "Space Stations: The hexagons on the scanner indecate Space Stations. Space Stations" + Environment.NewLine +
                          "align themselves to the team with the greatest number of Fleet in proximity." + Environment.NewLine +
                          "Interaction: To interact with an object collide your Fleet into it." + Environment.NewLine +
                          "Objective: The Objective of this game is to eleminate all Enemy Fleets." + Environment.NewLine +
                          Environment.NewLine +
                          "-----General-----" + Environment.NewLine +
                          "Press 'Z' to zoom out" + Environment.NewLine +
                          "Press 'X' to zoom out" + Environment.NewLine)
        Console.WriteLine("Press the 'Enter' key to continue to the Menu...")
        Console.ReadLine()
        clientThread.Start() 'Opens the GUI
        OutputScreen.Focus() 'Sets the focus to the GUI
    End Sub

End Module
