Public Class Galaxy 'Object that encompances the game
    Public WithEvents GalaxyTick As New Timers.Timer With {.Interval = 100, .Enabled = False} 'A Timer object that Updates the Galaxy
    Public FocusedSector As Sector 'A reference to the Sector object that the game is currently focused in
    Public ClientFleet As Fleet 'A reference to the Fleet object that the Player's control
    Public Combat As New CombatSpace 'A CombatSpace object that contains the 'Ship to Ship' fights
    Public Sectors(0) As Sector 'An array of Sector objects inside the Galaxy
    Public State As Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates = Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Sector_Transit 'The active State of the Galaxy

    Public Sub New()
        Sectors(0) = New Sector(0)
        FocusedSector = Sectors(0)
    End Sub

    Public NotInheritable Class ClientCommands 'A Class which stores the values of what the Clients want the Ship/Fleet to do
        '-----Helm-----
        Public ThrottleUp As Boolean = False 'A Boolean value indicating whether the Ship/Fleet should increase their throttle
        Public ThrottleDown As Boolean = False 'A Boolean value indicating whether the Ship/Fleet should decrease their throttle
        Public ShipRight As Boolean = False 'A Boolean value indicating whether the Ship should turn right
        Public ShipLeft As Boolean = False 'A Boolean value indicating whether the Ship should turn left
        '--------------
        '-----Battery-----
        Public WeaponRight As Boolean = False 'A Boolean value indicating whether the Selected Weapon should turn right
        Public WeaponLeft As Boolean = False 'A Boolean value indicating whether the Selected Weapon should turn left
        Public FireWeapon As Boolean = False 'A Boolean value indicating whether the Selected Weapon should attempt to fire
        Public NextWeapon As Boolean = False 'A Boolean value indicating whether the Battery should select the next Weapon
        Public PreviousWeapon As Boolean = False 'A Boolean value indicating whether the Battery should select the previous Weapon
        Public SelectedWeapon As Integer = 0 'An Integer value indicating which Weapon the Player is in control of
        '-----------------
        '-----Shields-----
        Public ShieldRight As Boolean = False 'A Boolean value indicating whether the Shield should turn Right
        Public ShieldLeft As Boolean = False 'A Boolean value indicating whether the Shield should turn Left
        '-----------------
        '-----Engines-----
        Public StationToRepair As Star_Crew_Shared_Libraries.Shared_Values.StationTypes 'A StationTypes value indicating which ShipStation is being repaired
        '-----------------
    End Class
    Public ClientInteractions As New ClientCommands
    Public Sub Client_Commands() 'Runs the commands from the Clients
        '-----Helm-----
        If ClientInteractions.ThrottleUp = True Then 'Increase the Client Ship's Throttle
            Combat.ClientShip.Engineering.Throttle.Current = Combat.ClientShip.Engineering.Throttle.Current + Combat.ClientShip.Engineering.Acceleration 'Increase the throttle
            If Combat.ClientShip.Engineering.Throttle.Current > Combat.ClientShip.Engineering.Throttle.Maximum Then 'Turn back the throttle to the max
                Combat.ClientShip.Engineering.Throttle.Current = Combat.ClientShip.Engineering.Throttle.Maximum 'Set the throttle
            End If
        End If
        If ClientInteractions.ThrottleDown = True Then 'Decrease the Client Ship's Throttle
            Combat.ClientShip.Engineering.Throttle.Current = Combat.ClientShip.Engineering.Throttle.Current - Combat.ClientShip.Engineering.Acceleration 'Decrease the throttle
            If Combat.ClientShip.Engineering.Throttle.Current < Combat.ClientShip.Engineering.Throttle.Minimum Then 'Turn up the throttle to the min
                Combat.ClientShip.Engineering.Throttle.Current = Combat.ClientShip.Engineering.Throttle.Minimum 'Set the throttle
            End If
        End If
        If ClientInteractions.ShipRight = True Then 'Turn the Ship to the right
            Combat.ClientShip.Direction = Combat.ClientShip.Direction + Combat.ClientShip.TurnSpeed 'Turn the Ship
        End If
        If ClientInteractions.ShipLeft = True Then 'Turn the Ship to the left
            Combat.ClientShip.Direction = Combat.ClientShip.Direction - Combat.ClientShip.TurnSpeed 'Turn the Ship
        End If
        '--------------
        '-----Battery-----
        If ClientInteractions.WeaponRight = True Then 'Turn the Selected Weapon to the right
            Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current = Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current + Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).MountedWeapon.TurningSpeed 'Turn the Weapon
            If Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current > Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Maximum Then 'Turn the Weapon back to the maximum
                Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current = Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Maximum 'Set the Weapons Direction
            End If
        End If
        If ClientInteractions.WeaponLeft = True Then 'Turn the Selected Weapon to the left
            Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current = Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current - Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).MountedWeapon.TurningSpeed 'Turn the Weapon
            If Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current > Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Minimum Then 'Turn the Weapon back to the minimum
                Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Current = Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).Sweep.Minimum 'Set the Weapons Direction
            End If
        End If
        If ClientInteractions.NextWeapon = True Then 'Select the next Weapon
            Do Until ClientInteractions.NextWeapon = False 'Loop
                ClientInteractions.SelectedWeapon = ClientInteractions.SelectedWeapon + 1 'Select the next Weapon
                If ClientInteractions.SelectedWeapon = Combat.ClientShip.Mounts.Length Then 'The Selected index is to large
                    ClientInteractions.SelectedWeapon = 0 'Go back to the start
                End If
                If Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).MountedWeapon IsNot Nothing Then 'There is a Weapon mounted here
                    ClientInteractions.NextWeapon = False 'Stop Looping
                End If
            Loop
        End If
        If ClientInteractions.PreviousWeapon = True Then 'Select the previous Weapon
            Do Until ClientInteractions.PreviousWeapon = False 'Loop
                ClientInteractions.SelectedWeapon = ClientInteractions.SelectedWeapon - 1 'Select the next Weapon
                If ClientInteractions.SelectedWeapon < 0 Then 'The Selected index is to small
                    ClientInteractions.SelectedWeapon = Combat.ClientShip.Mounts.Length - 1 'Go back to the end
                End If
                If Combat.ClientShip.Mounts(ClientInteractions.SelectedWeapon).MountedWeapon IsNot Nothing Then 'There is a Weapon mounted here
                    ClientInteractions.PreviousWeapon = False 'Stop Looping
                End If
            Loop
        End If
        '-----------------
        '-----Shields-----
        If ClientInteractions.ShieldRight = True Then 'Turn the Shield right
            Combat.ClientShip.Shielding.Direction = Server.Normalise_Direction(Combat.ClientShip.Shielding.Direction + Combat.ClientShip.Shielding.TurnSpeed) 'Set the direction of the Shield
        End If
        If ClientInteractions.ShieldLeft = True Then 'Turn the Shield left
            Combat.ClientShip.Shielding.Direction = Server.Normalise_Direction(Combat.ClientShip.Shielding.Direction - Combat.ClientShip.Shielding.TurnSpeed) 'Set the direction of the Shield
        End If
        '-----------------
    End Sub

    Public Sub Update() Handles GalaxyTick.Elapsed 'Updates the Galaxy
        Try
            Client_Commands() 'Update the Clients commands
            Dim messageBuff() As Byte 'The array of Bytes that represent the message
            Dim header() As Byte 'Get a 4 Byte array representing the Galaxy's current state
            Select Case State
                Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Sector_Transit
                    '-----Update-----
                    For Each i As Sector In Sectors 'Loop through all Sectors
                        i.Update() 'Update the Sector
                        For Each e As Fleet In i.FleetList 'Loop through all the Fleets
                            e.Update() 'Update the Fleet
                        Next
                        For Each e As SpaceStation In i.SpaceStations 'Loop through all the SpaceStations
                            e.Update() 'Update the SpaceStation
                        Next
                    Next
                    '----------------
                    '-----Generate Message-----
                    Dim objectCount As Integer = (FocusedSector.FleetList.Count + FocusedSector.SpaceStations.Length) 'The number of objects in the Sector
                    Dim types(objectCount) As Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects 'Create an array of Networking_Messages.SectorView.SectorObjects values
                    Dim positions(objectCount) As System.Drawing.Point 'Create an array of point objects
                    Dim allegiances(objectCount) As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'Create an array of Allegiances values
                    Dim currentIndex As Integer = 1 'The current index inside the arrays that is being applied
                    types(0) = FocusedSector.MyPlanet.PlanetType 'Set the base value of Types to be the Planet
                    positions(0) = New System.Drawing.Point(0, 0) 'Set the base value of positions to be (0,0)
                    allegiances(0) = Star_Crew_Shared_Libraries.Shared_Values.Allegiances.nill 'Set the base value of allegiances to be nill
                    For Each i As SpaceStation In FocusedSector.SpaceStations 'Loop through all SpaceStation
                        types(currentIndex) = Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects.SpaceStation 'Set this object to a SpaceStation
                        positions(currentIndex) = i.Location 'Set this objects position
                        allegiances(currentIndex) = FocusedSector.myAllegiance 'Set this objects allegiance to the Sectors
                        currentIndex = currentIndex + 1 'Set the index to the next index
                    Next
                    For Each i As Fleet In FocusedSector.FleetList 'Loop through all the Fleets
                        types(currentIndex) = Star_Crew_Shared_Libraries.Networking_Messages.SectorView.SectorObjects.Fleet 'Set this object to a Fleet
                        positions(currentIndex) = New System.Drawing.Point(i.X, i.Y) 'Set this objects position
                        allegiances(currentIndex) = i.myAllegiance 'Set this objects allefiance
                        currentIndex = currentIndex + 1 'Set the index to the next index
                    Next
                    '--------------------------
                    '-----Send Messages to Clients-----
                    messageBuff = Game_Library.Serialisation.ToBytes(New Star_Crew_Shared_Libraries.Networking_Messages.SectorView(types, positions, allegiances)) 'Generate the message to send to the Clients
                    header = BitConverter.GetBytes(Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Sector_Transit) 'Get a 4 Byte array representing the Sector_Transit state
                    If Server.UseNetwork.WaitOne(100) = True Then 'The Galaxy can use the network
                        Server.Comms.Send_All(header, messageBuff) 'Send the message to all the Clients
                        Server.UseNetwork.ReleaseMutex() 'Release the mutex once networking is done
                    End If
                    '----------------------------------
                Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Ship_To_Ship
                    Combat.Update_Combat() 'Update the combat scenario
                    '-----Generate Message-----
                    Dim types(Combat.Combatants.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.ShipTypes 'Create an array of Ship.ShipTypes values
                    Dim positions(Combat.Combatants.Count - 1) As System.Drawing.Point 'Create an array of point objects
                    Dim allegiances(Combat.Combatants.Count - 1) As Star_Crew_Shared_Libraries.Shared_Values.Allegiances 'Create an array of Allegiances values
                    Dim directions(Combat.Combatants.Count - 1) As Double 'Create an array of Double values
                    For i As Integer = 0 To Combat.Combatants.Count - 1 'Loop through all Ships
                        types(i) = Combat.Combatants(i).Type 'Set this object to a it's type
                        positions(i) = New System.Drawing.Point((Combat.Combatants(i).X - Combat.ClientShip.X),
                                                                (Combat.Combatants(i).Y - Combat.ClientShip.Y)) 'Set this objects position relative to the ClientShip
                        directions(i) = Combat.Combatants(i).Direction 'Set this objects direction
                        allegiances(i) = Combat.Combatants(i).ParentFleet.myAllegiance 'Set this objects allegiance to it's Allegiance
                    Next
                    '--------------------------
                    '-----Send Message to Clients-----
                    messageBuff = Game_Library.Serialisation.ToBytes(New Star_Crew_Shared_Libraries.Networking_Messages.ShipView(types, positions, directions, allegiances)) 'Generate the message to send to the Clients
                    header = BitConverter.GetBytes(Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Ship_To_Ship) 'Get a 4 Byte array representing the Ship_To_Ship state
                    If Server.UseNetwork.WaitOne(100) = True Then 'The Galaxy can use the network
                        Server.Comms.Send_All(header, messageBuff) 'Send the message to all the Clients
                        Server.UseNetwork.ReleaseMutex() 'Release the mutex once networking is done
                    End If
                    '-----Send Message to Clients-----
                Case Star_Crew_Shared_Libraries.Shared_Values.GalaxyStates.Shop_Interface

            End Select
        Catch ex As Exception 'Catch any unhandled exception
            Console.WriteLine(Environment.NewLine + "ERROR : There was an unhandled exception while updating the Game World." +
                              Environment.NewLine + ex.ToString()) 'Write to the Console
            GalaxyTick.Stop() 'Stop the Galaxy updating
        End Try
    End Sub

End Class
