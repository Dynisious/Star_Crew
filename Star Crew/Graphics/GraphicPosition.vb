<Serializable()>
Public Class GraphicPosition
    Public Col As Color
    Public Hit As Boolean
    Public Firing As Boolean
    Public X As Integer
    Public Y As Integer
    Public Direction As Double

    Public Sub New(ByVal nCol As Color, ByVal nHit As Boolean, ByVal nFiring As Boolean, ByVal nX As Integer, ByVal nY As Integer, ByVal nDirection As Double)
        Col = nCol
        Hit = nHit
        Firing = nFiring
        X = nX
        Y = nY
        Direction = nDirection
    End Sub
End Class
