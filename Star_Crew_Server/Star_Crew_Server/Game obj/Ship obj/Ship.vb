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
    Public Bridge As New Helm 'A Helm object responsible for piloting the Ship
    Public Batteries As New Battery 'A Battery object responsible for aiming and firing the Ships weapons
    Public Shielding As Shields 'A Shield object responsible for powering the Ship's Shields
    Public Engineering As Engines 'An Engines object responsible for routing power through the Ship
    Public target As Integer 'An Integer value representing the CombatIndex of the Ship this Ship is targeting
    Public PowerNet() As PowerNode 'An array of PowerNode objects representing the Power Network of the Ship
    Public Mounts() As WeaponMount 'An array of WeaponMount objects where Weapons can be fitted

    Public Sub Take_Damage(ByVal IncomingVector As Double, ByVal IncomingDamage As Double, ByVal Type As Weapon.DamageTypes) 'Calculates how much damage is done to the Ship
        IncomingDamage = Shielding.Deflect_Damage(IncomingVector, IncomingDamage, Type) 'Pass the damage through the Shield
        Hull.Current = Hull.Current - IncomingDamage 'Take the remaining damage away from the hull
        If Hull.Current <= 0 Then 'Destroy the Ship
            Destroy()
        ElseIf IncomingDamage <> 0 Then 'Do damage to the power network
            Dim hitNode As PowerNode 'The PowerNode that may be hit
            For Each i As PowerNode In PowerNet
                If i.Operational = True Then 'This node can be hit
                    Dim offset As Double = IncomingVector - i.Direction 'The offset of the incoming vector from the nodes direction from the center of the Ship
                    If offset < 0 Then 'The offset needs to be positive
                        offset = -offset
                    End If
                    If offset <= Math.PI / 8 Then
                        If hitNode IsNot Nothing Then 'Compare which node is most outward
                            If hitNode.Distance < i.Distance Then 'This node is the most outward
                                hitNode = i 'Assign this node
                            End If
                        Else 'Assign this node
                            hitNode = i
                        End If
                    End If
                End If
            Next
            If Int(Rnd() * 100 / IncomingDamage) = 0 Then 'The node no longer transfers power
                hitNode.Operational = False
                hitNode.Update()
            End If
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
        For Each i As PowerNode In PowerNet
            i.ParentShip = Nothing 'Remove the reference to the Ship
        Next
        ReDim PowerNet(-1) 'Clear the array
        ReDim Mounts(-1) 'Clear the array
    End Sub

    Public Overrides Sub Update() 'Updates the Ship
        Firing = False 'Reset the indicator of whether the Ship is firing
        Hit = False 'Reset the indicator of whether the Ship has been hit
        Speed = (Engineering.Integrety.Current / Engineering.Integrety.Maximum) * Engineering.Throttle.Current 'Set the Speed of the Ship
        target = -1
        X = X + (Speed * Math.Cos(Direction)) 'Update the Ship's X position
        Y = Y + (Speed * Math.Sin(Direction)) 'Update the Ship's Y position
    End Sub

End Class
