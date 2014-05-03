<Serializable()>
Public Class StatDbl 'Holds 2 Double values, a current and a max
    Public current As Double 'The current value of the Stat
    Public max As Double 'The maximum value of the Stat

    Public Sub New(ByVal nCurrent As Double, ByVal nMax As Double)
        current = nCurrent
        max = nMax
    End Sub
End Class