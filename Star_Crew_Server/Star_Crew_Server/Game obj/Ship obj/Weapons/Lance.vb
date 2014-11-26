Public Class Lance
    Inherits Weapon

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, New Game_Library.Game_Objects.StatInt(0, 16, 16, True), 80)
    End Sub

    Protected Overrides Sub Fire_Weapon()
        Server.Combat.adding.Add(New Missile(Parent.target, 0.6, 4200, 200, Parent.Speed, 21, Parent.Direction, 20, Parent.X, Parent.Y, Parent.Allegiance))
    End Sub

End Class
