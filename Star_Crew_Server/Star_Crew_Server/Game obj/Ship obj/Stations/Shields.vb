Public MustInherit Class Shields 'Object responsible for rotating the Shield to point towards a threat
    Inherits ShipStation
    Public Influx As Double 'How much power this Station gets to use per Update
    Public Shield As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Shield
    Public Direction As Double 'A Double value representing the direction the Shield is facing relative to the front of the Ship
    Public TurnSpeed As Double 'A Double value representing how quickly the Shields turn
    Public Sweep As Double 'A Double value representing the coverage of the Shield
    Protected PowerCost As Double 'A Double value indicating how much power it costs to recharge one point of Shield
    Private Modifiers As New Dictionary(Of Weapon.DamageTypes, Double)

    Public Sub New(ByRef nParent As Ship, ByVal nIntegrity As Game_Library.StatInt, ByVal nRepairCost As Double, ByVal nShield As Game_Library.StatDbl, ByVal nTurnSpeed As Double, ByVal nSweep As Double, ByVal nPowerCost As Double)
        MyBase.New(nParent, nIntegrity, nRepairCost)
        Shield = nShield
        TurnSpeed = nTurnSpeed
        Sweep = nSweep
        PowerCost = nPowerCost
        For i As Weapon.DamageTypes = 0 To Weapon.DamageTypes.max - 1 'Loop through each type of damage
            Modifiers.Add(i, 1) 'Set the modifier
        Next
    End Sub

    Public Function Deflect_Damage(ByVal Vector As Double, ByVal Damage As Double, ByVal Type As Weapon.DamageTypes) As Double 'Calculates the damage that breakes through the Ship's Shield and subtracts the incomming damage from the Ship's Shield
        Dim impactDirection As Double = Server.Normalise_Direction(Vector + Math.PI - ParentShip.Direction - Direction + (Sweep / 2)) 'The direction of the shot relative to the right edge of the Shield
        If impactDirection <= Sweep Then 'The attack hit the Shield
            Damage = Damage * Modifiers(Type) 'Calculate the damage depending on how effective the shield is at stopping it
            Dim shieldPercentage As Double = ((impactDirection - (Sweep / 2)) / Sweep) 'The percentage of the Shield's power that can be used to block damage
            If shieldPercentage < 0 Then shieldPercentage = -shieldPercentage 'Make sure it's positive
            Dim shieldPower As Double = Shield.Current * shieldPercentage 'Calculate how much power can be used to deflect the damage
            Dim temp As Double = Shield.Current - Damage 'Take damage to the Shield
            If temp < Shield.Minimum Then 'The shields are bellow the minimum
                Shield.Current = Shield.Minimum 'Set the Shields to the minimum
            Else 'The shield will be within the bounds
                Shield.Current = temp 'Set the Shield
            End If
            Damage = Damage - shieldPower 'Remove the Shield's power from the damage
            If Damage < 0 Then 'Set damage to 0
                Damage = 0 'Set damage to 0
            End If
        End If
        Return Damage
    End Function

    Protected Overrides Sub Finalise_Destroy() 'Removes all references to the Shields object
        ParentShip.Shielding = Nothing 'Remove the reference
    End Sub

    Public MustOverride Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    Public Overrides Sub Update() 'Rotates the Ship's Shield to face the closest enemy and charges the Shield
        If AIControled = True And Powered = True Then
            ParentShip.Engineering.ShieldsCost = 0 'Clear the Shields cost
            '-----Turn Shield-----
            Dim turnOffset As Double = Server.Normalise_Direction(ParentShip.targetDirection - ParentShip.Direction) - Direction 'How far the Shield need to turn to face the enemy
            Dim turnRight As Boolean = False 'A Boolean value indicating whether the Shield should turn right
            If Math.Sign(turnOffset) = -1 Then 'Make sure it's positive
                turnOffset = -turnOffset
                turnRight = True
            End If
            If turnOffset < TurnSpeed Then 'The offset is within turning distance
                Direction = Server.Normalise_Direction(turnOffset + Direction) 'Set the direction
            ElseIf turnRight = False Then 'Turn left
                Direction = Server.Normalise_Direction(Direction + TurnSpeed) 'Turn
            Else 'Turn right
                Direction = Server.Normalise_Direction(Direction - TurnSpeed) 'Turn
            End If
            '---------------------

            '-----Send off power costs-----
            If ParentShip.Engineering.AIControled = True Then 'Engineering is AI Controlled
                ParentShip.Engineering.ShieldsCost = (Shield.Maximum - Shield.Current) * PowerCost * (Integrity.Maximum / Integrity.Current) 'How much power the Shields need to be fully charged
                Influx = 0 'Set the influx to 0
            End If
            '------------------------------
        End If

        '-----Charge Shield-----
        Dim power As Double = (Influx * Integrity.Current) / (PowerCost * Integrity.Maximum) 'How many points of Shield can be regenerated
        Shield.Current = Shield.Current + power
        If Shield.Current > Shield.Maximum Then
            Shield.Current = Shield.Maximum
        End If
        '-----------------------
    End Sub

End Class
