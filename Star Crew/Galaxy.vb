Public Class Galaxy
    Public WithEvents GalaxyTimer As New Timer With {.Interval = 100, .Enabled = True}
    Public Shared clientShip As New PlayerShip(New Clunker)
    Public xList(3) As Ship
    Public Shared Bmp As New Bitmap(600, 600)
    Public Shared BaseBmp As New Bitmap(600, 600)
    Private shipPositions(-1) As Point
    Private directionPositions(-1) As Point
    Private scanner As Double
    Private stars(50) As Star

    Private Class Star
        Public Position As Point
        Private Flash As Boolean = False
        Private count As Integer

        Public Sub New(ByVal nPosition As Point)
            Position = nPosition
        End Sub

        Public Sub Update()
            If count > 1 Then
                count = count - 1
            ElseIf count = 1 Then
                count = count - 1
                For x As Integer = -2 To 2
                    For y As Integer = -2 To 2
                        BaseBmp.SetPixel(Position.X + x, Position.Y + y, Color.White)
                    Next
                Next
            ElseIf Int(80 * Rnd()) = 0 Then
                count = 20
                Flash = True
            End If

            If Flash = True Then
                For x As Integer = -2 To 2
                    For y As Integer = -2 To 2
                        BaseBmp.SetPixel(Position.X + x, Position.Y + y, Color.Black)
                    Next
                Next
                BaseBmp.SetPixel(Position.X, Position.Y, Color.White)
                Flash = False
            End If
        End Sub
    End Class

    Public Sub New(ByRef nServer As Server)
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
        For i As Integer = 0 To 50
            stars(i) = New Star(New Point(Int((594 * Rnd()) + 3), Int((594 * Rnd())) + 3))
        Next

        clientShip.Parent = Me
        xList(0) = clientShip
        xList(1) = New Ship(Me, New Clunker)
        xList(1).Position = New Point(200, 200)
        xList(2) = New PirateShip(Me, New Clunker)
        xList(2).Position = New Point(-600, -600)
        xList(3) = New PirateShip(Me, New Clunker)
        xList(3).Position = New Point(-300, 600)
    End Sub

    Public Sub RemoveShip(ByRef nShip As Ship)
        Dim index As Integer = 0
        While True
            If xList(index) Is nShip Then
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

    Public Sub UpdateGalaxy() Handles GalaxyTimer.Tick
        For Each i As Ship In xList
            i.UpdateShip()
        Next
        For Each i As Star In stars
            i.Update()
        Next

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
            directionPositions(i) = New Point(
                shipPositions(i).X + (Math.Cos(xList(i).Helm.Direction) * 10),
                shipPositions(i).Y + (Math.Sin(xList(i).Helm.Direction) * 10))
        Next
        scanner = scanner + ((100 * Math.PI) / 1256)
        '---------------------------

        '-----Render-----
        For Each i As Star In stars
            For x As Integer = -2 To 2
                For y As Integer = -2 To 2
                    Bmp.SetPixel(i.Position.X + x, i.Position.Y + y, BaseBmp.GetPixel(i.Position.X + x, i.Position.Y + y))
                Next
            Next
        Next
        For Each i As Point In shipPositions
            Dim index As Integer = Array.IndexOf(shipPositions, i)
            Dim direction As Double = Math.Tanh(i.Y / i.X)
            If i.X < ((Bmp.Width / 2) - 1) Then
                direction = (direction + Math.PI) Mod (2 * Math.PI)
            End If
            Dim nX As Integer = (Math.Cos(direction) * 200) + ((Bmp.Width / 2) - 1)
            Dim nY As Integer = (Math.Sin(direction) * 200) + ((Bmp.Height / 2) - 1)
            Dim nColour As Color
            If xList(index).hit = True Then
                nColour = Color.White
                xList(index).hit = False
            Else
                Select Case xList(index).MyAllegence
                    Case Ship.Allegence.Neutral
                        nColour = Color.LightBlue
                    Case Ship.Allegence.Pirate
                        nColour = Color.Red
                    Case Ship.Allegence.Player
                        nColour = Color.Green
                End Select
            End If
            For x As Integer = -3 To 3
                For y As Integer = -3 To 3
                    Bmp.SetPixel(i.X + x, i.Y + y, nColour)
                Next
            Next
        Next
        For Each i As Point In directionPositions
            Dim index As Integer = Array.IndexOf(directionPositions, i)
            Dim nColour As Color
            Select Case xList(index).MyAllegence
                Case Ship.Allegence.Neutral
                    nColour = Color.LightBlue
                Case Ship.Allegence.Pirate
                    nColour = Color.Red
                Case Ship.Allegence.Player
                    nColour = Color.Green
            End Select
            Bmp.SetPixel(i.X, i.Y, nColour)
        Next
        For i As Integer = 1 To 200
            Dim x As Integer = ((Bmp.Width / 2) - 1) + (Math.Cos(scanner) * i)
            Dim y As Integer = ((Bmp.Width / 2) - 1) + (Math.Sin(scanner) * i)
            Bmp.SetPixel(x, y, Color.Yellow)
        Next
        OutputScreen.picGraphicBox.Image = Bmp
        '----------------
        '-------------------

        OutputScreen.lblHull.Text = "Hull: " + Str(Int(clientShip.Hull.current))
        OutputScreen.lblFront.Text = "Front: " + Str(Int(clientShip.Shielding.ShipShields(Shields.Sides.FrontShield).current))
        OutputScreen.lblRight.Text = "Right: " + Str(Int(clientShip.Shielding.ShipShields(Shields.Sides.RightShield).current))
        OutputScreen.lblBack.Text = "Back: " + Str(Int(clientShip.Shielding.ShipShields(Shields.Sides.BackShield).current))
        OutputScreen.lblLeft.Text = "Left: " + Str(Int(clientShip.Shielding.ShipShields(Shields.Sides.LeftShield).current))
    End Sub

End Class
