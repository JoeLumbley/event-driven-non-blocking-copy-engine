'' Event-Driven Non-Blocking Copy Engine is a Windows Forms application that
'' allows users to copy files and folders with a responsive UI. It includes
'' validation to prevent copying protected system files and provides overwrite
'' warnings. The copy operation is performed asynchronously to keep the UI
'' responsive.

'' MIT License
'' Copyright(c) 2026 Joseph W. Lumbley

'' Permission is hereby granted, free of charge, to any person obtaining a copy
'' of this software and associated documentation files (the "Software"), to deal
'' in the Software without restriction, including without limitation the rights
'' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'' copies of the Software, and to permit persons to whom the Software is
'' furnished to do so, subject to the following conditions:

'' The above copyright notice and this permission notice shall be included in all
'' copies or substantial portions of the Software.

'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'' SOFTWARE.


'Imports System.IO

'Public Class Form1

'    Private Shared ReadOnly ProtectedRoots As String() = {
'    "c:\windows",
'    "c:\windows\system32",
'    "c:\program files",
'    "c:\program files (x86)",
'    "c:\programdata",
'    "c:\system volume information",
'    "c:\$recycle.bin",
'    "c:\perflogs",
'    "c:\system.sav",
'    "c:\recovery",
'    "c:\documents and settings",
'    "c:\msocache",
'    "c:\$winreagent",
'    "c:\$windows.~bt",
'    "c:\$windows.~ws",
'    "c:\onedrivetemp",
'    "c:\dumpstack.log.tmp",' Kernel-locked system files
'    "c:\hiberfil.sys",
'    "c:\pagefile.sys",
'    "c:\swapfile.sys",
'    "c:\users\all users",' Legacy junctions (XP-era compatibility)
'    "c:\users\default user",
'    "c:\users\default\appdata\local\application data",
'    "c:\users\default\appdata\local\history",
'    "c:\users\default\appdata\local\temporary internet files"
'}

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

'        Dim src As String = txtSource.Text.Trim()
'        Dim dst As String = txtDest.Text.Trim()

'        '===============================
'        ' VALIDATION
'        '===============================

'        ' 1. Must exist
'        If Not File.Exists(src) AndAlso Not Directory.Exists(src) Then
'            ShowValidationError("The source path does not exist.")
'            Return
'        End If

'        If Not Directory.Exists(dst) Then
'            ShowValidationError("The destination folder does not exist.")
'            Return
'        End If

'        ' 2. Protected source path
'        If IsProtectedPath(src) Then
'            ShowValidationError("This folder or file is protected by Windows and cannot be copied." &
'                                vbCrLf & src)
'            Return
'        End If

'        ' 3. Cannot copy folder into itself or subfolder
'        If Directory.Exists(src) Then
'            Dim s = src.TrimEnd("\"c).ToLower()
'            Dim d = dst.TrimEnd("\"c).ToLower()

'            If d = s OrElse d.StartsWith(s & "\") Then
'                ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
'                Return
'            End If
'        End If

'        ' 4. If source is a file, destination must be a folder
'        If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
'            ShowValidationError("When copying a file, the destination must be a folder.")
'            Return
'        End If

'        '===============================
'        ' OVERWRITE WARNINGS
'        '===============================

'        ' 5. File-level overwrite warning
'        If File.Exists(src) Then
'            Dim fileName As String = Path.GetFileName(src)
'            Dim destFile As String = Path.Combine(dst, fileName)

'            If File.Exists(destFile) Then
'                Dim msg As String =
'                "The file '" & fileName & "' already exists in the destination folder." & vbCrLf &
'                "Do you want to overwrite it?"

'                Dim result = MessageBox.Show(Me, msg, "Confirm File Replace",
'                                         MessageBoxButtons.YesNo,
'                                         MessageBoxIcon.Warning)

'                If result = DialogResult.No Then
'                    Return
'                End If
'            End If
'        End If

'        ' 6. Folder-level overwrite warning (custom dialog)
'        If Directory.Exists(src) Then
'            Dim folderName As String = Path.GetFileName(src.TrimEnd("\"c))
'            Dim destFolder As String = Path.Combine(dst, folderName)

'            If Directory.Exists(destFolder) Then
'                Using dlg As New FolderMergeDialog(folderName)
'                    Dim result = dlg.ShowDialog(Me)
'                    If result = DialogResult.No Then
'                        Return
'                    End If
'                End Using
'            End If
'        End If

'        '===============================
'        ' VALIDATION PASSED
'        '===============================

'        Dim engine As New CopyEngine()
'        engine.StartCopy(src, dst)

'        Using dlg As New CopyDialog(engine)
'            dlg.ShowDialog(Me)
'        End Using
'    End Sub




'    '' Synchronous, blocking example inside a button click handler
'    'Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
'    '    Dim src = txtSource.Text.Trim()
'    '    Dim dst = txtDest.Text.Trim()

'    '    ' Quick validation omitted for brevity

