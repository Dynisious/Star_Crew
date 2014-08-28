Public Class Bunter
    Inherits Shields

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, New Game_Library.StatInt(0, 50, 50), 10, New Game_Library.StatDbl(0, 20, 20), (Math.PI / 15), (Math.PI / 4), 5)
    End Sub

    Public Overrides Function To_Item() As Item

    End Function
End Class
