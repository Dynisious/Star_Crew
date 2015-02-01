Public Class Fleet 'Holds a number of Ships and is an Entity which moves about a Sector
    Inherits Game_Library.Game_Objects.Entity
    Private updateRoutine As fleetUpdate = AddressOf AI_Update 'The Sub that updates the Ships actions
    Public allegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'The allegiance of the Fleet
    Private ships As List(Of Ship) 'A List of Ship objects contained within this Fleet
    Private _direction As Double 'A Double value indicating the direction the Fleet is facing
    Public Property direction As Double
        Set(ByVal value As Double)
            _direction = Normalise_Direction(value) 'Make sure the value is normalised
        End Set
        Get
            Return _direction
        End Get
    End Property
    Public turnSpeed As Double = QuarterCircle / 15 'A Double value representing how far the Fleet can turn per update
    Public speed As Game_Library.Game_Objects.StatDbl 'A StatDbl object indicating the speed of the Fleet
    Public acceleration As Double = 0.1 'A Double value representing how much the Fleet can change it's speed each update

    Public Sub New(ByVal nAllegiance As Star_Crew_Shared_Libraries.Shared_Values.Allegiances, ByRef nShips As List(Of Ship))
        allegiance = nAllegiance 'Set the new allegiance
        ships = nShips 'Set the List of Ships
        speed = New Game_Library.Game_Objects.StatDbl(0, 0, 10, True)
        X = Int(Rnd() * 101) - 50
        Y = Int(Rnd() * 101) - 50
    End Sub

    Public Overrides Sub Destroy()
        Dead = True
        allegiance = -1 'Clear the allegiance
        For Each i As Ship In ships 'Loop through all Ships
            i.Destroy() 'Destroy the Ship
        Next
    End Sub

    Public Sub Switch_Control(ByRef method As fleetUpdate) 'Changes the fleet's update method
        updateRoutine = If((method Is Nothing), (AddressOf AI_Update), (method)) 'Set the update method of the Fleet
    End Sub

    Public Delegate Sub fleetUpdate() 'The delegate object used to either update a Fleet with AI or player commands
    Private Sub AI_Update() 'Analysis and updates the Fleet's actions

    End Sub
    Public Overrides Sub Update() 'Updates the Fleet's direction
        updateRoutine.Invoke() 'Update the Fleet
        X += speed.Current * Math.Cos(direction)
        Y += speed.Current * Math.Sin(direction)
        Dim distance As Integer = Math.Sqrt((X ^ 2) + (Y ^ 2)) 'Get the distance of the Fleet from the center of the Sector
        If distance < 50 Then 'The Fleet is too close to the Sector's Planet
            If distance <> 0 Then 'There will not be a divid by 0 error
                Dim factor As Double = 60 / distance 'Get the ratio of 50 units to the Fleet's distance
                X *= factor
                Y *= factor
            Else
                X = 60 'Set the X coord to 60
            End If
            Dim dir As Double = Math.Atan2(Y, X) 'Get the direction of the Fleet from the center of the Sector
            Dim relDir As Double = Server.Normalise_Direction(dir - direction) 'Get the direction of the center of the Sector relative to the Fleet
            direction = dir + If((relDir > Math.PI), (QuarterCircle), (-QuarterCircle)) 'Set the Fleet's direction to be tangent to the Planet
        End If
    End Sub

End Class
