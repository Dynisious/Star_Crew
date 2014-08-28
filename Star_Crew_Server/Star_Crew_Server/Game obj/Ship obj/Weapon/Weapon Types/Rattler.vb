Public Class Rattler
    Inherits Weapon

    Public Sub New(ByRef nParent As Ship, ByVal nIndex As Integer)
        MyBase.New(nParent, nIndex, DamageTypes.Pulse, 1, (Math.PI / 6))
        Range = New Game_Library.StatInt(0, 70, 70)
        Ammunition = New Game_Library.StatInt(0, 25, 25)
        Ready = New Game_Library.StatInt(0, 3, 3)
        Damage = New Game_Library.StatDbl(0, 10, 10)
        Integrity = New Game_Library.StatInt(0, 30, 30)
    End Sub

    Protected Overrides Sub Do_Damage(ByRef target As Ship, ByVal firingDirection As Double, ByVal targetDistance As Integer)
        If targetDistance < Range.Current Then
            Ammunition.Current = Ammunition.Current - 1
            Ready.Current = 0
            target.Take_Damage(firingDirection, Damage.Current, Type)
        End If
    End Sub

    Public Overrides Function To_Item() As Object

    End Function
End Class
