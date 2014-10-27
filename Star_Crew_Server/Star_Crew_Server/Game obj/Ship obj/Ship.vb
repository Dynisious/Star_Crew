Public MustInherit Class Ship 'Object that combats other Ships
    Inherits Game_Library.Game_Objects.Entity
    Private ReadOnly HitboxXDistance As Integer = 20 'The maximum distance in the x direction from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxYDistance As Integer = 20 'The maximum distance in the y direction from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxMaxDistance As Integer = Math.Sqrt((HitboxXDistance ^ 2) + (HitboxYDistance ^ 2)) 'The maximum distance from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxAngles() As Double = {
        Math.Atan2(HitboxYDistance, HitboxXDistance),
        Math.Atan2(HitboxYDistance, -HitboxXDistance)} 'An array of two radian angles indecating the direction of the top two corners of the hitbox
    Private _Hull As New Game_Library.Game_Objects.StatDbl(0, 100, 100, True) 'The actual value of Hull
    Public ReadOnly Property Hull As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing the minimum, current and maximum values of the Ship's Hull
        Get
            Return _Hull
        End Get
    End Property
    Public Throttle As New Game_Library.Game_Objects.StatDbl(0, 0, 15, True) 'A StatDbl object representing the throttle of the Ship
    Private _Acceleration As Double = 0.5 'The actual value of Acceleration
    Public ReadOnly Property Acceleration As Double 'A Double value that controls how quickely the throttle changes
        Get
            Return _Acceleration
        End Get
    End Property
    Public ReadOnly Property Speed As Double 'The Ship's current speed
        Get
            Return Throttle.Current 'Calculate the Speed of the Ship
        End Get
    End Property
    Private _TurnSpeed As Double = Math.PI / 15 'The actual value of TurnSpeed
    Public ReadOnly Property TurnSpeed As Double 'The radians that the Ship can rotate per update
        Get
            Return _TurnSpeed 'Calculate the Turnspeed
        End Get
    End Property
    Private _Direction As Double 'The actual value of Direction
    Public Property Direction As Double 'The Direction of the Ship in world space in radians
        Get
            Return _Direction
        End Get
        Set(ByVal value As Double)
            _Direction = Server.Normalise_Direction(value)
        End Set
    End Property
    Protected _Gun As Weapon 'The actual value of Mounts
    Public ReadOnly Property Gun As Weapon 'A Weapon objects for the Ship
        Get
            Return _Gun
        End Get
    End Property
    Public CombatIndex As Integer = -1 'An Integer value indicating the Ship's index in the CombatSpace's list
    Public Name As String = "Unnamed Ship" 'The name of the Ship
    Public firing As Boolean = False 'A Boolean value indicating whether the Ship is firing

    Public Sub Take_Damage(ByVal incomingDamage As Double)
        Hull.Current -= incomingDamage
        If Hull.Current = 0 Then
            Dead = True
        End If
    End Sub

    Public Overrides Sub Destroy()
        _Hull = Nothing
        Throttle = Nothing
        Name = "Wreckage"
        If Gun IsNot Nothing Then Gun.Destroy()
        _Gun = Nothing
    End Sub

    Public Function Get_Collision_Radia(ByVal objectDirection As Double) As Integer 'Returns the distance of the edge of the collision box in the direction of the calling object
        objectDirection += Math.PI - Direction 'Get the direction of the calling object in object space
        Dim xCoord As Integer = Math.Cos(objectDirection) * HitboxMaxDistance 'Get the xCoord of edge of the hit box if it was a circle
        If xCoord < 0 Then xCoord = -xCoord 'Make sure the coord is positive
        If xCoord > HitboxXDistance Then xCoord = HitboxXDistance 'Make sure the xCoord is on the actual hitbox
        Dim yCoord As Integer = Math.Sin(objectDirection) * HitboxMaxDistance 'Get the xCoord of edge of the hit box if it was a circle
        If yCoord < 0 Then yCoord = -yCoord 'Make sure the coord is positive
        If yCoord > HitboxYDistance Then yCoord = HitboxYDistance 'Make sure the xCoord is on the actual hitbox
        Return Math.Sqrt((xCoord ^ 2) + (yCoord ^ 2)) 'Return the distance to the edge of the hitbox in the direction of the calling object
    End Function

    Public Function Get_FOV_Cord(ByVal objectDirection As Double) As Integer
        objectDirection -= objectDirection 'convert the direction into an object space angle
        Dim angles() = {
            Server.Normalise_Direction(objectDirection + Server.QuarterCircle - HitboxAngles(0)),
            Server.Normalise_Direction(objectDirection + Server.QuarterCircle - HitboxAngles(1))}
        For i As Integer = 0 To 1 'Loop through both angles
            If angles(i) > Math.PI Then angles(i) = (FullCircle - angles(i)) 'Make sure the angle is the smaller part
        Next
        Dim cord As Integer 'An integer value representing the FOV Cord
        If angles(0) < angles(1) Then 'Use the larger angle
            cord = Math.Cos(angles(1)) * HitboxMaxDistance 'Return half of the FOV cord
        Else
            cord = Math.Cos(angles(0)) * HitboxMaxDistance 'Return half the FOV cord
        End If
        Return If((cord > 0), cord, -cord) 'Return the positive cord
    End Function

    Public Overrides Sub Update() 'Updates the Ship
        firing = False 'The Ship has not fired this update
        X = X + (Speed * Math.Cos(Direction)) 'Update the Ship's X position
        Y = Y + (Speed * Math.Sin(Direction)) 'Update the Ship's Y position
        If Gun IsNot Nothing Then Gun.Update() 'Update the gun
    End Sub

End Class
