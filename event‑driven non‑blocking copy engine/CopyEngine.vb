'Public Class CopyEngine
'    Public Event ProgressChanged(percent As Integer, currentFile As String)
'    Public Event Completed(success As Boolean, message As String)

'    Private WithEvents worker As New System.ComponentModel.BackgroundWorker With {
'        .WorkerReportsProgress = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest
'        files = IO.Directory.GetFiles(sourcePath, "*.*", IO.SearchOption.AllDirectories).ToList()
'        worker.RunWorkerAsync()
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles worker.DoWork
'        Dim total = files.Count
'        Dim index = 0

'        For Each file In files
'            index += 1

'            Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'            Dim target = IO.Path.Combine(destPath, relative)

'            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(target))
'            IO.File.Copy(file, target, True)

'            Dim pct = CInt((index / total) * 100)
'            worker.ReportProgress(pct, file)
'        Next
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles worker.ProgressChanged
'        RaiseEvent ProgressChanged(e.ProgressPercentage, CStr(e.UserState))
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        RaiseEvent Completed(True, "Copy finished.")
'    End Sub
'End Class


'Public Class CopyEngine
'    Public Event ProgressChanged(percent As Integer, currentFile As String)
'    Public Event Completed(success As Boolean, message As String)

'    Private WithEvents worker As New System.ComponentModel.BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest
'        files = IO.Directory.GetFiles(sourcePath, "*.*", IO.SearchOption.AllDirectories).ToList()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles worker.DoWork
'        Dim total = files.Count
'        Dim index = 0

'        For Each file In files
'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            index += 1

'            Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'            Dim target = IO.Path.Combine(destPath, relative)

'            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(target))
'            IO.File.Copy(file, target, True)

'            Dim pct = CInt((index / total) * 100)
'            worker.ReportProgress(pct, file)
'        Next
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles worker.ProgressChanged
'        RaiseEvent ProgressChanged(e.ProgressPercentage, CStr(e.UserState))
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        If e.Cancelled Then
'            RaiseEvent Completed(False, "Copy canceled.")
'        Else
'            RaiseEvent Completed(True, "Copy finished.")
'        End If
'    End Sub
'End Class



'Imports System.ComponentModel
'Imports System.IO
'Imports System.Threading

'Public Enum CopyErrorAction
'    Skip
'    SkipAll
'    Cancel
'End Enum

'Public Class CopyEngine

'    Public Event ProgressChanged(percent As Integer, currentFile As String)
'    Public Event Completed(success As Boolean, message As String)
'    Public Event ErrorOccurred(file As String, ex As Exception)

'    Private WithEvents worker As New BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    ' Error-handling state
'    Private skipAll As Boolean = False
'    Private pendingAction As CopyErrorAction? = Nothing
'    Private actionReceived As New AutoResetEvent(False)

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest
'        skipAll = False
'        pendingAction = Nothing

'        files = IO.Directory.GetFiles(source, "*.*", IO.SearchOption.AllDirectories).ToList()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Public Sub SetErrorAction(action As CopyErrorAction)
'        pendingAction = action
'        actionReceived.Set()
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
'        Dim total = files.Count
'        Dim index = 0

'        For Each file In files
'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            index += 1

'            Try
'                Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'                Dim target = IO.Path.Combine(destPath, relative)

'                'IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(target))
'                'IO.File.Copy(file, target, True)


'                ' Force an error for testing
'                Throw New IOException("Test error: pretend this file failed.")


'            Catch ex As Exception
'                If skipAll Then
'                    Continue For
'                End If

'                ' Notify UI
'                RaiseEvent ErrorOccurred(file, ex)

'                ' Wait for UI decision
'                actionReceived.WaitOne()

'                Select Case pendingAction
'                    Case CopyErrorAction.Skip
'                        ' continue loop

'                    Case CopyErrorAction.SkipAll
'                        skipAll = True

'                    Case CopyErrorAction.Cancel
'                        e.Cancel = True
'                        Return
'                End Select
'            End Try

