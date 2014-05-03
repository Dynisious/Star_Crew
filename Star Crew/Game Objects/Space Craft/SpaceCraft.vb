<Serializable()>
Public MustInherit Class SpaceCraft 'Holds the values within all SpaceCraft and represents an object that can fly in combat or in a Sector
    <NonSerialized()>
    Public Index As Integer 'An Integer representing the SpaceCrafts index in either it's Sector or Combat
    <NonSerialized()>
    Public Position As Point 'A Point object representing the SpaceCrafts position in (X,Y) space
    <NonSerialized()>
    Public MyAllegence As Galaxy.Allegence 'The SpaceCrafts allegience as Galaxy.Allegence
    <NonSerialized()>
    Public Shared ReadOnly SpawnBox As Integer = 6000 'The Square in which a SpaceCraft can spawn
    <NonSerialized()>
    Public Direction As Double 'A Double representing the SpaceCraft's direction of travel
    <NonSerialized()>
    Public Speed As New StatDbl(0, 0) 'A StatDbl object repesenting the SpaceCraft's current and max speeds
    <NonSerialized()>
    Public Acceleration As New StatDbl(0, 0) 'A StatDbl object representing the SpaceCraft's current and max acceleration
    <NonSerialized()>
    Public Dead As Boolean = False 'A Boolean Value indecating whether or not the SpaceCraft is Active or waiting to be garbage collected

End Class
