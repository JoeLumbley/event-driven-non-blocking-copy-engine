
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        txtSource = New TextBox()
        txtDestination = New TextBox()
        btnBrowseFolderSource = New Button()
        btnBrowseFileSource = New Button()
        btnBrowseDestination = New Button()
        btnStart = New Button()
        btnCancel = New Button()
        lblSource = New Label()
        lblDest = New Label()
        SuspendLayout()
        ' 
        ' txtSource
        ' 
        txtSource.Location = New Point(15, 35)
        txtSource.Name = "txtSource"
        txtSource.Size = New Size(360, 23)
        txtSource.TabIndex = 0
        ' 
        ' txtDestination
        ' 
        txtDestination.Location = New Point(15, 105)
        txtDestination.Name = "txtDestination"
        txtDestination.Size = New Size(360, 23)
        txtDestination.TabIndex = 1
        ' 
        ' btnBrowseFolderSource
        ' 
        btnBrowseFolderSource.Location = New Point(385, 35)
        btnBrowseFolderSource.Name = "btnBrowseFolderSource"
        btnBrowseFolderSource.Size = New Size(75, 23)
        btnBrowseFolderSource.TabIndex = 2
        btnBrowseFolderSource.Text = "Folder..."
        btnBrowseFolderSource.UseVisualStyleBackColor = True
        ' 
        ' btnBrowseFileSource
        ' 
        btnBrowseFileSource.Location = New Point(465, 35)
        btnBrowseFileSource.Name = "btnBrowseFileSource"
        btnBrowseFileSource.Size = New Size(75, 23)
        btnBrowseFileSource.TabIndex = 3
        btnBrowseFileSource.Text = "File..."
        btnBrowseFileSource.UseVisualStyleBackColor = True
        ' 
        ' btnBrowseDestination
        ' 
        btnBrowseDestination.Location = New Point(385, 105)
        btnBrowseDestination.Name = "btnBrowseDestination"
        btnBrowseDestination.Size = New Size(75, 23)
        btnBrowseDestination.TabIndex = 4
        btnBrowseDestination.Text = "Browse..."
        btnBrowseDestination.UseVisualStyleBackColor = True
        ' 
        ' btnStart
        ' 
        btnStart.Location = New Point(445, 150)
        btnStart.Name = "btnStart"
        btnStart.Size = New Size(95, 30)
        btnStart.TabIndex = 5
        btnStart.Text = "Start Copy"
        btnStart.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.Location = New Point(345, 150)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(95, 30)
        btnCancel.TabIndex = 6
        btnCancel.Text = "Cancel"
        btnCancel.UseVisualStyleBackColor = True
        btnCancel.Visible = False
        ' 
        ' lblSource
        ' 
        lblSource.AutoSize = True
        lblSource.Location = New Point(12, 15)
        lblSource.Name = "lblSource"
        lblSource.Size = New Size(73, 15)
        lblSource.TabIndex = 6
        lblSource.Text = "Source Path:"
        ' 
        ' lblDest
        ' 
        lblDest.AutoSize = True
        lblDest.Location = New Point(12, 85)
        lblDest.Name = "lblDest"
        lblDest.Size = New Size(106, 15)
        lblDest.TabIndex = 7
        lblDest.Text = "Destination Folder:"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(560, 200)
        Controls.Add(lblDest)
        Controls.Add(lblSource)
        Controls.Add(btnStart)
        Controls.Add(btnBrowseDestination)
        Controls.Add(btnBrowseFileSource)
        Controls.Add(btnBrowseFolderSource)
        Controls.Add(txtDestination)
        Controls.Add(txtSource)
        Controls.Add(btnCancel)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "File Copy Launcher"
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents txtSource As TextBox
    Friend WithEvents txtDestination As TextBox
    Friend WithEvents btnBrowseFolderSource As Button
    Friend WithEvents btnBrowseFileSource As Button
    Friend WithEvents btnBrowseDestination As Button
    Friend WithEvents btnStart As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblSource As Label
    Friend WithEvents lblDest As Label

End Class
