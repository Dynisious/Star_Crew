﻿<Serializable()>
Public Class Engineering
    Inherits Station
    Public batteriesDraw As Integer 'How many units of Power the Batteries want
    Public shieldingDraw As Integer 'How many units of Power Shielding wants
    Public SubSystem As EngineSystem 'An EngineSystem object representing the Ships Engines and Power Core
    Private Heating As Boolean = True 'A Boolean value indecating whether the Power Core's temperature is rising or falling
    Public Heat As Double = 50 'The Heat of the Power Core
    Public Rate As Double 'The rate at which the Power Core's temperature is rising
    Public Shared ReadOnly RateOfChange = 0.002 'A constant value indecating how quickly the Power Core increases it's Rate
    Public Enum Commands
        Heat
        Cool
    End Enum

    Public Sub New(ByRef nParent As Ship, ByRef nSystem As EngineSystem, ByRef nPower As StatInt)
        MyBase.New(nParent)
        SubSystem = nSystem
        Power = nPower
    End Sub

    Public Overrides Sub Update()
        Dim enginesDraw As Integer = (SubSystem.Engines.max - SubSystem.Engines.current) 'How many units of Power the Engines want
        Dim powerCoreDraw As Integer = (SubSystem.PowerCore.max - SubSystem.PowerCore.current) 'How many units of Power the Power Core wants
        Dim primaryDraw As Integer = (Parent.Batteries.Primary.Integrety.max - Parent.Batteries.Primary.Integrety.current) 'How many units
        'of Power needed to repair the Primary Weapon
        Dim secondaryDraw As Integer = (Parent.Batteries.Secondary.Integrety.max - Parent.Batteries.Secondary.Integrety.current) 'How many
        'units of Power needed to repair the Secondary Weapon
        Dim totalPowerDraw As Integer = batteriesDraw + shieldingDraw + enginesDraw + powerCoreDraw + primaryDraw + secondaryDraw 'How
        'many units of Power are being asked for in total
        Dim CoreStability As Double = 1 'The stability of the Power Core
        If PlayerControled = True Then 'The Power Core's temperature needs to fluctuate
            Heat = Heat + Rate 'Change the Heat
            If totalPowerDraw <> 0 Then 'There is power to be provided
                If Int(30 * Rnd()) = 0 Then 'A chance whether to start heating or cooling
                    If Heating = True Then
                        Heating = False
                    Else
                        Heating = True
                    End If
                End If
                If Heating = True Then 'The rate needs to change to heating
                    Rate = Rate + RateOfChange
                Else 'The rate needs to change to cooling
                    Rate = Rate - RateOfChange
                End If
            Else 'There is no power to be provided
                '-----Stablise the Rate of Heating/Cooling-----
                If Rate > 0 Then
                    Rate = Rate - RateOfChange
                    If Rate < 0 Then
                        Rate = 0
                    End If
                ElseIf Rate < 0 Then
                    Rate = Rate + RateOfChange
                    If Rate > 0 Then
                        Rate = 0
                    End If
                End If
                '----------------------------------------------
            End If
            CoreStability = 1 / ((Math.Sqrt((Int(Heat - 50) / 100) ^ 2)) + 1) 'How close to completely stable the Power Core's temperature is
        End If
        Power.current = Power.current + (SubSystem.PowerCore.current * CoreStability) 'Add Power to the stored power in the system

        If totalPowerDraw <> 0 Then 'There will not be a divide by 0 error
            '-----Batteries-----
            Dim batteriesCost As Integer = Power.current * (batteriesDraw / totalPowerDraw) 'The fraction of the usable power allowed for
            'powering the Batteries
            If batteriesCost > batteriesDraw Then 'Too much power is being allowed for the Batteries so bring it down
                batteriesCost = batteriesDraw
            End If
            Parent.Batteries.Influx = batteriesCost 'Power the Batteries
            '-------------------

            '-----Shielding-----
            Dim shieldingCost As Integer = Power.current * (shieldingDraw / totalPowerDraw) 'The fraction of the usable power allowed for
            'powering the Shields
            If shieldingCost > shieldingDraw Then 'Too much power is being allowed for the Shields so bring it down
                shieldingCost = shieldingDraw
            End If
            Parent.Shielding.Influx = shieldingCost 'Power the Shields
            '-------------------

            '-----Power Core-----
            Dim powerCoreCost As Integer = Power.current * (powerCoreDraw / totalPowerDraw) 'The fraction of the usable power allowed for
            'repairing the Power Core
            If powerCoreCost > (powerCoreDraw * 10) Then 'Too much power is being allowed for the Power Core so bring it down
                powerCoreCost = (powerCoreDraw * 10)
            End If
            SubSystem.PowerCore.current = SubSystem.PowerCore.current + (powerCoreCost / 10) 'Repair the Power Core
            '--------------------

            '-----Primary Weapon-----
            Dim primaryCost As Integer = Power.current * (primaryDraw / totalPowerDraw) 'The fraction of the usable power allowed for
            'repairing the Primary Weapon
            If primaryCost > (primaryDraw * 10) Then 'Too much power is being allowed for repairing the Primary Weapon so bring it down
                primaryCost = (primaryDraw * 10)
            End If
            Parent.Batteries.Primary.Integrety.current =
            Parent.Batteries.Primary.Integrety.current + (primaryCost / 10) 'Repair the Primary Weapon
            Parent.Batteries.Primary.ChangeStats() 'Update the statistics inside the Primary Weapon
            '------------------------

            '-----Secondary Weapon-----
            Dim secondaryCost As Integer = Power.current * (secondaryDraw / totalPowerDraw) 'The fraction of the usable power allowed for
            'repairing the Secondary Weapon
            If secondaryCost > (secondaryDraw * 10) Then 'Too much power is being allowed for repairing the Secondary Weapon so bring it down
                secondaryCost = (secondaryDraw * 10)
            End If
            Parent.Batteries.Secondary.Integrety.current =
            Parent.Batteries.Secondary.Integrety.current + (secondaryCost / 10) 'Repair the Secondary Weapon
            Parent.Batteries.Secondary.ChangeStats() 'Update the statistics inside the Secondary Weapon
            '------------------------

            '-----Engines-----
            Dim enginesCost As Integer = Power.current * (enginesDraw / totalPowerDraw) 'The fraction of the usable power allowd
            'for repairing the Engines
            If enginesCost > (enginesDraw * 10) Then 'Too much power is being allowed for repairing the Engines
                enginesCost = (enginesDraw * 10)
            End If
            SubSystem.Engines.current = SubSystem.Engines.current + (enginesCost / 10) 'Repair the Engines
            '-----------------

            Power.current = Power.current - batteriesCost - shieldingCost - primaryCost - secondaryCost - enginesCost - powerCoreCost 'Remove all
            'the power used from the stored power
            If Power.current > Power.max Then 'Release the excess power
                Power.current = Power.max 'Set the current stored power to the maximum value
            End If
        End If
    End Sub

End Class
