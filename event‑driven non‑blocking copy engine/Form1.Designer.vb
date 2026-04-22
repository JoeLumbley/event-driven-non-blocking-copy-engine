
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.txtSource = New System.Windows.Forms.TextBox()
        Me.txtDest = New System.Windows.Forms.TextBox()
        Me.btnBrowseSource = New System.Windows.Forms.Button()
        Me.btnBrowseFile = New System.Windows.Forms.Button()
        Me.btnBrowseDest = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.lblSource = New System.Windows.Forms.Label()
        Me.lblDest = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'txtSource
        '
        Me.txtSource.Location = New System.Drawing.Point(15, 35)
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(360, 23)
        Me.txtSource.TabIndex = 0
        '
        'txtDest
        '
        Me.txtDest.Location = New System.Drawing.Point(15, 105)
        Me.txtDest.Name = "txtDest"
        Me.txtDest.Size = New System.Drawing.Size(360, 23)
        Me.txtDest.TabIndex = 1
        '
        'btnBrowseSource
        '
        Me.btnBrowseSource.Location = New System.Drawing.Point(385, 35)
        Me.btnBrowseSource.Name = "btnBrowseSource"
        Me.btnBrowseSource.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseSource.TabIndex = 2
        Me.btnBrowseSource.Text = "Folder..."
        Me.btnBrowseSource.UseVisualStyleBackColor = True
        '
        'btnBrowseFile
        '
        Me.btnBrowseFile.Location = New System.Drawing.Point(465, 35)
        Me.btnBrowseFile.Name = "btnBrowseFile"
        Me.btnBrowseFile.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseFile.TabIndex = 3
        Me.btnBrowseFile.Text = "File..."
        Me.btnBrowseFile.UseVisualStyleBackColor = True
        '
        'btnBrowseDest
        '
        Me.btnBrowseDest.Location = New System.Drawing.Point(385, 105)
        Me.btnBrowseDest.Name = "btnBrowseDest"
        Me.btnBrowseDest.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseDest.TabIndex = 4
        Me.btnBrowseDest.Text = "Browse..."
        Me.btnBrowseDest.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(445, 150)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(95, 30)
        Me.btnStart.TabIndex = 5
        Me.btnStart.Text = "Start Copy"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'lblSource
        '
        Me.lblSource.AutoSize = True
        Me.lblSource.Location = New System.Drawing.Point(12, 15)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New System.Drawing.Size(82, 15)
        Me.lblSource.TabIndex = 6
        Me.lblSource.Text = "Source Path:"
        '
        'lblDest
        '
        Me.lblDest.AutoSize = True
        Me.lblDest.Location = New System.Drawing.Point(12, 85)
        Me.lblDest.Name = "lblDest"
        Me.lblDest.Size = New System.Drawing.Size(113, 15)
        Me.lblDest.TabIndex = 7
        Me.lblDest.Text = "Destination Folder:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(560, 200)
        Me.Controls.Add(Me.lblDest)
        Me.Controls.Add(Me.lblSource)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnBrowseDest)
        Me.Controls.Add(Me.btnBrowseFile)
        Me.Controls.Add(Me.btnBrowseSource)
        Me.Controls.Add(Me.txtDest)
        Me.Controls.Add(Me.txtSource)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "File Copy Launcher"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtSource As TextBox
    Friend WithEvents txtDest As TextBox
    Friend WithEvents btnBrowseSource As Button
    Friend WithEvents btnBrowseFile As Button
    Friend WithEvents btnBrowseDest As Button
    Friend WithEvents btnStart As Button
    Friend WithEvents lblSource As Label
    Friend WithEvents lblDest As Label

End Class
