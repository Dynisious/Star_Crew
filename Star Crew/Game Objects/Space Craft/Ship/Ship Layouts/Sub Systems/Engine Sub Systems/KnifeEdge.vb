<Serializable()>
Public Class KnifeEdge
    Inherits EngineSystem

    Public Sub New()
        Engines = New StatDbl(100, 100)
        PowerCore = New StatDbl(6, 6)
    End Sub

End Class
