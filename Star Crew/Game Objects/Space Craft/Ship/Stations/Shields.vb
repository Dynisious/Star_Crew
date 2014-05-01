<Serializable()>
Public Class Shields
    Inherits Station
    <NonSerialized()>
    Public DefenceBonuses(Weapon.DamageTypes.Max - 1) As Double
    Public Enum Sides
        FrontShield
        RightShield
        BackShield
        LeftShield
        Max
    End Enum
    Public Enum ShieldingCommands
        ChangeForward
        ChangeRight
        ChangeRear
        ChangeLeft
        Tune
    End Enum
    Public ShipShields(Sides.Max - 1) As StatDbl
    <NonSerialized()>
    Public DamagePerSide(Sides.Max - 1) As Integer
    Public LastHit As Sides 'The side that was last hit
    Public Enum Commands
        BoostForward
        BoostRight
        BoostBack
        BoostLeft
    End Enum

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Function DeflectHit(ByVal side As Sides, ByVal nWeapon As Weapon) As Integer
        If PlayerControled = False Then
            LastHit = side
        End If
        DamagePerSide(side) = DamagePerSide(side) + nWeapon.Damage.current

        Dim recivedDamage As Integer = nWeapon.Damage.current
        recivedDamage = recivedDamage * DefenceBonuses(nWeapon.DamageType)
        If ShipShields(side).current > recivedDamage Then
            ShipShields(side).current = ShipShields(side).current - recivedDamage
            recivedDamage = 0
        Else
            recivedDamage = recivedDamage - ShipShields(side).current
            ShipShields(side).current = 0
        End If
        Return recivedDamage
    End Function

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Dim totalDamage As Integer = DamagePerSide(Sides.FrontShield) +
                DamagePerSide(Sides.LeftShield) + DamagePerSide(Sides.BackShield) +
                DamagePerSide(Sides.RightShield) 'The total damage that has been taken by all sides

            '-----Send off the costs-----
            Parent.Engineering.shieldingDraw = (ShipShields(Sides.FrontShield).max - ShipShields(Sides.FrontShield).current) +
                (ShipShields(Sides.RightShield).max - ShipShields(Sides.RightShield).current) +
                (ShipShields(Sides.BackShield).max - ShipShields(Sides.BackShield).current) +
                (ShipShields(Sides.LeftShield).max - ShipShields(Sides.LeftShield).current)
            '----------------------------

            '-----Distribute the power-----
            If totalDamage <> 0 Then
                '-----Add up the power-----
                Dim usablePower As Integer = Power + Influx + (ShipShields(Sides.FrontShield).current +
                    ShipShields(Sides.LeftShield).current +
                    ShipShields(Sides.BackShield).current + ShipShields(Sides.RightShield).current)
                '--------------------------

                For i As Integer = 0 To 3
                    ShipShields(i).current = usablePower * (DamagePerSide(i) / totalDamage)
                    If ShipShields(i).current > ShipShields(i).max Then
                        ShipShields(i).current = ShipShields(i).max
                    End If
                Next
                '------------------------------

                '-----See if theres remaining usablePower-----
                usablePower = usablePower - ShipShields(Sides.FrontShield).current -
                    ShipShields(Sides.LeftShield).current - ShipShields(Sides.BackShield).current -
                    ShipShields(Sides.RightShield).current
                '---------------------------------------

                '-----Remaining usablePower-----
                If usablePower > 0 Then 'Theres spare usablePower
                    Dim nCost As Integer = ShipShields(LastHit).max - ShipShields(LastHit).current
                    If nCost > usablePower Then
                        nCost = usablePower
                    End If
                    ShipShields(LastHit).current = ShipShields(LastHit).current + nCost
                    usablePower = usablePower - nCost
                    If ShipShields(LastHit).current > ShipShields(LastHit).max Then
                        ShipShields(LastHit).current = ShipShields(LastHit).max
                    End If
                    While usablePower <> 0 'Distribute remaining power
                        Dim maxed As Integer
                        For i As Integer = 0 To Sides.Max - 1
                            If ShipShields(i).current = ShipShields(i).max Then
                                maxed = maxed + 1
                            End If
                        Next
                        If maxed = 4 Then
                            Exit While
                        Else
                            Dim factor As Integer = 4 - maxed
                            For i As Integer = 0 To Sides.Max - 1
                                If ShipShields(i).current <> ShipShields(i).max Then
                                    ShipShields(i).current = ShipShields(i).current + (usablePower / factor)
                                End If
                            Next
                            usablePower = 0
                            For i As Integer = 0 To Sides.Max - 1
                                If ShipShields(i).current > ShipShields(i).max Then
                                    usablePower = usablePower + (ShipShields(i).current - ShipShields(i).max)
                                    ShipShields(i).current = ShipShields(i).max
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
