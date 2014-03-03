<Serializable()>
Public Class Stat
    Public current As Double
    Public max As Double

    Public Sub New(ByVal nCurrent As Double, ByVal nMax As Double)
        current = nCurrent
        max = nMax
    End Sub
End Class