Imports System.Windows.Forms

Public Class FolderMergeDialog
    Inherits Form

    Private lblMain As Label
    Private lblSub As Label
    Private btnYes As Button
    Private btnNo As Button
    Private picIcon As PictureBox

    'Public Sub New(folderName As String)
    'Me.Text = "Confirm Folder Merge"
    'Me.FormBorderStyle = FormBorderStyle.FixedDialog
    'Me.StartPosition = FormStartPosition.CenterParent
    'Me.MaximizeBox = False
    'Me.MinimizeBox = False
    'Me.ClientSize = New Drawing.Size(420, 160)
    'Me.ShowInTaskbar = False

    '' Warning icon
    'picIcon = New PictureBox() With {
    '    .Image = SystemIcons.Warning.ToBitmap(),
    '    .Left = 20,
    '    .Top = 20,
    '    .Width = 32,
    '    .Height = 32,
    '    .SizeMode = PictureBoxSizeMode.StretchImage
    '}

    '' Main message
    'lblMain = New Label() With {
    '    .AutoSize = False,
    '    .Left = 70,
    '    .Top = 25,
    '    .Width = 330,
    '    .Height = 40,
    '    .Text = "The destination already contains a folder named '" & folderName & "'."
    '}

    '' Sub message
    'lblSub = New Label() With {
    '    .AutoSize = False,
    '    .Left = 70,
    '    .Top = 75,
    '    .Width = 330,
    '    .Height = 30,
    '    .Text = "Do you want to merge these folders?"
    '}

    '' Yes button
    'btnYes = New Button() With {
    '    .Text = "Yes",
    '    .Width = 80,
    '    .Height = 28,
    '    .Left = Me.ClientSize.Width - 180,
    '    .Top = Me.ClientSize.Height - 45,
    '    .DialogResult = DialogResult.Yes
    '}

    '' No button
    'btnNo = New Button() With {
    '    .Text = "No",
    '    .Width = 80,
    '    .Height = 28,
    '    .Left = Me.ClientSize.Width - 90,
    '    .Top = Me.ClientSize.Height - 45,
    '    .DialogResult = DialogResult.No
    '}

    'Me.Controls.Add(picIcon)
    'Me.Controls.Add(lblMain)
    'Me.Controls.Add(lblSub)
    'Me.Controls.Add(btnYes)
    'Me.Controls.Add(btnNo)

    'Me.AcceptButton = btnYes
    'Me.CancelButton = btnNo


    'End Sub


    Public Sub New(folderName As String)
        Me.Text = "Confirm Folder Merge"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Drawing.Size(420, 160)
        Me.ShowInTaskbar = False

        ' Warning icon
        picIcon = New PictureBox() With {
        .Image = SystemIcons.Warning.ToBitmap(),
        .Left = 20,
        .Top = 30,
        .Width = 32,
        .Height = 32,
        .SizeMode = PictureBoxSizeMode.StretchImage
    }

        ' Combined message
        Dim message As String =
        "The destination already contains a folder named: " & Environment.NewLine & Environment.NewLine & folderName & Environment.NewLine & Environment.NewLine &
        "Do you want to merge these folders?"

        Dim txtMessage As New TextBox() With {
        .Multiline = True,
        .ReadOnly = True,
        .BorderStyle = BorderStyle.None,
        .TabStop = False,
        .Left = 70,
        .Top = 10,
        .Width = 330,
        .Height = 100,
        .Text = message,
        .BackColor = Me.BackColor
    }

        ' Yes button
        btnYes = New Button() With {
        .Text = "Yes",
        .Width = 80,
        .Height = 28,
        .Left = Me.ClientSize.Width - 180,
        .Top = Me.ClientSize.Height - 45,
        .DialogResult = DialogResult.Yes
    }

        ' No button
        btnNo = New Button() With {
        .Text = "No",
        .Width = 80,
        .Height = 28,
        .Left = Me.ClientSize.Width - 90,
        .Top = Me.ClientSize.Height - 45,
        .DialogResult = DialogResult.No
    }

        Me.Controls.Add(picIcon)
        Me.Controls.Add(txtMessage)
        Me.Controls.Add(btnYes)
        Me.Controls.Add(btnNo)

        Me.AcceptButton = btnYes
        Me.CancelButton = btnNo
    End Sub


End Class
