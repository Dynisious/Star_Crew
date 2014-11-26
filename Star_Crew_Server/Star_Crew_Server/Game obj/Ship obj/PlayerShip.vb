Public Class PlayerShip
    Inherits Ship
    Private Client As ServerClient 'A reference to the ServerClient object in control of the Ship
    Private _Primary As Weapon 'The actual value of Primary
    Public ReadOnly Property Primary As Weapon 'A Weapon object for the Ship
        Get
            Return _Primary
        End Get
    End Property
    Private _Secondary As Weapon 'The actual value of Secondary
    Public ReadOnly Property Secondary As Weapon 'A Secondary Weapon object for the Ship
        Get
            Return _Secondary
        End Get
    End Property
    Public targetDistance As Integer = -1 'An Integer indicating the distance to the target

    Public Sub New(ByRef nClient As ServerClient)
        MyBase.New(Star_Crew_Shared_Libraries.Shared_Values.ObjectTypes.Screamer, True, True,
                   New Game_Library.Game_Objects.StatDbl(0, 100, 100, True), New Game_Library.Game_Objects.StatDbl(1, 1, 20, True),
                   0.5, (Math.PI / 30), 20, 20, Star_Crew_Shared_Libraries.Shared_Values.Allegiances.Emperial_Forces,
                  New Game_Library.Game_Objects.StatInt(0, 40, 40, True), New Game_Library.Game_Objects.StatDbl(0, 30, 30, True))
        Client = nClient
        _Primary = New Rattler(Me)
        _Secondary = New Lance(Me)
    End Sub

    Public Overrides Sub Destroy()
        MyBase.Destroy()
        If Primary IsNot Nothing Then
            Primary.Destroy()
            _Primary = Nothing
        End If
        If Secondary IsNot Nothing Then
            Secondary.Destroy()
            _Secondary = Nothing
        End If
        Client.Send_Message({BitConverter.GetBytes(Star_Crew_Shared_Libraries.Networking_Messages.Server_Message_Header.Client_Lost)},
                            {"ERROR : There was an error while sending the Client_Lost message to the " + Name + ". They will now be disconnected."})
        Client.sendingAlive = False
        Client.receivingAlive = False
        Client = Nothing
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Primary.Update() 'Update Primary
        Secondary.Update() 'Update Secondary

        '-----Targeting-----
        target = Nothing 'Clear target
        Dim obj As Object() = Analyse_Situation() 'Get the arrays
        For i As Integer = 0 To obj(0).Length - 1 'Loop through all the indexes
            Dim tar As Ship = obj(0)(i) 'Get the current Ship object
            If tar.Allegiance <> Allegiance Then 'This is the target to select
                target = tar 'Get the target object for the Ship
                targetDistance = obj(1)(i) 'Get the target distance for the Ship
                Exit For
            End If
        Next
        '-------------------

        If Client IsNot Nothing Then 'Change the Ship
            If Client.throttleUp Then Throttle.Current += Acceleration
            If Client.throttleDown Then Throttle.Current -= Acceleration
            If Client.turnRight Then Direction += TurnSpeed
            If Client.turnLeft Then Direction -= TurnSpeed
            If Client.firePrimary Then Primary.Fire()
            If Client.fireSecondary Then Secondary.Fire()
        End If
    End Sub

End Class
