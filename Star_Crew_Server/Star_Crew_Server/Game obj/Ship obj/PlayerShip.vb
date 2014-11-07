Public Class PlayerShip
    Inherits Ship
    Private Client As ServerClient 'A reference to the ServerClient object in control of the Ship

    Public Sub New(ByRef nClient As ServerClient)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Ship, True, New Game_Library.Game_Objects.StatDbl(0, 100, 100, True),
                   New Game_Library.Game_Objects.StatDbl(0, 0, 15, True), 0.5, (Math.PI / 15))
        Client = nClient
        _Primary = New Rattler(Me)
    End Sub

    Public Overrides Sub Destroy()
        MyBase.Destroy()
        Client = Nothing
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        If Client IsNot Nothing Then 'Change the Ship
            If Client.throttleUp Then Throttle.Current += Acceleration
            If Client.throttleDown Then Throttle.Current -= Acceleration
            If Client.turnRight Then Direction += TurnSpeed
            If Client.turnLeft Then Direction -= TurnSpeed
            If Client.fireWeapons Then Primary.Fire()
        End If
    End Sub

End Class
