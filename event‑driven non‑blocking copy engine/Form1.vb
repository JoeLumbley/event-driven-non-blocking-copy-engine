
Imports System.Windows.Forms

Public Class Form1

    Private Sub btnBrowseSource_Click(sender As Object, e As EventArgs) Handles btnBrowseSource.Click
        Using f As New FolderBrowserDialog()

            If f.ShowDialog(Me) = DialogResult.OK Then
                txtSource.Text = f.SelectedPath
            End If
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

    Private Sub btnBrowseDest_Click(sender As Object, e As EventArgs) Handles btnBrowseDest.Click
        Using f As New FolderBrowserDialog()
            If f.ShowDialog(Me) = DialogResult.OK Then
                txtDest.Text = f.SelectedPath
            End If
        End Using
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

        ' Validate inputs
        If String.IsNullOrWhiteSpace(txtSource.Text) OrElse String.IsNullOrWhiteSpace(txtDest.Text) Then
            MessageBox.Show(Me, "Please select both source and destination folders.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' If the source is a file, ensure the destination is a folder
        If IO.File.Exists(txtSource.Text) AndAlso IO.Directory.Exists(txtDest.Text) Then
            ' Valid

            ' No special handling needed, CopyEngine will copy the file into the destination folder
        ElseIf IO.Directory.Exists(txtSource.Text) AndAlso IO.Directory.Exists(txtDest.Text) Then
            ' Valid







        End If



        Dim engine As New CopyEngine()
        engine.StartCopy(txtSource.Text, txtDest.Text)

        Using dlg As New CopyDialog(engine)
            dlg.ShowDialog(Me)
        End Using
    End Sub

End Class
