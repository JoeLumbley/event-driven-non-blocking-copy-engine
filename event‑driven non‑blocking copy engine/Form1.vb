
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

    Private Shared ReadOnly ProtectedRoots As String() = {
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
    "c:\dumpstack.log.tmp",' Kernel-locked system files
    "c:\hiberfil.sys",
    "c:\pagefile.sys",
    "c:\swapfile.sys",
    "c:\users\all users",' Legacy junctions (XP-era compatibility)
    "c:\users\default user",
    "c:\users\default\appdata\local\application data",
    "c:\users\default\appdata\local\history",
    "c:\users\default\appdata\local\temporary internet files"
}


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

    'Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

    '    Dim src As String = txtSource.Text.Trim()
    '    Dim dst As String = txtDest.Text.Trim()

    '    '===============================
    '    ' VALIDATION
    '    '===============================

    '    ' 1. Must exist
    '    If Not File.Exists(src) AndAlso Not Directory.Exists(src) Then
    '        ShowValidationError("The source path does not exist.")
    '        Return
    '    End If

    '    If Not Directory.Exists(dst) Then
    '        ShowValidationError("The destination folder does not exist.")
    '        Return
    '    End If

    '    ' 2. Cannot copy folder into itself or subfolder
    '    If Directory.Exists(src) Then
    '        Dim s = src.TrimEnd("\"c).ToLower()
    '        Dim d = dst.TrimEnd("\"c).ToLower()

    '        If d = s OrElse d.StartsWith(s & "\") Then
    '            ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
    '            Return
    '        End If
    '    End If

    '    ' 3. Protected system roots (you can add more if you want)
    '    ' This is to prevent users from accidentally doing something really bad
    '    ' like copying their entire Windows folder somewhere else and screwing
    '    ' up their system
    '    'Dim protectedRoots As String() = {
    '    '    "c:\windows",
    '    '    "c:\windows\system32",
    '    '    "c:\program files",
    '    '    "c:\program files (x86)",
    '    '    "c:\programdata",
    '    '    "c:\system volume information",
    '    '    "c:\$recycle.bin",
    '    '    "c:\perflogs",
    '    '    "c:\system.sav",
    '    '    "c:\recovery",
    '    '    "c:\documents and settings",
    '    '    "c:\msocache",
    '    '    "c:\$winreagent",
    '    '    "c:\$windows.~bt",
    '    '    "c:\$windows.~ws",
    '    '    "c:\onedrivetemp",
    '    '    "c:\users\all users",
    '    '    "c:\users\default user",
    '    '    "c:\users\default\appdata\local\application data",
    '    '    "c:\users\default\appdata\local\history",
    '    '    "c:\users\default\appdata\local\temporary internet files",
    '    '    "c:\users\default\appdata\local\virtualstore",
    '    '    "c:\users\public\desktop",
    '    '    "c:\users\public\documents",
    '    '    "c:\users\public\downloads",
    '    '    "c:\users\public\music",
    '    '    "c:\users\public\pictures",
    '    '    "c:\users\public\videos",
    '    '    "c:\users\public\appdata",
    '    '    "c:\users\public\appdata\local"
    '    '}

    '    Dim srcLower = src.ToLower()

    '    For Each p In ProtectedRoots
    '        If srcLower = p OrElse srcLower.StartsWith(p & "\") Then
    '            ShowValidationError("This folder is protected by Windows and cannot be copied:" & vbCrLf & p)
    '            Return
    '        End If
    '    Next

    '    ' 4. Cannot copy drive root
    '    If srcLower Like "?:\" Then
    '        ShowValidationError("Copying an entire drive root is not supported.")
    '        Return
    '    End If

    '    ' 5. If source is a file, destination must be a folder
    '    If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
    '        ShowValidationError("When copying a file, the destination must be a folder.")
    '        Return
    '    End If

    '    '===============================
    '    ' VALIDATION PASSED
    '    '===============================

    '    Dim engine As New CopyEngine()
    '    engine.StartCopy(src, dst)

    '    Using dlg As New CopyDialog(engine)
    '        dlg.ShowDialog(Me)
    '    End Using
    'End Sub



    'Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

    '    Dim src As String = txtSource.Text.Trim()
    '    Dim dst As String = txtDest.Text.Trim()

    '    '===============================
    '    ' VALIDATION
    '    '===============================

    '    ' 1. Must exist
    '    If Not File.Exists(src) AndAlso Not Directory.Exists(src) Then
    '        ShowValidationError("The source path does not exist.")
    '        Return
    '    End If

    '    If Not Directory.Exists(dst) Then
    '        ShowValidationError("The destination folder does not exist.")
    '        Return
    '    End If

    '    ' 2. Protected source path
    '    If IsProtectedPath(src) Then
    '        ShowValidationError("This folder or file is protected by Windows and cannot be copied." &
    '                        vbCrLf & src)
    '        Return
    '    End If

    '    ' 3. Cannot copy folder into itself or subfolder
    '    If Directory.Exists(src) Then
    '        Dim s = src.TrimEnd("\"c).ToLower() ' Normalize source path for comparison
    '        Dim d = dst.TrimEnd("\"c).ToLower() ' Normalize destination path for comparison

    '        If d = s OrElse d.StartsWith(s & "\") Then
    '            ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
    '            Return
    '        End If
    '    End If

    '    ' 4. If source is a file, destination must be a folder
    '    If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
    '        ShowValidationError("When copying a file, the destination must be a folder.")
    '        Return
    '    End If

    '    ' 5. Overwrite warning (single file copy)
    '    If File.Exists(src) Then
    '        Dim fileName As String = Path.GetFileName(src)
    '        Dim destFile As String = Path.Combine(dst, fileName)

    '        If File.Exists(destFile) Then
    '            Dim msg As String =
    '        "The file '" & fileName & "' already exists in the destination folder." & vbCrLf &
    '        "Do you want to overwrite it?"

    '            Dim result = MessageBox.Show(Me, msg, "Confirm File Replace",
    '                                 MessageBoxButtons.YesNo,
    '                                 MessageBoxIcon.Warning)

    '            If result = DialogResult.No Then
    '                Return
    '            End If
    '        End If
    '    End If


    '    '===============================
    '    ' VALIDATION PASSED
    '    '===============================

    '    Dim engine As New CopyEngine()
    '    engine.StartCopy(src, dst)

    '    Using dlg As New CopyDialog(engine)
    '        dlg.ShowDialog(Me)
    '    End Using
    'End Sub




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

        ' 2. Protected source path
        If IsProtectedPath(src) Then
            ShowValidationError("This folder or file is protected by Windows and cannot be copied." &
                                vbCrLf & src)
            Return
        End If

        ' 3. Cannot copy folder into itself or subfolder
        If Directory.Exists(src) Then
            Dim s = src.TrimEnd("\"c).ToLower()
            Dim d = dst.TrimEnd("\"c).ToLower()

            If d = s OrElse d.StartsWith(s & "\") Then
                ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
                Return
            End If
        End If

        ' 4. If source is a file, destination must be a folder
        If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
            ShowValidationError("When copying a file, the destination must be a folder.")
            Return
        End If

        '===============================
        ' OVERWRITE WARNINGS
        '===============================

        ' 5. File-level overwrite warning
        If File.Exists(src) Then
            Dim fileName As String = Path.GetFileName(src)
            Dim destFile As String = Path.Combine(dst, fileName)

            If File.Exists(destFile) Then
                Dim msg As String =
                "The file '" & fileName & "' already exists in the destination folder." & vbCrLf &
                "Do you want to overwrite it?"

                Dim result = MessageBox.Show(Me, msg, "Confirm File Replace",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning)

                If result = DialogResult.No Then
                    Return
                End If
            End If
        End If

        ' 6. Folder-level overwrite warning
        'If Directory.Exists(src) Then
        '    Dim folderName As String = Path.GetFileName(src.TrimEnd("\"c))
        '    Dim destFolder As String = Path.Combine(dst, folderName)

        '    If Directory.Exists(destFolder) Then
        '        Dim msg As String =
        '        "The destination already contains a folder named '" & folderName & "'." & vbCrLf &
        '        "Do you want to merge these folders?"

        '        Dim result = MessageBox.Show(Me, msg, "Confirm Folder Merge",
        '                                 MessageBoxButtons.YesNo,
        '                                 MessageBoxIcon.Warning)

        '        If result = DialogResult.No Then
        '            Return
        '        End If
        '    End If
        'End If


        ' 6. Folder-level overwrite warning (custom dialog)
        If Directory.Exists(src) Then
            Dim folderName As String = Path.GetFileName(src.TrimEnd("\"c))
            Dim destFolder As String = Path.Combine(dst, folderName)

            If Directory.Exists(destFolder) Then
                Using dlg As New FolderMergeDialog(folderName)
                    Dim result = dlg.ShowDialog(Me)
                    If result = DialogResult.No Then
                        Return
                    End If
                End Using
            End If
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


    Public Shared Function IsProtectedPath(path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then Return False

        Dim normalized As String = path.Trim().TrimEnd("\"c).ToLowerInvariant()

        ' Drive root protection (C:\)
        If normalized.Length = 2 AndAlso normalized(1) = ":"c Then
            Return True
        End If

        For Each root In ProtectedRoots
            Dim r = root.TrimEnd("\"c).ToLowerInvariant()

            ' Exact match
            If normalized = r Then
                Return True
            End If

            ' Subdirectory match
            If normalized.StartsWith(r & "\") Then
                Return True
            End If
        Next

        Return False
    End Function


End Class
