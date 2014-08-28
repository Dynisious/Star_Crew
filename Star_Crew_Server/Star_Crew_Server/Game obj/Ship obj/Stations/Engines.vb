Public MustInherit Class Engines 'Object responsible for the routing of power through a Ship
    Inherits ShipStation
    Public Generation As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum power output
    Public Throttle As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Ship's throttle
    '-----Power Costs-----
    Public ShieldsCost As Double 'A Double value representing how much power the Shields need
    Public WeaponsCosts(-1) As Double 'An array of Double values representing how much power each Weapon needs
    '---------------------

    Public Sub New(ByRef nParent As Ship, ByVal nIntegrity As Game_Library.StatInt, ByVal nRepairCost As Double, ByVal nGeneration As Game_Library.StatDbl, ByVal nThrottle As Game_Library.StatDbl)
        MyBase.New(nParent, nIntegrity, nRepairCost)
        Generation = nGeneration
        Throttle = nThrottle
    End Sub

    Protected Overrides Sub Finalise_Destroy() 'Removes all references to the Engines object
        ParentShip.Engineering = Nothing 'Remove the reference
    End Sub

    Public MustOverride Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    Public Shared Sub Client_Power_Distribution(ByVal buff() As Byte) 'Sets the Client's power distribution
        Dim Shields(3) As Byte '4 Bytes for an integer representing the power to be distributed to the Shields
        Dim Weapons((4 * Server.GameWorld.Combat.ClientShip.Mounts.Length) - 1) As Byte
        Array.ConstrainedCopy(buff, 0, Shields, 0, 4) 'Get 4 Bytes representing the power to Shields
        Array.ConstrainedCopy(buff, 4, Weapons, 0, buff.Length - 4) 'Get the remaining Bytes representing the power to the Weapons
        Server.GameWorld.Combat.ClientShip.Engineering.ShieldsCost = BitConverter.ToInt32(Shields, 0) 'Set the power to be distributed to Shields
        For i As Integer = 0 To Server.GameWorld.Combat.ClientShip.Mounts.Length - 1 'Loop through each WeaponMount
            If Server.GameWorld.Combat.ClientShip.Mounts(i).MountedWeapon IsNot Nothing Then 'There is a mounted Weapon
                Dim temp(3) As Byte 'An array of 4 Bytes to store the Bytes to be converted
                Array.ConstrainedCopy(Weapons, (i * 4), temp, 0, 4) 'Get the Bytes from the array
                Server.GameWorld.Combat.ClientShip.Engineering.WeaponsCosts(i) = BitConverter.ToInt32(temp, 0) 'Set the power to be distributed to the Weapon
            End If
        Next
    End Sub

    Public Overrides Sub Update() 'Distributes power around the Ship and repairs damaged PowerNode
        If Powered = True Then 'Power is going out to the Ship
            Dim power As Double = Generation.Current 'The power the Ship gets to use this Update
            If AIControled = False Then 'Repair Stations
                Select Case Server.GameWorld.ClientInteractions.StationToRepair 'Select which station to repair
                    Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Helm
                        If ParentShip.Bridge.Integrity.Current <> ParentShip.Bridge.Integrity.Maximum Then 'Repair the Helm
                            If power >= ParentShip.Bridge.RepairCost Then 'There's enough power
                                power = power - ParentShip.Bridge.RepairCost 'Remove the power cost
                                ParentShip.Bridge.Integrity.Current = ParentShip.Bridge.Integrity.Current + 1 'Add one integrety
                                If Powered = True Then 'The engines can route Power
                                    ParentShip.Bridge.Powered = True 'Power the Helm
                                End If
                            End If
                        End If
                    Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Battery
                        If ParentShip.Batteries.Integrity.Current <> ParentShip.Batteries.Integrity.Maximum Then 'Repair the Batteries
                            If power >= ParentShip.Batteries.RepairCost Then 'There's enough power
                                power = power - ParentShip.Batteries.RepairCost 'Remove the power cost
                                ParentShip.Batteries.Integrity.Current = ParentShip.Batteries.Integrity.Current + 1 'Add one integrity
                                If Powered = True Then 'The engines can route power
                                    ParentShip.Batteries.Powered = True 'Power the Battery
                                End If
                            End If
                        End If
                    Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Shields
                        If ParentShip.Shielding.Integrity.Current <> ParentShip.Shielding.Integrity.Maximum Then 'Repair the Shields
                            If power >= ParentShip.Shielding.RepairCost Then 'There's enough power
                                power = power - ParentShip.Shielding.RepairCost 'Remove the power cost
                                ParentShip.Shielding.Integrity.Current = ParentShip.Shielding.Integrity.Current + 1 'Add one integrity
                                If Powered = True Then 'The engines can route power
                                    ParentShip.Shielding.Powered = True 'Power the Shields
                                End If
                            End If
                        End If
                    Case Star_Crew_Shared_Libraries.Shared_Values.StationTypes.Engines
                        If Integrity.Current <> Integrity.Maximum Then 'Repair the Engines
                            If power >= RepairCost Then 'There's enough power
                                power = power - RepairCost 'Remove the power cost
                                Integrity.Current = Integrity.Current + 1 'Add one integrity
                                Dim percentage As Double = (Integrity.Current / Integrity.Maximum) 'The percentage of the Engines integrety
                                Generation.Current = (Generation.Maximum * percentage)
                                If Generation.Current < Generation.Minimum Then Generation.Current = Generation.Minimum 'Set the Generation value to the minimum
                                Powered = True
                                If ParentShip.Bridge.Integrity.Current <> 0 Then
                                    ParentShip.Bridge.Powered = True
                                End If
                                If ParentShip.Batteries.Integrity.Current <> 0 Then
                                    ParentShip.Batteries.Powered = True
                                End If
                                If ParentShip.Shielding.Integrity.Current <> 0 Then
                                    ParentShip.Shielding.Powered = True
                                End If
                            End If
                        End If
                End Select
            End If

            Dim totalPowerCost As Double 'The total demand on power this update
            If ParentShip.Shielding.Powered = True Then 'The Shields do receive power
                totalPowerCost = ShieldsCost 'Add the power to the Shields
            Else
                ShieldsCost = 0
            End If
            If ParentShip.Batteries.Powered = True Then 'The Batteries do receive power
                For Each i As Double In WeaponsCosts
                    totalPowerCost = totalPowerCost + i 'Add the Weapons demand on power to the total cost
                Next
            Else
                For i As Integer = 0 To WeaponsCosts.Length - 1
                    WeaponsCosts(i) = 0
                Next
            End If

            ParentShip.Shielding.Influx = (ShieldsCost / totalPowerCost) * power   'Send power to the Shields
            For Each i As WeaponMount In ParentShip.Mounts 'Loop through all the Weapons
                If i.MountedWeapon IsNot Nothing Then 'There's a mounted Weapon
                    i.MountedWeapon.Influx = ((WeaponsCosts(i.MountedWeapon.Index) / totalPowerCost) * power) 'Send power to this Weapon
                End If
            Next
        End If
    End Sub

End Class
