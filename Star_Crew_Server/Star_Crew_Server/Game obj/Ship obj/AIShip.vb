Public Class AIShip
    Inherits Ship

    Public Sub New()
        Throttle.Current = Throttle.Maximum * 0.8
        X = Int(Rnd() * 2000) - 1000
        Y = Int(Rnd() * 2000) - 1000
        Take_Damage(80) 'Damage the Ship
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Direction += TurnSpeed
    End Sub

End Class
