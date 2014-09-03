Public Class Screamer
    Inherits Ship

    Public Sub New(ByRef nParent As Fleet, Optional ByRef nBridge As Helm = Nothing, Optional ByRef nBatteries As Battery = Nothing,
                   Optional ByRef nShielding As Shields = Nothing, Optional ByRef nEngineering As Engines = Nothing,
                   Optional ByRef weapon0 As Weapon = Nothing)
        MyBase.New(nParent, 30, New Game_Library.StatDbl(0, 125, 125), (Math.PI / 15), 1, Star_Crew_Shared_Libraries.Shared_Values.ShipTypes.Screamer)
        Mounts(0) = New WeaponMount(New Game_Library.StatDbl(-(Math.PI / 4), 0, (Math.PI / 4)), 0) 'Initialise the Ship's WeaponMount
        If nBridge Is Nothing Then nBridge = New Helm(Me, New Game_Library.StatInt(0, 50, 50), 8) 'Make sure nBridge is not nothing
        Bridge = nBridge 'Set the new Bridge
        If nBatteries Is Nothing Then nBatteries = New Battery(Me, New Game_Library.StatInt(0, 50, 50), 8) 'Make sure nBatteries is not nothing
        Batteries = nBatteries 'Set the new Batteries
        If nShielding Is Nothing Then nShielding = New Bunter(Me) 'Make sure nShielding is not nothing
        Shielding = nShielding 'Set the new Shielding
        If nEngineering Is Nothing Then nEngineering = New Knife_Edge(Me) 'Make sure nEngineering is not nothing
        Engineering = nEngineering 'Set the new Engineering
        If weapon0 Is Nothing Then weapon0 = New Rattler(Me, 0) 'Make sure weapon0 is not nothing
        Mounts(0).MountedWeapon = weapon0 'Set the new Weapon on mount 0
    End Sub

End Class
