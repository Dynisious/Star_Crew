Public Class Galaxy 'Object that encompances the game
    Public GalaxyTick As New Timers.Timer With {.Interval = 100, .Enabled = False}
    Public FocusedSector As Sector 'The Sector object that the Players' Fleet is currently inside
    Public Combat As CombatSpace 'A CombatSpace object that contains the 'Ship to Ship' fights
    Public Sectors(-1) As Sector 'An array of Sector objects inside the Galaxy
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
                For Each i As Sector In Sectors 'Loop through all Sectors
                    For Each e As Fleet In i.fleetList 'Loop through all the Fleets
                        e.Update() 'Update the Fleet
                    Next
                    For Each e As SpaceStation In i.spaceStations 'Loop through all the SpaceStations
                        e.Update() 'Update the SpaceStation
                    Next
                Next
            Case GalaxyStates.Ship_To_Ship
                Combat.Update_Combat() 'Update the combat scenario
            Case GalaxyStates.Shop_Interface

        End Select
    End Sub

End Class
