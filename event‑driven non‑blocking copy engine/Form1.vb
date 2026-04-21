
'Public Class Form1
'    Private engine As New CopyEngine()

'    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        AddHandler engine.ProgressChanged, AddressOf OnCopyProgress
'        AddHandler engine.Completed, AddressOf OnCopyCompleted
'    End Sub

'    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
'        'engine.StartCopy("C:\Users\j_lum\Music\Test Main", "C:\Users\j_lum\Test")
'        engine.StartCopy(txtSourceFolder.Text, txtDestinationFolder.Text)

'    End Sub

'    Private Sub OnCopyProgress(percent As Integer, file As String)
'        ProgressBar1.Value = percent
'        lblStatus.Text = $"Copying: {IO.Path.GetFileName(file)}"
'    End Sub

'    'Private Sub OnCopyCompleted(success As Boolean, message As String)
'    '    lblStatus.Text = message
'    'End Sub

'    Private Sub OnCopyCompleted(success As Boolean, message As String)
'        lblStatus.Text = message
'        ProgressBar1.Value = If(success, 100, 0)
'    End Sub









'    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
'        engine.Cancel()
'        lblStatus.Text = "Canceling..."
'    End Sub
















'End Class


'Public Class Form1

'    Private engine As New CopyEngine()

'    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        AddHandler engine.ProgressChanged, AddressOf OnCopyProgress
'        AddHandler engine.Completed, AddressOf OnCopyCompleted
'        AddHandler engine.ErrorOccurred, AddressOf OnCopyError
'    End Sub

'    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
'        engine.StartCopy(txtSourceFolder.Text, txtDestinationFolder.Text)
'        lblStatus.Text = "Copying..."
'    End Sub

'    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
'        engine.Cancel()
'        lblStatus.Text = "Canceling..."
'    End Sub

'    Private Sub OnCopyProgress(percent As Integer, file As String)
'        ProgressBar1.Value = percent
'        lblStatus.Text = $"Copying: {IO.Path.GetFileName(file)}"
'    End Sub

'    Private Sub OnCopyCompleted(success As Boolean, message As String)
'        lblStatus.Text = message
'        If success Then
'            ProgressBar1.Value = 100
'        End If
'    End Sub

'    Private Sub OnCopyError(file As String, ex As Exception)
'        Using dlg As New ErrorDialog(file, ex)
'            Dim result = dlg.ShowDialog()

'            Select Case result
'                Case DialogResult.Ignore
'                    engine.SetErrorAction(CopyErrorAction.Skip)

'                Case DialogResult.Yes ' Skip All
'                    engine.SetErrorAction(CopyErrorAction.SkipAll)

'                Case DialogResult.Cancel
'                    engine.SetErrorAction(CopyErrorAction.Cancel)
'            End Select
'        End Using
'    End Sub

'End Class




Imports System.Windows.Forms

Public Class Form1

    Private Sub btnBrowseSource_Click(sender As Object, e As EventArgs) Handles btnBrowseSource.Click
        Using f As New FolderBrowserDialog() With {.Site = Me.Site}

            If f.ShowDialog(Me) = DialogResult.OK Then
                txtSource.Text = f.SelectedPath
            End If
        End Using
    End Sub

    Private Sub btnBrowseDest_Click(sender As Object, e As EventArgs) Handles btnBrowseDest.Click
        Using f As New FolderBrowserDialog()
            If f.ShowDialog(Me) = DialogResult.OK Then
                txtDest.Text = f.SelectedPath
            End If
        End Using
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        If String.IsNullOrWhiteSpace(txtSource.Text) OrElse String.IsNullOrWhiteSpace(txtDest.Text) Then
            MessageBox.Show(Me, "Please select both source and destination folders.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim engine As New CopyEngine()
        engine.StartCopy(txtSource.Text, txtDest.Text)

        Using dlg As New CopyDialog(engine)
            dlg.ShowDialog(Me)
        End Using
    End Sub







    Private Sub btnBrowseFile_Click(sender As Object, e As EventArgs) Handles btnBrowseFile.Click
        Using f As New OpenFileDialog()
            f.Title = "Select a file to copy"
            f.Filter = "All Files (*.*)|*.*"

            If f.ShowDialog(Me) = DialogResult.OK Then
                txtSource.Text = f.FileName
            End If
        End Using
    End Sub






End Class
