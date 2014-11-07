Public NotInheritable Class Rattler
    Inherits Weapon

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, 2, New Game_Library.Game_Objects.StatInt(-1, -1, -1, True), 2)
    End Sub

    Protected Overrides Sub Fire_Weapon()
        Server.Combat.adding.Add(New Projectile(200, 20, Parent.Direction, 2,
                                              (Parent.X + (1.2 * Parent.HitboxXDistance * Math.Cos(Parent.Direction))),
                                              (Parent.Y + (1.2 * Parent.HitboxYDistance * Math.Sin(Parent.Direction)))))
    End Sub

End Class
