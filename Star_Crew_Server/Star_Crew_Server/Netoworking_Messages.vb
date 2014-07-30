Namespace Networking_Messages 'A Namespace of Classes that are sent/received to/from connected Clients

    Public Class SectorView 'The message sent to Clients when the Galaxy is in Sector_Transit state
        Public Enum SectorObjects 'An enumerator of the different objects that can be seen in a Sector
            Planet
            Fleet
            SpaceStation
        End Enum
        Public Types() As SectorObjects 'An array of SectorObject values representing what type of object needs to be displayed
        Public Positions() As System.Drawing.Point 'An array of Point objects representing the positions of the objects in the Sector
        Public Allegiancies() As Galaxy.Allegiances 'An array of Galaxy.Allegiances values indicating where the objects in the Sector are aligned

        Public Sub New(ByRef nTypes() As SectorObjects, ByRef nPositions() As System.Drawing.Point, ByRef nAllegiances() As Galaxy.Allegiances) 'Creates a new SectorView object
            Types = nTypes
            Positions = nPositions
            Allegiancies = nAllegiances
        End Sub

    End Class

End Namespace