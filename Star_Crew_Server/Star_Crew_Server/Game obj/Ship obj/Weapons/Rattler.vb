Public NotInheritable Class Rattler
    Inherits Weapon

    Public Sub New(ByRef nParent As Ship)
        MyBase.New(nParent, 100, 2, New Game_Library.Game_Objects.StatInt(-1, -1, -1, True), 2)
    End Sub

    Public Overrides Sub Do_Damage(ByVal targets As Ship())
        targets(0).Take_Damage(Damage) 'Damage the target
    End Sub

End Class
