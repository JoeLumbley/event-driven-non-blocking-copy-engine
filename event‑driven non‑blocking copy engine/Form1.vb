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
        Dim sourceDirectory As String = txtSource.Text.Trim()
        Dim destinationDirectory As String = txtDest.Text.Trim()

        ' Quick sanity checks for empty fields.
        If String.IsNullOrWhiteSpace(sourceDirectory) Then
            ShowValidationError("Please select a source file or folder.")
            Return
        End If

        If String.IsNullOrWhiteSpace(destinationDirectory) Then
            ShowValidationError("Please select a destination folder.")
            Return
        End If

        '===============================
        '  VALIDATION
        '===============================

        ' 1. Source must exist (either file or folder).
        If Not File.Exists(sourceDirectory) AndAlso Not Directory.Exists(sourceDirectory) Then
            ShowValidationError("The source path does not exist." & Environment.NewLine & sourceDirectory)
            Return
        End If

        ' 2. Destination must be an existing folder.
        If Not Directory.Exists(destinationDirectory) Then
            ShowValidationError("The destination folder does not exist." & Environment.NewLine & destinationDirectory)
            Return
        End If

        ' 3. Source must not be a protected system path.
        If IsProtectedPath(sourceDirectory) Then
            ShowValidationError("This folder or file is protected by Windows and cannot be copied." &
                                vbCrLf & sourceDirectory)
            Return
        End If

        ' 4. Prevent copying a folder into itself or one of its subfolders.
        If Directory.Exists(sourceDirectory) Then
            Dim s = sourceDirectory.TrimEnd("\"c).ToLowerInvariant()
            Dim d = destinationDirectory.TrimEnd("\"c).ToLowerInvariant()

            ' Example: copying C:\Data into C:\Data or C:\Data\Subfolder is not allowed.
            If d = s OrElse d.StartsWith(s & "\") Then
                ShowValidationError("You cannot copy a folder into itself or one of its subfolders.")
                Return
            End If
        End If

        ' 5. When copying a file, destination must be a folder (Explorer-style behavior).
        If File.Exists(sourceDirectory) AndAlso Not Directory.Exists(destinationDirectory) Then
            ShowValidationError("When copying a file, the destination must be a folder.")
            Return
        End If

        ' 6. Prevent file/folder name collisions where types differ (file vs folder).
        '    Explorer does not allow copying a file over a folder or a folder over a file.
        Dim sourceName As String = Path.GetFileName(sourceDirectory.TrimEnd("\"c))
        Dim destinationChildPath As String = Path.Combine(destinationDirectory, sourceName)

        If File.Exists(sourceDirectory) AndAlso Directory.Exists(destinationChildPath) Then

            Dim errorMsg As String =
                "A folder with the same name already exists in the destination." & Environment.NewLine &
                destinationChildPath & Environment.NewLine &
                "You cannot copy a file over a folder. Please choose a different destination or rename the source file."

            ShowValidationError(errorMsg)

            Return

        End If

        If Directory.Exists(sourceDirectory) AndAlso File.Exists(destinationChildPath) Then

            Dim errorMsg As String =
                "A file with the same name already exists in the destination." & Environment.NewLine &
                destinationChildPath & Environment.NewLine &
                "You cannot copy a folder over a file. Please choose a different destination or rename the source folder."

            ShowValidationError(errorMsg)

            Return

        End If

        '===============================
        '  OVERWRITE / MERGE PROMPTS
        '===============================

        ' 7. File-level overwrite warning (single file copy).
        If File.Exists(sourceDirectory) Then
            Dim fileName As String = Path.GetFileName(sourceDirectory)
            Dim destinationFile As String = Path.Combine(destinationDirectory, fileName)

            If File.Exists(destinationFile) Then
                Dim errorMsg As String =
                    "The file '" & fileName &
                    "' already exists in the destination folder." & Environment.NewLine &
                    "Do you want to overwrite it?"

                Dim result = MessageBox.Show(Me,
                                             errorMsg,
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
        If Directory.Exists(sourceDirectory) Then
            Dim folderName As String = Path.GetFileName(sourceDirectory.TrimEnd("\"c))
            Dim destinationFolder As String = Path.Combine(destinationDirectory, folderName)

            If Directory.Exists(destinationFolder) Then
                Using folderMergeDialog As New FolderMergeDialog(folderName)
                    Dim result = folderMergeDialog.ShowDialog(Me)
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

        ' Disable the Start button and show the Cancel button.
        btnStart.Enabled = False
        btnCancel.Visible = True

        ' Create the engine and keep a reference so we can cancel later.
        engine = New CopyEngine()

        ' Correct event subscription — must happen AFTER engine is created.
        AddHandler engine.Completed, AddressOf OnCompleted

        ' Start the copy on a background thread.
        engine.StartCopy(sourceDirectory, destinationDirectory)

        ' Show a modeless progress dialog that listens to engine events.
        Dim copyDialog As New CopyDialog(engine)
        copyDialog.Show(Me)

    End Sub


    ' ===============================
    ' BLOCKING EXAMPLE (DO NOT USE)
    ' ==============================

    '' Synchronous, blocking example inside a button click handler

    'Private Sub BtnStart_Click(sender As Object, e As EventArgs) _
    '    Handles btnStart.Click

    '    Dim sourceDirectory = txtSource.Text.Trim()
    '    Dim destinationDirectory = txtDest.Text.Trim()

    '    ' Quick validation omitted for brevity

    '    If IO.File.Exists(sourceDirectory) Then

    '        ' This call blocks the UI until the copy finishes
    '        IO.File.Copy(sourceDirectory,
    '                     IO.Path.Combine(destinationDirectory,
    '                                     IO.Path.GetFileName(sourceDirectory)),
    '                     True)

    '    ElseIf IO.Directory.Exists(sourceDirectory) Then

    '        ' Recursive synchronous copy — also blocks the UI
    '        DirectoryCopy(sourceDirectory,
    '                      IO.Path.Combine(destinationDirectory,
    '                                      IO.Path.GetFileName(sourceDirectory)))

    '    End If

    '    MessageBox.Show("Copy finished") ' UI was frozen until this point

    'End Sub

    'Private Sub DirectoryCopy(sourceDirectory As String,
    '                          destinationDirectory As String)

    '    IO.Directory.CreateDirectory(destinationDirectory)

    '    For Each file In IO.Directory.GetFiles(sourceDirectory)

    '        IO.File.Copy(file,
    '                     IO.Path.Combine(destinationDirectory,
    '                                     IO.Path.GetFileName(file)),
    '                     True)

    '    Next

    '    For Each directory In IO.Directory.GetDirectories(sourceDirectory)

    '        ' Recursive call to copy subdirectories
    '        DirectoryCopy(directory,
    '                      IO.Path.Combine(destinationDirectory,
    '                                      IO.Path.GetFileName(directory)))

    '    Next

    'End Sub


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
    '  CANCEL BUTTON
    '===============================
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        ' Cancel cooperatively: the engine checks for cancellation between file operations.
        engine?.Cancel()

    End Sub

    Private Sub OnCompleted(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)
        ' This event is raised when the engine finishes copying (either successfully, with skips/errors, or cancelled).
        ' You can use it to re-enable the Start button or show a message box if you want.


        'If Me.InvokeRequired Then
        '    Me.Invoke(New Action(Of Boolean, Boolean, Boolean)(AddressOf OnCompleted), success, hadSkips, hadErrors)
        '    Return
        'End If

        'Dim msg As String
        'If Not success Then
        '    msg = "Copy canceled."
        'ElseIf hadErrors OrElse hadSkips Then
        '    msg = "Copy finished (some files were skipped or failed)."
        'Else
        '    msg = "Copy finished successfully."
        'End If
        'MessageBox.Show(Me, msg, "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'If hadErrors OrElse hadSkips Then
        '    Dim summary As New ErrorSummaryDialog(engine.ErrorList)
        '    summary.ShowDialog(Me)
        'End If

        ' Enable the Start button and hide the Cancel button
        If Not btnStart.Enabled Then btnStart.Enabled = True
        If btnCancel.Visible Then btnCancel.Visible = False

    End Sub

End Class
