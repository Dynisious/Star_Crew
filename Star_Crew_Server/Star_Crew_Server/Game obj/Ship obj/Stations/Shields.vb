Public MustInherit Class Shields 'Object responsible for rotating the Shield to point towards a threat
    Inherits ShipStation
    Public Influx As Double 'How much power this Station gets to use per Update
    Public Shield As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Shield
    Public Direction As Double ' A Double value representing the direction the Shield is facing
    Public Sweep As Double 'A Double value representing the coverage of the Shield
    Public Powered As Boolean = True 'A Boolean value indecating whether or not the Station is receiving power
    Private Modifiers As New Dictionary(Of Weapon.DamageTypes, Double) From {
        {Weapon.DamageTypes.Laser, 1}}

    Public Function Deflect_Damage(ByVal Vector As Double, ByVal Damage As Double, ByVal Type As Weapon.DamageTypes) As Double 'Calculates the damage that breakes through the Ship's Shield and subtracts the incomming damage from the Ship's Shield

    End Function

    Public Sub Destroy() 'Removes all references to the Shields object

    End Sub

    Public Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    End Function

    Public Overrides Sub Update() 'Rotates the Ship's Shield to face the closest enemy and charges the Shield

    End Sub

End Class
