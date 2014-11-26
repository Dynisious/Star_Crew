Public Class Projectile
    Inherits Ship
    Private Range As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing how far the projectile can travel before disappearing
    Private Damage As Double 'The damage delt by the projectile

    Public Sub New(ByVal nRange As Integer, ByVal nSpeed As Integer, ByVal nDirection As Double, ByVal nDamage As Double, ByVal nX As Integer, ByVal nY As Integer, ByVal nAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile, False, True,
                   Nothing, New Game_Library.Game_Objects.StatDbl(0, nSpeed, nSpeed, False), 0, 0, 0, 0, nAllegiance,
                  New Game_Library.Game_Objects.StatInt(0, 0, 0, True), New Game_Library.Game_Objects.StatDbl(0, 0, 0, True))
        range = New Game_Library.Game_Objects.StatDbl(0, 0, nRange, True)
        _Direction = nDirection
        Damage = (nDamage - CollisionDamage)
        X = nX
        Y = nY
    End Sub

    Public Overrides Sub Collide(ByRef Collider As Ship) 'Handles the projectile hitting the Ship
        Collider.Take_Damage(Damage)
        Dead = True 'Kill the projectile
    End Sub

    Public Overrides Sub Destroy()
        MyBase.Destroy()
        Range = Nothing
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        range.Current += Speed
        If range.Current = range.Maximum Then Dead = True 'Kill the projectile
    End Sub

End Class
