Public Class Galaxy 'Object that encompances the game
    Public GalaxyTick As New Timers.Timer With {.Interval = 100, .Enabled = False}
    Public FocusedSector As Sector 'The Sector object that the Players' Fleet is currently inside
    Public Combat As CombatSpace 'A CombatSpace object that contains the 'Ship to Ship' fights
    Public Planets(-1) As Planet 'An array of references to Planet objects inside the Galaxy's Sectors
    Public Enum Allegiances 'The different allegiances in the Galaxy
        Emperial_Forces
        Pirate_Alliance
        Trade_Federation
        nill
        max
    End Enum
    Public Enum GalaxyStates 'The different paths the Galaxy needs to do when it calls Update()
        Sector_Transit
        Ship_To_Ship
        Shop_Interface
    End Enum
    Public State As GalaxyStates = GalaxyStates.Sector_Transit 'The active State of the Galaxy

    Public Sub Update() 'Updates the Galaxy
        Select Case State
            Case GalaxyStates.Sector_Transit

            Case GalaxyStates.Ship_To_Ship

            Case GalaxyStates.Shop_Interface

        End Select
    End Sub

    Public Function Normalise_Radian(ByVal radian As Double) As Double

    End Function

End Class