'            Dim pct = CInt((index / total) * 100)
'            worker.ReportProgress(pct, file)
'        Next
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
'        RaiseEvent ProgressChanged(e.ProgressPercentage, CStr(e.UserState))
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        If e.Cancelled Then
'            RaiseEvent Completed(False, "Copy canceled.")
'        Else
'            RaiseEvent Completed(True, "Copy finished.")
'        End If
'    End Sub

'End Class




'Imports System.ComponentModel
'Imports System.Threading
'Imports System.IO

'Public Enum CopyErrorAction
'    Skip
'    SkipAll
'    Cancel
'End Enum

'Public Class CopyEngine

'    Public Event ProgressChanged(percent As Integer, currentFile As String)
'    Public Event Completed(success As Boolean, message As String)
'    Public Event ErrorOccurred(file As String, ex As Exception)

'    Private WithEvents worker As New BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    ' Explorer-style skip tracking
'    Private skipAll As Boolean = False
'    Private hadSkips As Boolean = False

'    ' Error dialog synchronization
'    Private pendingAction As CopyErrorAction? = Nothing
'    Private actionReceived As New AutoResetEvent(False)

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest

'        skipAll = False
'        hadSkips = False
'        pendingAction = Nothing

'        files = IO.Directory.GetFiles(source, "*.*", IO.SearchOption.AllDirectories).ToList()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Public Sub SetErrorAction(action As CopyErrorAction)
'        pendingAction = action
'        actionReceived.Set()
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
'        Dim total = files.Count
'        Dim index = 0

'        For Each file In files
'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            index += 1

'            Try
'                Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'                Dim target = IO.Path.Combine(destPath, relative)

'                'IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(target))
'                'IO.File.Copy(file, target, True)

'                ' Uncomment this to test error handling:
'                Throw New System.IO.IOException("Test error")

'            Catch ex As Exception
'                If skipAll Then
'                    hadSkips = True
'                    Continue For
'                End If

'                RaiseEvent ErrorOccurred(file, ex)

'                actionReceived.WaitOne()

'                Select Case pendingAction
'                    Case CopyErrorAction.Skip
'                        hadSkips = True

'                    Case CopyErrorAction.SkipAll
'                        skipAll = True
'                        hadSkips = True

'                    Case CopyErrorAction.Cancel
'                        e.Cancel = True
'                        Return
'                End Select
'            End Try

'            Dim pct = CInt((index / total) * 100)
'            worker.ReportProgress(pct, file)
'        Next
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
'        RaiseEvent ProgressChanged(e.ProgressPercentage, CStr(e.UserState))
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        If e.Cancelled Then
'            RaiseEvent Completed(False, "Copy canceled.")
'        ElseIf hadSkips Then
'            RaiseEvent Completed(True, "Copy finished (some files were skipped).")
'        Else
'            RaiseEvent Completed(True, "Copy finished.")
'        End If
'    End Sub

'End Class





'Imports System.ComponentModel
'Imports System.IO
'Imports System.Threading
'Imports System.Diagnostics

'Public Enum CopyErrorAction
'    Skip
'    SkipAll
'    Cancel
'End Enum

'Public Class CopyErrorEntry
'    Public Property FilePath As String
'    Public Property Message As String
'End Class

'Public Class CopyProgressInfo
'    Public Property Percent As Integer
'    Public Property BytesCopied As Long
'    Public Property TotalBytes As Long
'    Public Property FilesDone As Integer
'    Public Property TotalFiles As Integer
'    Public Property CurrentFile As String
'    Public Property SpeedBytesPerSec As Double
'    Public Property Eta As TimeSpan
'End Class

'Public Class CopyEngine

'    Public Event ProgressChanged(info As CopyProgressInfo)
'    Public Event ErrorOccurred(file As String, ex As Exception)
'    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

'    Private WithEvents worker As New BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)
'    Private totalBytes As Long
'    Private totalFiles As Integer

'    Private bytesCopied As Long
'    Private filesDone As Integer

'    Private skipAll As Boolean = False
'    Private hadSkips As Boolean = False
'    Private hadErrors As Boolean = False

'    Private pendingAction As CopyErrorAction? = Nothing
'    Private actionReceived As New AutoResetEvent(False)

