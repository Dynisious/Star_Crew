<Serializable()>
Public Class ServerMessage 'Represents all the information necessary for a Client to render a full message
    Public TargetIndex As Integer = -1 'The index of the Players targeted Ship
    Public Speed As StatDbl 'The Speed of the craft
    Public Direction As Double = -1 'The direction the craft is traveling in
    Public ShipCount As Integer = -1 'The number of Ships in the Players Fleet
    Public CenterCraft As Ship  'The Players ship if in combat
    Public Primary As Weapon 'The Primary Weapon object
    Public PrimaryMount As Double = -1 'The Primary Weapons offset
    Public Secondary As Weapon 'The Secondary Weapon object
    Public SecondaryMount As Double = -1 'The Secondary Weapons offset
    Public Firing As Boolean = False 'A Boolean indecating if the Player is firing weapons
    Public Positions() As GraphicPosition 'An Array of GraphicPosition objects representing the Ships
    Public Warping As Galaxy.Warp = -1 'The 'warp' state of the Galaxy
    Public State As Galaxy.Scenario = -1 'The update state of the Galaxy

    Public Sub New(ByVal nTargetIndex As Integer, ByVal nSpeed As StatDbl, ByVal nDirection As Double,
                   ByVal nShipCount As Integer, ByVal nCenterCraft As Ship, ByVal nPositions() As GraphicPosition,
                   ByVal nWarping As Galaxy.Warp, ByVal nState As Galaxy.Scenario)
        TargetIndex = nTargetIndex
        Speed = nSpeed
        Direction = nDirection
        ShipCount = nShipCount
        CenterCraft = nCenterCraft
        If nCenterCraft IsNot Nothing Then
            Primary = nCenterCraft.Batteries.Primary
            PrimaryMount = nCenterCraft.Batteries.PrimaryMount
            Secondary = nCenterCraft.Batteries.Secondary
            SecondaryMount = nCenterCraft.Batteries.SecondaryMount
            Firing = nCenterCraft.Firing
        End If
        Positions = nPositions
        Warping = nWarping
        State = nState
    End Sub

End Class