
'Imports System.Windows.Forms

'Public Class Form1

'    Private Sub btnBrowseSource_Click(sender As Object, e As EventArgs) Handles btnBrowseSource.Click
'        Using f As New FolderBrowserDialog()

'            If f.ShowDialog(Me) = DialogResult.OK Then
'                txtSource.Text = f.SelectedPath
'            End If
'        End Using
'    End Sub
'    Private Sub btnBrowseFile_Click(sender As Object, e As EventArgs) Handles btnBrowseFile.Click
'        Using f As New OpenFileDialog()
'            f.Title = "Select a file to copy"
'            f.Filter = "All Files (*.*)|*.*"

'            If f.ShowDialog(Me) = DialogResult.OK Then
'                txtSource.Text = f.FileName
'            End If
'        End Using
'    End Sub

'    Private Sub btnBrowseDest_Click(sender As Object, e As EventArgs) Handles btnBrowseDest.Click
'        Using f As New FolderBrowserDialog()
'            If f.ShowDialog(Me) = DialogResult.OK Then
'                txtDest.Text = f.SelectedPath
'            End If
'        End Using
'    End Sub

'    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

'        ' Validate inputs
'        If String.IsNullOrWhiteSpace(txtSource.Text) OrElse String.IsNullOrWhiteSpace(txtDest.Text) Then
'            MessageBox.Show(Me, "Please select both source and destination folders.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
'            Return
'        End If

'        ' If the source is a file, ensure the destination is a folder
'        If IO.File.Exists(txtSource.Text) AndAlso IO.Directory.Exists(txtDest.Text) Then
'            ' Valid

'            ' No special handling needed, CopyEngine will copy the file into the destination folder
'        ElseIf IO.Directory.Exists(txtSource.Text) AndAlso IO.Directory.Exists(txtDest.Text) Then
'            ' Valid

'        End If


'        Dim engine As New CopyEngine()
'        engine.StartCopy(txtSource.Text, txtDest.Text)

'        Using dlg As New CopyDialog(engine)
'            dlg.ShowDialog(Me)
'        End Using
'    End Sub

'End Class


Imports System.Windows.Forms
Imports System.IO

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

        Dim src As String = txtSource.Text.Trim()
        Dim dst As String = txtDest.Text.Trim()

        '===============================
        ' VALIDATION
        '===============================

        ' 1. Must exist
        If Not File.Exists(src) AndAlso Not Directory.Exists(src) Then
            ShowValidationError("The source path does not exist.")
            Return
        End If

        If Not Directory.Exists(dst) Then
            ShowValidationError("The destination folder does not exist.")
            Return
        End If

        ' 2. Cannot copy folder into itself or subfolder
        If Directory.Exists(src) Then
            Dim s = src.TrimEnd("\"c).ToLower()
            Dim d = dst.TrimEnd("\"c).ToLower()

            If d = s OrElse d.StartsWith(s & "\") Then
                ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
                Return
            End If
        End If

        ' 3. Protected system roots
        Dim protectedRoots As String() = {
            "c:\windows",
            "c:\windows\system32",
            "c:\program files",
            "c:\program files (x86)",
            "c:\programdata",
            "c:\system volume information",
            "c:\$recycle.bin",
            "c:\perflogs",
            "c:\system.sav",
            "c:\recovery",
            "c:\documents and settings",
            "c:\msocache",
            "c:\$winreagent",
            "c:\$windows.~bt",
            "c:\$windows.~ws",
            "c:\onedrivetemp",
            "c:\users\all users",
            "c:\users\default user",
            "c:\users\default\appdata\local\application data",
            "c:\users\default\appdata\local\history",
            "c:\users\default\appdata\local\temporary internet files"
        }



        Dim srcLower = src.ToLower()

        For Each p In protectedRoots
            If srcLower = p OrElse srcLower.StartsWith(p & "\") Then
                ShowValidationError("This folder is protected by Windows and cannot be copied:" & vbCrLf & p)
                Return
            End If
        Next

        ' 4. Cannot copy drive root
        If srcLower Like "?:\" Then
            ShowValidationError("Copying an entire drive root is not supported.")
            Return
        End If

        ' 5. If source is a file, destination must be a folder
        If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
            ShowValidationError("When copying a file, the destination must be a folder.")
            Return
        End If

        '===============================
        ' VALIDATION PASSED
        '===============================

        Dim engine As New CopyEngine()
        engine.StartCopy(src, dst)

        Using dlg As New CopyDialog(engine)
            dlg.ShowDialog(Me)
        End Using
    End Sub

    '===============================
    ' WINDOWS 7 STYLE ERROR DIALOG
    '===============================
    Private Sub ShowValidationError(message As String)
        Using dlg As New ValidationErrorDialog(message)
            dlg.ShowDialog(Me)
        End Using
    End Sub

End Class
