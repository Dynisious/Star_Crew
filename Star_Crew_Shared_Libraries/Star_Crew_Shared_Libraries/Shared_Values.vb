Namespace Shared_Values 'A Namespace of general resources that are used by both the Client and the Server
    Public Structure Values 'A Structure that stores readonly values
        Public Shared ReadOnly ServicePort As Integer = 1225 'An Integer value representing the Port that the Server listens for connection requests on and the Clients connect to

    End Structure

    Public Enum GameStates 'The different states the game can be in
        Ship_To_Ship_Combat = Networking_Messages.General_Headers.max 'The Player is engaged in Ship to Ship combat
        Fleet_Transit 'The Player is flying their Fleet about
        Trading 'The Player is engaged in trade
    End Enum
    Public Enum Allegiances 'The different allegiances inside the game
        Federation
        Empire
        Neutral
        max
    End Enum

End Namespace