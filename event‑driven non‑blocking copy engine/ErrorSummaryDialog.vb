
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
