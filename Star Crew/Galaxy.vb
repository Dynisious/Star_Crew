Public Class Galaxy
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = False}
    Public Shared clientShip As PlayerShip
    Private primaryDirection As Double
    Private primaryRadius As Integer
    Private secondaryDirection As Double
    Private secondaryRadius As Integer
    Public xList(300) As Ship
    Private Shared Bmp As New Bitmap(600, 600)
    Private Shared BaseBmp As New Bitmap(600, 600)
    Private shipPositions(-1) As Point
    Private directionPositions(-1) As Point
    Private weaponDirections(1) As Double
    Private scanner As Double
    Private stars(50) As Star
    Private Event NewClientMessage(ByVal nMessage As Station, ByVal nStation As Station.StationTypes)
    Private Event StartGame()

    Private Class Star
        Public Position As Point
        Public Flash As Boolean = False
        Private count As Integer

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

            Dim direction As Double = Helm.NormalizeDirection(clientShip.Helm.Direction + Math.PI)
            Position = New Point(Position.X + (clientShip.Helm.Throttle.current * Math.Cos(direction)), Position.Y + (clientShip.Helm.Throttle.current * Math.Sin(direction)))
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

    Public Sub CreateWorld() Handles Me.StartGame
        Randomize()
        For x As Integer = 0 To Bmp.Width - 1
            For y As Integer = 0 To Bmp.Height - 1
                Bmp.SetPixel(x, y, Color.Black)
                BaseBmp.SetPixel(x, y, Color.Black)
            Next
        Next
        For i As Integer = 1 To 3768
            Dim radian As Double = (2 * Math.PI * 1256) / i
            Dim x As Integer = ((Bmp.Width / 2) - 1) + (Math.Cos(radian) * 200)
            Dim y As Integer = ((Bmp.Height / 2) - 1) + (Math.Sin(radian) * 200)
            Bmp.SetPixel(x, y, Color.LightBlue)
            BaseBmp.SetPixel(x, y, Color.LightBlue)
        Next
        For i As Integer = 0 To UBound(stars)
            stars(i) = New Star(New Point(Int((594 * Rnd()) + 3), Int((594 * Rnd())) + 3))
        Next

        Randomize()
        clientShip = New PlayerShip(New Clunker)
        clientShip.Parent = Me
        xList(0) = clientShip
        xList(0).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        For i As Integer = 1 To UBound(xList)
            If Int(2 * Rnd()) = 0 Then
                xList(i) = New FriendlyShip(Me, New Clunker)
            Else
                xList(i) = New PirateShip(Me, New Clunker)
            End If
            xList(i).Position = New Point((6000 * Rnd()) - 3000, (6000 * Rnd()) - 3000)
        Next
        primaryDirection = clientShip.Helm.Direction + clientShip.Batteries.Primary.TurnDistance.current
        primaryRadius = clientShip.Batteries.Primary.WeaponStats(Weapon.Stats.Range).current
        secondaryDirection = clientShip.Helm.Direction + clientShip.Batteries.Secondary.TurnDistance.current
        secondaryRadius = clientShip.Batteries.Secondary.WeaponStats(Weapon.Stats.Range).current
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
            Select Case i.MyAllegence
                Case Ship.Allegence.Player
                    friendly = friendly + 1
                Case Ship.Allegence.Pirate
                    enemy = enemy + 1
            End Select
        Next

        '-----Primary Arc-----
        For i As Integer = 1 To primaryRadius
            Dim x As Integer = Math.Cos(primaryDirection + -(Battery.HitArc / 2)) * i
            Dim y As Integer = Math.Sin(primaryDirection + -(Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
            x = Math.Cos(primaryDirection + (Battery.HitArc / 2)) * i
            y = Math.Sin(primaryDirection + (Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
        Next
        For i As Double = -(Battery.HitArc / 2) To (Battery.HitArc / 2) Step Battery.HitArc / (2 * Math.PI)
            Dim x As Integer = Math.Cos(primaryDirection + i) * primaryRadius
            Dim y As Integer = Math.Sin(primaryDirection + i) * primaryRadius
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
        Next
        '---------------------
        '-----Secondary Arc-----
        For i As Integer = 1 To secondaryRadius
            Dim x As Integer = Math.Cos(secondaryDirection + -(Battery.HitArc / 2)) * i
            Dim y As Integer = Math.Sin(secondaryDirection + -(Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
            x = Math.Cos(secondaryDirection + (Battery.HitArc / 2)) * i
            y = Math.Sin(secondaryDirection + (Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
        Next
        For i As Double = -(Battery.HitArc / 2) To (Battery.HitArc / 2) Step Battery.HitArc / (2 * Math.PI)
            Dim x As Integer = Math.Cos(secondaryDirection + i) * secondaryRadius
            Dim y As Integer = Math.Sin(secondaryDirection + i) * secondaryRadius
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), BaseBmp.GetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1)))
        Next
        '-----------------------
        primaryDirection = clientShip.Helm.Direction + clientShip.Batteries.Primary.TurnDistance.current
        primaryRadius = clientShip.Batteries.Primary.WeaponStats(Weapon.Stats.Range).current
        secondaryDirection = clientShip.Helm.Direction + clientShip.Batteries.Secondary.TurnDistance.current
        secondaryRadius = clientShip.Batteries.Secondary.WeaponStats(Weapon.Stats.Range).current

        '-----Rendering-----
        '-----Clear old-----
        For Each i As Point In shipPositions
            For x As Integer = -3 To 3
                For y As Integer = -3 To 3
                    Bmp.SetPixel(i.X + x, i.Y + y, BaseBmp.GetPixel(i.X + x, i.Y + y))
                Next
            Next
        Next
        For Each i As Point In directionPositions
            Bmp.SetPixel(i.X, i.Y, BaseBmp.GetPixel(i.X, i.Y))
        Next
        For Each i As Star In stars
            For x As Integer = -2 To 2
                For y As Integer = -2 To 2
                    Bmp.SetPixel(i.Position.X + x, i.Position.Y + y, BaseBmp.GetPixel(i.Position.X + x, i.Position.Y + y))
                Next
            Next
        Next
        For i As Integer = 1 To 200
            Dim x As Integer = ((Bmp.Width / 2) - 1) + (Math.Cos(scanner) * i)
            Dim y As Integer = ((Bmp.Width / 2) - 1) + (Math.Sin(scanner) * i)
            Bmp.SetPixel(x, y, BaseBmp.GetPixel(x, y))
        Next
        ReDim shipPositions(UBound(xList))
        ReDim directionPositions(UBound(xList))
        '-------------------

        '-----Set new positions-----
        For i As Integer = 0 To UBound(xList)
            shipPositions(i) = New Point(
                xList(i).Position.X - clientShip.Position.X + ((Bmp.Width / 2) - 1),
                xList(i).Position.Y - clientShip.Position.Y + ((Bmp.Height / 2) - 1))
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
        scanner = scanner + ((100 * Math.PI) / 1256)
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
        '-----Batteries Arc-----
        '-----Primary Arc-----
        For i As Integer = 1 To primaryRadius
            Dim x As Integer = Math.Cos(primaryDirection + -(Battery.HitArc / 2)) * i
            Dim y As Integer = Math.Sin(primaryDirection + -(Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            x = Math.Cos(primaryDirection + (Battery.HitArc / 2)) * i
            y = Math.Sin(primaryDirection + (Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
        Next
        For i As Double = -(Battery.HitArc / 2) To (Battery.HitArc / 2) Step Battery.HitArc / (2 * Math.PI)
            Dim x As Integer = Math.Cos(primaryDirection + i) * primaryRadius
            Dim y As Integer = Math.Sin(primaryDirection + i) * primaryRadius
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
        Next
        '---------------------
        '-----Secondary Arc-----
        For i As Integer = 1 To secondaryRadius
            Dim x As Integer = Math.Cos(secondaryDirection + -(Battery.HitArc / 2)) * i
            Dim y As Integer = Math.Sin(secondaryDirection + -(Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
            x = Math.Cos(secondaryDirection + (Battery.HitArc / 2)) * i
            y = Math.Sin(secondaryDirection + (Battery.HitArc / 2)) * i
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
        Next
        For i As Double = -(Battery.HitArc / 2) To (Battery.HitArc / 2) Step Battery.HitArc / (2 * Math.PI)
            Dim x As Integer = Math.Cos(secondaryDirection + i) * secondaryRadius
            Dim y As Integer = Math.Sin(secondaryDirection + i) * secondaryRadius
            Bmp.SetPixel(x + ((Bmp.Width / 2) - 1), y + ((Bmp.Height / 2) - 1), Color.Yellow)
        Next
        '-----------------------
        '-----------------------
        For Each i As Point In shipPositions
            '-----Set Ship Colour-----
            Dim index As Integer = Array.IndexOf(shipPositions, i)
            Dim nColour As Color
            If xList(index).hit = True Then
                nColour = Color.Orange
                xList(index).hit = False
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
        Next

        '-------------------
        If clientShip.Parent Is Nothing Then
            'GalaxyTimer.Enabled = False
            Console.WriteLine("Client is Dead")
        End If
        If xList.Length = 1 Then
            GalaxyTimer.Enabled = False
            Console.WriteLine("Client is Last standing")
        End If
        Dim temp As New Bitmap(Bmp)
        Server.Communications.UpdateServerMessage_Call(clientShip, temp)
    End Sub

End Class
