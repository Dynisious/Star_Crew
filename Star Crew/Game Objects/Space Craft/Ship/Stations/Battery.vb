
Public Class Battery
    Inherits Station
    Public PrimaryMount As Double 'A Double value representing the primary Weapon's offset from forward facing
    Public SecondaryMount As Double 'A Double value representing the secondary Weapon's offset from forward facing
    Public Primary As Weapon 'A Weapon object that will fire at enemies
    Public Secondary As Weapon 'A Weapon object that will fire at enemies
    Public Shared ReadOnly HitArc As Double = Math.PI / 5 'The arc that the AI must get the enemy inside to hit it
    Public Shared ReadOnly PlayerArc As Double = HitArc * 1.5 'The arc that the player must get the enemy inside to hit it
    Public Enum Commands
        TurnRight
        TurnLeft
        FirePrimary
        FireSecondary
        SetTarget
    End Enum

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power = Power + Influx
            Parent.Engineering.batteriesDraw = 0
            Primary.UpdateWeapon()
            Secondary.UpdateWeapon()

            If PlayerControled = False And ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList.Count > 0 Then
                If Parent.TargetLock = False Then
                    Parent.Helm.Target = Nothing
                End If
                Dim target As Ship
                Dim targetDistance As Integer = -1
                Dim targetDirection As Double = -1
                ReDim Parent.Helm.evadeList(-1)
                '-----Targeting-----
                For Each i As Ship In ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList
                    If i.Index <> Parent.Index Then
                        Dim X As Integer = i.Position.X - Parent.Position.X
                        Dim Y As Integer = i.Position.Y - Parent.Position.Y
                        Dim distance As Integer = Math.Sqrt((X ^ 2) + (Y ^ 2))
                        Dim direction As Double
                        '-----Set the Direction-----
                        If X <> 0 Then
                            direction = Math.Tanh(Y / X)
                            If X < 0 Then
                                direction = direction + Math.PI
                            End If
                            direction = Helm.NormalizeDirection(direction)
                        ElseIf Y > 0 Then
                            direction = Math.PI / 2
                        Else
                            direction = (3 * Math.PI) / 2
                        End If
                        '---------------------------
                        If (distance < targetDistance Or targetDistance = -1) And i.MyAllegence <> Parent.MyAllegence Then
                            If Parent.TargetLock = False Then 'Make this Ship the new target
                                Parent.Helm.Target = i
                            End If
                            Dim relativeDirection As Double = direction - Parent.Direction
                            If relativeDirection > -Math.PI And relativeDirection < Math.PI Then 'It's a valid Target for the Batteries
                                target = i
                                targetDistance = distance
                                targetDirection = direction
                            End If
                        End If
                        If distance - Helm.MinimumDistance < (((Parent.Speed.current - i.Speed.current) ^ 2) / Parent.Acceleration.current) Or
                            distance < Helm.MinimumDistance Then
                            'The target is too close and must be evaded
                            ReDim Preserve Parent.Helm.evadeList(Parent.Helm.evadeList.Length)
                            Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) = Helm.NormalizeDirection(direction - Parent.Direction)
                            'Add the direction to the evasion list
                            If Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) > Math.PI / 2 And
                               Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) < 3 * Math.PI / 2 Then 'The direction needs to be changed to
                                'simulate being in front of the Ship for the evasion code to interpret it correctly
                                Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) =
                                    Helm.NormalizeDirection(Parent.Helm.evadeList(UBound(Parent.Helm.evadeList)) - Math.PI)
                            End If
                        End If
                    End If
                Next
                '-------------------

                If target IsNot Nothing Then
                    '-----Aim at Target-----
                    '-----Primary-----
                    If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount < 0 Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current - Primary.TurnSpeed.current
                        If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount > 0 Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction) - PrimaryMount
                        End If
                        If Primary.TurnDistance.current < -(Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = -(Primary.TurnDistance.max / 2)
                        End If
                    ElseIf targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount > 0 Then
                        Primary.TurnDistance.current = Primary.TurnDistance.current + Primary.TurnSpeed.current
                        If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount < 0 Then
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction) - PrimaryMount
                        End If
                        If Primary.TurnDistance.current > (Primary.TurnDistance.max / 2) Then
                            Primary.TurnDistance.current = (Primary.TurnDistance.max / 2)
                        End If
                    End If
                    '-----------------

                    '-----Secondary-----
                    If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount < 0 Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current
                        If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount > 0 Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction) - SecondaryMount
                        End If
                        If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                        End If
                    ElseIf targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount > 0 Then
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current + Secondary.TurnSpeed.current
                        If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount < 0 Then
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction) - SecondaryMount
                        End If
                        If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then
                            Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                        End If
                    End If
                    '-------------------
                    '-----------------------

                    '-----Fire at Target-----
                    '-----Primary-----
                    Dim primaryAim As Double = targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount
                    If (HitArc / 2) > primaryAim And primaryAim > -(HitArc / 2) Then
                        Primary.FireWeapon(targetDistance, target, targetDirection)
                    End If
                    '-----------------

                    '-----Secondary-----
                    Dim secondaryAim As Double = targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount
                    If (HitArc / 2) > secondaryAim And secondaryAim > -(HitArc / 2) Then
                        Secondary.FireWeapon(targetDistance, target, targetDirection)
                    End If
                    '-------------------
                    '------------------------
                End If
            Else
                If Parent.Helm.Target IsNot Nothing Then
                    If Parent.Helm.Target.Dead = True Then
                        Parent.Helm.Target = Nothing
                    End If
                End If
            End If
        End If
    End Sub

End Class