'    Private errors As New List(Of CopyErrorEntry)
'    Private sw As Stopwatch

'    Public ReadOnly Property ErrorList As List(Of CopyErrorEntry)
'        Get
'            Return errors
'        End Get
'    End Property

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest

'        skipAll = False
'        hadSkips = False
'        hadErrors = False
'        pendingAction = Nothing
'        errors.Clear()
'        bytesCopied = 0
'        filesDone = 0

'        PreScan()

'        sw = Stopwatch.StartNew()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Public Sub SetErrorAction(action As CopyErrorAction)
'        pendingAction = action
'        actionReceived.Set()
'    End Sub

'    Private Sub PreScan()
'        files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).ToList()
'        totalFiles = files.Count
'        totalBytes = 0

'        For Each f In files
'            Dim fi As New FileInfo(f)
'            totalBytes += fi.Length
'        Next
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
'        Dim buffer(64 * 1024 - 1) As Byte

'        For Each file In files
'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'            Dim target = Path.Combine(destPath, relative)

'            Try
'                Directory.CreateDirectory(Path.GetDirectoryName(target))

'                Using src As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
'                    Using dst As New FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None)
'                        Dim read As Integer
'                        Do
'                            If worker.CancellationPending Then
'                                e.Cancel = True
'                                Return
'                            End If

'                            read = src.Read(buffer, 0, buffer.Length)
'                            If read <= 0 Then Exit Do

'                            dst.Write(buffer, 0, read)
'                            bytesCopied += read

'                            ReportProgress(file)
'                        Loop
'                    End Using
'                End Using

'                filesDone += 1
'                ReportProgress(file)

'            Catch ex As Exception
'                hadErrors = True

'                If skipAll Then
'                    hadSkips = True
'                    errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})
'                    Continue For
'                End If

'                RaiseEvent ErrorOccurred(file, ex)

'                actionReceived.WaitOne()

'                Select Case pendingAction
'                    Case CopyErrorAction.Skip
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.SkipAll
'                        skipAll = True
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.Cancel
'                        e.Cancel = True
'                        Return
'                End Select
'            End Try
'        Next
'    End Sub

'    Private Sub ReportProgress(currentFile As String)
'        Dim percent As Integer = If(totalBytes > 0, CInt((bytesCopied * 100L) \ totalBytes), 0)

'        Dim speed As Double = 0
'        Dim eta As TimeSpan = TimeSpan.Zero

'        If sw IsNot Nothing AndAlso sw.Elapsed.TotalSeconds > 0.1 Then
'            speed = bytesCopied / sw.Elapsed.TotalSeconds
'            Dim remaining As Long = totalBytes - bytesCopied
'            If speed > 0 Then
'                eta = TimeSpan.FromSeconds(remaining / speed)
'            End If
'        End If

'        Dim info As New CopyProgressInfo With {
'            .Percent = percent,
'            .BytesCopied = bytesCopied,
'            .TotalBytes = totalBytes,
'            .FilesDone = filesDone,
'            .TotalFiles = totalFiles,
'            .CurrentFile = currentFile,
'            .SpeedBytesPerSec = speed,
'            .Eta = eta
'        }

'        worker.ReportProgress(percent, info)
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
'        Dim info = DirectCast(e.UserState, CopyProgressInfo)
'        RaiseEvent ProgressChanged(info)
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        sw?.Stop()

'        If e.Cancelled Then
'            RaiseEvent Completed(False, hadSkips, hadErrors)
'        Else
'            RaiseEvent Completed(True, hadSkips, hadErrors)
'        End If
'    End Sub

'End Class













'Imports System.ComponentModel
'Imports System.IO
'Imports System.Threading
'Imports System.Diagnostics

'Public Enum CopyErrorAction
'    Skip
'    SkipAll
'    Cancel
'End Enum

'Public Class CopyErrorEntry
'    Public Property FilePath As String
'    Public Property Message As String
'End Class

