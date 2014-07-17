Module Server 'Runs the Game and allows Clients to connect and interact
    Public GameWorld As Galaxy 'The Galaxy object that encompasses the game
    Public ClientList As New List(Of Game_Library.Networking.Client) 'A List of Client objects that the 

    Public Sub Main() 'The initialising code of the application

    End Sub

    Public Sub New_Game() 'Generates a fresh game

    End Sub

    Public Sub Load_Game() 'Loads a previous game

    End Sub

    Public Sub Send_To_Clients() 'Sends a message to all Clients

    End Sub

    Public Function Normalise_Direction(ByVal nDirection As Double) As Double 'Returns a radian between the range of 0-2*Pi

    End Function

End Module
