Public Class AIShip
    Inherits Ship

    Public Sub New()
        Throttle.Current = Throttle.Maximum * 0.8
        X = Int(Rnd() * 2000) - 1000
        Y = Int(Rnd() * 2000) - 1000
        _Hull.Set_Bounds(0, 10)
        _Hull.Current = 10
        _TurnSpeed = Math.PI / 30
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Direction += TurnSpeed
    End Sub

End Class
