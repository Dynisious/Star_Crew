<Serializable()>
Public Class ServerMessage 'Represents all the information necessary for a Client to render a full message
    Public TargetIndex As Integer = -1 'The index of the Players targeted Ship
    Public Speed As StatDbl 'The Speed of the craft
    Public Direction As Double = -1 'The direction the craft is traveling in
    Public ShipCount As Integer = -1 'The number of Ships in the Players Fleet
    '-----Batteries-----
    Public Primary As Weapon 'The Primary Weapon object
    Public PrimaryMount As Double = -1 'The Primary Weapons offset
    Public Secondary As Weapon 'The Secondary Weapon object
    Public SecondaryMount As Double = -1 'The Secondary Weapons offset
    Public Firing As Boolean = False 'A Boolean indecating if the Player is firing weapons
    '-------------------
    '-----Shielding-----
    Public ForeShield As StatDbl 'A StatDbl Object representing the Forward shield of the Player's Ship
    Public StarboardShield As StatDbl 'A StatDbl Object representing the Right shield of the Player's Ship
    Public AftShield As StatDbl 'A StatDbl Object representing the Rear shield of the Player's Ship
    Public PortShield As StatDbl 'A StatDbl Object representing the Left shield of the Player's Ship
    Public LastHit As Shields.Sides 'An Enumerator specifying which side of the Player's Ship was last shot
    '-------------------
    '-----Engineering----
    Public PowerCore As StatDbl 'A StatDbl Object representing the integrety of the Player's Ship's Power Core
    Public Engines As StatDbl 'A StatDbl Object representing the integrety of the Player's Ship's Engines
    Public Heat As Double 'A Double value representing the temperature of the Player's Ship's Power Core
    Public Rate As Double 'A Double value representing the rate of change of the temperature in the Player's Ship's Power Core
    '--------------------
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
        If nCenterCraft IsNot Nothing Then
            '-----Batteries-----
            Primary = nCenterCraft.Batteries.Primary
            PrimaryMount = nCenterCraft.Batteries.PrimaryMount
            Secondary = nCenterCraft.Batteries.Secondary
            SecondaryMount = nCenterCraft.Batteries.SecondaryMount
            Firing = nCenterCraft.Firing
            '-------------------
            '----Shielding-----
            ForeShield = nCenterCraft.Shielding.SubSystem.Defences(Shields.Sides.FrontShield)
            StarboardShield = nCenterCraft.Shielding.SubSystem.Defences(Shields.Sides.RightShield)
            AftShield = nCenterCraft.Shielding.SubSystem.Defences(Shields.Sides.BackShield)
            PortShield = nCenterCraft.Shielding.SubSystem.Defences(Shields.Sides.LeftShield)
            LastHit = nCenterCraft.Shielding.LastHit
            '------------------
            '-----Engineering----
            PowerCore = nCenterCraft.Engineering.SubSystem.PowerCore
            Engines = nCenterCraft.Engineering.SubSystem.Engines
            Heat = nCenterCraft.Engineering.Heat
            Rate = nCenterCraft.Engineering.Rate
            '--------------------
        End If
        Positions = nPositions
        Warping = nWarping
        State = nState
    End Sub

End Class