'Public Class CopyProgressInfo
'    Public Property Percent As Integer
'    Public Property BytesCopied As Long
'    Public Property TotalBytes As Long
'    Public Property FilesDone As Integer
'    Public Property TotalFiles As Integer
'    Public Property CurrentFile As String
'    Public Property SpeedBytesPerSec As Double
'    Public Property Eta As TimeSpan
'End Class

'Public Class CopyEngine

'    Public Event ProgressChanged(info As CopyProgressInfo)
'    Public Event ErrorOccurred(file As String, ex As Exception)
'    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

'    Private WithEvents worker As New BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    Private totalBytes As Long
'    Private totalFiles As Integer

'    Private bytesCopied As Long
'    Private filesDone As Integer

'    Private skipAll As Boolean = False
'    Private hadSkips As Boolean = False
'    Private hadErrors As Boolean = False

'    Private pendingAction As CopyErrorAction? = Nothing
'    Private actionReceived As New AutoResetEvent(False)

'    Private errors As New List(Of CopyErrorEntry)
'    Private sw As Stopwatch

'    Public ReadOnly Property ErrorList As List(Of CopyErrorEntry)
'        Get
'            Return errors
'        End Get
'    End Property

'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source
'        destPath = dest

'        skipAll = False
'        hadSkips = False
'        hadErrors = False
'        pendingAction = Nothing
'        errors.Clear()
'        bytesCopied = 0
'        filesDone = 0

'        PreScan()

'        sw = Stopwatch.StartNew()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Public Sub SetErrorAction(action As CopyErrorAction)
'        pendingAction = action
'        actionReceived.Set()
'    End Sub

'    Private Sub PreScan()
'        files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).ToList()
'        totalFiles = files.Count
'        totalBytes = 0

'        For Each f In files
'            Dim fi As New FileInfo(f)
'            totalBytes += fi.Length
'        Next
'    End Sub

'    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
'        Dim buffer(64 * 1024 - 1) As Byte

'        For Each file In files
'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'            Dim target = Path.Combine(destPath, relative)

'            Try
'                Directory.CreateDirectory(Path.GetDirectoryName(target))

'                Using src As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
'                    Using dst As New FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None)
'                        Dim read As Integer
'                        Do
'                            If worker.CancellationPending Then
'                                e.Cancel = True
'                                Return
'                            End If

'                            read = src.Read(buffer, 0, buffer.Length)
'                            If read <= 0 Then Exit Do

'                            dst.Write(buffer, 0, read)
'                            bytesCopied += read

'                            ReportProgress(file)
'                        Loop
'                    End Using
'                End Using

'                filesDone += 1
'                ReportProgress(file)

'            Catch ex As Exception
'                hadErrors = True

'                If skipAll Then
'                    hadSkips = True
'                    errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})
'                    Continue For
'                End If

'                RaiseEvent ErrorOccurred(file, ex)

'                actionReceived.WaitOne()

'                Select Case pendingAction
'                    Case CopyErrorAction.Skip
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.SkipAll
'                        skipAll = True
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.Cancel
'                        e.Cancel = True
'                        Return
'                End Select
'            End Try
'        Next
'    End Sub

'    Private Sub ReportProgress(currentFile As String)
'        Dim percent As Integer = If(totalBytes > 0, CInt((bytesCopied * 100L) \ totalBytes), 0)

'        Dim speed As Double = 0
'        Dim eta As TimeSpan = TimeSpan.Zero

'        If sw IsNot Nothing AndAlso sw.Elapsed.TotalSeconds > 0.1 Then
'            speed = bytesCopied / sw.Elapsed.TotalSeconds
'            Dim remaining As Long = totalBytes - bytesCopied
'            If speed > 0 Then
'                eta = TimeSpan.FromSeconds(remaining / speed)
'            End If
'        End If

'        Dim info As New CopyProgressInfo With {
'            .Percent = percent,
'            .BytesCopied = bytesCopied,
'            .TotalBytes = totalBytes,
'            .FilesDone = filesDone,
'            .TotalFiles = totalFiles,
'            .CurrentFile = currentFile,
'            .SpeedBytesPerSec = speed,
'            .Eta = eta
'        }

'        worker.ReportProgress(percent, info)
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
'        Dim info = DirectCast(e.UserState, CopyProgressInfo)
'        RaiseEvent ProgressChanged(info)
'    End Sub

