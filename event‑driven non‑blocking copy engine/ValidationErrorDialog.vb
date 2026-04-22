Imports System.Windows.Forms

Public Class ValidationErrorDialog
    Inherits Form

    Private lblMessage As Label
    Private btnOK As Button

    Public Sub New(message As String)
        Me.Text = "Copy"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Drawing.Size(420, 150)

        lblMessage = New Label() With {
            .AutoSize = False,
            .Text = message,
            .Left = 20,
            .Top = 20,
            .Width = 380,
            .Height = 70
        }

        btnOK = New Button() With {
            .Text = "OK",
            .Width = 80,
            .Height = 28,
            .Left = Me.ClientSize.Width - 100,
            .Top = Me.ClientSize.Height - 45,
            .DialogResult = DialogResult.OK
        }

        Me.Controls.Add(lblMessage)
        Me.Controls.Add(btnOK)

        Me.AcceptButton = btnOK
    End Sub

End Class
