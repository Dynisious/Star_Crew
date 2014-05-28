Public MustInherit Class ShipLayout
    Public Enum Formats
        SectorMin 'The minimum value of SpaceCraft that are not Ships
        Station = SectorMin 'A Space Station that heals Fleets and acts as a spawn point for Fleets
        Fleet 'A Fleet that represents a group of Ship's moving together through a Sector
        SectorMax 'The maximum value of SpaceCraft that are not Ships
        ShipsMin = SectorMax 'The minimum value of SpaceCraft that are Ships
        Screamer = SectorMax 'A small light fighter
        Thunder 'A heavier more powerful fighter
        ShipsMax 'The maximum value of SpaceCraft that are Ships
    End Enum

    '-----Ship-----
    Public Format As Integer 'An Enumerator specifying the Format of the Ship
    Public Hull As StatDbl 'A StatDbl object representing the current and maximum values of the Ship's hull
    Public Speed As StatDbl 'A StatDbl object representing the current and maximum values of the Ship's speed
    Public Acceleration As StatDbl 'A StatDbl object representing the current and maximum values of the Ship's acceleration
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
