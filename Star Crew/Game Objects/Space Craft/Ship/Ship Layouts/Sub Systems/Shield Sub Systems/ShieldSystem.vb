<Serializable()>
Public MustInherit Class ShieldSystem
    <NonSerialized()>
    Public DefenceModifiers(Weapon.DamageTypes.Max - 1) As Double
    Public Defences(Shields.Sides.Max - 1) As StatDbl

    Public Sub New(ByVal mustBeCalled As Boolean) 'Forces the constructor to be called
        For i As Integer = 0 To Weapon.DamageTypes.Max - 1 'Set all damage modifiers to 1 so that they have no effect until altered
            DefenceModifiers(i) = 1
        Next
    End Sub
End Class
