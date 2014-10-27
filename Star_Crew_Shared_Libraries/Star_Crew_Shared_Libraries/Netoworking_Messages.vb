﻿Namespace Networking_Messages 'A Namespace of Classes that are sent/received to/from connected Clients
    Public Enum General_Headers 'An enumerator of messages that can be sent/received to/from the Client
        Client_Connection_Successful 'This is sent to the Client when they succesfully connect to the Server
        Station_Already_Manned_Exception 'This is sent to the Client when they try to connect to a Station that is already manned
        Server_Closed_Exception 'This is sent to the Client when the Server they're connected to closes
        Client_Kicked_Exception 'This is sent to the Client when they get kicked from the Server
        Bad_Message_Exception 'This is sent to/from the Client to/from the Server to specify that a message received was not valid
        Client_Disconnecting 'This is sent to the Server when a Client is going to disconnect
        max 'The maximum value for the enumorator
    End Enum
    Public Enum Server_Message_Header 'The types of messages sent to the Client
        Ship_To_Ship = General_Headers.max 'The message is a Ship to Ship message
    End Enum
    Public Enum Ship_Control_Header 'An enumerator of message Types that are sent to control the Clients' Ship
        Throttle_Up = General_Headers.max 'Increase the Ship's throttle
        Throttle_Down 'Decrease the Ship's throttle
        Turn_Right 'Turn the Ship to the right
        Turn_Left 'Turn the Ship to the left
        Fire_Weapons 'Fire the Weapon
    End Enum

    <Serializable()>
    Public Class SectorView 'The message sent to Clients when the Galaxy is in Sector_Transit state
        Public Enum SectorObjects 'An enumerator of the different objects that can be seen in a Sector
            Planet1
            PlanetMax
            Fleet = PlanetMax
            SpaceStation
            max
        End Enum
        Public Types() As SectorObjects 'An array of SectorObject values representing what type of object needs to be displayed
        Public Positions() As System.Drawing.Point 'An array of Point objects representing the positions of the objects in the Sector
        Public Allegiancies() As Shared_Values.Allegiances 'An array of Shared_Values.Allegiances values indicating where the objects in the Sector are aligned

        Public Sub New(ByRef nTypes() As SectorObjects, ByRef nPositions() As System.Drawing.Point, ByRef nAllegiances() As Shared_Values.Allegiances) 'Creates a new SectorView object
            Types = nTypes
            Positions = nPositions
            Allegiancies = nAllegiances
        End Sub

    End Class

End Namespace