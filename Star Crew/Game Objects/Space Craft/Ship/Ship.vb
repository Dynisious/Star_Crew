﻿<Serializable()>
Public MustInherit Class Ship
    Inherits SpaceCraft
    <NonSerialized()>
    Public InCombat As Boolean = False
    <NonSerialized()>
    Public TargetLock As Boolean = False
    Public Hull As StatDbl
    <NonSerialized()>
    Public Firing As Boolean = False
    <NonSerialized()>
    Public Hit As Boolean = False
    '-----Helm-----
    <NonSerialized()>
    Public Helm As New Helm(Me)
    '--------------
    '-----Batteries-----
    <NonSerialized()>
    Public Batteries As New Battery(Me)
    '-------------------
    '-----Shielding-----
    Public Shielding As New Shields(Me)
    '-------------------
    '-----Engineering-----
    Public Engineering As New Engineering(Me)
    '---------------------

    Public Sub New(ByVal nShipStats As Layout, ByVal nIndex As Integer, ByVal nAllegence As Galaxy.Allegence)
        MyAllegence = nAllegence
        nShipStats.SetLayout(Me)
        Index = nIndex
        Position = New Point(Int(SpawnBox * Rnd()), Int(SpawnBox * Rnd()))
    End Sub

    Public Sub TakeDamage(ByRef nWeapon As Weapon, ByRef shooter As Ship, ByVal direction As Double)
        Dim sideHit As Shields.Sides
        Dim adjacent As Integer = (nWeapon.Parent.Parent.Position.X - Position.X)
        Dim opposite As Integer = (nWeapon.Parent.Parent.Position.Y - Position.Y)
        Dim incomingVector As Double
        If adjacent <> 0 Then
            incomingVector = Math.Tanh(opposite / adjacent)
            If adjacent < 0 Then
                incomingVector = incomingVector + Math.PI
            End If
            incomingVector = Helm.NormalizeDirection(incomingVector)
        ElseIf opposite > 0 Then
            incomingVector = Math.PI / 2
        Else
            incomingVector = (3 * Math.PI) / 2
        End If
        incomingVector = Helm.NormalizeDirection(incomingVector - direction)

        If incomingVector <= Math.PI / 4 Or incomingVector >= (7 * Math.PI) / 4 Then
            sideHit = Shields.Sides.FrontShield
        ElseIf incomingVector >= Math.PI / 4 And incomingVector <= (3 * Math.PI) / 4 Then
            sideHit = Shields.Sides.RightShield
        ElseIf incomingVector >= (3 * Math.PI) / 4 And incomingVector <= (5 * Math.PI) / 4 Then
            sideHit = Shields.Sides.BackShield
        ElseIf incomingVector >= (5 * Math.PI) / 4 And incomingVector <= (7 * Math.PI) / 4 Then
            sideHit = Shields.Sides.LeftShield
        End If

        Dim incomingDamage As Integer = Shielding.DeflectHit(sideHit, nWeapon)
        Hull.current = Hull.current - incomingDamage
        Select Case sideHit
            Case Shields.Sides.FrontShield
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
            Case Shields.Sides.RightShield Or Shields.Sides.LeftShield
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
                    Engineering.Engines.current = Int(Engineering.Engines.current - (incomingDamage * 0.05))
                    If Engineering.Engines.current < 0 Then
                        Engineering.Engines.current = 0
                    End If
                    Engineering.PowerCore.current = Int(Engineering.PowerCore.current - (incomingDamage * 0.05))
                    If Engineering.PowerCore.current < 1 Then
                        Engineering.PowerCore.current = 1
                    End If
                End If
            Case Shields.Sides.BackShield
                Engineering.Engines.current = Int(Engineering.Engines.current - (incomingDamage * 0.05))
                If Engineering.Engines.current < 0 Then
                    Engineering.Engines.current = 0
                End If
                Engineering.PowerCore.current = Int(Engineering.PowerCore.current - (incomingDamage * 0.05))
                If Engineering.PowerCore.current < 1 Then
                    Engineering.PowerCore.current = 1
                End If
        End Select
        Hit = True
        If Hull.current <= 0 Then
            DestroyShip()
        End If
    End Sub

    Public Overridable Sub DestroyShip()
        If Dead = False Then
            If InCombat = True Then
                ConsoleWindow.GameServer.GameWorld.CombatSpace.RemoveShip(Me)
                If ReferenceEquals(ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip, Me) = True Then
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.Recenter()
                End If
            End If
            Helm.Parent = Nothing
            Batteries.Parent = Nothing
            Batteries.Primary.Parent = Nothing
            Batteries.Secondary.Parent = Nothing
            Shielding.Parent = Nothing
            Engineering.Parent = Nothing
            Dead = True
        End If
    End Sub

    Private Shared Event ShipUpdate()
    Public Shared Sub UpdateShip_Call()
        RaiseEvent ShipUpdate()
    End Sub
    Public Overridable Sub UpdateShip_Handle() Handles MyClass.ShipUpdate
        If InCombat = True Then
            Hit = False
            Firing = False
            Batteries.Update()
            Engineering.Update()
            Shielding.Update()
            Helm.Update()
            Position.X = Position.X + (Math.Cos(Direction) * (Speed.current * (Engineering.Engines.current / Engineering.Engines.max)))
            Position.Y = Position.Y + (Math.Sin(Direction) * (Speed.current * (Engineering.Engines.current / Engineering.Engines.max)))
        End If
    End Sub

End Class