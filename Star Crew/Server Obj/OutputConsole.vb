Public Class OutputConsole
    Private WithEvents tick As New Timer With {.Interval = 1000, .Enabled = True}
    Private messages(-1) As String
    Private Enum ConsoleCommands
        Close
        Help
        Start
        Clear
        Suspend
        Play
        max
    End Enum
    Private Commands As New Dictionary(Of ConsoleCommands, String) From {
        {ConsoleCommands.Close, "/cls"},
        {ConsoleCommands.Help, "/help"},
        {ConsoleCommands.Start, "/start"},
        {ConsoleCommands.Clear, "/clr"},
        {ConsoleCommands.Suspend, "/suspend"},
        {ConsoleCommands.Play, "/play"}
    }
    Private CommandHelp As New Dictionary(Of ConsoleCommands, String) From {
        {ConsoleCommands.Close, "/cls:      Closes down the server"},
        {ConsoleCommands.Help, "/help:      Displays help for commands"},
        {ConsoleCommands.Start, "/start:        Begins the game server"},
        {ConsoleCommands.Clear, "/clr:      Clears the command line text"},
        {ConsoleCommands.Suspend, "/suspend:        Pauses the execution of the world"},
        {ConsoleCommands.Play, "/play:      Resumes the execution of the world"}
    }
    Private LastLineText As String = ""

    Public Sub New()
        InitializeComponent()
        WriteLine("for help with commands type '/help' (all commands must be lowercase)")
    End Sub

    Public Function ReadLine() As String
        LastLineText = ""

        While LastLineText = ""
        End While

        Return LastLineText
    End Function

    Private Sub outsideMessages() Handles tick.Tick
        For Each i As String In messages
            WriteLine(i)
        Next
        ReDim messages(-1)
    End Sub

    Public Sub WriteLine(Optional ByVal nString As String = "")
        If lblOutput.InvokeRequired = True Then
            ReDim Preserve messages(messages.Length)
            messages(UBound(messages)) = nString
        Else
            lblOutput.Text = lblOutput.Text + Environment.NewLine + nString
            pnlOutput.VerticalScroll.Value = pnlOutput.VerticalScroll.Maximum
        End If
    End Sub

    Private Sub TxtInputEnterPress(ByVal sender As System.Object, ByVal e As PreviewKeyDownEventArgs) Handles txtInput.PreviewKeyDown
        If e.KeyCode = Keys.Enter Then
            txtInput.Text = txtInput.Text.Trim(ChrW(0))
            LastLineText = txtInput.Text
            WriteLine(txtInput.Text)
            If txtInput.Text.StartsWith("/") = True Then
                RunCommand()
            End If
            txtInput.Text = ""
        End If
    End Sub

    Private Sub RunCommand()
        lblOutput.Text = lblOutput.Text + Environment.NewLine
        Dim command As ConsoleCommands = ConsoleCommands.max
        For Each i As KeyValuePair(Of ConsoleCommands, String) In Commands
            If i.Value = txtInput.Text Then
                command = i.Key
            End If
        Next
        Select Case command
            Case ConsoleCommands.Close
                Server.comms.Abort()
                If Screen.MyClient IsNot Nothing Then
                    Screen.MyClient.comms.Abort()
                End If
                End
            Case ConsoleCommands.Help
                For Each i As KeyValuePair(Of ConsoleCommands, String) In CommandHelp
                    WriteLine(i.Value)
                Next
            Case ConsoleCommands.Start
                Server.StartServer()
            Case ConsoleCommands.Clear
                lblOutput.Text = ""
            Case ConsoleCommands.Suspend
                If Server.GameWorld.clientShip IsNot Nothing Then
                    Server.GameWorld.GalaxyTimer.Enabled = False
                    WriteLine("Execution suspended")
                Else
                    WriteLine("Game has not been started: type '/start' to start game")
                End If
            Case ConsoleCommands.Play
                If Server.GameWorld.clientShip IsNot Nothing Then
                    Server.GameWorld.GalaxyTimer.Enabled = True
                    WriteLine("Execution resumed")
                Else
                    WriteLine("Game has not been started: type '/start' to start game")
                End If
            Case ConsoleCommands.max
                WriteLine("Unknown command: '" + txtInput.Text + "' please check for capitals and spelling before trying again")
        End Select
    End Sub

    Private Sub OutputConsole_FormClosing(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        Server.comms.Abort()
        If Screen.MyClient IsNot Nothing Then
            Screen.MyClient.comms.Abort()
        End If
        End
    End Sub
End Class