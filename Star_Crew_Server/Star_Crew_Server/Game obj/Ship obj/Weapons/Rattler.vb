﻿Public NotInheritable Class Rattler
    Inherits Weapon

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, 2, New Game_Library.Game_Objects.StatInt(-1, -1, -1, True), 4)
    End Sub

    Protected Overrides Sub Fire_Weapon()
        Server.Combat.adding.Add(New Projectile(950, (Parent.Speed + 19), Parent.Direction, 2,
                                              (Parent.X + (1.2 * Parent.HitboxXDistance * Math.Cos(Parent.Direction))),
                                              (Parent.Y + (1.2 * Parent.HitboxYDistance * Math.Sin(Parent.Direction)))))
    End Sub

End Class
