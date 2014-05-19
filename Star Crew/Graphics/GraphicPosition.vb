<Serializable()>
Public Class GraphicPosition 'Holds all the information for Client to render a ship model
    Public Allegience As Galaxy.Allegence 'The allegience of the craft
    Public Format As Integer 'The type of ship it is
    Public Hit As Boolean 'Indecates whether the craft has been hit
    Public X As Integer 'Indecates the X coordinate of the craft
    Public Y As Integer 'Indecares the Y coordinate of the craft
    Public Direction As Single 'Indecates the direction the craft is traveling
    Public Hull As StatInt 'A StatInt object that represents the Ship's hull

    Public Sub New(ByVal nAllegience As Galaxy.Allegence, ByVal nFormat As Integer, ByVal nHit As Boolean,
                   ByVal nX As Integer, ByVal nY As Integer, ByVal nDirection As Single, ByVal nHull As StatInt)
        Allegience = nAllegience
        Format = nFormat
        Hit = nHit
        X = nX
        Y = nY
        Direction = nDirection
        Hull = nHull
    End Sub

End Class
