<Serializable()>
Public Class Shields
    Inherits Station
    Public SubSystem As ShieldSystem 'A ShieldSystem object that contains the values for the Ships shields and the damage modifiers for the type of
    'shield it is
    Public Enum Sides
        FrontShield
        RightShield
        BackShield
        LeftShield
        Max
    End Enum
    <NonSerialized()>
    Public DamagePerSide(Sides.Max - 1) As Integer 'The amount of damage done to each side of the ship to allow the AI to decide which shields to
    'prioritise
    Public LastHit As Sides 'The side that was last hit
    Public Enum Commands
        BoostForward
        BoostRight
        BoostBack
        BoostLeft
    End Enum

    Public Sub New(ByRef nParent As Ship, ByVal nSystem As ShieldSystem)
        MyBase.New(nParent)
        SubSystem = nSystem
    End Sub

    Public Function DeflectHit(ByVal side As Sides, ByVal nWeapon As Weapon) As Integer
        If PlayerControled = False Then
            LastHit = side
        End If
        DamagePerSide(side) = DamagePerSide(side) + nWeapon.Damage.current

        Dim recivedDamage As Integer = nWeapon.Damage.current
        recivedDamage = recivedDamage * SubSystem.DefenceModifiers(nWeapon.DamageType)
        If SubSystem.Defences(side).current > recivedDamage Then
            SubSystem.Defences(side).current = SubSystem.Defences(side).current - recivedDamage
            recivedDamage = 0
        Else
            recivedDamage = recivedDamage - SubSystem.Defences(side).current
            SubSystem.Defences(side).current = 0
        End If
        Return recivedDamage
    End Function

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Dim totalDamage As Integer = DamagePerSide(Sides.FrontShield) +
                DamagePerSide(Sides.LeftShield) + DamagePerSide(Sides.BackShield) +
                DamagePerSide(Sides.RightShield) 'The total damage that has been taken by all sides

            '-----Send off the costs-----
            Parent.Engineering.shieldingDraw = (SubSystem.Defences(Sides.FrontShield).max - SubSystem.Defences(Sides.FrontShield).current) +
                (SubSystem.Defences(Sides.RightShield).max - SubSystem.Defences(Sides.RightShield).current) +
                (SubSystem.Defences(Sides.BackShield).max - SubSystem.Defences(Sides.BackShield).current) +
                (SubSystem.Defences(Sides.LeftShield).max - SubSystem.Defences(Sides.LeftShield).current)
            '----------------------------

            '-----Distribute the power-----
            If totalDamage <> 0 Then
                '-----Add up the power-----
                Dim usablePower As Integer = Power + Influx + (SubSystem.Defences(Sides.FrontShield).current +
                    SubSystem.Defences(Sides.LeftShield).current +
                    SubSystem.Defences(Sides.BackShield).current + SubSystem.Defences(Sides.RightShield).current)
                '--------------------------

                For i As Integer = 0 To 3
                    SubSystem.Defences(i).current = usablePower * (DamagePerSide(i) / totalDamage)
                    If SubSystem.Defences(i).current > SubSystem.Defences(i).max Then
                        SubSystem.Defences(i).current = SubSystem.Defences(i).max
                    End If
                Next
                '------------------------------

                '-----See if theres remaining usablePower-----
                usablePower = usablePower - SubSystem.Defences(Sides.FrontShield).current -
                    SubSystem.Defences(Sides.LeftShield).current - SubSystem.Defences(Sides.BackShield).current -
                    SubSystem.Defences(Sides.RightShield).current
                '---------------------------------------

                '-----Remaining usablePower-----
                If usablePower > 0 Then 'Theres spare usablePower
                    Dim nCost As Integer = SubSystem.Defences(LastHit).max - SubSystem.Defences(LastHit).current
                    If nCost > usablePower Then
                        nCost = usablePower
                    End If
                    SubSystem.Defences(LastHit).current = SubSystem.Defences(LastHit).current + nCost
                    usablePower = usablePower - nCost
                    If SubSystem.Defences(LastHit).current > SubSystem.Defences(LastHit).max Then
                        SubSystem.Defences(LastHit).current = SubSystem.Defences(LastHit).max
                    End If
                    While usablePower <> 0 'Distribute remaining power
                        Dim maxed As Integer
                        For i As Integer = 0 To Sides.Max - 1
                            If SubSystem.Defences(i).current = SubSystem.Defences(i).max Then
                                maxed = maxed + 1
                            End If
                        Next
                        If maxed = 4 Then
                            Exit While
                        Else
                            Dim factor As Integer = 4 - maxed
                            For i As Integer = 0 To Sides.Max - 1
                                If SubSystem.Defences(i).current <> SubSystem.Defences(i).max Then
                                    SubSystem.Defences(i).current = SubSystem.Defences(i).current + (usablePower / factor)
                                End If
                            Next
                            usablePower = 0
                            For i As Integer = 0 To Sides.Max - 1
                                If SubSystem.Defences(i).current > SubSystem.Defences(i).max Then
                                    usablePower = usablePower + (SubSystem.Defences(i).current - SubSystem.Defences(i).max)
                                    SubSystem.Defences(i).current = SubSystem.Defences(i).max
                                End If
                            Next
                        End If
                    End While
                    Power = usablePower
                End If
            End If
            '-------------------------

            For i As Integer = 0 To Sides.Max - 1
                If DamagePerSide(i) > 0 Then
                    DamagePerSide(i) = DamagePerSide(i) - 1
                End If
            Next
        End If
    End Sub

End Class
