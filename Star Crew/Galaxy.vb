Public Class Galaxy
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Public Shared centerShip As Ship
    Private primaryDirection As Double
    Private primaryRadius As Integer
    Private secondaryDirection As Double
    Private secondaryRadius As Integer
    Public ReadOnly ShipCount As Integer = 50
    Public Shared xList(-1) As Ship
    Private Shared Bmp As New Bitmap(600, 600)
    Private shipPositions(-1) As Point
    Private directionPositions(-1) As Point
    Private weaponDirections(1) As Double
    Private stars(200) As Star
    Public Enum Warp
        None
        Entering
        Exiting
        Warping
    End Enum
    Public Shared Warping As Warp = Warp.None
    Private Event NewClientMessage(ByVal nMessage As Station, ByVal nStation As Station.StationTypes)
    Private Event StartGame()

    Public Class Star
        Public Position As Point
        Public Flash As Boolean = False
        Private count As Integer
        Public Shared Speed As Double = 1
        Public Shared ReadOnly WarpSpeed As Integer = 20

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Public Sub Update()
            If Flash = False Then
                If Int(80 * Rnd()) = 0 Then
                    Flash = True
                End If
                count = 8
            ElseIf count = 1 Then
                count = 0
                Flash = False
            Else
                count = count - 1
            End If


            Dim direction As Double = centerShip.Helm.Direction + Math.PI
            Position = New Point(Position.X + (centerShip.Helm.Throttle.current * Math.Cos(direction) * Speed), Position.Y + (centerShip.Helm.Throttle.current * Math.Sin(direction) * Speed))
            If Position.X <= 2 Then
                Position = New Point(Position.X + Bmp.Width - 5, Position.Y)
            ElseIf Position.X >= Bmp.Width - 3 Then
                Position = New Point(Position.X - Bmp.Width + 5, Position.Y)
            End If
            If Position.Y <= 2 Then
                Position = New Point(Position.X, Position.Y + Bmp.Height - 5)
            ElseIf Position.Y >= Bmp.Height - 3 Then
                Position = New Point(Position.X, Position.Y - Bmp.Height + 5)
            End If
        End Sub
    End Class

    Public Sub StartGame_Call()
        RaiseEvent StartGame()
    End Sub
    Public Sub StartGame_Handle() Handles Me.StartGame
        Warping = Warp.None
        Star.Speed = 1
        ReDim xList(ShipCount - 1)
        Randomize()
        For i As Integer = 0 To UBound(stars)
            stars(i) = New Star(New Point(Int((594 * Rnd()) + 3), Int((594 * Rnd())) + 3))
        Next

        centerShip = New FriendlyShip(Me, New Clunker)
        centerShip.Parent = Me
        xList(0) = centerShip
        xList(0).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        For i As Integer = 1 To UBound(xList)
            If Int(2 * Rnd()) = 0 Then
                xList(i) = New FriendlyShip(Me, New Clunker)
            Else
                xList(i) = New PirateShip(Me, New Clunker)
            End If
            xList(i).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        Next
        primaryDirection = centerShip.Helm.Direction + centerShip.Batteries.Primary.TurnDistance.current
        primaryRadius = centerShip.Batteries.Primary.WeaponStats(Weapon.Stats.Range).current
        secondaryDirection = centerShip.Helm.Direction + centerShip.Batteries.Secondary.TurnDistance.current
        secondaryRadius = centerShip.Batteries.Secondary.WeaponStats(Weapon.Stats.Range).current
        GalaxyTimer.Enabled = True
    End Sub

    Private Event RunCommand(ByVal clientCommand As ClientMessage)
    Public Sub RunCommand_Call(ByVal clientCommand As ClientMessage)
        RaiseEvent RunCommand(clientCommand)
    End Sub
    Private Sub RunCommand_Handle(ByVal clientCommand As ClientMessage) Handles Me.RunCommand
        Select Case clientCommand.Station
            Case Star_Crew.Station.StationTypes.Helm
                Select Case clientCommand.Command
                    Case Helm.Commands.ThrottleUp
                        If clientCommand.Value = 1 Then
                            PlayerControl.ThrottleUpCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ThrottleUpCheck = False
                        End If
                    Case Helm.Commands.ThrottleDown
                        If clientCommand.Value = 1 Then
                            PlayerControl.ThrottleDownCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ThrottleDownCheck = False
                        End If
                    Case Helm.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.TurnRightCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.TurnRightCheck = False
                        End If
                    Case Helm.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.TurnLeftCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.TurnLeftCheck = False
                        End If
                    Case Helm.Commands.WarpDrive
                        If clientCommand.Value = 1 Then
                            PlayerControl.WarpDriveCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.WarpDriveCheck = False
                        End If
                    Case Helm.Commands.MatchSpeed
                        If clientCommand.Value = 1 Then
                            PlayerControl.MatchSpeedCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.MatchSpeedCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Batteries
                Select Case clientCommand.Command
                    Case Battery.Commands.TurnRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.RotateRightCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RotateRightCheck = False
                        End If
                    Case Battery.Commands.TurnLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.RotateLeftCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RotateLeftCheck = False
                        End If
                    Case Battery.Commands.FirePrimary
                        If clientCommand.Value = 1 Then
                            PlayerControl.FirePrimaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.FirePrimaryCheck = False
                        End If
                    Case Battery.Commands.FireSecondary
                        If clientCommand.Value = 1 Then
                            PlayerControl.FireSecondaryCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.FireSecondaryCheck = False
                        End If
                    Case Battery.Commands.SetTarget
                        If clientCommand.Value = 1 Then
                            PlayerControl.SelectTargetCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.SelectTargetCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Shielding
                Select Case clientCommand.Command
                    Case Shields.Commands.BoostForward
                        If clientCommand.Value = 1 Then
                            PlayerControl.ForwardBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.ForwardBoostCheck = False
                        End If
                    Case Shields.Commands.BoostRight
                        If clientCommand.Value = 1 Then
                            PlayerControl.RightBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RightBoostCheck = False
                        End If
                    Case Shields.Commands.BoostBack
                        If clientCommand.Value = 1 Then
                            PlayerControl.RearBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.RearBoostCheck = False
                        End If
                    Case Shields.Commands.BoostLeft
                        If clientCommand.Value = 1 Then
                            PlayerControl.LeftBoostCheck = True
                        ElseIf clientCommand.Value = 0 Then
                            PlayerControl.LeftBoostCheck = False
                        End If
                End Select
            Case Star_Crew.Station.StationTypes.Engineering
                Select Case clientCommand.Command

                End Select
        End Select
    End Sub
    Private Class PlayerControl
        '-----Helm-----
        Public Shared ThrottleUpCheck As Boolean = False
        Public Shared ThrottleDownCheck As Boolean = False
        Public Shared TurnRightCheck As Boolean = False
        Public Shared TurnLeftCheck As Boolean = False
        Public Shared WarpDriveCheck As Boolean = False
        Public Shared MatchSpeedCheck As Boolean = False
        Private Shared Sub ThrottleUp()
            If ThrottleUpCheck = True Then
                centerShip.Helm.MatchSpeed = False
                centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current + centerShip.Helm.Acceleration.current
                If centerShip.Helm.Throttle.current > centerShip.Helm.Throttle.max Then
                    centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.max
                End If
            End If
        End Sub
        Private Shared Sub ThrottleDown()
            If ThrottleDownCheck = True Then
                centerShip.Helm.MatchSpeed = False
                centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current - centerShip.Helm.Acceleration.current
                If centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                    centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                End If
            End If
        End Sub
        Private Shared Sub TurnRight()
            If TurnRightCheck = True Then
                centerShip.Helm.Direction = Helm.NormalizeDirection(centerShip.Helm.Direction + centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub TurnLeft()
            If TurnLeftCheck = True Then
                centerShip.Helm.Direction = Helm.NormalizeDirection(centerShip.Helm.Direction - centerShip.Helm.TurnSpeed.current)
            End If
        End Sub
        Private Shared Sub WarpDrive()
            If WarpDriveCheck = True Then
                If centerShip.Helm.Throttle.current > Helm.MinimumSpeed Then
                    centerShip.Helm.Throttle.current = centerShip.Helm.Throttle.current - centerShip.Helm.Acceleration.current
                    If centerShip.Helm.Throttle.current < Helm.MinimumSpeed Then
                        centerShip.Helm.Throttle.current = Helm.MinimumSpeed
                    End If
                End If
                If centerShip.Helm.Direction < Math.PI Then
                    centerShip.Helm.Direction = centerShip.Helm.Direction - centerShip.Helm.TurnSpeed.current
                    If centerShip.Helm.Direction < 0 Then
                        centerShip.Helm.Direction = 0
                    End If
                ElseIf centerShip.Helm.Direction > Math.PI Then
                    centerShip.Helm.Direction = centerShip.Helm.Direction + centerShip.Helm.TurnSpeed.current
                    If centerShip.Helm.Direction > 2 * Math.PI Then
                        centerShip.Helm.Direction = 0
                    End If
                End If
                If Galaxy.centerShip.Helm.Throttle.current = Helm.MinimumSpeed And
                    Galaxy.Warping <> Galaxy.Warp.Warping And
                    Galaxy.centerShip.Helm.Direction = 0 Then
                    Galaxy.Warping = Galaxy.Warp.Entering
                End If
            End If
            If Galaxy.Warping = Warp.Warping Then
                If Galaxy.Star.Speed <> Galaxy.Star.WarpSpeed And
                    Galaxy.Warping = Galaxy.Warp.Warping Then
                    Galaxy.Star.Speed = Galaxy.Star.Speed + (Galaxy.Star.WarpSpeed / 50)
                    If Galaxy.Star.Speed > Galaxy.Star.WarpSpeed Then
                        Galaxy.Star.Speed = Galaxy.Star.WarpSpeed
                    End If
                End If
            End If
        End Sub
        Private Shared Sub MatchSpeed()
            If MatchSpeedCheck = True Then
                centerShip.Helm.MatchSpeed = True
            End If
        End Sub
        '--------------

        '-----Batteries-----
        Public Shared RotateRightCheck As Boolean = False
        Public Shared RotateLeftCheck As Boolean = False
        Public Shared FirePrimaryCheck As Boolean = False
        Public Shared FireSecondaryCheck As Boolean = False
        Public Shared SelectTargetCheck As Boolean = False
        Private Shared Sub RotateRight()
            If RotateRightCheck = True Then
                '-----Primary-----
                centerShip.Batteries.Primary.TurnDistance.current =
                    centerShip.Batteries.Primary.TurnDistance.current +
                    centerShip.Batteries.Primary.TurnSpeed.current
                If centerShip.Batteries.Primary.TurnDistance.current > centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                    centerShip.Batteries.Primary.TurnDistance.current = centerShip.Batteries.Primary.TurnDistance.max / 2
                End If
                '-----------------
                If centerShip.Batteries.Primary.TurnDistance.current > centerShip.Batteries.Secondary.TurnDistance.current Then
                    '-----Secondary-----
                    centerShip.Batteries.Secondary.TurnDistance.current =
                        centerShip.Batteries.Secondary.TurnDistance.current +
                        centerShip.Batteries.Secondary.TurnSpeed.current
                    If centerShip.Batteries.Secondary.TurnDistance.current > centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                        centerShip.Batteries.Secondary.TurnDistance.current = centerShip.Batteries.Secondary.TurnDistance.max / 2
                    End If
                    '-------------------
                End If
            End If
        End Sub
        Private Shared Sub RotateLeft()
            If RotateLeftCheck = True Then
                '-----Primary-----
                centerShip.Batteries.Primary.TurnDistance.current =
                    centerShip.Batteries.Primary.TurnDistance.current -
                    centerShip.Batteries.Primary.TurnSpeed.current
                If centerShip.Batteries.Primary.TurnDistance.current < -centerShip.Batteries.Primary.TurnDistance.max / 2 Then
                    centerShip.Batteries.Primary.TurnDistance.current = -centerShip.Batteries.Primary.TurnDistance.max / 2
                End If
                '-----------------
                If centerShip.Batteries.Primary.TurnDistance.current < centerShip.Batteries.Secondary.TurnDistance.current Then
                    '-----Secondary-----
                    centerShip.Batteries.Secondary.TurnDistance.current =
                        centerShip.Batteries.Secondary.TurnDistance.current -
                        centerShip.Batteries.Secondary.TurnSpeed.current
                    If centerShip.Batteries.Secondary.TurnDistance.current < -centerShip.Batteries.Secondary.TurnDistance.max / 2 Then
                        centerShip.Batteries.Secondary.TurnDistance.current = -centerShip.Batteries.Secondary.TurnDistance.max / 2
                    End If
                    '-------------------
                End If
            End If
        End Sub
        Private Shared Sub FirePrimary()
            If FirePrimaryCheck = True Then
                For i As Integer = 0 To xList.Length - 1
                    Dim adjacent As Integer = xList(i).Position.X - centerShip.Position.X
                    Dim opposite As Integer = xList(i).Position.Y - centerShip.Position.Y
                    Dim distance = Math.Sqrt((opposite ^ 2) + (adjacent ^ 2))
                    Dim direction As Double
                    If adjacent <> 0 Then
                        direction = Math.Tanh(opposite / adjacent)
                        If adjacent < 0 Then
                            direction = direction + Math.PI
                        End If
                        direction = Helm.NormalizeDirection(direction)
                    ElseIf opposite > 0 Then
                        direction = Math.PI / 2
                    Else
                        direction = (3 * Math.PI) / 2
                    End If

                    If distance < centerShip.Batteries.Primary.WeaponStats(Weapon.Stats.Range).current And distance <> 0 Then
                        If direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current < Battery.PlayerArc / 2 And
                            direction - centerShip.Helm.Direction - centerShip.Batteries.Primary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                            centerShip.Batteries.Primary.FireWeapon(distance, xList(i))
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub FireSecondary()
            If FireSecondaryCheck = True Then
                For i As Integer = 0 To xList.Length - 1
                    If xList(i).MyAllegence <> Ship.Allegence.Player Then
                        Dim adjacent As Integer = xList(i).Position.X - centerShip.Position.X
                        Dim opposite As Integer = xList(i).Position.Y - centerShip.Position.Y
                        Dim distance = Math.Sqrt((opposite ^ 2) + (adjacent ^ 2))
                        Dim direction As Double
                        If adjacent <> 0 Then
                            direction = Math.Tanh(opposite / adjacent)
                            If adjacent < 0 Then
                                direction = direction + Math.PI
                            End If
                            direction = Helm.NormalizeDirection(direction)
                        ElseIf opposite > 0 Then
                            direction = Math.PI / 2
                        Else
                            direction = (3 * Math.PI) / 2
                        End If

                        If distance < centerShip.Batteries.Secondary.WeaponStats(Weapon.Stats.Range).current And distance <> 0 Then
                            If direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current < Battery.PlayerArc / 2 And
                                direction - centerShip.Helm.Direction - centerShip.Batteries.Secondary.TurnDistance.current > -Battery.PlayerArc / 2 Then
                                centerShip.Batteries.Secondary.FireWeapon(distance, xList(i))
                                Exit Sub
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub SelectTarget()
            If SelectTargetCheck = True Then
                Dim lastDistance As Integer
                For i As Integer = 0 To xList.Length - 1
                    If xList(i).MyAllegence <> Ship.Allegence.Player Then
                        Dim distance As Integer = Math.Sqrt(((xList(i).Position.X - centerShip.Position.X) ^ 2) + ((xList(i).Position.Y - centerShip.Position.Y) ^ 2))
                        If (distance < lastDistance And distance <> 0) Or lastDistance = 0 Then
                            lastDistance = distance
                            centerShip.Helm.Target = xList(i)
                        End If
                    End If
                Next
            End If
        End Sub
        '-------------------

        '-----Shielding-----
        Public Shared ForwardBoostCheck As Boolean = False
        Public Shared RightBoostCheck As Boolean = False
        Public Shared RearBoostCheck As Boolean = False
        Public Shared LeftBoostCheck As Boolean = False
        Private Shared Sub ForwardBoost()
            If ForwardBoostCheck = True Then
                centerShip.Shielding.LastHit = centerShip.Shielding.Sides.FrontShield
            End If
        End Sub
        Private Shared Sub RightBoost()
            If RightBoostCheck = True Then
                centerShip.Shielding.LastHit = centerShip.Shielding.Sides.RightShield
            End If
        End Sub
        Private Shared Sub RearBoost()
            If RearBoostCheck = True Then
                centerShip.Shielding.LastHit = centerShip.Shielding.Sides.BackShield
            End If
        End Sub
        Private Shared Sub LeftBoost()
            If LeftBoostCheck = True Then
                centerShip.Shielding.LastHit = centerShip.Shielding.Sides.LeftShield
            End If
        End Sub
        '-------------------

        Public Shared Sub RunPlayerControls()
            '-----Helm-----
            ThrottleUp()
            ThrottleDown()
            TurnRight()
            TurnLeft()
            WarpDrive()
            MatchSpeed()
            '--------------
            '-----Batteries-----
            RotateRight()
            RotateLeft()
            FirePrimary()
            FireSecondary()
            SelectTarget()
            '-------------------
            '-----Shielding-----
            ForwardBoost()
            RightBoost()
            RearBoost()
            LeftBoost()
            '-------------------
        End Sub
    End Class

    Public Sub RemoveShip(ByRef nShip As Ship)
        Dim index As Integer = 0
        While True
            If ReferenceEquals(xList(index), nShip) = True Then
                Exit While
            End If
            index = index + 1
        End While

        For i As Integer = index To UBound(xList)
            If i <> UBound(xList) Then
                xList(i) = xList(i + 1)
            End If
        Next
        ReDim Preserve xList(UBound(xList) - 1)
    End Sub

    Public Sub Recenter()
        For Each i As Ship In xList
            If i.MyAllegence = Ship.Allegence.Player Then
                centerShip = i
                For Each e As ServerSideClient In Server.Communications.Clients
                    Select Case e.MyStation
                        Case Station.StationTypes.Helm
                            centerShip.Helm.PlayerControled = True
                        Case Station.StationTypes.Batteries
                            centerShip.Batteries.PlayerControled = True
                        Case Station.StationTypes.Shielding
                            centerShip.Batteries.PlayerControled = True
                        Case Station.StationTypes.Engineering
                            centerShip.Engineering.PlayerControled = True
                    End Select
                Next
                Exit Sub
            End If
        Next
        GalaxyTimer.Enabled = False
        Console.WriteLine("Player is Defeated")
    End Sub

    Public Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        Dim friendly As Integer
        Dim enemy As Integer
        PlayerControl.RunPlayerControls()
        For Each i As Ship In xList
            i.UpdateShip()
            If i.MyAllegence = Ship.Allegence.Player Then
                friendly = friendly + 1
            Else
                enemy = enemy + 1
            End If
        Next

        '-----Rendering-----
        '-----Clear old-----
        Select Case Warping
            Case Warp.None
                Bmp = My.Resources.NormalSpace.Clone
            Case Warp.Entering
                Warping = Warp.Warping
            Case Warp.Exiting
                Warping = Warp.None
            Case Warp.Warping
                Bmp = My.Resources.Warping.Clone
        End Select
        ReDim shipPositions(UBound(xList))
        ReDim directionPositions(UBound(xList))
        '-------------------

        '-----Set new positions-----
        For i As Integer = 0 To UBound(xList)
            shipPositions(i) = New Point(
                xList(i).Position.X - centerShip.Position.X + ((Bmp.Width / 2) - 1),
                xList(i).Position.Y - centerShip.Position.Y + ((Bmp.Height / 2) - 1))
            Dim opposite As Integer = shipPositions(i).Y - ((Bmp.Width / 2) - 1)
            Dim adjacent As Integer = shipPositions(i).X - ((Bmp.Height / 2) - 1)
            Dim distance As Integer = Math.Sqrt((opposite * opposite) + (adjacent * adjacent))
            Dim scale As Double = distance / 200
            If distance > 200 Then
                Dim x As Integer = (adjacent / scale) + ((Bmp.Width / 2) - 1)
                Dim y As Integer = (opposite / scale) + ((Bmp.Height / 2) - 1)
                shipPositions(i) = New Point(x, y)
            End If
            directionPositions(i) = New Point((shipPositions(i).X + (Math.Cos(xList(i).Helm.Direction) * 10)),
                                         (shipPositions(i).Y + (Math.Sin(xList(i).Helm.Direction) * 10)))
        Next
        For Each i As Star In stars
            i.Update()
        Next
        '---------------------------

        '-----Render-----
        For Each i As Star In stars
            If i.Flash = True Then
                For x As Integer = -2 To 2
                    For y As Integer = -2 To 2
                        Bmp.SetPixel(i.Position.X + x, i.Position.Y + y, Color.White)
                    Next
                Next
            Else
                Bmp.SetPixel(i.Position.X, i.Position.Y, Color.White)
            End If
        Next

        If Warping <> Warp.Warping Then
            '-----Batteries Arc-----
            '-----Primary Arc-----
            primaryDirection = centerShip.Helm.Direction + centerShip.Batteries.Primary.TurnDistance.current
            primaryRadius = centerShip.Helm.Direction + centerShip.Batteries.Primary.WeaponStats(Weapon.Stats.Range).current
            For i As Integer = 1 To primaryRadius
                Dim x As Integer = Math.Cos(primaryDirection + -(Battery.PlayerArc / 2)) * i
                Dim y As Integer = Math.Sin(primaryDirection + -(Battery.PlayerArc / 2)) * i
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
                x = Math.Cos(primaryDirection + (Battery.PlayerArc / 2)) * i
                y = Math.Sin(primaryDirection + (Battery.PlayerArc / 2)) * i
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            Next
            For i As Double = -(Battery.PlayerArc / 2) To (Battery.PlayerArc / 2) Step Battery.PlayerArc / (2 * Math.PI)
                Dim x As Integer = Math.Cos(primaryDirection + i) * primaryRadius
                Dim y As Integer = Math.Sin(primaryDirection + i) * primaryRadius
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            Next
            '---------------------
            '-----Secondary Arc-----
            secondaryDirection = centerShip.Helm.Direction + centerShip.Batteries.Secondary.TurnDistance.current
            secondaryRadius = centerShip.Helm.Direction + centerShip.Batteries.Secondary.WeaponStats(Weapon.Stats.Range).current
            For i As Integer = 1 To secondaryRadius
                Dim x As Integer = Math.Cos(secondaryDirection + -(Battery.PlayerArc / 2)) * i
                Dim y As Integer = Math.Sin(secondaryDirection + -(Battery.PlayerArc / 2)) * i
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
                x = Math.Cos(secondaryDirection + (Battery.PlayerArc / 2)) * i
                y = Math.Sin(secondaryDirection + (Battery.PlayerArc / 2)) * i
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            Next
            For i As Double = -(Battery.PlayerArc / 2) To (Battery.PlayerArc / 2) Step Battery.PlayerArc / (2 * Math.PI)
                Dim x As Integer = Math.Cos(secondaryDirection + i) * secondaryRadius
                Dim y As Integer = Math.Sin(secondaryDirection + i) * secondaryRadius
                Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            Next
            '-----------------------
            '-----------------------
        End If

        Dim count As Integer = 0
        For Each i As Point In shipPositions
            If (Warping = Warp.None) Or (Warping = Warp.Warping And ReferenceEquals(xList(count), centerShip)) Then
                '-----Set Ship Colour-----
                Dim index As Integer = Array.IndexOf(shipPositions, i)
                Dim nColour As Color
                If xList(index).Hit = True Then
                    nColour = Color.Orange
                Else
                    Select Case xList(index).MyAllegence
                        Case Ship.Allegence.Pirate
                            nColour = Color.Red
                        Case Ship.Allegence.Player
                            nColour = Color.Green
                    End Select
                End If
                '-------------------------
                For x As Integer = -3 To 3
                    For y As Integer = -3 To 3
                        Bmp.SetPixel(i.X + x, i.Y + y, nColour)
                    Next
                Next
                Bmp.SetPixel(directionPositions(index).X, directionPositions(index).Y, nColour)
            End If
            count = count + 1
        Next

        '-----Batteries Target-----
        If centerShip.Helm.Target IsNot Nothing Then
            If centerShip.Helm.Target.Hit = False Then
                Dim targetIndex = -1
                While True
                    targetIndex = targetIndex + 1
                    If targetIndex = UBound(xList) Or ReferenceEquals(xList(targetIndex), centerShip.Helm.Target) = True Then
                        Exit While
                    End If
                End While
                If targetIndex <> -1 Then
                    For x As Integer = -3 To 3
                        For y As Integer = -3 To 3
                            Bmp.SetPixel(shipPositions(targetIndex).X + x, shipPositions(targetIndex).Y + y, Color.Blue)
                        Next
                    Next
                    Bmp.SetPixel(directionPositions(targetIndex).X, directionPositions(targetIndex).Y, Color.Blue)
                End If
            End If
        End If
        '--------------------------
        '-------------------
        Dim temp As New Bitmap(Bmp)
        Server.Communications.UpdateServerMessage_Call(centerShip, temp)
    End Sub

End Class
