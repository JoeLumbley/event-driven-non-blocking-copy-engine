'<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
'Partial Class Form1
'    Inherits System.Windows.Forms.Form

'    'Form overrides dispose to clean up the component list.
'    <System.Diagnostics.DebuggerNonUserCode()>
'    Protected Overrides Sub Dispose(disposing As Boolean)
'        Try
'            If disposing AndAlso components IsNot Nothing Then
'                components.Dispose()
'            End If
'        Finally
'            MyBase.Dispose(disposing)
'        End Try
'    End Sub

'    'Required by the Windows Form Designer
'    Private components As System.ComponentModel.IContainer

'    'NOTE: The following procedure is required by the Windows Form Designer
'    'It can be modified using the Windows Form Designer.
'    'Do not modify it using the code editor.
'    <System.Diagnostics.DebuggerStepThrough()>
'    Private Sub InitializeComponent()
'        btnCopy = New Button()
'        ProgressBar1 = New ProgressBar()
'        lblStatus = New Label()
'        txtSourceFolder = New TextBox()
'        txtDestinationFolder = New TextBox()
'        lblSourceFolder = New Label()
'        lblDestinationFolder = New Label()
'        btnCancel = New Button()
'        SuspendLayout()
'        ' 
'        ' btnCopy
'        ' 
'        btnCopy.Location = New Point(12, 122)
'        btnCopy.Name = "btnCopy"
'        btnCopy.Size = New Size(75, 23)
'        btnCopy.TabIndex = 0
'        btnCopy.Text = "Copy"
'        btnCopy.UseVisualStyleBackColor = True
'        ' 
'        ' ProgressBar1
'        ' 
'        ProgressBar1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
'        ProgressBar1.Location = New Point(12, 176)
'        ProgressBar1.Name = "ProgressBar1"
'        ProgressBar1.Size = New Size(776, 27)
'        ProgressBar1.TabIndex = 1
'        ' 
'        ' lblStatus
'        ' 
'        lblStatus.AutoSize = True
'        lblStatus.Location = New Point(15, 153)
'        lblStatus.Name = "lblStatus"
'        lblStatus.Size = New Size(0, 15)
'        lblStatus.TabIndex = 2
'        ' 
'        ' txtSourceFolder
'        ' 
'        txtSourceFolder.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
'        txtSourceFolder.Location = New Point(12, 29)
'        txtSourceFolder.Name = "txtSourceFolder"
'        txtSourceFolder.Size = New Size(776, 23)
'        txtSourceFolder.TabIndex = 3
'        ' 
'        ' txtDestinationFolder
'        ' 
'        txtDestinationFolder.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
'        txtDestinationFolder.Location = New Point(12, 86)
'        txtDestinationFolder.Name = "txtDestinationFolder"
'        txtDestinationFolder.Size = New Size(776, 23)
'        txtDestinationFolder.TabIndex = 4
'        ' 
'        ' lblSourceFolder
'        ' 
'        lblSourceFolder.AutoSize = True
'        lblSourceFolder.Location = New Point(12, 9)
'        lblSourceFolder.Name = "lblSourceFolder"
'        lblSourceFolder.Size = New Size(79, 15)
'        lblSourceFolder.TabIndex = 5
'        lblSourceFolder.Text = "Source Folder"
'        ' 
'        ' lblDestinationFolder
'        ' 
'        lblDestinationFolder.AutoSize = True
'        lblDestinationFolder.Location = New Point(12, 66)
'        lblDestinationFolder.Name = "lblDestinationFolder"
'        lblDestinationFolder.Size = New Size(103, 15)
'        lblDestinationFolder.TabIndex = 6
'        lblDestinationFolder.Text = "Destination Folder"
'        ' 
'        ' btnCancel
'        ' 
'        btnCancel.Location = New Point(93, 122)
'        btnCancel.Name = "btnCancel"
'        btnCancel.Size = New Size(75, 23)
'        btnCancel.TabIndex = 7
'        btnCancel.Text = "Cancel"
'        btnCancel.UseVisualStyleBackColor = True
'        ' 
'        ' Form1
'        ' 
'        AutoScaleDimensions = New SizeF(7F, 15F)
'        AutoScaleMode = AutoScaleMode.Font
'        ClientSize = New Size(800, 215)
'        Controls.Add(btnCancel)
'        Controls.Add(lblDestinationFolder)
'        Controls.Add(lblSourceFolder)
'        Controls.Add(txtDestinationFolder)
'        Controls.Add(txtSourceFolder)
'        Controls.Add(lblStatus)
'        Controls.Add(ProgressBar1)
'        Controls.Add(btnCopy)
'        Name = "Form1"
'        Text = "Form1"
'        ResumeLayout(False)
'        PerformLayout()
'    End Sub

