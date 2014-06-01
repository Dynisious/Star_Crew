<Serializable()>
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

    Public Sub New(ByRef nParent As Ship, ByVal nPrimaryMount As Double, ByVal nSecondaryMount As Double,
                   ByRef nPrimary As Weapon, ByRef nSecondary As Weapon, ByRef nPower As StatInt)
        MyBase.New(nParent)
        PrimaryMount = nPrimaryMount
        SecondaryMount = nSecondaryMount
        Primary = nPrimary
        Primary.Parent = Me
        Secondary = nSecondary
        Secondary.Parent = Me
        Power = nPower
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power.current = Power.current + Influx 'Adds power to the stored power inside the Station for use
            Parent.Engineering.batteriesDraw = 0 'Sets the draw of the batteries on the power supply to 0
            Primary.UpdateWeapon() 'Reloads the Weapon and adds any draw it takes on the power supply to engineering
            Secondary.UpdateWeapon() 'Reloads the Weapon and adds any draw it takes on the power supply to engineering
            If Parent.Engineering.batteriesDraw < 0 Then 'Ask for no power
                Parent.Engineering.batteriesDraw = 0
            End If
            If Power.current > Power.max Then 'Send the excess power to engineering
                Parent.Engineering.Power.current = Parent.Engineering.Power.current + Power.current - Power.max 'Put the excess
                'power into engineering
                Power.current = Power.max 'Set the current power to the maximum value
            End If

            If PlayerControled = False Then 'This Station is AI controlled
                If Parent.TargetLock = False Then 'The Ship can switch to the best suited target
                    Parent.Target = Nothing
                End If
                Dim target As Ship 'The Ship that the Batteries are targeting
                Dim targetDistance As Integer = -1 'The distance of the target from the Ship
                Dim targetDirection As Double = -1 'The direction of the target relative to the Ship in world space

                '-----Targeting----- 'Find the best Ship to target
                For Each i As Ship In ConsoleWindow.GameServer.GameWorld.CombatSpace.shipList 'Loop through all Ships
                    If i.Index <> Parent.Index Then 'The Ship is not targeting itself
                        Dim X As Integer = i.Position.X - Parent.Position.X 'The X position of the possible target relative to the Ship
                        Dim Y As Integer = i.Position.Y - Parent.Position.Y 'The Y position of the possible target relative to the Ship
                        Dim distance As Integer = Math.Sqrt((X ^ 2) + (Y ^ 2)) 'The distance of the possible target relative to the Ship
                        Dim direction As Double 'The direction of the possible target relative to the Ship in world space

                        '-----Set the Direction-----
                        If X <> 0 Then 'There won't be a divide by 0 error
                            direction = Math.Tanh(Y / X) 'Set the direction of the possible target in world space
                            If X < 0 Then 'The function returns a direction reflected in the Y axis so we must reflect it again
                                direction = direction + Math.PI
                            End If
                            direction = Helm.NormaliseDirection(direction) 'Normalise the direction to be between 0 and 2*Pi
                        ElseIf Y > 0 Then 'The possible target is directly above the Ship
                            direction = Math.PI / 2
                        Else 'The possible target is directly bellow the Ship
                            direction = (3 * Math.PI) / 2
                        End If
                        '---------------------------
                        If (distance < targetDistance Or targetDistance = -1) And i.MyAllegence <> Parent.MyAllegence Then 'The possible target
                            'is an enemy Ship and it is either the first target being measured or it is better than the previous
                            If Parent.TargetLock = False Then 'The Helm's target can be changed
                                Parent.Target = i
                            End If
                            Dim relativeDirection As Double = Helm.NormaliseDirection(direction - Parent.Direction + (Math.PI / 2)) 'The
                            'direction of the target relative to the Ship
                            If relativeDirection < Math.PI Then 'The target is in front of the Ship and can be targeted
                                target = i 'Set the Batteries to target this target
                                targetDistance = distance 'Set the target's distance
                                targetDirection = direction 'Set the target's direction
                            End If
                        End If
                        If i.Direction - Parent.Direction <> 0 Then 'The two Ship's will eventually intersect
                            Dim xVel As Integer = (Math.Cos(i.Direction) * i.Speed.current) - (Math.Cos(Parent.Direction) * Parent.Speed.current)
                            'The velocity of change in distance along the x-axis of the two ships
                            If xVel > 0 Or (i.Position.X - Parent.Position.X) = 0 Then 'They are closing along the x-axis or they
                                'are already in-line
                                Dim yVel As Integer = (Math.Sin(i.Direction) * i.Speed.current) - (Math.Sin(Parent.Direction) * Parent.Speed.current)
                                If yVel > 0 Or (i.Position.Y - Parent.Position.Y = 0 And xVel > 0) Then 'They are also closing along
                                    'the y-axis or they are already in-line
                                    Dim timeOfIntersect As Integer = (distance - Helm.MinimumDistance) /
                                        Math.Sqrt((xVel ^ 2) + (yVel ^ 2)) 'The time when the two Ships intersect
                                    Dim evadeDirection As Double = Helm.NormaliseDirection(targetDirection - Parent.Direction)
                                    If evadeDirection < 3 * Math.PI / 4 And evadeDirection > Math.PI / 2 Then 'It needs to be mirrored to
                                        'be calculated correctly
                                        evadeDirection = Helm.NormaliseDirection(evadeDirection - Math.PI) 'Mirror the direction
                                    End If
                                    If timeOfIntersect <= Math.Sqrt((evadeDirection / Parent.Helm.TurnSpeed.current) ^ 2) Then
                                        'They will intersect before the craft can turn out of the way
                                        '-----Decide whether to turn left or right to evade-----
                                        If evadeDirection < 0 Then
                                            Parent.Helm.evasion = Parent.Helm.evasion + 1
                                        Else
                                            Parent.Helm.evasion = Parent.Helm.evasion - 1
                                        End If
                                        '-------------------------------------------------------
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
                '-------------------

                If target IsNot Nothing Then 'The Batteries have a valid target to Shoot at
                    '-----Aim at Target-----
                    '-----Primary-----
                    If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount < 0 Then 'The primary
                        'weapon needs to turn left
                        Primary.TurnDistance.current = Primary.TurnDistance.current - Primary.TurnSpeed.current 'Turn left
                        If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount > 0 Then 'Turn back to be pointing
                            'directly at the target
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction) - PrimaryMount
                        End If
                        If Primary.TurnDistance.current < -(Primary.TurnDistance.max / 2) Then 'Turn back to the maximum turn distance
                            'of the weapon
                            Primary.TurnDistance.current = -(Primary.TurnDistance.max / 2)
                        End If
                    ElseIf targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount > 0 Then 'The secondary
                        'weapon needs to turn right
                        Primary.TurnDistance.current = Primary.TurnDistance.current + Primary.TurnSpeed.current 'Turn right
                        If targetDirection - Parent.Direction - Primary.TurnDistance.current - PrimaryMount < 0 Then 'Turn back to be pointing
                            'directly at the target
                            Primary.TurnDistance.current = (targetDirection - Parent.Direction) - PrimaryMount
                        End If
                        If Primary.TurnDistance.current > (Primary.TurnDistance.max / 2) Then 'Turn back to the maximum turn distance
                            'of the weapon
                            Primary.TurnDistance.current = (Primary.TurnDistance.max / 2)
                        End If
                    End If
                    '-----------------

                    '-----Secondary-----
                    If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount < 0 Then 'The secondary weapon
                        'needs to turn left
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current - Secondary.TurnSpeed.current 'Turn left
                        If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount > 0 Then 'Turn back to
                            'be pointing at the target
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction) - SecondaryMount
                        End If
                        If Secondary.TurnDistance.current < -(Secondary.TurnDistance.max / 2) Then 'Turn back to the maximum turn distance
                            Secondary.TurnDistance.current = -(Secondary.TurnDistance.max / 2)
                        End If
                    ElseIf targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount > 0 Then 'The secondary
                        'weapon needs to turn right
                        Secondary.TurnDistance.current = Secondary.TurnDistance.current + Secondary.TurnSpeed.current 'Turn right
                        If targetDirection - Parent.Direction - Secondary.TurnDistance.current - SecondaryMount < 0 Then 'Turn back to
                            'be pointing at the target
                            Secondary.TurnDistance.current = (targetDirection - Parent.Direction) - SecondaryMount
                        End If
                        If Secondary.TurnDistance.current > (Secondary.TurnDistance.max / 2) Then 'Turn back to the maximum turn distance
                            Secondary.TurnDistance.current = (Secondary.TurnDistance.max / 2)
                        End If
                    End If
                    '-------------------
                    '-----------------------

                    '-----Fire at Target-----
                    '-----Primary-----
                    Dim primaryAim As Double = targetDirection - Parent.Direction -
                        Primary.TurnDistance.current - PrimaryMount + (HitArc / 2) 'The offset of the primary weapon's
                    'aim relative to the targets direction
                    If (HitArc / 2) > primaryAim Then 'The target is within the Arc neccessary to hit the target
                        Primary.FireWeapon(targetDistance, target, targetDirection) 'Attempt to fire the target
                    End If
                    '-----------------

                    '-----Secondary-----
                    Dim secondaryAim As Double = targetDirection - Parent.Direction -
                        Secondary.TurnDistance.current - SecondaryMount + (HitArc / 2) 'The offset of the secondary weapon's
                    'aim relative to the targets direction
                    If (HitArc / 2) > secondaryAim And secondaryAim > -(HitArc / 2) Then 'The target is within the
                        'Arc neccessary to hit the target
                        Secondary.FireWeapon(targetDistance, target, targetDirection) 'Attempt to fire at the target
                    End If
                    '-------------------
                    '------------------------
                End If
            Else 'The Station is controlled by a Player
                If Parent.Target IsNot Nothing Then 'There is an active target
                    If Parent.Target.Dead = True Then 'The target is dead and so must be untargeted
                        Parent.Target = Nothing 'Clear the target
                    End If
                End If
            End If
        End If
    End Sub

End Class
