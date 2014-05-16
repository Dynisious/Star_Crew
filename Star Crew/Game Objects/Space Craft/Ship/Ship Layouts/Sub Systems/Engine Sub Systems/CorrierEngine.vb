<Serializable()>
Public Class CorrierEngine
    Inherits EngineSystem

    Public Sub New()
        Engines = New StatDbl(150, 150)
        PowerCore = New StatDbl(4, 4)
    End Sub

End Class
