<Serializable()>
Public Class ServerMessage
    Public TargetIndex As Integer = -1
    Public Speed As StatDbl
    Public Direction As Double
    Public CenterShip As FriendlyShip
    Public Primary As Weapon
    Public Secondary As Weapon
    Public Positions() As GraphicPosition
    Public Warping As Galaxy.Warp = -1
    Public State As Galaxy.Scenario = -1
    Public Shared ReadOnly MessageSize As Integer = 512

    Public Sub New(ByVal nTargetIndex As Integer, ByVal nCenterShip As FriendlyShip,
                   ByVal nPositions() As GraphicPosition, ByVal nWarping As Galaxy.Warp,
                   ByVal nState As Galaxy.Scenario, ByVal nSpeed As StatDbl, ByVal nDirection As Double)
        TargetIndex = nTargetIndex
        CenterShip = nCenterShip
        Speed = nSpeed
        Direction = nDirection
        If nCenterShip IsNot Nothing Then
            Primary = nCenterShip.Batteries.Primary
            Secondary = nCenterShip.Batteries.Secondary
        End If
        Positions = nPositions
        Warping = nWarping
        State = nState
    End Sub

End Class