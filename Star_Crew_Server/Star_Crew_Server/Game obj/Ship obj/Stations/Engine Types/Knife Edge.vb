Public Class Knife_Edge
    Inherits Engines

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, New Game_Library.StatInt(0, 50, 50), 10, New Game_Library.StatDbl(0, 10, 10), New Game_Library.StatDbl(3, 3, 15), 0.4)
        ReDim WeaponsCosts(ParentShip.Mounts.Length - 1)
    End Sub

    Public Overrides Function To_Item() As Item

    End Function
End Class
