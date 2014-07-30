Public MustInherit Class Weapon 'Object that represents a Weapon on a Ship
    Public Influx As Double 'How much power this Weapon gets to use per Update
    Public Index As Int16 'An Int16 value representing the index of the WeaponMount in the list of WeaponMounts on the Ship
    Public ParentShip As Ship 'A reference to the Ship that the Weapon is on
    Public Range As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ranges of the Weapon
    Public Ammunition As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ammunition count
    Public Ready As Game_Library.StatInt 'A StatInt object used to control the rate of fire of the Weapon
    Public Damage As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum damage values
    Public PowerCost As Double 'A Double value representing how much power it costs to generate ammunition for this Weapon
    Public RepairCost As Double 'A Double value representing how much power is required to repair 1 integrity
    Public TurningSpeed As Double 'A Double value representing how quickly the Weapon turns
    Public Integrity As Game_Library.StatInt 'A StatInt object representing the integrity of the Weapon
    Public Type As DamageTypes 'A DamageTypes value representing what type of damage this Weapon does
    Public Enum DamageTypes
        Pulse
        max
    End Enum

    Public Sub Fire_Weapon(ByRef target As Ship) 'Checks if the Weapon is ready and able to fire, then calls Do_Damage() if it is
        If Integrity.Current <> 0 And
            Ammunition.Current <> 0 And
            Ready.Current = Ready.Maximum Then 'The Weapon is ready to fire
            Do_Damage(target) 'Do damage
            Ammunition.Current = Ammunition.Current - 1
            Ready.Current = 0
        End If
    End Sub

    Protected MustOverride Sub Do_Damage(ByRef target As Ship) 'Does damage to enemy Ships

    Public Sub Update() 'Generates ammunition for the Weapon and updates how many more updates before the Weapon can fire
        ParentShip.Engineering.WeaponsCosts(Index) = 0 'Clear the cost for this Weapon
        If Ready.Current <> Ready.Maximum Then 'Update the count
            Ready.Current = Ready.Current + 1 'Count another tick
        End If
        If Ammunition.Current <> Ammunition.Maximum And Influx >= PowerCost Then 'Generate ammunition
            Ammunition.Current = Ammunition.Current + 1 'Add one more shot to the ammunition count
            Influx = Influx - PowerCost 'Remove the required power from the
        End If
        If Integrity.Current <> Integrity.Maximum And Influx >= RepairCost Then 'Repair the Weapon
            Integrity.Current = Integrity.Current + Int(Influx / RepairCost) 'Repair the Weapon
            If Integrity.Current > Integrity.Maximum Then 'Set the integrity to the maximum
                Integrity.Current = Integrity.Maximum
            End If
            Dim percentage As Double = (Integrity.Current / Integrity.Maximum) 'The percentage of the Weapons integrity
            Damage.Current = Damage.Maximum * percentage 'Set the damage relative to the integrity of the Ship
            If Damage.Current < Damage.Minimum Then 'Set the damage to the minimum
                Damage.Current = Damage.Minimum
            End If
            Range.Current = Range.Maximum * percentage 'Set the range relative to the integrity of the Ship
            If Range.Current < Range.Minimum Then 'Set the range to the minimum
                Range.Current = Range.Minimum
            End If
        End If
        If ParentShip.Engineering.AIControled = True Then 'Engineering is AI Controlled
            If Ammunition.Current <> Ammunition.Maximum Then ParentShip.Engineering.WeaponsCosts(Index) = PowerCost 'Set the Cost for the Weapon
            If Integrity.Current <> Integrity.Maximum Then ParentShip.Engineering.WeaponsCosts(Index) = (ParentShip.Engineering.WeaponsCosts(Index) + ((Integrity.Maximum - Integrity.Current) * RepairCost)) 'Send off the repair cost
        End If
        Influx = 0
    End Sub

    Public Sub Destroy() 'Removes all references to and within the Weapon
        ParentShip.Mounts(Index) = Nothing 'Remove the reference to the Weapon
        ParentShip = Nothing 'Clear the reference
    End Sub

    Public MustOverride Function To_Item() 'Converts the Weapon to an item and stores it in the Fleet's invetory

End Class
