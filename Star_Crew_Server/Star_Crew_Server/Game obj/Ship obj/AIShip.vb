Public Class AIShip
    Inherits Ship
    Private target As Ship 'A reference to a Ship indicating the closest target to the Player
    Private targetDistance As Integer = -1 'An Integer indicating the distance to the target
    Private targetDirection As Double 'A Double value indicating the direction to the target in object space
    Private ReadOnly Primary As New Rattler(Me) 'The Ships weapon

    Public Sub New()
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Ship, True, True,
                   New Game_Library.Game_Objects.StatDbl(0, 30, 30, True), New Game_Library.Game_Objects.StatDbl(1, 1, 10, True),
                   1, (Math.PI / 30), 20, 20, Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Pirate_Alliance)
        X = Int(Rnd() * 6000) - 3000
        Y = Int(Rnd() * 6000) - 3000
    End Sub

    Private Sub Steering(ByVal evade As Boolean)
        If evade Then 'Evade the target
            Throttle.Current += If((Server.Normalise_Direction(targetDirection + (Math.PI / 2)) > Math.PI), Acceleration, -Acceleration) 'Increment the throttle
            Direction += If((targetDirection < Math.PI), -TurnSpeed, TurnSpeed) 'Turn away from the target
        Else 'Charge the target
            Throttle.Current += Acceleration 'Increase the throttle
            Direction += If((targetDirection < Math.PI), TurnSpeed, -TurnSpeed) 'Turn towards the target
        End If
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Primary.Update()

        '-----Targeting-----
        target = Nothing 'Clear target
        Dim obj As Object() = Analyse_Situation() 'Get the arrays
        For i As Integer = 0 To obj(0).Length - 1 'Loop through all the indexes
            Dim tar As Ship = obj(0)(i) 'Get the current Ship object
            Dim tarDirection As Double = Math.Atan2((tar.Y - Y), (tar.X - X)) 'Get the direction to the target in world space
            Dim closingSpeed As Integer = ((Speed * Math.Cos(tarDirection - Direction)) - (tar.Speed * Math.Cos(tar.Direction - Direction))) 'Get the speed at which the target is approaching
            If closingSpeed < 0 And (Direction > (Math.PI / 2)) And (Direction < (3 * Math.PI / 2)) Then closingSpeed = -closingSpeed 'Make sure the 
            Dim collision As Boolean = If((closingSpeed > 0), (((obj(1)(i) - Get_Collision_Radia(tarDirection) - tar.Get_Collision_Radia(tarDirection + Math.PI)) / closingSpeed) < EvadeTime), False) 'Get a Boolean indicating whether a collision is incomming

            If collision Or (target Is Nothing And (tar.Allegiance <> Allegiance)) Then 'Select this target
                target = obj(0)(i) 'Get the target object for the Ship
                targetDistance = obj(1)(i) 'Get the target distance for the Ship
                targetDirection = Server.Normalise_Direction(tarDirection - Direction) 'Get the direction to the target in object space
                If (tar.Allegiance <> Allegiance) And If((tarDirection < Math.PI), (TurnSpeed > tarDirection),
                                                         (TurnSpeed > -(tarDirection - (2 * Math.PI)))) Then Primary.Fire() 'Fire the weapon
                Steering(collision) 'Steer the Ship
                Exit For
            End If
        Next
        '-------------------
    End Sub

End Class
