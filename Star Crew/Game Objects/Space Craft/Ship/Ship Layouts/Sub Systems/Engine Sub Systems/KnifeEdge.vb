<Serializable()>
Public Class KnifeEdge
    Inherits EngineSystem

    Public Sub New()
        Engines = New StatDbl(100, 100)
        PowerCore = New StatDbl(3, 3)
    End Sub

End Class
