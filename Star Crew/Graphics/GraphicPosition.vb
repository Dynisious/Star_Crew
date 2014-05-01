<Serializable()>
Public Class GraphicPosition
    Public Allegience As Galaxy.Allegence
    Public Hit As Boolean
    Public Firing As Boolean
    Public X As Integer
    Public Y As Integer
    Public Direction As Single

    Public Sub New(ByVal nAllegience As Galaxy.Allegence, ByVal nHit As Boolean, ByVal nFiring As Boolean, ByVal nX As Integer, ByVal nY As Integer, ByVal nDirection As Single)
        Allegience = nAllegience
        Hit = nHit
        Firing = nFiring
        X = nX
        Y = nY
        Direction = nDirection
    End Sub

End Class
