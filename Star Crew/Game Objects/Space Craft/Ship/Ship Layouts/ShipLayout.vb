Public MustInherit Class ShipLayout
    Public Enum Formats
        OmniMin
        Station = OmniMin
        Fleet
        OmniMax
        ShipsMin = OmniMax
        Screamer = OmniMax
        Thunder
        ShipsMax
    End Enum

    '-----Ship-----
    Public Format As Integer
    Public Hull As StatDbl
    Public Speed As StatDbl
    Public Acceleration As StatDbl
    '--------------

    '-----Helm-----
    Public TurnSpeed As StatDbl  'A StatDbl object that sets the max and current values for turn speed of the Ship
    '--------------

    '-----Batteries-----
    Public PrimaryMount As Double 'A Double value representing the primary Weapon's offset from forward facing
    Public SecondaryMount As Double 'A Double value representing the secondary Weapon's offset from forward facing
    Public Primary As Weapon 'A Weapon object that goes in the primary Weapon slot of the Ship
    Public Secondary As Weapon 'A Weapon object that goes in the secondary Weapon slot of the Ship
    '-------------------

    '-----Shielding-----
    Public ShieldingStats As ShieldSystem 'A ShieldingSystem object that contains the shields and shields modidiers
    '-------------------

    '-----Engineering-----
    Public EngineeringStats As EngineSystem 'An EngineSystem object that contains the engines and power core
    '---------------------

    Public Sub Initialise(ByRef nShip As Ship)
        nShip.Hull = Hull
        nShip.Speed = Speed
        nShip.Acceleration = Acceleration
        nShip.Helm.TurnSpeed = TurnSpeed
        nShip.Batteries.PrimaryMount = PrimaryMount
        nShip.Batteries.SecondaryMount = SecondaryMount
        nShip.Batteries.Primary = Primary
        Primary.Parent = nShip.Batteries
        nShip.Batteries.Secondary = Secondary
        Secondary.Parent = nShip.Batteries
        nShip.Shielding = New Shields(nShip, ShieldingStats)
        nShip.Engineering = New Engineering(nShip, EngineeringStats)
    End Sub
End Class
