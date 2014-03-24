Public Class Galaxy
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Public Shared centerShip As Ship
    Private primaryDirection As Double
    Private primaryRadius As Integer
    Private secondaryDirection As Double
    Private secondaryRadius As Integer
    Public ReadOnly ShipCount As Integer = 10
    Public xList(-1) As Ship
    Private Shared Bmp As New Bitmap(600, 600)
    Private shipPositions(-1) As Point
    Private directionPositions(-1) As Point
    Private weaponDirections(1) As Double
    Private stars(150) As Star
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
                Exit Sub
            End If
        Next
        GalaxyTimer.Enabled = False
        Console.WriteLine("Player is Defeated")
    End Sub

    Public Sub NewClientMessage_Call(ByVal nMessage As Station, ByVal nStation As Station.StationTypes)
        RaiseEvent NewClientMessage(nMessage, nStation)
    End Sub

    Private Sub NewClientMessage_Handle(ByVal nMessage As Station, ByVal nStation As Station.StationTypes) Handles Me.NewClientMessage

    End Sub

    Public Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        Dim friendly As Integer
        Dim enemy As Integer
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

        Dim count As Integer
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
