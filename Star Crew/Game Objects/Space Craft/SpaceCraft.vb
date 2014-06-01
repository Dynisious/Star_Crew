<Serializable()>
Public MustInherit Class SpaceCraft 'Holds the values within all SpaceCraft and represents an object that can fly in combat or in a Sector
    Public Format As ShipLayout.Formats 'An Enumorator representing what layout the SpaceCraft is taking on
    Public Position As Point 'A Point Object representing the SpaceCrafts position in (X,Y) Space
    Public Index As Integer 'An Integer representing the SpaceCrafts index in either it's Sector or Combat
    Public MyAllegence As Galaxy.Allegence 'The SpaceCrafts allegience as Galaxy.Allegence
    Public Shared ReadOnly SpawnBox As Integer = 3000 'The Square in which a SpaceCraft can spawn
    Public Direction As Double 'A Double representing the SpaceCraft's direction of travel
    Public Speed As New StatDbl(0, 0) 'A StatDbl object repesenting the SpaceCraft's current and max speeds
    Public Acceleration As New StatDbl(0, 0) 'The rate at which the SpaceCraft changes it's Speed
    Public Dead As Boolean = False 'A Boolean Value indecating whether or not the SpaceCraft is Active or waiting to be garbage collected
    Public TargetLock As Boolean = False 'The SpaceCraft can change its own target
    Public Target As SpaceCraft 'The SpaceCraft's target

    Public Sub New(ByVal nAllegence As Galaxy.Allegence, ByVal nFormat As ShipLayout.Formats, ByVal nIndex As Integer, ByVal nPosition As Point)
        MyAllegence = nAllegence 'Set the Allegence of the SpaceCraft
        Format = nFormat 'Set the layout of the SpaceCraft for graphics
        Index = nIndex 'Set the index of the SpaceCraft
        Position = nPosition 'Set the position of the SpaceCraft
    End Sub

End Class
