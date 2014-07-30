Public MustInherit Class Shields 'Object responsible for rotating the Shield to point towards a threat
    Inherits ShipStation
    Public Influx As Double 'How much power this Station gets to use per Update
    Public Shield As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum values for the Shield
    Public Direction As Double ' A Double value representing the direction the Shield is facing
    Private TurnSpeed As Double 'A Double value representing how quickly the Shields turn
    Public Sweep As Double 'A Double value representing the coverage of the Shield
    Protected PowerCost As Double 'A Double value indicating how much power it costs to recharge on point of Shield
    Private Modifiers As New Dictionary(Of Weapon.DamageTypes, Double) From {
        {Weapon.DamageTypes.Pulse, 1}}

    Public Function Deflect_Damage(ByVal Vector As Double, ByVal Damage As Double, ByVal Type As Weapon.DamageTypes) As Double 'Calculates the damage that breakes through the Ship's Shield and subtracts the incomming damage from the Ship's Shield
        Dim impactDirection As Double = Server.Normalise_Direction(Vector + Math.PI - ParentShip.Direction - Direction + (Sweep / 2)) 'The direction of the shot relative to the right edge of the Shield
        If impactDirection <= Sweep Then 'The attack hit the Shield
            Damage = Damage * Modifiers(Type) 'Calculate the damage depending on how effective the shield is at stopping it
            Dim shieldPercentage As Double = ((impactDirection - (Sweep / 2)) / Sweep) 'The percentage of the Shield's power that can be used to block damage
            If shieldPercentage < 0 Then shieldPercentage = -shieldPercentage 'Make sure it's positive
            Dim shieldPower As Double = Shield.Current * shieldPercentage 'Calculate how much power can be used
            Shield.Current = Shield.Current - Damage 'Take damage to the Shield
            If Shield.Current < Shield.Minimum Then 'The shields are bellow the minimum
                Shield.Current = Shield.Minimum
            End If
            Damage = Damage - shieldPower 'Remove the Shield's power from the damage
            If Damage < 0 Then 'Set damage to 0
                Damage = 0 'Set damage to 0
            End If
        End If
        Return Damage
    End Function

    Public Sub Destroy() 'Removes all references to the Shields object
        ParentShip.Shielding = Nothing 'Remove the reference
        ParentShip = Nothing 'Clear reference
    End Sub

    Public MustOverride Function To_Item() As Item 'Creates an Item representing the Shields object and adds it to the Fleet's inventory before destroying the Shields object

    Public Overrides Sub Update() 'Rotates the Ship's Shield to face the closest enemy and charges the Shield
        If AIControled = True And Powered = True Then
            '-----Turn Shield-----
            Dim turnOffset As Double = Server.Normalise_Direction(ParentShip.targetDirection - ParentShip.Direction) - Direction 'How far the Shield need to turn to face the enemy
            Dim turnRight As Boolean = False 'A Boolean value indicating whether the Shield should turn right
            If turnOffset < 0 Then 'Make sure it's positive
                turnOffset = -turnOffset
                turnRight = True
            End If
            If turnOffset < TurnSpeed Then 'The offset is within turning distance
                Direction = turnOffset + Direction 'Set the direction
            ElseIf turnRight = False Then 'Turn left
                Direction = Server.Normalise_Direction(Direction + TurnSpeed) 'Turn
            Else 'Turn right
                Direction = Server.Normalise_Direction(Direction - TurnSpeed) 'Turn
            End If
            '---------------------

            '-----Send off power costs-----
            If ParentShip.Engineering.AIControled = True Then 'Engineering is AI Controlled
                ParentShip.Engineering.ShieldsCost = (Shield.Maximum - Shield.Current) * PowerCost 'How much power the Shields need to be fully charged
                Influx = 0 'Set the influx to 0
            End If
            '------------------------------
        End If

        '-----Charge Shield-----
        Dim power As Double = Influx / PowerCost
        Shield.Current = Shield.Current + power
        If Shield.Current > Shield.Maximum Then
            Shield.Current = Shield.Maximum
        End If
        '-----------------------
    End Sub

End Class
