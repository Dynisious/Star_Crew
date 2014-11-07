Public Class AIShip
    Inherits Ship

    Public Sub New()
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Ship, True, True, 5, True,
                   New Game_Library.Game_Objects.StatDbl(0, 10, 10, True),
                   New Game_Library.Game_Objects.StatDbl(0, 5, 5, True), 0.5, (Math.PI / 80), 20, 20)
        X = Int(Rnd() * 6000) - 3000
        Y = Int(Rnd() * 6000) - 3000
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Direction += TurnSpeed
    End Sub

End Class
