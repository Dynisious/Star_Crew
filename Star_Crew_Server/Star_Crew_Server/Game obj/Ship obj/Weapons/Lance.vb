﻿Public Class Lance
    Inherits WeaponBase

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, New Game_Library.Game_Objects.StatInt(0, 16, 16, True), 80)
    End Sub

    Protected Overrides Sub Fire_Weapon()
        If Parent.target IsNot Nothing Then 'There is a target to shoot
            Server.Combat.adding.Add(New Missile(Parent.target, 0.6, 4200, 200, Parent.Speed, 21, Parent.Direction, 20, Parent.X, Parent.Y, Parent.Allegiance))
        Else
            Ammunition.Current += 1 'The ammunition gets replenished by one
        End If
    End Sub

End Class