'    Friend WithEvents btnCopy As Button
'    Friend WithEvents ProgressBar1 As ProgressBar
'    Friend WithEvents lblStatus As Label
'    Friend WithEvents txtSourceFolder As TextBox
'    Friend WithEvents txtDestinationFolder As TextBox
'    Friend WithEvents lblSourceFolder As Label
'    Friend WithEvents lblDestinationFolder As Label
'    Friend WithEvents btnCancel As Button

'End Class




'<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
'Partial Class Form1
'    Inherits System.Windows.Forms.Form

'    Private components As System.ComponentModel.IContainer

'    <System.Diagnostics.DebuggerStepThrough()>
'    Private Sub InitializeComponent()
'        Me.txtSource = New System.Windows.Forms.TextBox()
'        Me.txtDest = New System.Windows.Forms.TextBox()
'        Me.btnBrowseSource = New System.Windows.Forms.Button()
'        Me.btnBrowseDest = New System.Windows.Forms.Button()
'        Me.btnStart = New System.Windows.Forms.Button()
'        Me.lblSource = New System.Windows.Forms.Label()
'        Me.lblDest = New System.Windows.Forms.Label()
'        Me.SuspendLayout()
'        '
'        'txtSource
'        '
'        Me.txtSource.Location = New System.Drawing.Point(15, 35)
'        Me.txtSource.Name = "txtSource"
'        Me.txtSource.Size = New System.Drawing.Size(360, 23)
'        Me.txtSource.TabIndex = 0
'        '
'        'txtDest
'        '
'        Me.txtDest.Location = New System.Drawing.Point(15, 95)
'        Me.txtDest.Name = "txtDest"
'        Me.txtDest.Size = New System.Drawing.Size(360, 23)
'        Me.txtDest.TabIndex = 1
'        '
'        'btnBrowseSource
'        '
'        Me.btnBrowseSource.Location = New System.Drawing.Point(385, 35)
'        Me.btnBrowseSource.Name = "btnBrowseSource"
'        Me.btnBrowseSource.Size = New System.Drawing.Size(75, 23)
'        Me.btnBrowseSource.TabIndex = 2
'        Me.btnBrowseSource.Text = "Browse..."
'        Me.btnBrowseSource.UseVisualStyleBackColor = True
'        '
'        'btnBrowseDest
'        '
'        Me.btnBrowseDest.Location = New System.Drawing.Point(385, 95)
'        Me.btnBrowseDest.Name = "btnBrowseDest"
'        Me.btnBrowseDest.Size = New System.Drawing.Size(75, 23)
'        Me.btnBrowseDest.TabIndex = 3
'        Me.btnBrowseDest.Text = "Browse..."
'        Me.btnBrowseDest.UseVisualStyleBackColor = True
'        '
'        'btnStart
'        '
'        Me.btnStart.Location = New System.Drawing.Point(365, 140)
'        Me.btnStart.Name = "btnStart"
'        Me.btnStart.Size = New System.Drawing.Size(95, 30)
'        Me.btnStart.TabIndex = 4
'        Me.btnStart.Text = "Start Copy"
'        Me.btnStart.UseVisualStyleBackColor = True
'        '
'        'lblSource
'        '
'        Me.lblSource.AutoSize = True
'        Me.lblSource.Location = New System.Drawing.Point(12, 15)
'        Me.lblSource.Name = "lblSource"
'        Me.lblSource.Size = New System.Drawing.Size(82, 15)
'        Me.lblSource.TabIndex = 5
'        Me.lblSource.Text = "Source Folder:"
'        '
'        'lblDest
'        '
'        Me.lblDest.AutoSize = True
'        Me.lblDest.Location = New System.Drawing.Point(12, 75)
'        Me.lblDest.Name = "lblDest"
'        Me.lblDest.Size = New System.Drawing.Size(113, 15)
'        Me.lblDest.TabIndex = 6
'        Me.lblDest.Text = "Destination Folder:"
'        '
'        'Form1
'        '
'        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
'        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
'        Me.ClientSize = New System.Drawing.Size(480, 190)
'        Me.Controls.Add(Me.lblDest)
'        Me.Controls.Add(Me.lblSource)
'        Me.Controls.Add(Me.btnStart)
'        Me.Controls.Add(Me.btnBrowseDest)
'        Me.Controls.Add(Me.btnBrowseSource)
'        Me.Controls.Add(Me.txtDest)
'        Me.Controls.Add(Me.txtSource)
'        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
'        Me.MaximizeBox = False
'        Me.MinimizeBox = False
'        Me.Name = "Form1"
'        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
'        Me.Text = "File Copy Launcher"
'        Me.ResumeLayout(False)
'        Me.PerformLayout()

'    End Sub

'    Friend WithEvents txtSource As TextBox
'    Friend WithEvents txtDest As TextBox
'    Friend WithEvents btnBrowseSource As Button
'    Friend WithEvents btnBrowseDest As Button
'    Friend WithEvents btnStart As Button
'    Friend WithEvents lblSource As Label
'    Friend WithEvents lblDest As Label

'End Class


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
