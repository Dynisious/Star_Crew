<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OutputConsole
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pnlOutput = New System.Windows.Forms.Panel()
        Me.lblOutput = New System.Windows.Forms.Label()
        Me.txtInput = New System.Windows.Forms.TextBox()
        Me.pnlOutput.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlOutput
        '
        Me.pnlOutput.AutoScroll = True
        Me.pnlOutput.BackColor = System.Drawing.Color.Black
        Me.pnlOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlOutput.Controls.Add(Me.lblOutput)
        Me.pnlOutput.Location = New System.Drawing.Point(12, 12)
        Me.pnlOutput.Name = "pnlOutput"
        Me.pnlOutput.Size = New System.Drawing.Size(455, 211)
        Me.pnlOutput.TabIndex = 0
        '
        'lblOutput
        '
        Me.lblOutput.AutoSize = True
        Me.lblOutput.BackColor = System.Drawing.Color.Black
        Me.lblOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOutput.ForeColor = System.Drawing.Color.White
        Me.lblOutput.Location = New System.Drawing.Point(-1, -4)
        Me.lblOutput.MaximumSize = New System.Drawing.Size(430, 0)
        Me.lblOutput.MinimumSize = New System.Drawing.Size(430, 0)
        Me.lblOutput.Name = "lblOutput"
        Me.lblOutput.Size = New System.Drawing.Size(430, 13)
        Me.lblOutput.TabIndex = 0
        '
        'txtInput
        '
        Me.txtInput.BackColor = System.Drawing.Color.Black
        Me.txtInput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtInput.ForeColor = System.Drawing.Color.White
        Me.txtInput.Location = New System.Drawing.Point(12, 229)
        Me.txtInput.Name = "txtInput"
        Me.txtInput.Size = New System.Drawing.Size(455, 20)
        Me.txtInput.TabIndex = 1
        '
        'OutputConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(479, 261)
        Me.Controls.Add(Me.txtInput)
        Me.Controls.Add(Me.pnlOutput)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "OutputConsole"
        Me.Text = "OutputConsole"
        Me.pnlOutput.ResumeLayout(False)
        Me.pnlOutput.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pnlOutput As System.Windows.Forms.Panel
    Friend WithEvents lblOutput As System.Windows.Forms.Label
    Friend WithEvents txtInput As System.Windows.Forms.TextBox
End Class
