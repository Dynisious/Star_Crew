<Serializable()>
Public MustInherit Class Ship
    <NonSerialized()>
    Public Parent As Galaxy
    Public hit As Boolean = False
    Public Enum Allegence
        Player
        Pirate
    End Enum
    Public MyAllegence As Allegence
    Public Event ShipUpdate()
    Public Hull As Stat
    Public Position As Point
    Public Target As Ship
    '-----Helm-----
    Public Helm As New Helm(Me)
    '--------------
    '-----Batteries-----
    Public Batteries As New Battery(Me)
    '-------------------
    '-----Shielding-----
    Public Shielding As New Shields(Me)
    '-------------------
    '-----Engineering-----
    Public Engineering As New Engineering(Me)
    '---------------------

    Public Sub New(ByRef nParent As Galaxy, ByVal nShipStats As Layout, ByVal nAllegence As Allegence)
        Parent = nParent
        MyAllegence = nAllegence
        nShipStats.SetLayout(Me)
    End Sub

    Public Sub TakeDamage(ByRef nWeapon As Weapon, ByRef shooter As Ship)
        Dim sideHit As Shields.Sides
        Dim adjacent As Integer = (Position.X - nWeapon.Parent.Parent.Position.X)
        Dim opposite As Integer = (Position.Y - nWeapon.Parent.Parent.Position.Y)
        Dim incomingVector As Double = Math.Tanh(opposite / adjacent)
        If adjacent > 0 Then
            incomingVector = incomingVector + Math.PI
        End If
        incomingVector = Helm.NormalizeDirection(incomingVector)

        If incomingVector >= (Math.PI / 4) And incomingVector <= ((3 * Math.PI) / 4) Then
            sideHit = Shields.Sides.FrontShield
        ElseIf incomingVector >= ((3 * Math.PI) / 4) And incomingVector <= ((5 * Math.PI) / 4) Then
            sideHit = Shields.Sides.LeftShield
        ElseIf incomingVector >= ((5 * Math.PI) / 4) And incomingVector <= ((7 * Math.PI) / 4) Then
            sideHit = Shields.Sides.BackShield
        ElseIf incomingVector >= ((7 * Math.PI) / 4) And incomingVector <= (Math.PI / 4) Then
            sideHit = Shields.Sides.RightShield
        End If
        Dim incomingDamage As Integer = Shielding.DeflectHit(sideHit, nWeapon)
        Hull.current = Hull.current - incomingDamage
        Select Case sideHit
            Case Shields.Sides.FrontShield
                Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current = Int(Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current - (incomingDamage / 10))
                If Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current < 0 Then
                    Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current = 0
                End If
                Batteries.Primary.ChangeStats()
                Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current = Int(Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current - (incomingDamage / 10))
                If Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current < 0 Then
                    Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current = 0
                End If
                Batteries.Secondary.ChangeStats()
            Case Shields.Sides.RightShield Or Shields.Sides.LeftShield
                If Int(2 * Rnd()) = 0 Then
                    Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current = Int(Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current - (incomingDamage / 10))
                    If Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current < 0 Then
                        Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current = 0
                    End If
                    Batteries.Primary.ChangeStats()
                    Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current = Int(Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current - (incomingDamage / 10))
                    If Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current < 0 Then
                        Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current = 0
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
        hit = True
        If Hull.current <= 0 Then
            DestroyShip()
        End If
    End Sub

    Public Sub DestroyShip()
        If Parent IsNot Nothing Then
            Parent.RemoveShip(Me)
            Helm.Parent = Nothing
            Batteries.Parent = Nothing
            Batteries.Primary.Parent = Nothing
            Batteries.Secondary.Parent = Nothing
            Shielding.Parent = Nothing
            Engineering.Parent = Nothing
            Parent = Nothing
        End If
    End Sub

    Public Overridable Sub UpdateShip()
        Batteries.Update()
        Engineering.Update()
        Shielding.Update()
        Helm.Update()
        Dim fl As Decimal = Position.X + (Math.Cos(Helm.Direction) *
                                   (Helm.Throttle.current * (Engineering.Engines.current / Engineering.Engines.max)))
        Position.X = Position.X + (Math.Cos(Helm.Direction) *
                                   (Helm.Throttle.current * (Engineering.Engines.current / Engineering.Engines.max)))
        Position.Y = Position.Y + (Math.Sin(Helm.Direction) *
                                   (Helm.Throttle.current * (Engineering.Engines.current / Engineering.Engines.max)))
        RaiseEvent ShipUpdate()
    End Sub

End Class
