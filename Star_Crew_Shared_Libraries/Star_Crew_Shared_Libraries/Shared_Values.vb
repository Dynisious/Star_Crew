Namespace Shared_Values 'A Namespace of general resources that are used by both the Client and the Server
    Public Structure Values 'A Structure that stores readonly values
        Public Shared ReadOnly ServicePort As Integer = 1225 'An Integer value representing the Port that the Server listens for connection requests on and the Clients connect to

    End Structure

    Public Enum GalaxyStates 'The different states of the Server's Galaxy
        Sector_Transit = Networking_Messages.General_Headers.max 'Starts at the maximum for the General headers
        Ship_To_Ship
        Shop_Interface
    End Enum
    Public Enum Allegiances 'The different allegiances in the Galaxy
        Emperial_Forces
        Pirate_Alliance
        Trade_Federation
        Contested
        Environment
        nill
        max
    End Enum
    Public Enum ObjectTypes 'The different Types of objects in the game
        Projectile
        Missile
        ShipsStart
        Screamer = ShipsStart
        Thunder
    End Enum

End Namespace