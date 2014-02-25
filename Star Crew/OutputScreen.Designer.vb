<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OutputScreen
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
        Me.picGraphicBox = New System.Windows.Forms.PictureBox()
        Me.lblHull = New System.Windows.Forms.Label()
        Me.lblFront = New System.Windows.Forms.Label()
        Me.lblRight = New System.Windows.Forms.Label()
        Me.lblLeft = New System.Windows.Forms.Label()
        Me.lblBack = New System.Windows.Forms.Label()
        CType(Me.picGraphicBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picGraphicBox
        '
        Me.picGraphicBox.Location = New System.Drawing.Point(149, 12)
        Me.picGraphicBox.Name = "picGraphicBox"
        Me.picGraphicBox.Size = New System.Drawing.Size(600, 600)
        Me.picGraphicBox.TabIndex = 0
        Me.picGraphicBox.TabStop = False
        '
        'lblHull
        '
        Me.lblHull.BackColor = System.Drawing.SystemColors.Control
        Me.lblHull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblHull.Location = New System.Drawing.Point(839, 46)
        Me.lblHull.Name = "lblHull"
        Me.lblHull.Size = New System.Drawing.Size(140, 20)
        Me.lblHull.TabIndex = 1
        Me.lblHull.Text = "Label1"
        '
        'lblFront
        '
        Me.lblFront.BackColor = System.Drawing.SystemColors.Control
        Me.lblFront.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblFront.Location = New System.Drawing.Point(839, 77)
        Me.lblFront.Name = "lblFront"
        Me.lblFront.Size = New System.Drawing.Size(140, 20)
        Me.lblFront.TabIndex = 2
        Me.lblFront.Text = "Label1"
        '
        'lblRight
        '
        Me.lblRight.BackColor = System.Drawing.SystemColors.Control
        Me.lblRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRight.Location = New System.Drawing.Point(922, 112)
        Me.lblRight.Name = "lblRight"
        Me.lblRight.Size = New System.Drawing.Size(140, 20)
        Me.lblRight.TabIndex = 3
        Me.lblRight.Text = "Label1"
        '
        'lblLeft
        '
        Me.lblLeft.BackColor = System.Drawing.SystemColors.Control
        Me.lblLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblLeft.Location = New System.Drawing.Point(766, 112)
        Me.lblLeft.Name = "lblLeft"
        Me.lblLeft.Size = New System.Drawing.Size(140, 20)
        Me.lblLeft.TabIndex = 5
        Me.lblLeft.Text = "Label1"
        '
        'lblBack
        '
        Me.lblBack.BackColor = System.Drawing.SystemColors.Control
        Me.lblBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblBack.Location = New System.Drawing.Point(839, 149)
        Me.lblBack.Name = "lblBack"
        Me.lblBack.Size = New System.Drawing.Size(140, 20)
        Me.lblBack.TabIndex = 4
        Me.lblBack.Text = "Label4"
        '
        'OutputScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1184, 661)
        Me.Controls.Add(Me.lblLeft)
        Me.Controls.Add(Me.lblBack)
        Me.Controls.Add(Me.lblRight)
        Me.Controls.Add(Me.lblFront)
        Me.Controls.Add(Me.lblHull)
        Me.Controls.Add(Me.picGraphicBox)
        Me.Name = "OutputScreen"
        Me.Text = "Form1"
        CType(Me.picGraphicBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents picGraphicBox As System.Windows.Forms.PictureBox
    Friend WithEvents lblHull As System.Windows.Forms.Label
    Friend WithEvents lblFront As System.Windows.Forms.Label
    Friend WithEvents lblRight As System.Windows.Forms.Label
    Friend WithEvents lblLeft As System.Windows.Forms.Label
    Friend WithEvents lblBack As System.Windows.Forms.Label

End Class
