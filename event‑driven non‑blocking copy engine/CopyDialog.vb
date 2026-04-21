Imports System.Windows.Forms

Public Class CopyDialog
    Inherits Form

    Private lblTitle As Label
    Private progress As ProgressBar
    Private lblItems As Label
    Private lblBytes As Label
    Private lblSpeed As Label
    Private lblTime As Label
    Private lblCurrent As Label
    Private btnCancel As Button

    Private ReadOnly engine As CopyEngine

    Public Sub New(engine As CopyEngine)
        Me.engine = engine
        InitializeComponent()

        AddHandler engine.ProgressChanged, AddressOf OnProgressChanged
        AddHandler engine.Completed, AddressOf OnCompleted
        AddHandler engine.ErrorOccurred, AddressOf OnErrorOccurred
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Copying..."
        Me.Size = New Size(500, 250)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        lblTitle = New Label() With {
            .Text = "Copying files...",
            .Location = New Point(10, 10),
            .AutoSize = True
        }

        progress = New ProgressBar() With {
            .Location = New Point(10, 40),
            .Size = New Size(460, 20)
        }

        lblItems = New Label() With {
            .Location = New Point(10, 70),
            .Size = New Size(460, 20)
        }

        lblBytes = New Label() With {
            .Location = New Point(10, 90),
            .Size = New Size(460, 20)
        }

        lblSpeed = New Label() With {
            .Location = New Point(10, 110),
            .Size = New Size(460, 20)
        }

        lblTime = New Label() With {
            .Location = New Point(10, 130),
            .Size = New Size(460, 20)
        }

        lblCurrent = New Label() With {
            .Location = New Point(10, 150),
            .Size = New Size(460, 20)
        }

        btnCancel = New Button() With {
            .Text = "Cancel",
            .Location = New Point(380, 170),
            .Size = New Size(90, 25)
        }
        AddHandler btnCancel.Click, Sub()
                                        engine.Cancel()
                                        btnCancel.Enabled = False
                                    End Sub

        Me.Controls.Add(lblTitle)
        Me.Controls.Add(progress)
        Me.Controls.Add(lblItems)
        Me.Controls.Add(lblBytes)
        Me.Controls.Add(lblSpeed)
        Me.Controls.Add(lblTime)
        Me.Controls.Add(lblCurrent)
        Me.Controls.Add(btnCancel)
    End Sub

    Private Sub OnProgressChanged(info As CopyProgressInfo)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of CopyProgressInfo)(AddressOf OnProgressChanged), info)
            Return
        End If

        progress.Value = Math.Max(0, Math.Min(100, info.Percent))

        lblItems.Text = $"Items: {info.FilesDone} of {info.TotalFiles}"

        lblBytes.Text = $"Bytes: {FormatBytes(info.BytesCopied)} of {FormatBytes(info.TotalBytes)}"

        Dim mbps As Double = info.SpeedBytesPerSec / (1024.0 * 1024.0)
        lblSpeed.Text = $"Speed: {mbps:F1} MB/s"

        If info.Eta = TimeSpan.Zero Then
            lblTime.Text = "Time remaining: calculating..."
        Else
            lblTime.Text = $"Time remaining: {CInt(info.Eta.TotalSeconds)} seconds"
        End If

        lblCurrent.Text = $"Current: {System.IO.Path.GetFileName(info.CurrentFile)}"
    End Sub

    Private Function FormatBytes(value As Long) As String
        Dim dbl As Double = value
        Dim units() As String = {"B", "KB", "MB", "GB", "TB"}
        Dim idx As Integer = 0
        While dbl > 1024 AndAlso idx < units.Length - 1
            dbl /= 1024
            idx += 1
        End While
        Return $"{dbl:F1} {units(idx)}"
    End Function

    Private Sub OnCompleted(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of Boolean, Boolean, Boolean)(AddressOf OnCompleted), success, hadSkips, hadErrors)
            Return
        End If

        btnCancel.Enabled = False

        Dim msg As String
        If Not success Then
            msg = "Copy canceled."
        ElseIf hadErrors OrElse hadSkips Then
            msg = "Copy finished (some files were skipped or failed)."
        Else
            msg = "Copy finished successfully."
        End If

        MessageBox.Show(Me, msg, "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information)

        If hadErrors OrElse hadSkips Then
            Dim summary As New ErrorSummaryDialog(engine.ErrorList)
            summary.ShowDialog(Me)
        End If

        Me.Close()
    End Sub

    Private Sub OnErrorOccurred(file As String, ex As Exception)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String, Exception)(AddressOf OnErrorOccurred), file, ex)
            Return
        End If

        Using dlg As New ErrorDialog(file, ex)
            Dim result = dlg.ShowDialog(Me)

            Select Case result
                Case DialogResult.Ignore
                    engine.SetErrorAction(CopyErrorAction.Skip)
                Case DialogResult.Yes
                    engine.SetErrorAction(CopyErrorAction.SkipAll)
                Case DialogResult.Cancel
                    engine.SetErrorAction(CopyErrorAction.Cancel)
            End Select
        End Using
    End Sub

End Class
