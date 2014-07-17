Public MustInherit Class Weapon 'Object that represents a Weapon on a Ship
    Public Influx As Double 'How much power this Weapon gets to use per Update
    Public Range As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ranges of the Weapon
    Public Ammunition As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ammunition count
    Public Ready As Game_Library.StatInt 'A StatInt object used to control the rate of fire of the Weapon
    Public Damage As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum damage values
    Public PowerCost As Double 'A Double value representing how much power it costs to generate ammunition for this Weapon
    Public TurningSpeed As Double 'A Double value representing how quickly the Weapon turns
    Public Enum DamageTypes
        Laser
        max
    End Enum

    Public Sub Fire_Weapon() 'Checks if the Weapon is ready and able to fire, then calls Do_Damage() if it is

    End Sub

    Protected MustOverride Sub Do_Damage() 'Does damage to enemy Ships

    Public Sub Update() 'Generates ammunition for the Weapon and updates how many more updates before the Weapon can fire

    End Sub

    Public Sub Destroy() 'Removes all references to and within the Weapon

    End Sub

    Public MustOverride Function To_Item() 'Converts the Weapon to an item and stores it in the Fleet's invetory

End Class
