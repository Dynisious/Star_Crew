<Serializable()>
Public Class StatInt 'Holds 2 Integer values, a current and a max
    Public current As Integer 'The current value of the Stat
    Public max As Integer 'The max value of the Stat

    Public Sub New(ByVal nCurrent As Integer, ByVal nMax As Integer)
        current = nCurrent
        max = nMax
    End Sub
End Class