Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Structure MessageRendering 'Used to render a graphical representation of the messages sent by the server
    Private Shared rendering As Boolean = False 'A Boolean value indicating whether there is currently rendering
    Private Shared ReadOnly renderSemaphore As New Threading.Semaphore(0, 1) 'A Semaphore object used to block the thread when there is nothing to render
    Private Shared renderRoutine As Render_Screen 'A Delegate to use to render the message
    Public Shared scaler As Double = 1 'A Double value indicating how to scale the locations and sizes of objects on screen
    Private Shared data() As Object 'The message to render
    Private Shared ReadOnly renderThread As New Threading.Thread(Sub()
                                                                     Do
                                                                         renderSemaphore.WaitOne() 'Wait for a signal
                                                                         renderRoutine.Invoke() 'Render
                                                                     Loop While True 'Loop forever
                                                                 End Sub) 'The thread used to render messages
    Private Shared ReadOnly displayArea As New Bitmap(1200, 700) 'The Bitmap to use to draw on
    Private Shared ReadOnly displayPaddingX As Integer = 550 'Padding from the left side of the screen for the display area
    Private Shared ReadOnly displayPaddingY As Integer = 300 'Padding from the top edge of the screen for the display area
    Private Shared ReadOnly friendlyFleet As Bitmap = My.Resources.FriendlyFleet.Clone() 'Create a copy of FriendlyFleet.bmp

    Public Shared Sub Initialise() 'Initialise the Rendering values
        renderThread.Start() 'Start the render thread
        friendlyFleet.MakeTransparent()
    End Sub

    Public Shared Sub Render(ByVal isFleetTransit As Boolean, ByRef messageData As Byte()) 'Begins the process of rendering a message
        If Not rendering Then 'Que the message
            rendering = True 'There is now a message rendering
            If isFleetTransit Then 'Render Fleet_Transit
                renderRoutine = New Render_Screen(AddressOf Fleet_Transit)
            Else 'Render Ship_To_Ship_Combat
                renderRoutine = New Render_Screen(AddressOf Ship_To_Ship_Combat)
            End If
            data = Game_Library.Serialisation.FromBytes(messageData) 'Set the message to render
            renderSemaphore.Release() 'Begin the render
        End If
    End Sub

    Private Delegate Sub Render_Screen() 'Renders a message
    Private Shared Sub Fleet_Transit() 'Render a Fleet_Transit game state
        Dim message() As Object = data 'Copy the message
        rendering = False 'Another message can be qued to render
        Dim backGround As Bitmap = displayArea.Clone() 'Get the image to draw on
        Dim gBackGround As Graphics = Graphics.FromImage(backGround) 'Get the graphics object for the back ground
        gBackGround.Clear(Color.Black)
        gBackGround.TranslateTransform((0 * Math.Cos(message(2)) * 100) + displayPaddingX,
                                       (0 * Math.Sin(message(2)) * 100) + displayPaddingY) 'Translate the image to be centered on the players perspective
        gBackGround.ScaleTransform(scaler, scaler) 'Scale the image

        gBackGround.FillEllipse(Brushes.ForestGreen, -50 - message(0), -50 - message(1), 100, 100) 'Draw in the planet

        '-----Render Entities-----
        Dim fleetsX() As Integer = message(3) 'The list of x positions for the fleets
        Dim fleetsY() As Integer = message(4) 'The list of y positions for the fleets
        Dim fleetDirections() As Double = message(5) 'The list of directions for the fleets
        For i As Integer = 0 To fleetsX.Length - 1 'Loop through all fleets
            fleetDirections(i) *= 180 / Math.PI 'Convert the radian to degrees
            Dim model As Bitmap = friendlyFleet.Clone() 'Clone model
            gBackGround.TranslateTransform(fleetsX(i), fleetsY(i)) 'Translate the image to be centered over the Fleet
            gBackGround.RotateTransform(fleetDirections(i)) 'Rotate the image to be in line with the Fleet
            gBackGround.DrawImage(model, -20, -20) 'Draw model onto background
            gBackGround.RotateTransform(-fleetDirections(i)) 'Rotate the image to be in line with world space
            gBackGround.TranslateTransform(-fleetsX(i), -fleetsY(i)) 'Translate the image to be centered over the players perspective
        Next
        '-------------------------

        gBackGround.DrawEllipse(New Pen(Brushes.White, 2), -100, -100, 200, 200) 'Draw a circle around the player's Fleet
        gBackGround.FillEllipse(Brushes.IndianRed, -5, -5, 10, 10) 'Put a marker on the player's Fleet
        outputScreen.CreateGraphics().DrawImage(backGround, 0, 0) 'Draw the image on
    End Sub
    Private Shared Sub Ship_To_Ship_Combat() 'Render a Ship_To_Ship_Combat game state
        Dim message() As Object = data 'Copy the message
        rendering = False 'Another message can be qued to render

    End Sub

End Structure
