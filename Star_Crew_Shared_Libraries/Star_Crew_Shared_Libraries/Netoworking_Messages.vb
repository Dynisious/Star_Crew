Namespace Networking_Messages 'A Namespace of Classes that are sent/received to/from connected Clients
    Public Enum General_Headers 'An enumerator of messages that can be sent/received to/from the Client
        Server_Closed_Exception 'This is sent to the Client when the Server they're connected to closes
        Client_Kicked_Exception 'This is sent to the Client when they get kicked from the Server
        Bad_Message_Exception 'This is sent to/from the Client to/from the Server to specify that a message received was not valid
        Client_Disconnecting 'This is sent to the Server when a Client is going to disconnect
        Ping_Check 'Signals that this message represents a ping Check
        Client_Lost 'The Client has lost and is now being disconnected
        Exceeded_Max_Ping_Exception 'This is sent to the Client when the Client's ping is too high
        max 'The maximum value for the enumorator
    End Enum
    Public Enum Ship_Control_Header 'An enumerator of message Types that are sent to control the Clients' Ship
        Throttle_Up = General_Headers.max 'Increase the Ship's throttle
        Throttle_Down 'Decrease the Ship's throttle
        Turn_Right 'Turn the Ship to the right
        Turn_Left 'Turn the Ship to the left
        Fire_Primary 'Fire the Weapon
        Fire_Missiles 'Fire the Missile
        Heal_Ship 'Heals the Ship
        Re_Arm 'Rearms the Ship
    End Enum

End Namespace