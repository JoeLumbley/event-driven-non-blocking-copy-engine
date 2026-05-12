Imports System.IO
Imports System.Windows.Forms

Public Class ValidatorResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property SourcePath As String
    Public Property DestinationPath As String
End Class

Public Class Validator

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
        "c:\dumpstack.log.tmp",
        "c:\hiberfil.sys",
        "c:\pagefile.sys",
        "c:\swapfile.sys",
        "c:\users\all users",
        "c:\users\default user",
        "c:\users\default\appdata\local\application data",
        "c:\users\default\appdata\local\history",
        "c:\users\default\appdata\local\temporary internet files"
    }

    Public Shared Function Validate(owner As IWin32Window,
                                    rawSource As String,
                                    rawDest As String) As ValidatorResult

        Dim result As New ValidatorResult With {
            .Success = False,
            .SourcePath = rawSource,
            .DestinationPath = rawDest
        }

        Dim sourceDirectory As String = rawSource.Trim()
        Dim destinationDirectory As String = rawDest.Trim()

        If String.IsNullOrWhiteSpace(sourceDirectory) Then
            ShowValidationError(owner, "Please select a source file or folder.")
            Return result
        End If

        If String.IsNullOrWhiteSpace(destinationDirectory) Then
            ShowValidationError(owner, "Please select a destination folder.")
            Return result
        End If

        ' 1. Source must exist (either file or folder).
        If Not File.Exists(sourceDirectory) AndAlso Not Directory.Exists(sourceDirectory) Then
            ShowValidationError(owner, "The source path does not exist." & Environment.NewLine & sourceDirectory)
            Return result
        End If

        ' 2. Destination must be an existing folder.
        If Not Directory.Exists(destinationDirectory) Then
            ShowValidationError(owner, "The destination folder does not exist." & Environment.NewLine & destinationDirectory)
            Return result
        End If

        ' 3. Source must not be a protected system path.
        If IsProtectedPath(sourceDirectory) Then
            ShowValidationError(owner,
                                "This folder or file is protected by Windows and cannot be copied." &
                                Environment.NewLine & sourceDirectory)
            Return result
        End If

        ' 4. Prevent copying a folder into itself or one of its subfolders.
        If Directory.Exists(sourceDirectory) Then
            Dim s = sourceDirectory.TrimEnd("\"c).ToLowerInvariant()
            Dim d = destinationDirectory.TrimEnd("\"c).ToLowerInvariant()

            If d = s OrElse d.StartsWith(s & "\") Then
                ShowValidationError(owner, "You cannot copy a folder into itself or one of its subfolders.")
                Return result
            End If
        End If

        ' 5. When copying a file, destination must be a folder.
        If File.Exists(sourceDirectory) AndAlso Not Directory.Exists(destinationDirectory) Then
            ShowValidationError(owner, "When copying a file, the destination must be a folder.")
            Return result
        End If

        ' 6. Prevent file/folder name collisions where types differ.
        Dim sourceName As String = Path.GetFileName(sourceDirectory.TrimEnd("\"c))
        Dim destinationChildPath As String = Path.Combine(destinationDirectory, sourceName)

        If File.Exists(sourceDirectory) AndAlso Directory.Exists(destinationChildPath) Then

            Dim errorMsg As String =
                "A folder with the same name already exists in the destination." & Environment.NewLine &
                destinationChildPath & Environment.NewLine &
                "You cannot copy a file over a folder. Please choose a different destination or rename the source file."

            ShowValidationError(owner, errorMsg)
            Return result

        End If

        If Directory.Exists(sourceDirectory) AndAlso File.Exists(destinationChildPath) Then

            Dim errorMsg As String =
                "A file with the same name already exists in the destination." & Environment.NewLine &
                destinationChildPath & Environment.NewLine &
                "You cannot copy a folder over a file. Please choose a different destination or rename the source folder."

            ShowValidationError(owner, errorMsg)
            Return result

        End If

        ' 7. File-level overwrite warning (single file copy).
        If File.Exists(sourceDirectory) Then
            Dim fileName As String = Path.GetFileName(sourceDirectory)
            Dim destinationFile As String = Path.Combine(destinationDirectory, fileName)

            If File.Exists(destinationFile) Then
                Dim errorMsg As String =
                    "The file '" & fileName &
                    "' already exists in the destination folder." & Environment.NewLine &
                    "Do you want to overwrite it?"

                Dim overwriteResult = MessageBox.Show(owner,
                                                      errorMsg,
                                                      "Confirm File Replace",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning)

                If overwriteResult = DialogResult.No Then
                    Return result
                End If
            End If
        End If

        ' 8. Folder-level merge warning (folder copy).
        If Directory.Exists(sourceDirectory) Then
            Dim folderName As String = Path.GetFileName(sourceDirectory.TrimEnd("\"c))
            Dim destinationFolder As String = Path.Combine(destinationDirectory, folderName)

            If Directory.Exists(destinationFolder) Then
                Using folderMergeDialog As New FolderMergeDialog(folderName)
                    Dim mergeResult = folderMergeDialog.ShowDialog(owner)
                    If mergeResult = DialogResult.No Then
                        Return result
                    End If
                End Using
            End If
        End If

        ' All good
        result.Success = True
        result.SourcePath = sourceDirectory
        result.DestinationPath = destinationDirectory
        Return result

    End Function

    Private Shared Sub ShowValidationError(owner As IWin32Window, message As String)
        Using dlg As New ValidationErrorDialog(message)
            dlg.ShowDialog(owner)
        End Using
    End Sub

    Public Shared Function IsProtectedPath(path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then Return False

        Dim normalized As String = path.Trim().TrimEnd("\"c).ToLowerInvariant()

        ' Block drive roots like "c:" or "d:"
        If normalized Like "[a-z]:" Then
            Return True
        End If

        For Each root In ProtectedRoots
            Dim r = root.TrimEnd("\"c).ToLowerInvariant()

            If normalized = r Then
                Return True
            End If

            If normalized.StartsWith(r & "\") Then
                Return True
            End If
        Next

        Return False
    End Function

End Class
