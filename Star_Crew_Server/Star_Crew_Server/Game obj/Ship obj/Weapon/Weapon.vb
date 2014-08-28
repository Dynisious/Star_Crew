Public MustInherit Class Weapon 'Object that represents a Weapon on a Ship
    Public Influx As Double 'How much power this Weapon gets to use per Update
    Private _Index As Int16 'The actual value of Index
    Public ReadOnly Property Index As Int16 'An Int16 value representing the index of the WeaponMount in the list of WeaponMounts on the Ship
        Get
            Return _Index
        End Get
    End Property
    Private _ParentShip As Ship 'The actual value of ParentShip
    Public ReadOnly Property ParentShip As Ship 'A reference to the Ship that the Weapon is on
        Get
            Return _ParentShip
        End Get
    End Property
    Public Range As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ranges of the Weapon
    Public Ammunition As Game_Library.StatInt 'A StatInt object representing the minimum, current and maximum ammunition count
    Public Ready As Game_Library.StatInt 'A StatInt object used to control the rate of fire of the Weapon
    Public Damage As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum damage values
    Private _PowerCost As Double 'The actual value of PowerCost
    Public ReadOnly Property PowerCost As Double 'A Double value representing how much power it costs to generate ammunition for this Weapon
        Get
            Return _PowerCost
        End Get
    End Property
    Private _RepairCost As Double 'The actual value of RepairCose
    Public ReadOnly Property RepairCost As Double 'A Double value representing how much power is required to repair 1 integrity
        Get
            Return _RepairCost
        End Get
    End Property
    Private _TurningSpeed As Double 'The actual value of TurningSpeed
    Public ReadOnly Property TurningSpeed As Double 'A Double value representing how quickly the Weapon turns
        Get
            Return _TurningSpeed
        End Get
    End Property
    Public Integrity As Game_Library.StatInt 'A StatInt object representing the integrity of the Weapon
    Private _Type As DamageTypes 'The actual value of Type
    Public ReadOnly Property Type As DamageTypes 'A DamageTypes value representing what type of damage this Weapon does
        Get
            Return _Type
        End Get
    End Property
    Public Enum DamageTypes
        Pulse
        max
    End Enum

    Public Sub New(ByRef nParent As Ship, ByVal nIndex As Integer, ByVal nType As DamageTypes, ByVal nPowerCost As Double, ByVal nTurningSpeed As Double)
        _ParentShip = nParent
        _Index = nIndex
        _Type = nType
        _PowerCost = nPowerCost
        _TurningSpeed = nTurningSpeed
    End Sub

    Public Sub Fire_Weapon(ByRef target As Ship, ByVal firingDirection As Double, ByVal targetDistance As Integer) 'Checks if the Weapon is ready and able to fire, then calls Do_Damage() if it is
        If Integrity.Current <> 0 And
            Ammunition.Current <> 0 And
            Ready.Current = Ready.Maximum Then 'The Weapon is ready to fire
            Do_Damage(target, firingDirection, targetDistance) 'Do damage
        End If
    End Sub

    Protected MustOverride Sub Do_Damage(ByRef target As Ship, ByVal firingDirection As Double, ByVal targetDistance As Integer) 'Does damage to enemy Ships

    Public Sub Update() 'Generates ammunition for the Weapon and updates how many more updates before the Weapon can fire
        If Ready.Current <> Ready.Maximum Then 'Update the count
            Ready.Current = Ready.Current + 1 'Count another tick
        End If
        If Ammunition.Current <> Ammunition.Maximum And Influx >= PowerCost Then 'Generate ammunition
            Ammunition.Current = Ammunition.Current + 1 'Add one more shot to the ammunition count
            Influx = Influx - PowerCost 'Remove the required power from the
        End If
        If Integrity.Current <> Integrity.Maximum And Influx >= RepairCost Then 'Repair the Weapon
            Integrity.Current = Integrity.Current + (Influx / RepairCost) 'Repair the Weapon
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
            ParentShip.Engineering.WeaponsCosts(Index) = 0 'Clear the cost for this Weapon
            If Ammunition.Current <> Ammunition.Maximum Then ParentShip.Engineering.WeaponsCosts(Index) = PowerCost 'Set the Cost for the Weapon
            If Integrity.Current <> Integrity.Maximum Then ParentShip.Engineering.WeaponsCosts(Index) = (ParentShip.Engineering.WeaponsCosts(Index) + ((Integrity.Maximum - Integrity.Current) * RepairCost)) 'Send off the repair cost
        End If
        Influx = 0
    End Sub

    Public Sub Destroy() 'Removes all references to and within the Weapon
        ParentShip.Mounts(Index).MountedWeapon = Nothing 'Remove the reference to the Weapon
        _Index = -1 'Clear the index
        _ParentShip = Nothing 'Clear the reference
    End Sub

    Public MustOverride Function To_Item() 'Converts the Weapon to an item and stores it in the Fleet's invetory

End Class
