Imports System.ComponentModel
Imports System.IO

'===============================
'  SUPPORTING TYPES
'===============================
Public Enum CopyErrorAction
    Skip
    SkipAll
    Cancel
End Enum

Public Class CopyErrorEntry
    Public Property FilePath As String
    Public Property Message As String
End Class

Public Class CopyProgressInfo
    Public Property Percent As Integer
    Public Property BytesCopied As Long
    Public Property TotalBytes As Long
    Public Property FilesDone As Integer
    Public Property TotalFiles As Integer
    Public Property CurrentFile As String
    Public Property SpeedBytesPerSec As Double
    Public Property Eta As TimeSpan
End Class

'===============================
'  COPY ENGINE
'===============================
Public Class CopyEngine

    ' Events
    ' These events are Public because they form the engine’s external contract.
    ' The copy engine runs internally, but the UI (or any consumer) must be able
    ' to subscribe to progress, errors, and completion signals. Exposing events
    ' publicly keeps the BackgroundWorker encapsulated while still allowing the
    ' outside world to react to what the engine is doing.
    Public Event ProgressChanged(info As CopyProgressInfo)
    Public Event ErrorOccurred(file As String, ex As Exception)
    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

    Private WithEvents Worker As New BackgroundWorker With {
        .WorkerReportsProgress = True,
        .WorkerSupportsCancellation = True
    }

    Private sourcePath As String
    Private destinationPath As String

    Private files As List(Of String)
    Private folders As List(Of String)

    Private totalBytes As Long
    Private totalFiles As Integer

    Private bytesCopied As Long
    Private filesDone As Integer

    Private skipAll As Boolean = False
    Private hadSkips As Boolean = False
    Private hadErrors As Boolean = False

    Private ReadOnly policy As New CopyPolicy()
    Private ReadOnly errors As New List(Of CopyErrorEntry)
    Private sw As Stopwatch
    Private ReadOnly throttler As New ProgressThrottler(200)

    Private rootFolderName As String = ""

    Public ReadOnly Property ErrorList As List(Of CopyErrorEntry)
        Get
            Return errors
        End Get
    End Property

    '===============================
    '  START COPY
    '===============================
    Public Sub StartCopy(source As String, dest As String)

        sourcePath = source.TrimEnd("\"c)
        destinationPath = dest.TrimEnd("\"c)

        skipAll = False
        hadSkips = False
        hadErrors = False
        errors.Clear()
        bytesCopied = 0
        filesDone = 0

        files = New List(Of String)()
        folders = New List(Of String)()
        totalBytes = 0
        totalFiles = 0
        rootFolderName = ""

        If File.Exists(sourcePath) Then
            ' SINGLE FILE
            files.Add(sourcePath)
            totalFiles = 1
            totalBytes = New FileInfo(sourcePath).Length
            rootFolderName = ""   ' no wrapping folder

        ElseIf Directory.Exists(sourcePath) Then
            ' FOLDER
            rootFolderName = Path.GetFileName(sourcePath)
            PreScanSafe(sourcePath)

        Else
            Throw New IOException("Source path does not exist.")
        End If

        sw = Stopwatch.StartNew()
        Worker.RunWorkerAsync()

    End Sub

    Public Sub Cancel()
        If Worker.IsBusy Then
            Worker.CancelAsync()
        End If
    End Sub

    Public Sub SetErrorAction(action As CopyErrorAction)
        policy.SetAction(action)
    End Sub

    '===============================
    '  SAFE PRE-SCAN
    '===============================
    Private Sub PreScanSafe(root As String)
        Try
            ScanFolderSafe(root)
        Catch
            ' Never let PreScan crash the copy
        End Try
    End Sub

    Private Sub ScanFolderSafe(folder As String)

        ' Record this folder
        folders.Add(folder)

        Dim subFiles As String()

        ' Files
        Try
            subFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
        Catch ex As Exception
            errors.Add(New CopyErrorEntry With {
                .FilePath = folder,
                .Message = ex.Message
            })
            hadErrors = True
            Return
        End Try

        For Each f In subFiles
            files.Add(f)
            totalFiles += 1
            Try
                totalBytes += New FileInfo(f).Length
            Catch
                ' Ignore unreadable file size
            End Try
        Next

        Dim subDirs As String()

        ' Subdirectories
        Try
            subDirs = Directory.GetDirectories(folder)
        Catch ex As Exception
            errors.Add(New CopyErrorEntry With {
                .FilePath = folder,
                .Message = ex.Message
            })
            hadErrors = True
            Return
        End Try

        For Each d In subDirs
            ScanFolderSafe(d)
        Next
    End Sub

    '===============================
    '  WORKER THREAD
    '===============================
    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles Worker.DoWork

        Dim buffer(64 * 1024 - 1) As Byte

        ' Create all folders first (for folder copies)
        If rootFolderName <> "" Then
            For Each folder In folders
                Dim relative = folder.Substring(sourcePath.Length).TrimStart("\"c)
                Dim targetFolder = Path.Combine(destinationPath, rootFolderName, relative)

                Try
                    Directory.CreateDirectory(targetFolder)
                Catch ex As Exception
                    hadErrors = True
                    errors.Add(New CopyErrorEntry With {
                        .FilePath = folder,
                        .Message = ex.Message
                    })
                End Try
            Next
        End If

        ' Copy files
        For Each file In files

            If Worker.CancellationPending Then
                e.Cancel = True
                Return
            End If

            Dim target As String

            If rootFolderName = "" Then
                ' SINGLE FILE
                target = Path.Combine(destinationPath, Path.GetFileName(file))
            Else
                ' FOLDER COPY
                Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
                target = Path.Combine(destinationPath, rootFolderName, relative)
            End If

            Try
                Directory.CreateDirectory(Path.GetDirectoryName(target))

                Using src As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using dst As New FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None)

                        Dim read As Integer
                        Do
                            If Worker.CancellationPending Then
                                e.Cancel = True
                                Return
                            End If

                            read = src.Read(buffer, 0, buffer.Length)
                            If read <= 0 Then Exit Do

                            dst.Write(buffer, 0, read)
                            bytesCopied += read

                            If throttler.ShouldReport() Then
                                ReportProgress(file)
                            End If
                        Loop

                    End Using
                End Using

                ' Copy attributes and timestamps
                Try
                    Dim srcInfo As New FileInfo(file)
                    Dim dstInfo As New FileInfo(target)

                    dstInfo.CreationTimeUtc = srcInfo.CreationTimeUtc
                    dstInfo.LastWriteTimeUtc = srcInfo.LastWriteTimeUtc
                    dstInfo.LastAccessTimeUtc = srcInfo.LastAccessTimeUtc

                    IO.File.SetAttributes(target, srcInfo.Attributes)

                Catch exAttr As Exception
                    hadErrors = True
                    errors.Add(New CopyErrorEntry With {
                        .FilePath = target,
                        .Message = "Failed to copy attributes: " & exAttr.Message
                    })
                End Try

                filesDone += 1
                ReportProgress(file)

            Catch ex As Exception
                hadErrors = True

                If skipAll Then
                    hadSkips = True
                    errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})
                    Continue For
                End If

                RaiseEvent ErrorOccurred(file, ex)

                Dim action = policy.WaitForAction()

                Select Case action
                    Case CopyErrorAction.Skip
                        hadSkips = True
                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

                    Case CopyErrorAction.SkipAll
                        skipAll = True
                        hadSkips = True
                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

                    Case CopyErrorAction.Cancel
                        e.Cancel = True
                        Return
                End Select
            End Try

        Next
    End Sub

    '===============================
    '  PROGRESS REPORTING
    '===============================
    Private Sub ReportProgress(currentFile As String)

        '----- SAFE PERCENT -----
        Dim pct As Double

        If totalBytes > 0 Then
            pct = (CDbl(bytesCopied) / CDbl(totalBytes)) * 100.0
        Else
            pct = 0
        End If

        If Double.IsNaN(pct) OrElse Double.IsInfinity(pct) Then
            pct = 0
        End If

        Dim percent As Integer = CInt(Math.Max(0, Math.Min(100, pct)))

        '----- SPEED & ETA -----
        Dim speed As Double = 0
        Dim eta As TimeSpan = TimeSpan.Zero

        If sw IsNot Nothing AndAlso sw.Elapsed.TotalSeconds > 0.1 Then
            speed = bytesCopied / sw.Elapsed.TotalSeconds

            Dim remaining As Long = totalBytes - bytesCopied
            If speed > 0 Then
                eta = TimeSpan.FromSeconds(remaining / speed)
            End If
        End If

        '----- PACKAGE -----
        Dim info As New CopyProgressInfo With {
            .Percent = percent,
            .BytesCopied = bytesCopied,
            .TotalBytes = totalBytes,
            .FilesDone = filesDone,
            .TotalFiles = totalFiles,
            .CurrentFile = currentFile,
            .SpeedBytesPerSec = speed,
            .Eta = eta
        }

        Worker.ReportProgress(percent, info)
    End Sub

    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles Worker.ProgressChanged
        Dim info = DirectCast(e.UserState, CopyProgressInfo)
        RaiseEvent ProgressChanged(info)
    End Sub

    '===============================
    '  COMPLETION
    '===============================
    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles Worker.RunWorkerCompleted
        sw?.Stop()

        If e.Cancelled Then
            RaiseEvent Completed(False, hadSkips, hadErrors)
        Else
            RaiseEvent Completed(True, hadSkips, hadErrors)
        End If
    End Sub

End Class
