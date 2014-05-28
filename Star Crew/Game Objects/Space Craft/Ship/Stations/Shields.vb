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

    Public Function DeflectHit(ByVal side As Sides, ByVal nWeapon As Weapon) As Double 'Returns the damage that does not get absorbed by the
        'Ship's shields
        If PlayerControled = False Then 'The AI can change the side to proritise
            LastHit = side 'Set the Shield to prioritise to the one that was just hit
        End If
        DamagePerSide(side) = DamagePerSide(side) + nWeapon.Damage.current 'Add the damage done to the count so that the Shields can
        'also proritise the other three Shields on a smalled level

        Dim recivedDamage As Integer = nWeapon.Damage.current 'Set the initial value of the received damage
        recivedDamage = recivedDamage * SubSystem.DefenceModifiers(nWeapon.DamageType) 'Change the received damage according to the
        'Shielding system's resistance to types of damage
        If SubSystem.Defences(side).current > recivedDamage Then 'There are enough Shields to absorb all the received damage
            SubSystem.Defences(side).current = SubSystem.Defences(side).current - recivedDamage 'Take away the damage from the Shields
            Return 0 'Return 0 because there is no left over damage
        Else
            recivedDamage = recivedDamage - SubSystem.Defences(side).current 'Take away all possible damage
            SubSystem.Defences(side).current = 0 'Set the Shield to 0
            Return recivedDamage 'Return the damage that was not absorbed
        End If
    End Function

    Public Overrides Sub Update()
        Dim totalDamage As Integer = DamagePerSide(Sides.FrontShield) +
            DamagePerSide(Sides.LeftShield) + DamagePerSide(Sides.BackShield) +
            DamagePerSide(Sides.RightShield) 'The total damage that has been taken by all sides

        '-----Send off the costs-----
        Parent.Engineering.shieldingDraw = (SubSystem.Defences(Sides.FrontShield).max - SubSystem.Defences(Sides.FrontShield).current) +
            (SubSystem.Defences(Sides.RightShield).max - SubSystem.Defences(Sides.RightShield).current) +
            (SubSystem.Defences(Sides.BackShield).max - SubSystem.Defences(Sides.BackShield).current) +
            (SubSystem.Defences(Sides.LeftShield).max - SubSystem.Defences(Sides.LeftShield).current) 'Let Engineering know how much power
        'will be needed to recharge all the Shields
        '----------------------------

        '-----Distribute the power-----
        If totalDamage <> 0 Then 'There won't be any divide by 0 errors
            '-----Add up the power-----
            Dim usablePower As Integer = Power + Influx + (SubSystem.Defences(Sides.FrontShield).current +
                SubSystem.Defences(Sides.LeftShield).current +
                SubSystem.Defences(Sides.BackShield).current + SubSystem.Defences(Sides.RightShield).current) 'How much power is available for
            'distribution
            '--------------------------

            '-----Distribute the power-----
            For i As Integer = 0 To Sides.Max - 1 'Loop through all the Shields
                SubSystem.Defences(i).current = usablePower * (DamagePerSide(i) / totalDamage) 'Give the Shield it's allowed fraction of
                'the power
                If SubSystem.Defences(i).current > SubSystem.Defences(i).max Then 'Set the Shield to it's maximum power value
                    SubSystem.Defences(i).current = SubSystem.Defences(i).max
                End If
            Next
            '------------------------------

            '-----See if theres remaining usablePower-----
            usablePower = usablePower - SubSystem.Defences(Sides.FrontShield).current -
                SubSystem.Defences(Sides.LeftShield).current - SubSystem.Defences(Sides.BackShield).current -
                SubSystem.Defences(Sides.RightShield).current 'Subtract the combined values of all the Shields from the usable power
            '---------------------------------------

            '-----Remaining usablePower-----
            If usablePower > 0 Then 'Theres spare Power to be distributed between the Shields
                Dim nCost As Integer = SubSystem.Defences(LastHit).max - SubSystem.Defences(LastHit).current 'How much power the side that
                'was last hit will need to be at full charge
                If nCost > usablePower Then 'There is not enough power to fully charge the Shield
                    nCost = usablePower 'Set the cost to the ramaining power
                End If
                SubSystem.Defences(LastHit).current = SubSystem.Defences(LastHit).current + nCost 'Add the available power into the Shield
                usablePower = usablePower - nCost 'Take away the spent power from the stored power

                Dim maxed As Int16 'A 16 Bit Integer representing how many Shields are at full charge
                For i As Integer = 0 To Sides.Max - 1 'Loop through all the Shields
                    If SubSystem.Defences(i).current = SubSystem.Defences(i).max Then 'The Shield is at maximum charge
                        maxed = maxed + 1 'Add it to the count of fully charged Shields
                    End If
                Next
                If maxed <> Sides.Max Then 'There are still Shields to be Charged
                    Dim factor As Integer = Sides.Max - maxed 'How many Shields there are to divide power between
                    For i As Integer = 0 To Sides.Max - 1 'Loop through all of the Shields
                        If SubSystem.Defences(i).current <> SubSystem.Defences(i).max Then 'The Shield is not at full charge
                            SubSystem.Defences(i).current = SubSystem.Defences(i).current + (usablePower / factor) 'Put the allowed
                            'fraction of the power into the Shield
                        End If
                    Next
                    usablePower = 0 'Set the stored power to 0
                    For i As Integer = 0 To Sides.Max - 1 'Loop through all the Shields
                        If SubSystem.Defences(i).current > SubSystem.Defences(i).max Then 'The Shield is overchargered
                            usablePower = usablePower + (SubSystem.Defences(i).current - SubSystem.Defences(i).max) 'Add the excess power
                            'to the stored power
                            SubSystem.Defences(i).current = SubSystem.Defences(i).max 'Set the Shield to it's maximum charge
                        End If
                    Next
                End If
                Power = usablePower 'Store the remaining power for later use
            End If
        End If
        '-------------------------

        For i As Integer = 0 To Sides.Max - 1 'Loop through all Shields
            If DamagePerSide(i) > 0 Then 'Take away one point of received damage to change the distribution scale
                DamagePerSide(i) = DamagePerSide(i) - 1
            End If
        Next
    End Sub

End Class
