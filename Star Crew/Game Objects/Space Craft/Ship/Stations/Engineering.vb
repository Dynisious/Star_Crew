<Serializable()>
Public Class Engineering
    Inherits Station
    <NonSerialized()>
    Public batteriesDraw As Integer
    <NonSerialized()>
    Public shieldingDraw As Integer
    Public SubSystem As EngineSystem
    <NonSerialized()>
    Private Heating As Boolean = True
    Public Heat As Double = 50
    Public Rate As Double
    <NonSerialized()>
    Public Shared ReadOnly RateOfChange = 0.002
    Public Enum Commands
        Heat
        Cool
    End Enum

    Public Sub New(ByRef nParent As Ship, ByVal nSystem As EngineSystem)
        MyBase.New(nParent)
        SubSystem = nSystem
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Dim primaryDraw As Integer = (Parent.Batteries.Primary.Integrety.max -
                 Parent.Batteries.Primary.Integrety.current)
            Dim secondaryDraw As Integer = (Parent.Batteries.Secondary.Integrety.max -
                 Parent.Batteries.Secondary.Integrety.current)
            Dim enginesDraw As Integer = (SubSystem.Engines.max - SubSystem.Engines.current)
            Dim powerCoreDraw As Integer = (SubSystem.PowerCore.max - SubSystem.PowerCore.current)
            Dim totalPowerDraw As Integer = batteriesDraw + shieldingDraw + enginesDraw + powerCoreDraw + primaryDraw + secondaryDraw
            Dim CoreStability As Double = 1
            If PlayerControled = True Then
                Heat = Heat + Rate
                If totalPowerDraw <> 0 Then
                    If Int(30 * Rnd()) = 0 Then
                        If Heating = True Then
                            Heating = False
                        Else
                            Heating = True
                        End If
                    End If
                    If Heating = True Then
                        Rate = Rate + RateOfChange
                    Else
                        Rate = Rate - RateOfChange
                    End If
                Else
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
                End If
                CoreStability = 1 / ((Math.Sqrt((Int(Heat - 50) / 100) ^ 2)) + 1)
            End If
            Power = Power + (SubSystem.PowerCore.current * CoreStability)

            If totalPowerDraw <> 0 And PlayerControled = False Then
                Dim usablePower As Integer = Power

                '-----Power Core-----
                Dim powerCoreCost As Integer = usablePower * (powerCoreDraw / totalPowerDraw)
                If powerCoreCost > (powerCoreDraw) Then
                    powerCoreCost = (powerCoreDraw)
                End If
                SubSystem.PowerCore.current = SubSystem.PowerCore.current + powerCoreCost
                '--------------------

                If SubSystem.PowerCore.current > (SubSystem.PowerCore.max / 2) Then
                    '-----Batteries-----
                    Dim batteriesCost As Integer = usablePower * (batteriesDraw / totalPowerDraw)
                    If batteriesCost > batteriesDraw Then
                        batteriesCost = batteriesDraw
                    End If
                    Parent.Batteries.Influx = batteriesCost
                    '-------------------

                    '-----Shielding-----
                    Dim shieldingCost As Integer = usablePower * (shieldingDraw / totalPowerDraw)
                    If shieldingCost > shieldingDraw Then
                        shieldingCost = shieldingDraw
                    End If
                    Parent.Shielding.Influx = shieldingCost
                    '-------------------

                    '-----Primary Weapon-----
                    Dim primaryCost As Integer = usablePower * (primaryDraw / totalPowerDraw)
                    If primaryCost > primaryDraw Then
                        primaryCost = primaryDraw
                    End If
                    Parent.Batteries.Primary.Integrety.current =
                    Parent.Batteries.Primary.Integrety.current + primaryCost
                    Parent.Batteries.Primary.ChangeStats()
                    '------------------------

                    '-----Secondary Weapon-----
                    Dim secondaryCost As Integer = usablePower * (secondaryDraw / totalPowerDraw)
                    If secondaryCost > secondaryDraw Then
                        secondaryCost = secondaryDraw
                    End If
                    Parent.Batteries.Secondary.Integrety.current =
                    Parent.Batteries.Secondary.Integrety.current + secondaryCost
                    Parent.Batteries.Secondary.ChangeStats()
                    '------------------------

                    '-----Engines-----
                    Dim enginesCost As Integer = usablePower * (enginesDraw / totalPowerDraw)
                    If enginesCost > (enginesDraw) Then
                        enginesCost = (enginesDraw)
                    End If
                    SubSystem.Engines.current = SubSystem.Engines.current + enginesCost
                    '-----------------

                    Power = usablePower - batteriesCost - shieldingCost - primaryCost - secondaryCost - enginesCost - powerCoreCost
                Else
                    Power = usablePower - powerCoreCost
                End If
            End If
        End If
    End Sub

End Class
