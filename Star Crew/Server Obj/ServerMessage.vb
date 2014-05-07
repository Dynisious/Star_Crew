<Serializable()>
Public Class ServerMessage 'Represents all the information necessary for a Client to render a full message
    Public TargetIndex As Integer = -1 'The index of the Players targeted Ship
    Public Speed As StatDbl 'The Speed of the craft
    Public Direction As Double 'The direction the craft is traveling in
    Public CenterShip As Ship 'The Players ship if in combat
    Public Primary As Weapon 'The Primary Weapon object
    Public PrimaryMount As Double 'The Primary Weapons offset
    Public Secondary As Weapon 'The Secondary Weapon object
    Public SecondaryMount As Double 'The Secondary Weapons offset
    Public Positions() As GraphicPosition 'An Array of GraphicPosition objects representing the Ships
    Public Warping As Galaxy.Warp = -1 'The 'warp' state of the Galaxy
    Public State As Galaxy.Scenario = -1 'The update state of the Galaxy

    Public Sub New(ByVal nTargetIndex As Integer, ByVal nCenterShip As Ship,
                   ByVal nPositions() As GraphicPosition, ByVal nWarping As Galaxy.Warp,
                   ByVal nState As Galaxy.Scenario, ByVal nSpeed As StatDbl, ByVal nDirection As Double)
        TargetIndex = nTargetIndex
        CenterShip = nCenterShip
        Speed = nSpeed
        Direction = nDirection
        If nCenterShip IsNot Nothing Then
            Primary = nCenterShip.Batteries.Primary
            PrimaryMount = nCenterShip.Batteries.PrimaryMount
            Secondary = nCenterShip.Batteries.Secondary
            SecondaryMount = nCenterShip.Batteries.SecondaryMount
        End If
        Positions = nPositions
        Warping = nWarping
        State = nState
    End Sub

End Class