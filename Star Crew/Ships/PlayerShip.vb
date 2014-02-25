Public Class PlayerShip
    Inherits Ship
    Public incomingMessage(3)

    Public Sub New(ByVal nShipStats As Layout)
        MyBase.New(Nothing, nShipStats, Allegence.Player)
    End Sub

    Public Overrides Sub UpdateShip()
        MyBase.UpdateShip()
    End Sub

End Class
