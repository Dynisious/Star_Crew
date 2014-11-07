Public Class Projectile
    Inherits Ship
    Private range As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing how far the projectile can travel before disappearing
    Private damage As Double 'A Double value representing how much damage the projectile does to the target

    Public Sub New(ByVal nRange As Integer, ByVal nSpeed As Integer, ByVal nDirection As Double, ByVal nDamage As Double, ByVal nX As Integer, ByVal nY As Integer)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Projectile, False, Nothing,
                   New Game_Library.Game_Objects.StatDbl(0, nSpeed, nSpeed, False), 0, 0)
        range = New Game_Library.Game_Objects.StatDbl(0, 0, nRange, True)
        _Direction = nDirection
        damage = nDamage
        X = nX
        Y = nY
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        range.Current += Speed
        If range.Current = range.Maximum Then Dead = True 'Kill the projectile
        For Each i As Ship In Server.Combat.ShipList
            If i.Trackable = True Then 'The target can be tracked/hit
                Dim targetX As Integer = (i.X - X) 'The x coord of the object relative to the projectile
                Dim targetY As Integer = (i.Y - Y) 'The y coord of the object relative to the projectile
                If Math.Sqrt((targetX ^ 2) + (targetY ^ 2)) < Get_Collision_Radia(Math.Atan2(targetY, targetX)) Then 'The projectile has hit the target
                    Dead = True 'Kill the projectile
                    i.Take_Damage(damage) 'Damage the target
                    Exit For
                End If
            End If
        Next
    End Sub

End Class
