<Serializable()>
Public Class StatInt
    Public current As Integer
    Public max As Integer

    Public Sub New(ByVal nCurrent As Integer, ByVal nMax As Integer)
        current = nCurrent
        max = nMax
    End Sub
End Class