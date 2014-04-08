<Serializable()>
Public Class Engineering
    Inherits Station
    Public batteriesDraw As Integer
    Public shieldingDraw As Integer
    Public Engines As Stat
    Public PowerCore As Stat
    Private Heating As Boolean = True
    Public Heat As Double = 50
    Public Rate As Double
    Public Shared ReadOnly RateOfChange = 0.003
    Public Enum Commands
        Heat
        Cool
    End Enum

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent)
    End Sub

    Public Overrides Sub Update()
        If Parent IsNot Nothing Then
            Power = Power + PowerCore.current
            Dim primaryDraw As Integer = (Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).max -
                 Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current)
            Dim secondaryDraw As Integer = (Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).max -
                 Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current)
            Dim enginesDraw As Integer = (Engines.max - Engines.current)
            Dim powerCoreDraw As Integer = (PowerCore.max - PowerCore.current)
            Dim totalPowerDraw As Integer = batteriesDraw + shieldingDraw + enginesDraw + powerCoreDraw + primaryDraw + secondaryDraw
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
                If Heat < 10 Or Heat > 100 Then
                    Parent.Hull.current = Parent.Hull.current - 0.0025
                End If
            End If

            If totalPowerDraw <> 0 And PlayerControled = False Then
                Dim usablePower As Integer = Power

                '-----Power Core-----
                Dim powerCoreCost As Integer = usablePower * (powerCoreDraw / totalPowerDraw)
                If powerCoreCost > (powerCoreDraw) Then
                    powerCoreCost = (powerCoreDraw)
                End If
                PowerCore.current = PowerCore.current + powerCoreCost
                '--------------------

                If PowerCore.current > (PowerCore.max / 2) Then
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
                    Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current =
                        Parent.Batteries.Primary.WeaponStats(Weapon.Stats.Integrety).current + primaryCost
                    Parent.Batteries.Primary.ChangeStats()
                    '------------------------

                    '-----Secondary Weapon-----
                    Dim secondaryCost As Integer = usablePower * (secondaryDraw / totalPowerDraw)
                    If secondaryCost > secondaryDraw Then
                        secondaryCost = secondaryDraw
                    End If
                    Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current =
                        Parent.Batteries.Secondary.WeaponStats(Weapon.Stats.Integrety).current + secondaryCost
                    Parent.Batteries.Secondary.ChangeStats()
                    '------------------------

                    '-----Engines-----
                    Dim enginesCost As Integer = usablePower * (enginesDraw / totalPowerDraw)
                    If enginesCost > (enginesDraw) Then
                        enginesCost = (enginesDraw)
                    End If
                    Engines.current = Engines.current + enginesCost
                    '-----------------

                    Power = usablePower - batteriesCost - shieldingCost - primaryCost - secondaryCost - enginesCost - powerCoreCost
                Else
                    Power = usablePower - powerCoreCost
                End If
            End If
        End If
    End Sub

End Class
