Public Class Planet
    Private _ParentSector As Sector 'The actual value of ParentSector
    Public ReadOnly Property ParentSector As Sector 'A reference to the Sector object the Planet is inside
        Get
            Return _ParentSector
        End Get
    End Property
    Private _PlanetType As Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects 'The actual value of PlanetType
    Public ReadOnly Property PlanetType As Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects 'A SectorObjects value indicating which Planet this one is
        Get
            Return _PlanetType
        End Get
    End Property
    Private _Name As String = "ERROR" 'A String value representing the name of the Planet
    Public ReadOnly Property Name As String 'A String value representing the name of the Planet
        Get
            Return _Name
        End Get
    End Property
    Public Stores As New List(Of Item) 'A list of Item objects that the Planet has to trade with
    Private ReadOnly Names As New Dictionary(Of Integer, String) From {
        {0, "Corporis"}
    }

    Public Sub New(ByRef nParentSector As Sector, ByVal creationIndex As Integer) 'Create a new Planet
        _ParentSector = nParentSector 'Set the parent Sector
        _Name = Names(creationIndex) 'Set the name of the Planet
        _PlanetType = Int(Rnd() * Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects.PlanetMax) 'Set the type of Planet this is
    End Sub

End Class
