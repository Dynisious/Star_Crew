Public Class Blank
    Inherits WeaponLayout

    Public Sub New()
        For i As Integer = 0 To Weapon.Stats.Max - 1
            stats(i) = New Stat(0, 0)
        Next
        nTurnDistance = New Stat(0, 0)
        nTurnSpeed = New Stat(0, 0)
    End Sub

End Class
