Imports System.IO

Public Class ValidationOutcome
    Public Property IsValid As Boolean
    Public Property ErrorMessage As String
    Public Property RequiresFileOverwritePrompt As Boolean
    Public Property RequiresFolderMergePrompt As Boolean
    Public Property NormalizedSource As String
    Public Property NormalizedDestination As String
End Class

Public Class ValidationEngine

    Public Shared Function ValidatePaths(rawSource As String,
                                         rawDest As String) As ValidationOutcome

        Dim outcome As New ValidationOutcome With {
            .IsValid = False,
            .NormalizedSource = rawSource,
            .NormalizedDestination = rawDest
        }

        Dim source = rawSource?.Trim()
        Dim dest = rawDest?.Trim()

        ' Empty fields
        If String.IsNullOrWhiteSpace(source) Then
            outcome.ErrorMessage = "Please select a source file or folder."
            Return outcome
        End If

        If String.IsNullOrWhiteSpace(dest) Then
            outcome.ErrorMessage = "Please select a destination folder."
            Return outcome
        End If

        ' Existence checks
        Dim sourceIsFile = File.Exists(source)
        Dim sourceIsDir = Directory.Exists(source)

        If Not sourceIsFile AndAlso Not sourceIsDir Then
            outcome.ErrorMessage = "The source path does not exist." & Environment.NewLine & source
            Return outcome
        End If

        If Not Directory.Exists(dest) Then
            outcome.ErrorMessage = "The destination folder does not exist." & Environment.NewLine & dest
            Return outcome
        End If

        ' Protected path check
        If IsProtectedPath(source) Then
            outcome.ErrorMessage =
                "This folder or file is protected by Windows and cannot be copied." &
                Environment.NewLine & source
            Return outcome
        End If

        ' Self-copy prevention
        If sourceIsDir Then
            Dim s = source.TrimEnd("\"c).ToLowerInvariant()
            Dim d = dest.TrimEnd("\"c).ToLowerInvariant()

            If d = s OrElse d.StartsWith(s & "\") Then
                outcome.ErrorMessage = "You cannot copy a folder into itself or one of its subfolders."
                Return outcome
            End If
        End If

        ' File → destination must be folder
        If sourceIsFile AndAlso Not Directory.Exists(dest) Then
            outcome.ErrorMessage = "When copying a file, the destination must be a folder."
            Return outcome
        End If

        ' Name collisions
        Dim sourceName = Path.GetFileName(source.TrimEnd("\"c))
        Dim destChild = Path.Combine(dest, sourceName)

        If sourceIsFile AndAlso Directory.Exists(destChild) Then
            outcome.ErrorMessage =
                "A folder with the same name already exists in the destination." & Environment.NewLine &
                destChild & Environment.NewLine &
                "You cannot copy a file over a folder."
            Return outcome
        End If

        If sourceIsDir AndAlso File.Exists(destChild) Then
            outcome.ErrorMessage =
                "A file with the same name already exists in the destination." & Environment.NewLine &
                destChild & Environment.NewLine &
                "You cannot copy a folder over a file."
            Return outcome
        End If

        ' Overwrite prompt needed?
        If sourceIsFile AndAlso File.Exists(destChild) Then
            outcome.RequiresFileOverwritePrompt = True
        End If

        ' Merge prompt needed?
        If sourceIsDir AndAlso Directory.Exists(destChild) Then
            outcome.RequiresFolderMergePrompt = True
        End If

        ' All good
        outcome.IsValid = True
        outcome.NormalizedSource = source
        outcome.NormalizedDestination = dest
        Return outcome

    End Function

    Public Shared Function IsProtectedPath(path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then Return False

        ' Normalize input
        Dim normalized As String
        Try
            normalized = IO.Path.GetFullPath(path).TrimEnd("\"c).ToLowerInvariant()
        Catch
            ' Invalid paths are treated as non-protected
            Return False
        End Try

        ' 1. Drive roots (C:, D:, etc.) are ALWAYS protected
        If normalized Like "[a-z]:" Then
            Return True
        End If

        ' 2. Build dynamic user AppData protected paths
        Dim userProfile As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim appDataLocal As String = IO.Path.Combine(userProfile, "AppData\Local")
        Dim appDataRoaming As String = IO.Path.Combine(userProfile, "AppData\Roaming")
        Dim appDataRoot As String = IO.Path.Combine(userProfile, "AppData")

        ' 3. Static system protected roots
        Dim staticProtected As String() = {
        "c:\windows",
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
        "c:\users\default user"
    }

        ' 4. Combine static + dynamic protected roots
        Dim protectedRoots As New List(Of String)(staticProtected) From {
        appDataRoot,
        appDataLocal,
        appDataRoaming
    }

        ' 5. Check exact or subdirectory match
        For Each root In protectedRoots
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

        ' 6. User working folders are NOT protected:
        '    - %USERPROFILE%
        '    - Documents, Desktop, Downloads, Pictures, Music, Videos
        '    - Any subfolders of those
        ' Your tests explicitly require these to return False.

        Return False
    End Function

    Public Sub RunTests()

        Debug.WriteLine("Running tests...")

        TestIsProtectedPath()

        TestValidatePaths()

        Debug.WriteLine("All tests executed.")

    End Sub

    Private Sub TestIsProtectedPath()

        Debug.WriteLine("→ Testing IsProtectedPath (Explorer‑accurate)")

        TestIsProtectedPath_SystemRoots()
        TestIsProtectedPath_AppData()
        TestIsProtectedPath_UserWorkingFolders()
        TestIsProtectedPath_Subdirectories()
        TestIsProtectedPath_EdgeCases()

        Debug.WriteLine("✓ All IsProtectedPath tests passed")

    End Sub
    Private Sub TestIsProtectedPath_SystemRoots()

        Debug.WriteLine("  → System roots")

        AssertTrue(IsProtectedPath("C:\Windows"), "Windows root should be protected")
        AssertTrue(IsProtectedPath("C:\Program Files"), "Program Files should be protected")
        AssertTrue(IsProtectedPath("C:\Program Files (x86)"), "Program Files (x86) should be protected")
        AssertTrue(IsProtectedPath("C:\ProgramData"), "ProgramData should be protected")
        AssertTrue(IsProtectedPath("C:\System Volume Information"), "System Volume Information should be protected")
        AssertTrue(IsProtectedPath("C:\$Recycle.Bin"), "$Recycle.Bin should be protected")

    End Sub
    Private Sub TestIsProtectedPath_AppData()

        Debug.WriteLine("  → AppData roots")

        Dim userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim appData = Path.Combine(userProfile, "AppData")
        Dim local = Path.Combine(appData, "Local")
        Dim roaming = Path.Combine(appData, "Roaming")

        AssertTrue(IsProtectedPath(appData), "AppData root should be protected")
        AssertTrue(IsProtectedPath(local), "AppData\Local should be protected")
        AssertTrue(IsProtectedPath(roaming), "AppData\Roaming should be protected")

        ' Subdirectories
        AssertTrue(IsProtectedPath(Path.Combine(local, "Temp")), "Subfolder under AppData\Local should be protected")
        AssertTrue(IsProtectedPath(Path.Combine(roaming, "Microsoft")), "Subfolder under AppData\Roaming should be protected")

    End Sub
    Private Sub TestIsProtectedPath_UserWorkingFolders()

        Debug.WriteLine("  → User working folders")

        Dim userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

        AssertFalse(IsProtectedPath(userProfile), "%USERPROFILE% should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Documents")), "Documents should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Desktop")), "Desktop should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Downloads")), "Downloads should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Pictures")), "Pictures should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Videos")), "Videos should NOT be protected")

        ' Subdirectories
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Documents\MyProject")), "Subfolder under Documents should NOT be protected")
        AssertFalse(IsProtectedPath(Path.Combine(userProfile, "Desktop\TestFolder")), "Subfolder under Desktop should NOT be protected")

    End Sub
    Private Sub TestIsProtectedPath_Subdirectories()

        Debug.WriteLine("  → Subdirectory behavior")

        AssertTrue(IsProtectedPath("C:\Windows\System32"), "System32 should be protected")
        AssertTrue(IsProtectedPath("C:\Program Files\MyApp"), "Subfolder under Program Files should be protected")

        AssertFalse(IsProtectedPath("C:\Temp"), "C:\Temp should NOT be protected")
        AssertFalse(IsProtectedPath("D:\Games"), "Unrelated drive should NOT be protected")

    End Sub
    Private Sub TestIsProtectedPath_EdgeCases()

        Debug.WriteLine("  → Edge cases")

        AssertFalse(IsProtectedPath(""), "Empty string should NOT be protected")
        AssertFalse(IsProtectedPath(Nothing), "Nothing should NOT be protected")

        AssertTrue(IsProtectedPath("c:\windows"), "Case-insensitive match should be protected")
        AssertTrue(IsProtectedPath("C:\Windows\"), "Trailing slash should still match protected path")

        ' Invalid paths → treated as NOT protected
        AssertFalse(IsProtectedPath("::invalid::path"), "Invalid paths should NOT be protected")

    End Sub

    Private Sub TestValidatePaths()

        Debug.WriteLine("→ Testing ValidatePaths (Explorer‑accurate)")

        TestValidate_EmptyFields()
        TestValidate_SourceMustExist()
        TestValidate_DestinationMustExist()
        TestValidate_FileToFolderRule()
        TestValidate_NoCopyIntoSelf()
        TestValidate_NameCollisions()
        TestValidate_OverwritePrompt()
        TestValidate_MergePrompt()

        Debug.WriteLine("✓ All ValidatePaths tests passed")
    End Sub


    Private Sub TestValidate_EmptyFields()

        Dim o

        o = ValidatePaths("", "C:\Dest")
        AssertFalse(o.IsValid, "Empty source should fail")

        o = ValidatePaths("C:\Source", "")
        AssertFalse(o.IsValid, "Empty destination should fail")

    End Sub
    Private Sub TestValidate_SourceMustExist()

        Dim o = ValidatePaths("C:\DefinitelyDoesNotExist_123", "C:\Windows")
        AssertFalse(o.IsValid, "Nonexistent source should fail")

    End Sub
    Private Sub TestValidate_DestinationMustExist()

        Dim o = ValidatePaths("C:\Windows", "C:\Nope_123")
        AssertFalse(o.IsValid, "Nonexistent destination should fail")

    End Sub
    Private Sub TestValidate_FileToFolderRule()

        Dim tempFile = Path.GetTempFileName()
        Dim tempFile2 = Path.GetTempFileName()

        Dim o = ValidatePaths(tempFile, tempFile2)
        AssertFalse(o.IsValid, "File → file is invalid; destination must be folder")

    End Sub
    Private Sub TestValidate_NoCopyIntoSelf()

        Dim root = Path.Combine(Path.GetTempPath(), "CopyTestRoot")
        Directory.CreateDirectory(root)

        Dim sub1 = Path.Combine(root, "Sub1")
        Directory.CreateDirectory(sub1)

        Dim o

        o = ValidatePaths(root, root)
        AssertFalse(o.IsValid, "Cannot copy folder into itself")

        o = ValidatePaths(root, sub1)
        AssertFalse(o.IsValid, "Cannot copy folder into its subfolder")

    End Sub
    Private Sub TestValidate_NameCollisions()

        Dim root = Path.Combine(Path.GetTempPath(), "CollisionTest")
        Directory.CreateDirectory(root)

        Dim fileSrc = Path.Combine(root, "Item")
        File.WriteAllText(fileSrc, "test")

        Dim folderDest = Path.Combine(root, "Dest")
        Directory.CreateDirectory(folderDest)

        Dim folderWithSameName = Path.Combine(folderDest, "Item")
        Directory.CreateDirectory(folderWithSameName)

        Dim o = ValidatePaths(fileSrc, folderDest)
        AssertFalse(o.IsValid, "Cannot copy file over folder")

    End Sub

    Private Sub TestValidate_OverwritePrompt()

        Dim userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim root = Path.Combine(userProfile, "Documents", "OverwriteTest")
        Directory.CreateDirectory(root)

        Dim src = Path.Combine(root, "File.txt")
        File.WriteAllText(src, "A")

        Dim destFolder = Path.Combine(root, "Dest")
        Directory.CreateDirectory(destFolder)

        Dim destFile = Path.Combine(destFolder, "File.txt")
        File.WriteAllText(destFile, "B")

        Dim o = ValidatePaths(src, destFolder)
        AssertTrue(o.RequiresFileOverwritePrompt, "Overwrite prompt should be required")

    End Sub

    Private Sub TestValidate_MergePrompt()

        Dim userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim root = Path.Combine(userProfile, "Documents", "MergeTest")
        Directory.CreateDirectory(root)

        Dim src = Path.Combine(root, "FolderA")
        Directory.CreateDirectory(src)

        Dim dest = Path.Combine(root, "Dest")
        Directory.CreateDirectory(dest)

        Dim destChild = Path.Combine(dest, "FolderA")
        Directory.CreateDirectory(destChild)

        Dim o = ValidatePaths(src, dest)
        AssertTrue(o.RequiresFolderMergePrompt, "Merge prompt should be required")

    End Sub

    Private Sub AssertTrue(condition As Boolean, message As String)
        Debug.Assert(condition, message)
    End Sub

    Private Sub AssertFalse(condition As Boolean, message As String)
        Debug.Assert(Not condition, message)
    End Sub

End Class