'    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        sw?.Stop()

'        If e.Cancelled Then
'            RaiseEvent Completed(False, hadSkips, hadErrors)
'        Else
'            RaiseEvent Completed(True, hadSkips, hadErrors)
'        End If
'    End Sub

'End Class



















'Imports System.ComponentModel
'Imports System.IO
'Imports System.Threading
'Imports System.Diagnostics

''===============================
''  SUPPORTING TYPES
''===============================
'Public Enum CopyErrorAction
'    Skip
'    SkipAll
'    Cancel
'End Enum

'Public Class CopyErrorEntry
'    Public Property FilePath As String
'    Public Property Message As String
'End Class

'Public Class CopyProgressInfo
'    Public Property Percent As Integer
'    Public Property BytesCopied As Long
'    Public Property TotalBytes As Long
'    Public Property FilesDone As Integer
'    Public Property TotalFiles As Integer
'    Public Property CurrentFile As String
'    Public Property SpeedBytesPerSec As Double
'    Public Property Eta As TimeSpan
'End Class

''===============================
''  COPY ENGINE
''===============================
'Public Class CopyEngine

'    ' EVENTS
'    Public Event ProgressChanged(info As CopyProgressInfo)
'    Public Event ErrorOccurred(file As String, ex As Exception)
'    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

'    Private WithEvents worker As New BackgroundWorker With {
'        .WorkerReportsProgress = True,
'        .WorkerSupportsCancellation = True
'    }

'    ' INTERNAL STATE
'    Private sourcePath As String
'    Private destPath As String
'    Private files As List(Of String)

'    Private totalBytes As Long
'    Private totalFiles As Integer

'    Private bytesCopied As Long
'    Private filesDone As Integer

'    Private skipAll As Boolean = False
'    Private hadSkips As Boolean = False
'    Private hadErrors As Boolean = False

'    Private pendingAction As CopyErrorAction? = Nothing
'    Private actionReceived As New AutoResetEvent(False)

'    Private errors As New List(Of CopyErrorEntry)
'    Private sw As Stopwatch

'    Private rootFolderName As String

'    Public ReadOnly Property ErrorList As List(Of CopyErrorEntry)
'        Get
'            Return errors
'        End Get
'    End Property

'    '===============================
'    '  START COPY
'    '===============================
'    Public Sub StartCopy(source As String, dest As String)
'        sourcePath = source.TrimEnd("\"c)
'        destPath = dest.TrimEnd("\"c)

'        rootFolderName = Path.GetFileName(sourcePath)

'        skipAll = False
'        hadSkips = False
'        hadErrors = False
'        pendingAction = Nothing
'        errors.Clear()
'        bytesCopied = 0
'        filesDone = 0

'        PreScan()

'        sw = Stopwatch.StartNew()
'        worker.RunWorkerAsync()
'    End Sub

'    Public Sub Cancel()
'        If worker.IsBusy Then
'            worker.CancelAsync()
'        End If
'    End Sub

'    Public Sub SetErrorAction(action As CopyErrorAction)
'        pendingAction = action
'        actionReceived.Set()
'    End Sub

'    '===============================
'    '  PRE-SCAN (Explorer behavior)
'    '===============================
'    Private Sub PreScan()
'        files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).ToList()
'        totalFiles = files.Count
'        totalBytes = 0

'        For Each f In files
'            Dim fi As New FileInfo(f)
'            totalBytes += fi.Length
'        Next
'    End Sub

'    '===============================
'    '  WORKER THREAD
'    '===============================
'    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
'        Dim buffer(64 * 1024 - 1) As Byte

'        For Each file In files

'            If worker.CancellationPending Then
'                e.Cancel = True
'                Return
'            End If

'            ' Build relative path INCLUDING the root folder
'            Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
'            Dim target = Path.Combine(destPath, rootFolderName, relative)

'            Try
'                Directory.CreateDirectory(Path.GetDirectoryName(target))

'                Using src As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
'                    Using dst As New FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None)

