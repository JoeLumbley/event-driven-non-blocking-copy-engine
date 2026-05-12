
Public Class ErrorDialog
    Inherits Form

    Private lblMessage As Label
    Private lblFile As Label
    Private txtError As TextBox
    Private btnIgnore As Button
    Private btnSkipAll As Button
    Private btnCancel As Button

    Private filePath As String
    Private err As Exception

    Public Sub New(file As String, ex As Exception)
        filePath = file
        err = ex

        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Copy Error"
        Me.Size = New Size(500, 300)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        lblMessage = New Label() With {
            .Text = "An error occurred while copying:",
            .Location = New Point(10, 10),
            .AutoSize = True
        }

        lblFile = New Label() With {
            .Text = filePath,
            .Location = New Point(10, 40),
            .AutoSize = True
        }

        txtError = New TextBox() With {
            .Text = err.Message,
            .Location = New Point(10, 70),
            .Size = New Size(460, 120),
            .Multiline = True,
            .ReadOnly = True,
            .ScrollBars = ScrollBars.Vertical
        }

        btnIgnore = New Button() With {
            .Text = "Skip",
            .Location = New Point(10, 210),
            .Size = New Size(100, 30)
        }
        AddHandler btnIgnore.Click, Sub()
                                        Me.DialogResult = DialogResult.Ignore
                                        Me.Close()
                                    End Sub

        btnSkipAll = New Button() With {
            .Text = "Skip All",
            .Location = New Point(120, 210),
            .Size = New Size(100, 30)
        }
        AddHandler btnSkipAll.Click, Sub()
                                         Me.DialogResult = DialogResult.Yes
                                         Me.Close()
                                     End Sub

        btnCancel = New Button() With {
            .Text = "Cancel",
            .Location = New Point(230, 210),
            .Size = New Size(100, 30)
        }
        AddHandler btnCancel.Click, Sub()
                                        Me.DialogResult = DialogResult.Cancel
                                        Me.Close()
                                    End Sub

        Me.Controls.Add(lblMessage)
        Me.Controls.Add(lblFile)
        Me.Controls.Add(txtError)
        Me.Controls.Add(btnIgnore)
        Me.Controls.Add(btnSkipAll)
        Me.Controls.Add(btnCancel)
    End Sub

End Class
