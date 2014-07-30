Public MustInherit Class Engines 'Object responsible for the routing of power through a Ship
    Inherits ShipStation
    Public Generation As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum power output
    Public Throttle As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Ship's throttle
    '-----Power Costs-----
    Public ShieldsCost As Double 'A Double value representing how much power the Shields need
    Public WeaponsCosts(-1) As Double 'An array of Double values representing how much power each Weapon needs
    '---------------------
    Public StationToRepair As ShipStation.StationTypes 'A StationTypes value indicating which ShipStation is being repaired

    Public MustOverride Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    Public Overrides Sub Update() 'Distributes power around the Ship and repairs damaged PowerNode
        Dim power As Double = Generation.Current
        If AIControled = False Then 'Repair Stations
            Select Case StationToRepair
                Case StationTypes.Helm
                    If ParentShip.Bridge.Integrity.Current <> ParentShip.Bridge.Integrity.Maximum Then 'Repair the Helm
                        If power >= ParentShip.Bridge.RepairCost Then 'There's enough power
                            power = power - ParentShip.Bridge.RepairCost
                            ParentShip.Bridge.Integrity.Current = ParentShip.Bridge.Integrity.Current + 1
                            If power = True Then
                                ParentShip.Bridge.Powered = True
                            End If
                        End If
                    End If
                Case StationTypes.Battery
                    If ParentShip.Batteries.Integrity.Current <> ParentShip.Batteries.Integrity.Maximum Then 'Repair the Batteries
                        If power >= ParentShip.Batteries.RepairCost Then 'There's enough power
                            power = power - ParentShip.Batteries.RepairCost
                            ParentShip.Batteries.Integrity.Current = ParentShip.Batteries.Integrity.Current + 1
                            If power = True Then
                                ParentShip.Batteries.Powered = True
                            End If
                        End If
                    End If
                Case StationTypes.Shields
                    If ParentShip.Shielding.Integrity.Current <> ParentShip.Shielding.Integrity.Maximum Then 'Repair the Shields
                        If power >= ParentShip.Shielding.RepairCost Then 'There's enough power
                            power = power - ParentShip.Shielding.RepairCost
                            ParentShip.Shielding.Integrity.Current = ParentShip.Shielding.Integrity.Current + 1
                            If power = True Then
                                ParentShip.Shielding.Powered = True
                            End If
                        End If
                    End If
                Case StationTypes.Engines
                    If Integrity.Current <> Integrity.Maximum Then 'Repair the Engines
                        If power >= RepairCost Then 'There's enough power
                            power = power - RepairCost
                            Integrity.Current = Integrity.Current + 1
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
        If Powered = True Then 'Power is going out to the Ship
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