'    '    If File.Exists(src) Then
'    '        ' This call blocks the UI until the copy finishes
'    '        File.Copy(src, Path.Combine(dst, Path.GetFileName(src)), True)
'    '    ElseIf Directory.Exists(src) Then
'    '        ' Recursive synchronous copy — also blocks the UI
'    '        DirectoryCopy(src, Path.Combine(dst, Path.GetFileName(src)))
'    '    End If

'    '    MessageBox.Show("Copy finished") ' UI was frozen until this point
'    'End Sub

'    'Private Sub DirectoryCopy(sourceDir As String, destDir As String)
'    '    Directory.CreateDirectory(destDir)
'    '    For Each file In Directory.GetFiles(sourceDir)
'    '        IO.File.Copy(file, IO.Path.Combine(destDir, IO.Path.GetFileName(file)), True)
'    '    Next
'    '    For Each Dir2Copy In Directory.GetDirectories(sourceDir)
'    '        DirectoryCopy(Dir2Copy, Path.Combine(destDir, Path.GetFileName(Dir2Copy)))
'    '    Next
'    'End Sub



'    '===============================
'    ' WINDOWS 7 STYLE ERROR DIALOG
'    '===============================
'    Private Sub ShowValidationError(message As String)
'        Using dlg As New ValidationErrorDialog(message)
'            dlg.ShowDialog(Me)
'        End Using
'    End Sub

'    Public Shared Function IsProtectedPath(path As String) As Boolean
'        If String.IsNullOrWhiteSpace(path) Then Return False

'        Dim normalized As String = path.Trim().TrimEnd("\"c).ToLowerInvariant()

'        ' Drive root protection (C:\)
'        If normalized.Length = 2 AndAlso normalized(1) = ":"c Then
'            Return True
'        End If

'        For Each root In ProtectedRoots
'            Dim r = root.TrimEnd("\"c).ToLowerInvariant()

'            ' Exact match
'            If normalized = r Then
'                Return True
'            End If

'            ' Subdirectory match
'            If normalized.StartsWith(r & "\") Then
'                Return True
'            End If
'        Next

'        Return False
'    End Function

'End Class



Imports System.IO

