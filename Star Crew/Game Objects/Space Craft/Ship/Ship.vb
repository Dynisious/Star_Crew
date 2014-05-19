<Serializable()>
Public Class Ship 'A Ship that flies in combat and fights other Ships
    Inherits SpaceCraft 'The base Class for all Ships and Fleets
    Public InCombat As Boolean = False
    Public TargetLock As Boolean = False 'A Boolean value indecating whether the Ship is allowed to switch targets
    Public Hull As StatDbl 'A StatDbl object representing the current and max Hull values
    Public Firing As Boolean = False 'A Boolean value indecating whether the Ship is actively firing
    Public Hit As Boolean = False 'A Boolean value indecating whether the Ship has just been hit
    '-----Helm-----
    Public Helm As New Helm(Me) 'A Helm object responsible for piloting the Ship using AI
    '--------------
    '-----Batteries-----
    Public Batteries As New Battery(Me) 'A Battery object responsible for aiming Weapons and Targeting Ships using AI
    '-------------------
    '-----Shielding-----
    Public Shielding As Shields 'A Shields object responsible for distributing power between the 4 shields using AI
    '-------------------
    '-----Engineering-----
    Public Engineering As Engineering 'A Engineering object responsible for distributing power between the other 3 stations and
    'repairing Ship systems
    '---------------------

    Public Sub New(ByVal nShipStats As ShipLayout, ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence)
        MyBase.New(nAllegence, nShipStats.Format, nIndex, New Point(Int(SpawnBox * Rnd()), Int(SpawnBox * Rnd())))
        nShipStats.Initialise(Me) 'Sets the Stats of the Ship
    End Sub

    Public Sub TakeDamage(ByRef nWeapon As Weapon, ByRef shooter As Ship, ByVal incomingVector As Double) 'Calculates how much damage is done to the
        'Hull
        Dim sideHit As Shields.Sides 'The side of the ship that has been hit
        incomingVector = Helm.NormalizeDirection(incomingVector - Direction + Math.PI) 'Invert the direction and make it relative
        'to the Ship's orientation

        '-----Calculate which side has been hit-----
        If incomingVector <= Math.PI / 4 Or incomingVector >= (7 * Math.PI) / 4 Then 'Fore
            sideHit = Shields.Sides.FrontShield
        ElseIf incomingVector >= Math.PI / 4 And incomingVector <= (3 * Math.PI) / 4 Then 'Starboard
            sideHit = Shields.Sides.RightShield
        ElseIf incomingVector >= (3 * Math.PI) / 4 And incomingVector <= (5 * Math.PI) / 4 Then 'Aft
            sideHit = Shields.Sides.BackShield
        ElseIf incomingVector >= (5 * Math.PI) / 4 And incomingVector <= (7 * Math.PI) / 4 Then 'Port
            sideHit = Shields.Sides.LeftShield
        End If
        '-------------------------------------------

        Dim incomingDamage As Integer = Shielding.DeflectHit(sideHit, nWeapon) 'The Damage that doesn't get absorbed by the shields
        Hull.current = Hull.current - incomingDamage 'Take away the remaining damage
        Select Case sideHit
            Case Shields.Sides.FrontShield 'Some damage is done to the Primary and Secondary Weapons
                Batteries.Primary.Integrety.current = Int(Batteries.Primary.Integrety.current - (incomingDamage / 10))
                If Batteries.Primary.Integrety.current < 0 Then
                    Batteries.Primary.Integrety.current = 0
                End If
                Batteries.Primary.ChangeStats()
                Batteries.Secondary.Integrety.current = Int(Batteries.Secondary.Integrety.current - (incomingDamage / 10))
                If Batteries.Secondary.Integrety.current < 0 Then
                    Batteries.Secondary.Integrety.current = 0
                End If
                Batteries.Secondary.ChangeStats()
            Case Shields.Sides.RightShield Or Shields.Sides.LeftShield 'Some damage is done to both the Weapons, the SubSystem.Engines and the Power Core
                If Int(2 * Rnd()) = 0 Then
                    Batteries.Primary.Integrety.current = Int(Batteries.Primary.Integrety.current - (incomingDamage / 10))
                    If Batteries.Primary.Integrety.current < 0 Then
                        Batteries.Primary.Integrety.current = 0
                    End If
                    Batteries.Primary.ChangeStats()
                    Batteries.Secondary.Integrety.current = Int(Batteries.Secondary.Integrety.current - (incomingDamage / 10))
                    If Batteries.Secondary.Integrety.current < 0 Then
                        Batteries.Secondary.Integrety.current = 0
                    End If
                    Batteries.Secondary.ChangeStats()
                End If
                If Int(2 * Rnd()) = 0 Then
                    Engineering.SubSystem.Engines.current = Int(Engineering.SubSystem.Engines.current - (incomingDamage * 0.05))
                    If Engineering.SubSystem.Engines.current < 0 Then
                        Engineering.SubSystem.Engines.current = 0
                    End If
                    Engineering.SubSystem.PowerCore.current = Int(Engineering.SubSystem.PowerCore.current - (incomingDamage * 0.05))
                    If Engineering.SubSystem.PowerCore.current < 1 Then
                        Engineering.SubSystem.PowerCore.current = 1
                    End If
                End If
            Case Shields.Sides.BackShield 'Some damage is done to the SubSystem.Engines and Power Core
                Engineering.SubSystem.Engines.current = Int(Engineering.SubSystem.Engines.current - (incomingDamage * 0.05))
                If Engineering.SubSystem.Engines.current < 0 Then
                    Engineering.SubSystem.Engines.current = 0
                End If
                Engineering.SubSystem.PowerCore.current = Int(Engineering.SubSystem.PowerCore.current - (incomingDamage * 0.05))
                If Engineering.SubSystem.PowerCore.current < 1 Then
                    Engineering.SubSystem.PowerCore.current = 1
                End If
        End Select
        Hit = True 'The Ship has just been hit
        If Hull.current <= 0 Then 'Ship is dead and needs to be destroyed
            DestroyShip() 'Destroys the Ship
        End If
    End Sub

    Public Overridable Sub DestroyShip() 'Removes all references to the Ship and all references within it's stations
        If Dead = False Then 'The ship hasn't already been destroyed
            If InCombat = True Then 'The Ship must be removed from the combat cenario's list
                ConsoleWindow.GameServer.GameWorld.CombatSpace.RemoveShip(Me) 'Remove Ship
                If ReferenceEquals(ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip, Me) = True Then 'There needs to be a new Player Ship
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.Recenter() 'Try to take over another Ship
                End If
                InCombat = False
            End If
            Helm.Parent = Nothing 'Remove the reference to the Ship in Helm
            Batteries.Parent = Nothing 'Remove the reference to the Ship in Batteries
            Batteries.Primary.Parent = Nothing 'Remove the reference to the Battery object in the Batteries.Primary Weapon object
            Batteries.Secondary.Parent = Nothing 'Remove the reference to the Battery object in the Batteries.Secondary Weapon object
            Shielding.Parent = Nothing 'Remove the reference to the Ship in Shielding
            Engineering.Parent = Nothing 'Remove the reference to the Ship in Engineering
            Dead = True 'Set the Dead Boolean to true to mean the Ship is dead
        End If
    End Sub

    Public Overridable Sub UpdateShip() 'Updates the Ship
        Hit = False 'Reset the Value
        Firing = False 'Reset the Value
        Batteries.Update() 'Selects the Best Target and turns the Weapons to face it
        Engineering.Update() 'Distributes power to all Stations and repairs damaged systems
        Shielding.Update() 'Distribute power among the 4 shields
        Helm.Update() 'Pilots the Ship towards the selected target
        Position.X = Position.X + (Math.Cos(Direction) * (Speed.current * (Engineering.SubSystem.Engines.current / Engineering.SubSystem.Engines.max)))
        'Sets the new X coordinate
        Position.Y = Position.Y + (Math.Sin(Direction) * (Speed.current * (Engineering.SubSystem.Engines.current / Engineering.SubSystem.Engines.max)))
        'Sets the new Y coordinate
    End Sub

End Class
