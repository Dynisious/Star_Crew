Public Class Ship 'An Entity held within a Fleet that is engaged in Ship to Ship battles
    Inherits Game_Library.Game_Objects.Entity
    Public allegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'The allegiance of the Ship
    Private _direction As Double 'A Double value indicating which way the Ship is facing
    Private Property direction As Double
        Set(ByVal value As Double)
            _direction = Normalise_Direction(value) 'Make sure the value is normalised
        End Set
        Get
            Return _direction
        End Get
    End Property
    Private speed As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing the speed of the Ship

    Public Overrides Sub Destroy()
        Dead = True
        allegiance = -1 'Clear the allegiance
    End Sub

    Public Overrides Sub Update()
        X += speed.Current * Math.Cos(direction)
        Y += speed.Current * Math.Sin(direction)
    End Sub

End Class
