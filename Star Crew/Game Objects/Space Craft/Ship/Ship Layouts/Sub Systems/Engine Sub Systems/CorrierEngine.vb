<Serializable()>
Public Class CorrierEngine
    Inherits EngineSystem

    Public Sub New()
        Engines = New StatDbl(150, 150)
        PowerCore = New StatDbl(8, 8)
    End Sub

End Class
