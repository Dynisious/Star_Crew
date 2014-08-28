Public Class SpaceStation
    Private _ParentSector As Sector 'The actual value of ParentSector
    Public ReadOnly Property ParentSector As Sector 'A reference to the Sector object that holds the SpaceStation
        Get
            Return _ParentSector
        End Get
    End Property
    Private _Location As System.Drawing.Point 'The actual value of Location
    Public ReadOnly Property Location As System.Drawing.Point 'A Point object representing the position of the SpaceStation in the Sector
        Get
            Return _Location
        End Get
    End Property
    Private _index As Integer 'The actual value of index
    Public ReadOnly Property index As Integer 'An Integer value representing the SpaceStation's index in the Sector's list of SpaceStations
        Get
            Return _index
        End Get
    End Property

    Public Sub New(ByRef nParentSector As Sector, ByVal nPosition As System.Drawing.Point, ByVal nIndex As Integer)
        _ParentSector = nParentSector
        _Location = nPosition
        _index = nIndex
    End Sub

    Public Sub Update() 'Set the Stations new position
        Dim distance As Integer = Math.Sqrt((Location.X ^ 2) + (Location.Y ^ 2)) 'The distance of the Fleet from the center of the Sector
        Dim direction As Double 'The direction of the Fleet from the center of the Sector
        If Location.X <> 0 Then 'The Fleet is not aligned with the Location.Y-axis
            direction = Math.Tanh(Location.Y / Location.X) 'Set the direction
            If Location.X < 0 Then 'The direction is reflected in the line Location.Y=Location.X
                direction = direction + Math.PI 'Reflect the direction
            End If
            direction = Server.Normalise_Direction(direction) 'Normalise the direction to be between 0 and 2Pi
        ElseIf Location.Y > 0 Then 'The Fleet is directly above the center of the Sector
            direction = (Math.PI / 2)
        Else 'The Fleet is directly bellow the center of the Sector
            direction = (3 * Math.PI / 2)
        End If
        direction = direction + Sector.Rotation 'Add on the radians traveled
        _Location.X = distance * Math.Cos(direction) 'Set the new Location.X coordinate
        _Location.Y = distance * Math.Cos(direction) 'Set the new Location.Y coordinate
    End Sub

End Class
