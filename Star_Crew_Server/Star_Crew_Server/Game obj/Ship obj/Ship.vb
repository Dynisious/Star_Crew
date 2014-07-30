Public MustInherit Class Ship 'Object that combats other Ships
    Inherits Game_Library.Entity
    Public ParentFleet As Fleet 'The Fleet object that the Ship is a part of
    Public myAllegiance As Galaxy.Allegiances = Galaxy.Allegiances.max 'An Integer value representing the Allegiance of the Ship
    Public ReadOnly CruiseSpeed As Int16 = 5 'An Int16 value representing the default speed for the Ship to move at
    Public MinimumDistance As Integer 'An Integer value representing the distance this Ship wants to keep from other Ships
    Public MaximumHull As Double 'A Double value representing the maximum Hull value
    Public Hull As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values of the Ship's Hull
    Public Acceleration As Double 'A Double value representing the acceleration of the Ship
    Public Speed As Double 'The Ship's current speed
    Public TurnSpeed As Double 'The radians that the Ship can rotate per update
    Public Direction As Double 'The Direction of the Ship in world space in radians
    Public FleetIndex As Integer 'The index of the Ship in it's parent Fleet
    Public CombatIndex As Integer 'The index of the Ship in the game's CombatSpace's list of Ships
    Public Firing As Boolean 'A Boolean value indecating whether the Ship fired this Update
    Public Hit As Boolean 'A Boolean value indicating whether the Ship has been hit this Update
    Public InCombat As Boolean 'A Boolean value indecating whether the Ship is actively in Ship to Ship combat
    Public Bridge As Helm 'A Helm object responsible for piloting the Ship
    Public Batteries As Battery 'A Battery object responsible for aiming and firing the Ships weapons
    Public Shielding As Shields 'A Shield object responsible for powering the Ship's Shields
    Public Engineering As Engines 'An Engines object responsible for routing power through the Ship
    Public target As Integer 'An Integer value representing the CombatIndex of the Ship this Ship is targeting
    Public targetDirection As Double 'A Double value representing the direction of the target in world space
    Public targetDistance As Integer 'An Integer value representing the distance of the target from the Ship
    Public Mounts() As WeaponMount 'An array of WeaponMount objects where Weapons can be fitted

    Public Sub Take_Damage(ByVal IncomingVector As Double, ByVal IncomingDamage As Double, ByVal Type As Weapon.DamageTypes) 'Calculates how much damage is done to the Ship
        IncomingDamage = Shielding.Deflect_Damage(IncomingVector, IncomingDamage, Type) 'Pass the damage through the Shield
        Hull.Current = Hull.Current - IncomingDamage 'Take the remaining damage away from the hull
        If Hull.Current <= 0 Then 'Destroy the Ship
            Destroy()
        ElseIf IncomingDamage <> 0 Then 'Do damage to the ShipStations
            Dim hitStation As ShipStation.StationTypes = Int(Server.Normalise_Direction(IncomingVector + (Math.PI / 4)) / (Math.PI / 2)) 'Which Station got hit
            If hitStation = ShipStation.StationTypes.max Then hitStation = hitStation - 1
            Select Case hitStation
                Case ShipStation.StationTypes.Helm
                    Bridge.Integrity.Current = Bridge.Integrity.Current - IncomingDamage
                    If Bridge.Integrity.Current <= 0 Then
                        Bridge.Integrity.Current = 0
                        Bridge.Powered = False
                    End If
                Case ShipStation.StationTypes.Battery
                    Batteries.Integrity.Current = Batteries.Integrity.Current - IncomingDamage
                    If Batteries.Integrity.Current <= 0 Then
                        Batteries.Integrity.Current = 0
                        Batteries.Powered = False
                    End If
                    For Each i As WeaponMount In Mounts
                        If i.MountedWeapon IsNot Nothing Then 'There's a mounted Weapon
                            i.MountedWeapon.Integrity.Current = i.MountedWeapon.Integrity.Current - IncomingDamage
                            If i.MountedWeapon.Integrity.Current < 0 Then
                                i.MountedWeapon.Integrity.Current = 0
                            End If
                            Dim percentage As Double = (i.MountedWeapon.Integrity.Current / i.MountedWeapon.Integrity.Maximum) 'The percentage of the Weapons integrity
                            i.MountedWeapon.Damage.Current = i.MountedWeapon.Damage.Maximum * percentage 'Set the damage relative to the integrity of the Ship
                            If i.MountedWeapon.Damage.Current < i.MountedWeapon.Damage.Minimum Then 'Set the damage to the minimum
                                i.MountedWeapon.Damage.Current = i.MountedWeapon.Damage.Minimum
                            End If
                            i.MountedWeapon.Range.Current = i.MountedWeapon.Range.Maximum * percentage 'Set the range relative to the integrity of the Ship
                            If i.MountedWeapon.Range.Current < i.MountedWeapon.Range.Minimum Then 'Set the range to the minimum
                                i.MountedWeapon.Range.Current = i.MountedWeapon.Range.Minimum
                            End If
                        End If
                    Next
                Case ShipStation.StationTypes.Shields
                    Shielding.Integrity.Current = Shielding.Integrity.Current - IncomingDamage
                    If Shielding.Integrity.Current <= 0 Then
                        Shielding.Integrity.Current = 0
                        Shielding.Powered = False
                    End If
                Case ShipStation.StationTypes.Engines
                    Engineering.Integrity.Current = Engineering.Integrity.Current - IncomingDamage
                    If Engineering.Integrity.Current <= 0 Then
                        Engineering.Integrity.Current = 0
                        Engineering.Powered = False
                        Batteries.Powered = False
                        Shielding.Powered = False
                        Bridge.Powered = False
                    End If
            End Select
        End If
    End Sub

    Public Overrides Sub Destroy() 'Removes all references to and within the Ship
        Dead = True 'The Ship is dead
        If InCombat = True Then 'The Ship must remove itself from the CombatSpace's list
            Server.GameWorld.Combat.Remove_Ship(CombatIndex) 'Remove the Ship
            If CombatIndex = Server.GameWorld.Combat.ClientShip.CombatIndex Then 'This is the ClientShip
                Server.GameWorld.Combat.Recentre() 'Set a new ClientShip
            End If
            InCombat = False
        End If
        ParentFleet.Remove_Ship(FleetIndex) 'Remove the Ship from the Fleet
        ParentFleet = Nothing 'Clear the reference
        Bridge.ParentShip = Nothing 'Remove the reference to the Ship
        Bridge = Nothing 'Clear the reference
        Batteries.ParentShip = Nothing 'Remove the reference to the Ship
        Batteries = Nothing 'Clear the reference
        Shielding.ParentShip = Nothing 'Remove the reference to the Ship
        Shielding = Nothing 'Clear the reference
        Engineering.ParentShip = Nothing 'Remove the reference to the Ship
        Engineering = Nothing 'Clear the reference
        For Each i As WeaponMount In Mounts
            i.MountedWeapon.ParentShip = Nothing 'Remove the reference to the Ship
        Next
        ReDim Mounts(-1) 'Clear the array
    End Sub

    Public Overrides Sub Update() 'Updates the Ship
        Firing = False 'Reset the indicator of whether the Ship is firing
        Hit = False 'Reset the indicator of whether the Ship has been hit
        Speed = (Engineering.Integrity.Current / Engineering.Integrity.Maximum) * Engineering.Throttle.Current 'Set the Speed of the Ship
        target = -1
        X = X + (Speed * Math.Cos(Direction)) 'Update the Ship's X position
        Y = Y + (Speed * Math.Sin(Direction)) 'Update the Ship's Y position
    End Sub

End Class
