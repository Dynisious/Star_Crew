<Serializable()>
Public Class CorrierEngine
    Inherits EngineSystem

    Public Sub New()
        Engines = New StatDbl(150, 150)
        PowerCore = New StatDbl(4.5, 4.5)
    End Sub

End Class
