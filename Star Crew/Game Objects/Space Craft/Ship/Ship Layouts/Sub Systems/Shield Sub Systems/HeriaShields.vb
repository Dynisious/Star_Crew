<Serializable()>
Public Class HeriaShields
    Inherits ShieldSystem

    Public Sub New()
        MyBase.New(True)

        '-----Set Shields----- 'Sets the values and capacities of the shields
        Defences(Shields.Sides.FrontShield) = New StatDbl(30, 30)
        Defences(Shields.Sides.RightShield) = New StatDbl(15, 15)
        Defences(Shields.Sides.BackShield) = New StatDbl(20, 20)
        Defences(Shields.Sides.LeftShield) = New StatDbl(15, 15)
        '---------------------
    End Sub

End Class
