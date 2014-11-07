Public Class Projectile
    Inherits Ship
    Private range As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing how far the projectile can travel before disappearing

    Public Sub New(ByVal nRange As Integer, ByVal nSpeed As Integer, ByVal nDirection As Double, ByVal nDamage As Double, ByVal nX As Integer, ByVal nY As Integer)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile, False, True, nDamage,False,
                   Nothing, New Game_Library.Game_Objects.StatDbl(0, nSpeed, nSpeed, False), 0, 0, 0, 0)
        range = New Game_Library.Game_Objects.StatDbl(0, 0, nRange, True)
        _Direction = nDirection
        X = nX
        Y = nY
    End Sub

    Public Overrides Sub Collide(ByVal impactDamage As Integer, ByVal impactDirection As Double, ByVal deflect As Boolean) 'Handles the projectile hitting the Ship
        Dead = True 'Kill the projectile
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        range.Current += Speed
        If range.Current = range.Maximum Then Dead = True 'Kill the projectile
    End Sub

End Class