'                        Dim read As Integer
'                        Do
'                            If worker.CancellationPending Then
'                                e.Cancel = True
'                                Return
'                            End If

'                            read = src.Read(buffer, 0, buffer.Length)
'                            If read <= 0 Then Exit Do

'                            dst.Write(buffer, 0, read)
'                            bytesCopied += read

'                            ReportProgress(file)
'                        Loop

'                    End Using
'                End Using

'                filesDone += 1
'                ReportProgress(file)

'            Catch ex As Exception
'                hadErrors = True

'                If skipAll Then
'                    hadSkips = True
'                    errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})
'                    Continue For
'                End If

'                RaiseEvent ErrorOccurred(file, ex)

'                actionReceived.WaitOne()

'                Select Case pendingAction
'                    Case CopyErrorAction.Skip
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.SkipAll
'                        skipAll = True
'                        hadSkips = True
'                        errors.Add(New CopyErrorEntry With {.FilePath = file, .Message = ex.Message})

'                    Case CopyErrorAction.Cancel
'                        e.Cancel = True
'                        Return
'                End Select
'            End Try

'        Next
'    End Sub

'    '===============================
'    '  PROGRESS REPORTING
'    '===============================
'    Private Sub ReportProgress(currentFile As String)
'        Dim percent As Integer = If(totalBytes > 0, CInt((bytesCopied * 100L) \ totalBytes), 0)

'        Dim speed As Double = 0
'        Dim eta As TimeSpan = TimeSpan.Zero

'        If sw IsNot Nothing AndAlso sw.Elapsed.TotalSeconds > 0.1 Then
'            speed = bytesCopied / sw.Elapsed.TotalSeconds
'            Dim remaining As Long = totalBytes - bytesCopied
'            If speed > 0 Then
'                eta = TimeSpan.FromSeconds(remaining / speed)
'            End If
'        End If

'        Dim info As New CopyProgressInfo With {
'            .Percent = percent,
'            .BytesCopied = bytesCopied,
'            .TotalBytes = totalBytes,
'            .FilesDone = filesDone,
'            .TotalFiles = totalFiles,
'            .CurrentFile = currentFile,
'            .SpeedBytesPerSec = speed,
'            .Eta = eta
'        }

'        worker.ReportProgress(percent, info)
'    End Sub

'    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
'        Dim info = DirectCast(e.UserState, CopyProgressInfo)
'        RaiseEvent ProgressChanged(info)
'    End Sub

'    '===============================
'    '  COMPLETION
'    '===============================
'    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
'        sw?.Stop()

'        If e.Cancelled Then
'            RaiseEvent Completed(False, hadSkips, hadErrors)
'        Else
'            RaiseEvent Completed(True, hadSkips, hadErrors)
'        End If
'    End Sub

