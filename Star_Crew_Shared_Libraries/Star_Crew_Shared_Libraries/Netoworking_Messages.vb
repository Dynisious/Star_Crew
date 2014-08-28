Namespace Networking_Messages 'A Namespace of Classes that are sent/received to/from connected Clients
    Public Enum General_Headers 'An enumerator of messages that can be sent/received to/from the Client
        Client_Connection_Successful 'This is sent to the Client when they succesfully connect to the Server
        Station_Already_Manned_Exception 'This is sent to the Client when they try to connect to a Station that is already manned
        Server_Closed_Exception 'This is sent to the Client when the Server they're connected to closes
        Client_Kicked_Exception 'This is sent to the Client when they get kicked from the Server
        Bad_Connection_Exception 'This is sent to the Client when they get kicked from the Server due to a bad connection
        Client_Disconnecting 'This is sent to the Server when a Client is going to disconnect
        Ship_Change 'Features of the Ship have changed
        max 'The maximum value for the enumorator
    End Enum
    Public Enum Ship_Control_Header 'An enumerator of message Types that are sent to control the Clients' Ship
        Throttle_Up = General_Headers.max 'Increase the Ship's throttle
        Throttle_Down 'Decrease the Ship's throttle
        Turn_Right 'Turn the Ship to the right
        Turn_Left 'Turn the Ship to the left
        Weapon_Right 'Rotate the Weapon to the Right
        Weapon_Left 'Rotate the Weapon to the Left
        Fire_Weapon 'Fire the Weapon
        Next_Weapon 'Go to the next Weapon in the list
        Previous_Weapon 'Go to the previous Weapon in the list
        Shield_Right 'Rotate the Shield to the Right
        Shield_Left 'Rotate the Shield to the Left
        Repair_Station 'Change which ShipStation is being repaired
        Change_Power 'Change the distribution of Power
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

    <Serializable()>
    Public Class ShipView 'The message sent to Clients when the Galaxy is in the Ship_To_Ship state
        Public Types() As Shared_Values.ShipTypes 'An array of Shared_Values.ShipTypes values representing what type of object needs to be displayed
        Public Positions() As System.Drawing.Point 'An array of Point objects representing the positions of the objects in the Combat Scenario
        Public Allegiancies() As Shared_Values.Allegiances 'An array of Shared_Values.Allegiances values indicating where the objects in the Combat Scenario are aligned

        Public Sub New(ByRef nTypes() As Shared_Values.ShipTypes, ByRef nPositions() As System.Drawing.Point, ByRef nAllegiances() As Shared_Values.Allegiances) 'Creates a new ShipView object
            Types = nTypes
            Positions = nPositions
            Allegiancies = nAllegiances
        End Sub

    End Class

End Namespace