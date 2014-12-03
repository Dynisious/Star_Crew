Public MustInherit Class Ship 'Object that combats other Ships
    Inherits Game_Library.Game_Objects.Entity
    Public ReadOnly HitboxXDistance As Integer 'The maximum distance in the x direction from the center of the Ship to the edge of the hitbox
    Public ReadOnly HitboxYDistance As Integer 'The maximum distance in the y direction from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxMaxDistance As Integer 'The maximum distance from the center of the Ship to the edge of the hitbox
    Private ReadOnly HitboxAngles() As Double 'An array of two radian angles indecating the direction of the top two corners of the hitbox
    Private _EvadeTime As Integer 'The actual value of EvadeTime
    Protected ReadOnly Property EvadeTime As Integer 'An Integer value indicating how many updates are necessary to for a quater turn
        Get
            Return _EvadeTime
        End Get
    End Property
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
    Public CombatIndex As Integer = -1 'An Integer value indicating the Ship's index in the CombatSpace's list
    Public Name As String = "Unnamed Ship" 'The name of the Ship
    Public firing As Boolean = False 'A Boolean value indicating whether the Ship is firing
    Public hit As Boolean = False 'A Boolean value indicating whether the Ship is firing
    Public Allegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.nill 'An Allegiance value indicating where this Ship is alligned
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
    Private _Type As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'The actual value of Type
    Public ReadOnly Property Type As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes 'An ObjectTypes value representing what type of object this Ship is
        Get
            Return _Type
        End Get
    End Property
    Public Shared ReadOnly CollisionDamage As Double = 0.1 'The damage done when two ships collide
    Private _ShieldRecharge As Game_Library.Game_Objects.StatInt 'The actual value of ShieldRecharge
    Public ReadOnly Property ShieldRecharge As Game_Library.Game_Objects.StatInt 'A StatInt object that is used to only let the Shield regenerate outside of combat
        Get
            Return _ShieldRecharge
        End Get
    End Property
    Private _Shield As Game_Library.Game_Objects.StatDbl 'The actual value of Shield
    Public ReadOnly Property Shield As Game_Library.Game_Objects.StatDbl 'A StatDbl object used to represent the Shield
        Get
            Return _Shield
        End Get
    End Property
    Public target As Ship 'A reference to a Ship to target
    Private RechargeValue As Double 'How much the Shield recharges each tick

    Public Sub New(ByVal nType As Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes, ByVal nTrackable As Boolean,
                   ByVal nPhysics As Boolean, ByVal nHull As Game_Library.Game_Objects.StatDbl,
                   ByVal nThrottle As Game_Library.Game_Objects.StatDbl, ByVal nAcceleration As Double,
                   ByVal nTurnSpeed As Double, ByVal nHitBoxXDistance As Integer, ByVal nHitBoxYDistance As Integer,
                   ByVal nAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances,
                   ByVal nShieldRecharge As Game_Library.Game_Objects.StatInt,
                   ByVal nShield As Game_Library.Game_Objects.StatDbl, ByVal nRechargeValue As Double)
        _Type = nType
        _Trackable = nTrackable
        _Physics = nPhysics
        _Hull = nHull
        _Throttle = nThrottle
        _Acceleration = nAcceleration
        _TurnSpeed = nTurnSpeed
        _EvadeTime = If((TurnSpeed = 0), 0, (Math.PI / (2 * TurnSpeed)))
        HitboxXDistance = nHitBoxXDistance
        HitboxYDistance = nHitBoxYDistance
        HitboxMaxDistance = Math.Sqrt((HitboxXDistance ^ 2) + (HitboxYDistance ^ 2))
        HitboxAngles = {Math.Atan2(HitboxYDistance, HitboxXDistance),
                        Math.Atan2(HitboxYDistance, -HitboxXDistance)}
        Allegiance = nAllegiance
        _ShieldRecharge = nShieldRecharge
        _Shield = nShield
        RechargeValue = nRechargeValue
    End Sub

    Public Sub Take_Damage(ByVal incomingDamage As Double)
        If Shield.Current >= incomingDamage Then 'The Shields can absorbe the damage
            Shield.Current -= incomingDamage 'Take the damage away from the Shield
        Else 'The Shields cannot absorbe all the damage
            incomingDamage -= Shield.Current 'Remove the damage from the Shield
            Shield.Current = 0 'Set the Shields to 0
            Hull.Current -= incomingDamage 'Remove the damage from the hull
        End If
        ShieldRecharge.Current = 0 'Set the recharge to 0
        hit = True
        If Hull.Current = 0 Then
            Dead = True
        End If
    End Sub

    Public Overrides Sub Destroy()
        _Hull = Nothing
        _Throttle = Nothing
        Name = "Wreckage"
        CombatIndex = -1
        Allegiance = -1
        _Type = -1
        _ShieldRecharge = Nothing
        _Shield = Nothing
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

    Public Overridable Sub Collide(ByRef Collider As Ship) 'Handles the Ship colliding with another Ship
        Take_Damage(CollisionDamage) 'Take 0.1 damage
    End Sub

    Protected Function Analyse_Situation() As Object() 'Returns an Array of Arrays
        Dim ships As New List(Of Ship)(Server.Combat.ShipList.Count) 'Create a list that can hold every Ship in combat
        Dim distances As New List(Of Integer)(Server.Combat.ShipList.Count) 'Create a list that can hold an Integer for every Ship in combat
        For Each i As Ship In Server.Combat.ShipList 'Loop through all Ships
            If (i.Trackable Or i.Allegiance = Allegiance) And i.CombatIndex <> CombatIndex Then 'The Ship can be tracked and is not this Ship
                Dim distance As Integer = Math.Sqrt(((i.X - X) ^ 2) + ((i.Y - Y) ^ 2)) 'Get the distance of the target from the Ship
                Dim index As Integer = 0 'The index to insert the object at
                Do Until index = distances.Count 'Loop through each index
                    If distances(index) >= distance Then Exit Do 'Exit the loop
                    index += 1 'Add one to the index
                Loop
                ships.Insert(index, i) 'Insert the object at this index
                distances.Insert(index, distance) 'Insert the distance at this index
            End If
        Next
        Return {ships.ToArray(), distances.ToArray()} 'Return the arrays
    End Function

    Public Overrides Sub Update() 'Updates the Ship
        firing = False 'The Ship has not fired this update
        hit = False 'The Ship has not been hit this update
        X = X + (Speed * Math.Cos(Direction)) 'Update the Ship's X position
        Y = Y + (Speed * Math.Sin(Direction)) 'Update the Ship's Y position
        ShieldRecharge.Current += 1 'Add one
        If ShieldRecharge.Current = ShieldRecharge.Maximum Then 'The shields can recharge
            Shield.Current += RechargeValue 'Add a point to the Shield
        End If
    End Sub

End Class