'End Class






























Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports System.Diagnostics

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

    ' EVENTS
    Public Event ProgressChanged(info As CopyProgressInfo)
    Public Event ErrorOccurred(file As String, ex As Exception)
    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

    Private WithEvents worker As New BackgroundWorker With {
        .WorkerReportsProgress = True,
        .WorkerSupportsCancellation = True
    }

    ' INTERNAL STATE
    Private sourcePath As String
    Private destPath As String
    Private files As List(Of String)

    Private totalBytes As Long
    Private totalFiles As Integer

    Private bytesCopied As Long
    Private filesDone As Integer

    Private skipAll As Boolean = False
    Private hadSkips As Boolean = False
    Private hadErrors As Boolean = False

    Private pendingAction As CopyErrorAction? = Nothing
    Private actionReceived As New AutoResetEvent(False)

    Private errors As New List(Of CopyErrorEntry)
    Private sw As Stopwatch

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
        destPath = dest.TrimEnd("\"c)

        skipAll = False
        hadSkips = False
        hadErrors = False
        pendingAction = Nothing
        errors.Clear()
        bytesCopied = 0
        filesDone = 0

        '===========================
        ' DETECT FILE OR FOLDER
        '===========================
        If File.Exists(sourcePath) Then
            ' SINGLE FILE
            files = New List(Of String) From {sourcePath}
            totalFiles = 1
            totalBytes = New FileInfo(sourcePath).Length
            rootFolderName = ""   ' No wrapping folder for single file

        ElseIf Directory.Exists(sourcePath) Then
            ' FOLDER
            rootFolderName = Path.GetFileName(sourcePath)
            PreScan()

        Else
            Throw New IOException("Source path does not exist.")
        End If

        sw = Stopwatch.StartNew()
        worker.RunWorkerAsync()
    End Sub

    Public Sub Cancel()
        If worker.IsBusy Then
            worker.CancelAsync()
        End If
    End Sub

    Public Sub SetErrorAction(action As CopyErrorAction)
        pendingAction = action
        actionReceived.Set()
    End Sub

    '===============================
    '  PRE-SCAN (Explorer behavior)
    '===============================
    'Private Sub PreScan()
    '    files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).ToList()
    '    totalFiles = files.Count
    '    totalBytes = 0

    '    For Each f In files
    '        Dim fi As New FileInfo(f)
    '        totalBytes += fi.Length
    '    Next
    'End Sub


    Private Sub PreScan()
        files = New List(Of String)
        totalBytes = 0
        totalFiles = 0

        Try
            ScanFolderSafe(sourcePath)
        Catch
            ' Never let PreScan crash the copy
        End Try
    End Sub

    Private Sub ScanFolderSafe(folder As String)
        Dim subFiles As String() = {}
        Dim subDirs As String() = {}

        Try
            subFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
        Catch ex As Exception
            ' Protected folder — record and skip
            errors.Add(New CopyErrorEntry With {
            .FilePath = folder,
            .Message = ex.Message
        })
            hadErrors = True
            Return
        End Try

        For Each f In subFiles
            files.Add(f)
            Try
                totalBytes += New FileInfo(f).Length
            Catch
                ' Ignore unreadable files
            End Try
        Next

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
    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
        Dim buffer(64 * 1024 - 1) As Byte

        For Each file In files

            If worker.CancellationPending Then
                e.Cancel = True
                Return
            End If

            '===========================
            ' BUILD TARGET PATH
            '===========================
            Dim target As String

            If rootFolderName = "" Then
                ' SINGLE FILE
                target = Path.Combine(destPath, Path.GetFileName(file))
            Else
                ' FOLDER COPY
                Dim relative = file.Substring(sourcePath.Length).TrimStart("\"c)
                target = Path.Combine(destPath, rootFolderName, relative)
            End If

            Try
                Directory.CreateDirectory(Path.GetDirectoryName(target))

                Using src As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using dst As New FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None)

                        Dim read As Integer
                        Do
                            If worker.CancellationPending Then
                                e.Cancel = True
                                Return
                            End If

                            read = src.Read(buffer, 0, buffer.Length)
                            If read <= 0 Then Exit Do

                            dst.Write(buffer, 0, read)
                            bytesCopied += read

                            ReportProgress(file)
                        Loop

                    End Using
                End Using

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

                actionReceived.WaitOne()

                Select Case pendingAction
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
        Dim percent As Integer = If(totalBytes > 0, CInt((bytesCopied * 100L) \ totalBytes), 0)

        Dim speed As Double = 0
        Dim eta As TimeSpan = TimeSpan.Zero

        If sw IsNot Nothing AndAlso sw.Elapsed.TotalSeconds > 0.1 Then
            speed = bytesCopied / sw.Elapsed.TotalSeconds
            Dim remaining As Long = totalBytes - bytesCopied
            If speed > 0 Then
                eta = TimeSpan.FromSeconds(remaining / speed)
            End If
        End If

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

        worker.ReportProgress(percent, info)
    End Sub

    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles worker.ProgressChanged
        Dim info = DirectCast(e.UserState, CopyProgressInfo)
        RaiseEvent ProgressChanged(info)
    End Sub

    '===============================
    '  COMPLETION
    '===============================
    Private Sub worker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
        sw?.Stop()

        If e.Cancelled Then
            RaiseEvent Completed(False, hadSkips, hadErrors)
        Else
            RaiseEvent Completed(True, hadSkips, hadErrors)
        End If
    End Sub

End Class