' Form1 is the main window where the user chooses a source and destination,
' runs validation, and then starts the non-blocking copy engine.
Public Class Form1

    ' Central copy engine instance so we can cancel or inspect it later.
    Private engine As CopyEngine

    ' List of protected system roots we never allow as a source.
    ' These are stored in lowercase without trailing backslashes for easier comparison.
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

    '===============================
    '  BROWSE BUTTONS
    '===============================

    ' Lets the user pick a folder as the source.
    Private Sub btnBrowseSource_Click(sender As Object, e As EventArgs) Handles btnBrowseSource.Click
        Using f As New FolderBrowserDialog()
            If f.ShowDialog(Me) = DialogResult.OK Then
                txtSource.Text = f.SelectedPath
            End If
        End Using
    End Sub

    ' Lets the user pick a single file as the source.
    Private Sub btnBrowseFile_Click(sender As Object, e As EventArgs) Handles btnBrowseFile.Click
        Using f As New OpenFileDialog()
            f.Title = "Select a file to copy"
            f.Filter = "All Files (*.*)|*.*"

            If f.ShowDialog(Me) = DialogResult.OK Then
                txtSource.Text = f.FileName
            End If
        End Using
    End Sub

    ' Lets the user pick the destination folder.
    Private Sub btnBrowseDest_Click(sender As Object, e As EventArgs) Handles btnBrowseDest.Click
        Using f As New FolderBrowserDialog()
            If f.ShowDialog(Me) = DialogResult.OK Then
                txtDest.Text = f.SelectedPath
            End If
        End Using
    End Sub

    '===============================
    '  START COPY (MAIN ENTRY POINT)
    '===============================
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

        ' Read and trim user input.
        Dim src As String = txtSource.Text.Trim()
        Dim dst As String = txtDest.Text.Trim()

        ' Quick sanity checks for empty fields.
        If String.IsNullOrWhiteSpace(src) Then
            ShowValidationError("Please select a source file or folder.")
            Return
        End If

        If String.IsNullOrWhiteSpace(dst) Then
            ShowValidationError("Please select a destination folder.")
            Return
        End If

        '===============================
        '  VALIDATION
        '===============================

        ' 1. Source must exist (either file or folder).
        If Not File.Exists(src) AndAlso Not Directory.Exists(src) Then
            ShowValidationError("The source path does not exist." & vbCrLf & src)
            Return
        End If

        ' 2. Destination must be an existing folder.
        If Not Directory.Exists(dst) Then
            ShowValidationError("The destination folder does not exist." & vbCrLf & dst)
            Return
        End If

        ' 3. Source must not be a protected system path.
        If IsProtectedPath(src) Then
            ShowValidationError("This folder or file is protected by Windows and cannot be copied." &
                                vbCrLf & src)
            Return
        End If

        ' 4. Prevent copying a folder into itself or one of its subfolders.
        If Directory.Exists(src) Then
            Dim s = src.TrimEnd("\"c).ToLowerInvariant()
            Dim d = dst.TrimEnd("\"c).ToLowerInvariant()

            ' Example: copying C:\Data into C:\Data or C:\Data\Subfolder is not allowed.
            If d = s OrElse d.StartsWith(s & "\") Then
                ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
                Return
            End If
        End If

        ' 5. When copying a file, destination must be a folder (Explorer-style behavior).
        If File.Exists(src) AndAlso Not Directory.Exists(dst) Then
            ShowValidationError("When copying a file, the destination must be a folder.")
            Return
        End If

        ' 6. Prevent file/folder name collisions where types differ (file vs folder).
        '    Explorer does not allow copying a file over a folder or a folder over a file.
        Dim srcName As String = Path.GetFileName(src.TrimEnd("\"c))
        Dim destChildPath As String = Path.Combine(dst, srcName)

        If File.Exists(src) AndAlso Directory.Exists(destChildPath) Then
            ShowValidationError("A folder with the same name already exists in the destination." &
                                vbCrLf & destChildPath)
            Return
        End If

        If Directory.Exists(src) AndAlso File.Exists(destChildPath) Then
            ShowValidationError("A file with the same name already exists in the destination." &
                                vbCrLf & destChildPath)
            Return
        End If

        '===============================
        '  OVERWRITE / MERGE PROMPTS
        '===============================

        ' 7. File-level overwrite warning (single file copy).
        If File.Exists(src) Then
            Dim fileName As String = Path.GetFileName(src)
            Dim destFile As String = Path.Combine(dst, fileName)

            If File.Exists(destFile) Then
                Dim msg As String =
                    "The file '" & fileName &
                    "' already exists in the destination folder." & vbCrLf &
                    "Do you want to overwrite it?"

                Dim result = MessageBox.Show(Me,
                                             msg,
                                             "Confirm File Replace",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning)

                If result = DialogResult.No Then
                    ' User chose not to overwrite, so we stop here.
                    Return
                End If
            End If
        End If

        ' 8. Folder-level merge warning (folder copy).
        '    This mimics the Windows 7 "Merge folders" dialog.
        If Directory.Exists(src) Then
            Dim folderName As String = Path.GetFileName(src.TrimEnd("\"c))
            Dim destFolder As String = Path.Combine(dst, folderName)

            If Directory.Exists(destFolder) Then
                Using fMDlg As New FolderMergeDialog(folderName)
                    Dim result = fMDlg.ShowDialog(Me)
                    If result = DialogResult.No Then
                        ' User chose not to merge folders.
                        Return
                    End If
                End Using
            End If
        End If

        '===============================
        '  VALIDATION PASSED
        '===============================

        ' At this point:
        ' - The source and destination are valid.
        ' - The user has confirmed any overwrites/merges.
        ' - We can safely start the non-blocking copy engine.

        ' Create the engine and keep a reference so we can cancel later.
        engine = New CopyEngine()

        ' Start the copy on a background thread.
        engine.StartCopy(src, dst)

        ' Show a modeless progress dialog that listens to engine events.
        ' This keeps the main form responsive, similar to Explorer.
        Dim dlg As New CopyDialog(engine)
        dlg.Show(Me)

    End Sub

    '===============================
    '  WINDOWS 7 STYLE ERROR DIALOG
    '===============================
    ' Shows a friendly, Explorer-style validation error dialog.
    Private Sub ShowValidationError(message As String)
        Using dlg As New ValidationErrorDialog(message)
            dlg.ShowDialog(Me)
        End Using
    End Sub

    '===============================
    '  PROTECTED PATH CHECK
    '===============================
    ' Returns True if the given path is a protected system location
    ' (for example C:\Windows or C:\pagefile.sys) that should never be copied.
    Public Shared Function IsProtectedPath(path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then Return False

        ' Normalize: trim spaces, remove trailing backslashes, lowercase.
        Dim normalized As String = path.Trim().TrimEnd("\"c).ToLowerInvariant()

        ' Block drive roots like "c:" or "d:"
        ' (Explorer does not let you copy the whole drive this way).
        If normalized Like "[a-z]:" Then
            Return True
        End If

        ' Compare against our protected roots list.
        For Each root In ProtectedRoots
            Dim r = root.TrimEnd("\"c).ToLowerInvariant()

            ' Exact match (e.g., "c:\windows").
            If normalized = r Then
                Return True
            End If

            ' Subdirectory match (e.g., "c:\windows\system32\drivers").
            If normalized.StartsWith(r & "\") Then
                Return True
            End If
        Next

        Return False
    End Function

    '===============================
    '  CANCEL BUTTON (OPTIONAL)
    '===============================
    ' If you have a Cancel button on the main form, you can wire it like this:
    'Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
    '    ' Cancel cooperatively: the engine checks for cancellation between file operations.
    '    engine?.Cancel()
    'End Sub

End Class
