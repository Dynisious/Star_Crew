Public MustInherit Class Ship 'Object that combats other Ships
    Inherits Game_Library.Game_Objects.Entity
    Public ReadOnly HitboxXDistance As Integer 'The maximum distance in the x direction from the center of the Ship to the edge of the hitbox
    Public ReadOnly HitboxYDistance As Integer 'The maximum distance in the y direction from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxMaxDistance As Integer 'The maximum distance from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxAngles() As Double 'An array of two radian angles indecating the direction of the top two corners of the hitbox
    Protected _Hull As Game_Library.Game_Objects.StatDbl 'The actual value of Hull
    Public ReadOnly Property Hull As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing the minimum, current and maximum values of the Ship's Hull
        Get
            Return _Hull
        End Get
    End Property
    Protected _Throttle As Game_Library.Game_Objects.StatDbl 'The actual value of Throttle
    Public ReadOnly Property Throttle As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing the throttle of the Ship
        Get
            Return _Throttle
        End Get
    End Property
    Protected _Acceleration As Double 'The actual value of Acceleration
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
    Protected _TurnSpeed As Double 'The actual value of TurnSpeed
    Public ReadOnly Property TurnSpeed As Double 'The radians that the Ship can rotate per update
        Get
            Return _TurnSpeed 'Calculate the Turnspeed
        End Get
    End Property
    Protected _Direction As Double 'The actual value of Direction
    Public Property Direction As Double 'The Direction of the Ship in world space in radians
        Get
            Return _Direction
        End Get
        Set(ByVal value As Double)
            _Direction = Server.Normalise_Direction(value)
        End Set
    End Property
    Protected _Primary As Weapon 'The actual value of Mounts
    Public ReadOnly Property Primary As Weapon 'A Weapon objects for the Ship
        Get
            Return _Primary
        End Get
    End Property
    Public CombatIndex As Integer = -1 'An Integer value indicating the Ship's index in the CombatSpace's list
    Public Name As String = "Unnamed Ship" 'The name of the Ship
    Public firing As Boolean = False 'A Boolean value indicating whether the Ship is firing
    Public hit As Boolean = False 'A Boolean value indicating whether the Ship is firing
    Protected _Trackable As Boolean 'The actual value of Trackable
    Public ReadOnly Property Trackable As Boolean 'A Boolean value indicating whether the ship can be tracked by other Ships
        Get
            Return _Trackable
        End Get
    End Property
    Protected _Physics As Boolean 'The actual value of Physics
    Public ReadOnly Property Physics As Boolean 'A Boolean value indicating whether the Ship is affected by physics
        Get
            Return _Physics
        End Get
    End Property
    Protected _CollideDamage As Integer 'The actual value of CollideDamage
    Public ReadOnly Property CollideDamage As Integer 'An Integer value indicating how much damage is done when another Ship collides into this one
        Get
            Return _CollideDamage
        End Get
    End Property
    Private _CauseDeflect As Boolean 'The actual value of CauseDeflect
    Public ReadOnly Property CauseDeflect As Boolean 'A Boolean value indicating whether the Ship causes a deflection when hit
        Get
            Return _CauseDeflect
        End Get
    End Property
    Private _Type As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'The actual value of Type
    Public ReadOnly Property Type As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'An ObjectTypes value representing what type of object this Ship is
        Get
            Return _Type
        End Get
    End Property

    Public Sub New(ByVal nType As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes, ByVal nTrackable As Boolean,
                   ByVal nPhysics As Boolean, ByVal nCollideDamage As Integer, ByVal nCauseDeflect As Boolean,
                   ByVal nHull As Game_Library.Game_Objects.StatDbl, ByVal nThrottle As Game_Library.Game_Objects.StatDbl,
                   ByVal nAcceleration As Double, ByVal nTurnSpeed As Double, ByVal nHitBoxXDistance As Integer,
                   ByVal nHitBoxYDistance As Integer)
        _Type = nType
        _Trackable = nTrackable
        _Physics = nPhysics
        _CollideDamage = nCollideDamage
        _CauseDeflect = nCauseDeflect
        _Hull = nHull
        _Throttle = nThrottle
        _Acceleration = nAcceleration
        _TurnSpeed = nTurnSpeed
        HitboxXDistance = nHitBoxXDistance
        HitboxYDistance = nHitBoxYDistance
        HitboxMaxDistance = Math.Sqrt((HitboxXDistance ^ 2) + (HitboxYDistance ^ 2))
        HitboxAngles = {Math.Atan2(HitboxYDistance, HitboxXDistance),
                        Math.Atan2(HitboxYDistance, -HitboxXDistance)}
    End Sub

    Public Sub Take_Damage(ByVal incomingDamage As Double)
        Hull.Current -= incomingDamage
        hit = True
        If Hull.Current = 0 Then
            Dead = True
        End If
    End Sub

    Public Overrides Sub Destroy()
        _Hull = Nothing
        _Throttle = Nothing
        Name = "Wreckage"
        If Primary IsNot Nothing Then
            Primary.Destroy()
            _Primary = Nothing
        End If
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

    Public Overridable Sub Collide(ByVal impactDamage As Integer, ByVal impactDirection As Double, ByVal deflect As Boolean) 'Handles the Ship colliding with another Ship
        Take_Damage(impactDamage) 'Take the damage

        If deflect Then 'Deflect from the collision
            X -= Speed * Math.Cos(impactDirection) 'Move the Ship along the x axis
            Y -= Speed * Math.Sin(impactDirection) 'Move the Ship along the y axis
        End If
    End Sub

    Public Overrides Sub Update() 'Updates the Ship
        firing = False 'The Ship has not fired this update
        hit = False 'The Ship has not been hit this update
        X = X + (Speed * Math.Cos(Direction)) 'Update the Ship's X position
        Y = Y + (Speed * Math.Sin(Direction)) 'Update the Ship's Y position
        If Primary IsNot Nothing Then Primary.Update() 'Update the gun
    End Sub

End Class
