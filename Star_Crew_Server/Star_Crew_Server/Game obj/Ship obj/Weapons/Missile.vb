Public Class Missile
    Inherits Ship
    Private Range As Game_Library.Game_Objects.StatDbl 'A StatDbl object representing how far the projectile can travel before disappearing
    Private Damage As Double 'The damage delt by the Missile
    Private Radius As Integer 'An Integer value indicating how far out the blast will hit
    Private Target As Ship 'The target the Missile is tracking

    Public Sub New(ByRef nTarget As Ship, ByVal nAccleration As Double, ByVal nRange As Integer, ByVal nRadius As Integer, ByVal nSpeed As Double, ByVal nMaxSpeed As Double, ByVal nDirection As Double, ByVal nDamage As Double, ByVal nX As Integer, ByVal nY As Integer, ByVal nAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Missile, False, True,
                   Nothing, New Game_Library.Game_Objects.StatDbl(0, nSpeed, nMaxSpeed, True), nAccleration, (Math.PI / 50), 0, 0, nAllegiance,
                  New Game_Library.Game_Objects.StatInt(0, 0, 0, True), New Game_Library.Game_Objects.StatDbl(0, 0, 0, True))
        Target = nTarget
        Range = New Game_Library.Game_Objects.StatDbl(0, 0, nRange, True)
        _Direction = nDirection
        Damage = nDamage
        Radius = nRadius
        X = nX
        Y = nY
    End Sub

    Public Overrides Sub Collide(ByRef Collider As Ship) 'Handles the projectile hitting the Ship
        If Not Dead Then
            Dim obj As Object = Analyse_Situation() 'Get the arrays
            Dim ships() As Ship = obj(0) 'Get the array of Ships
            Dim distances() As Integer = obj(1) 'Get the array of Integers

            Dim upper As Integer = distances.Length - 1
            Dim lower As Integer = 0
            Dim index As Integer
            Do
                index = lower + Math.Ceiling((upper - lower) / 2)
                If distances(index) > Radius Then 'Look lower in the array
                    If index = upper Then
                        index = lower
                    End If
                    upper = index
                ElseIf distances(index) < Radius Then 'Look higher in the array
                    lower = index
                Else 'Stop searching
                    upper = index
                    lower = index
                End If
            Loop Until (upper = lower)  'Loop until a value close to the edge of the radius is found
            If index <> distances.Length - 1 Then 'Look higher up
                Do
                    index += 1 'Add one to the index
                Loop Until (distances(index) > Radius) 'Loop until the last Ship inside the index has been found
            End If
            index -= 1 'Take one from the index

            For i As Integer = 0 To index 'Loop through every target in range
                If ships(i).Allegiance <> Allegiance Then ships(i).Take_Damage(Damage) 'Do Damage to the Ship
            Next

            Dead = True 'Kill the projectile
        End If
    End Sub

    Public Overrides Sub Destroy()
        MyBase.Destroy()
        Target = Nothing 'Clear the target
        Range = Nothing
    End Sub

    Public Overrides Sub Update()
        Throttle.Current += Acceleration 'Accelerate
        Direction += If((Server.Normalise_Direction(Math.Atan2((Target.Y - Y), (Target.X - X)) - Direction) < Math.PI),
                        TurnSpeed,
                        -TurnSpeed) 'Turn to face the target
        MyBase.Update()
        Range.Current += Speed
        If Range.Current = Range.Maximum Then Dead = True 'Kill the projectile
    End Sub

End Class
