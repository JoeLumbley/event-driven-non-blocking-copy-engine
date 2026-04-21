'Imports System.Windows.Forms

'Public Class ErrorSummaryDialog
'    Inherits Form

'    Private lst As ListBox
'    Private btnClose As Button
'    Private ReadOnly errors As List(Of CopyErrorEntry)

'    Public Sub New(errors As List(Of CopyErrorEntry))
'        Me.errors = errors
'        InitializeComponent()
'    End Sub

'    Private Sub InitializeComponent()
'        Me.Text = "Copy Summary"
'        Me.Size = New Size(500, 300)
'        Me.StartPosition = FormStartPosition.CenterParent
'        Me.FormBorderStyle = FormBorderStyle.FixedDialog
'        Me.MaximizeBox = False
'        Me.MinimizeBox = False

'        lst = New ListBox() With {
'            .Location = New Point(10, 10),
'            .Size = New Size(460, 220)
'        }

'        For Each Err In errors
'            lst.Items.Add($"{Err.FilePath} - {Err.Message}")
'        Next

'        btnClose = New Button() With {
'            .Text = "Close",
'            .Location = New Point(380, 240),
'            .Size = New Size(90, 25)
'        }
'        AddHandler btnClose.Click, Sub() Me.Close()

'        Me.Controls.Add(lst)
'        Me.Controls.Add(btnClose)
'    End Sub

'End Class


'Imports System.Windows.Forms

'Public Class ErrorDialog
'    Inherits Form

'    Private lblMessage As Label
'    Private lblFile As Label
'    Private txtError As TextBox
'    Private btnSkip As Button
'    Private btnSkipAll As Button
'    Private btnCancel As Button

'    Private filePath As String
'    Private err As Exception

'    Public Sub New(file As String, ex As Exception)
'        filePath = file
'        err = ex
'        InitializeComponent()
'    End Sub

'    Private Sub InitializeComponent()
'        Me.Text = "Copy Error"
'        Me.Size = New Size(500, 260)
'        Me.StartPosition = FormStartPosition.CenterParent
'        Me.FormBorderStyle = FormBorderStyle.FixedDialog
'        Me.MaximizeBox = False
'        Me.MinimizeBox = False

'        lblMessage = New Label() With {
'            .Text = "An error occurred while copying:",
'            .Location = New Point(10, 10),
'            .AutoSize = True
'        }

'        lblFile = New Label() With {
'            .Text = filePath,
'            .Location = New Point(10, 35),
'            .Size = New Size(460, 20)
'        }

'        txtError = New TextBox() With {
'            .Text = err.Message,
'            .Location = New Point(10, 60),
'            .Size = New Size(460, 110),
'            .Multiline = True,
'            .ReadOnly = True,
'            .ScrollBars = ScrollBars.Vertical
'        }

'        btnSkip = New Button() With {
'            .Text = "Skip",
'            .Location = New Point(10, 180),
'            .Size = New Size(90, 25)
'        }
'        AddHandler btnSkip.Click, Sub()
'                                      Me.DialogResult = DialogResult.Ignore
'                                      Me.Close()
'                                  End Sub

'        btnSkipAll = New Button() With {
'            .Text = "Skip All",
'            .Location = New Point(110, 180),
'            .Size = New Size(90, 25)
'        }
'        AddHandler btnSkipAll.Click, Sub()
'                                         Me.DialogResult = DialogResult.Yes
'                                         Me.Close()
'                                     End Sub

'        btnCancel = New Button() With {
'            .Text = "Cancel",
'            .Location = New Point(210, 180),
'            .Size = New Size(90, 25)
'        }
'        AddHandler btnCancel.Click, Sub()
'                                        Me.DialogResult = DialogResult.Cancel
'                                        Me.Close()
'                                    End Sub

'        Me.Controls.Add(lblMessage)
'        Me.Controls.Add(lblFile)
'        Me.Controls.Add(txtError)
'        Me.Controls.Add(btnSkip)
'        Me.Controls.Add(btnSkipAll)
'        Me.Controls.Add(btnCancel)
'    End Sub

'End Class



'Imports System.Windows.Forms

'Public Class ErrorSummaryDialog
'    Inherits Form

'    Private lst As ListBox
'    Private btnClose As Button
'    Private ReadOnly errors As List(Of CopyErrorEntry)

'    Public Sub New(errors As List(Of CopyErrorEntry))
'        Me.errors = errors
'        InitializeComponent()
'    End Sub

'    Private Sub InitializeComponent()
'        Me.Text = "Copy Summary"
'        Me.Size = New Size(500, 300)
'        Me.StartPosition = FormStartPosition.CenterParent
'        Me.FormBorderStyle = FormBorderStyle.FixedDialog
'        Me.MaximizeBox = False
'        Me.MinimizeBox = False

'        lst = New ListBox() With {
'            .Location = New Point(10, 10),
'            .Size = New Size(460, 220)
'        }

'        For Each Err In errors
'            lst.Items.Add($"{Err.FilePath} - {Err.Message}")
'        Next

'        btnClose = New Button() With {
'            .Text = "Close",
'            .Location = New Point(380, 240),
'            .Size = New Size(90, 25)
'        }
'        AddHandler btnClose.Click, Sub() Me.Close()

'        Me.Controls.Add(lst)
'        Me.Controls.Add(btnClose)
'    End Sub

'End Class




Imports System.Windows.Forms

Public Class ErrorSummaryDialog
    Inherits Form

    Private ReadOnly _errors As List(Of CopyErrorEntry)
    Private lst As ListBox
    Private btnClose As Button

    Public Sub New(errors As List(Of CopyErrorEntry))
        _errors = errors
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Copy Summary"
        Me.Size = New Size(500, 320)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        lst = New ListBox() With {
            .Location = New Point(10, 10),
            .Size = New Size(460, 220)
        }

        For Each entry In _errors
            lst.Items.Add($"{entry.FilePath} - {entry.Message}")
        Next

        btnClose = New Button() With {
            .Text = "Close",
            .Location = New Point(380, 240),
            .Size = New Size(90, 25)
        }
        AddHandler btnClose.Click, Sub() Me.Close()

        Me.Controls.Add(lst)
        Me.Controls.Add(btnClose)
    End Sub

End Class
