# Event‑Driven Non‑Blocking Copy Engine




**Event‑Driven Non‑Blocking Copy Engine** is a small Windows Forms project that demonstrates how to build a responsive file/folder copy subsystem. It validates sources, blocks protected system paths, shows Explorer‑style overwrite and folder‑merge dialogs, and performs the copy work on a background thread while reporting progress via events.

<img width="1280" height="640" alt="010" src="https://github.com/user-attachments/assets/2ec63830-eb2b-4d7d-9518-702e2eb12f18" />

---




## Features
- **Non‑blocking copy** using a background worker engine and events.  
- **Protected path policy** that prevents copying system and kernel‑locked files.  
- **Explorer‑style UX**: file overwrite confirmation and Windows‑7‑style folder merge dialog.  
- **Progress reporting** with bytes, percent, ETA, and file counts.  
- **Error handling** with Skip, Skip All, and Cancel semantics exposed via events.  
- **Cancellation support** so long operations stop cooperatively.


<img width="1280" height="640" alt="012" src="https://github.com/user-attachments/assets/6395a082-5935-4842-b8c1-1d50004421de" />

---


### Before — blocking copy (what freezes the UI)

**Problem:** this runs on the UI thread and blocks message processing while files copy.

```vbnet


    ' Synchronous, blocking example inside a button click handler
    Private Sub BtnStart_Click(sender As Object, e As EventArgs) _
        Handles btnStart.Click

        Dim sourceDirectory = txtSource.Text.Trim()
        Dim destinationDirectory = txtDest.Text.Trim()

        ' Quick validation omitted for brevity

        If IO.File.Exists(sourceDirectory) Then

            ' This call blocks the UI until the copy finishes
            IO.File.Copy(sourceDirectory,
                         IO.Path.Combine(destinationDirectory,
                                         IO.Path.GetFileName(sourceDirectory)),
                         True)

        ElseIf IO.Directory.Exists(sourceDirectory) Then

            ' Recursive synchronous copy — also blocks the UI
            DirectoryCopy(sourceDirectory,
                          IO.Path.Combine(destinationDirectory,
                                          IO.Path.GetFileName(sourceDirectory)))

        End If

        MessageBox.Show("Copy finished") ' UI was frozen until this point

    End Sub

    Private Sub DirectoryCopy(sourceDirectory As String,
                              destinationDirectory As String)

        IO.Directory.CreateDirectory(destinationDirectory)

        For Each file In IO.Directory.GetFiles(sourceDirectory)

            IO.File.Copy(file,
                         IO.Path.Combine(destinationDirectory,
                                         IO.Path.GetFileName(file)),
                         True)

        Next

        For Each directory In IO.Directory.GetDirectories(sourceDirectory)

            ' Recursive call to copy subdirectories
            DirectoryCopy(directory,
                          IO.Path.Combine(destinationDirectory,
                                          IO.Path.GetFileName(directory)))

        Next

    End Sub




```

---

### After — non‑blocking, event‑driven copy (UI stays responsive)

**Approach:** start the copy engine from the UI thread, subscribe to its events, and update the UI only from the UI thread. The engine does the heavy work on a background thread.

```vbnet





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

    ' Events
    Public Event ProgressChanged(info As CopyProgressInfo)
    Public Event ErrorOccurred(file As String, ex As Exception)
    Public Event Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

    Private WithEvents worker As New BackgroundWorker With {
        .WorkerReportsProgress = True,
        .WorkerSupportsCancellation = True
    }

    Private sourcePath As String
    Private destPath As String

    Private files As List(Of String)
    Private folders As List(Of String)

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

        Dim subFiles As String() = {}
        Dim subDirs As String() = {}

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
    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
        Dim buffer(64 * 1024 - 1) As Byte

        ' Create all folders first (for folder copies)
        If rootFolderName <> "" Then
            For Each folder In folders
                Dim relative = folder.Substring(sourcePath.Length).TrimStart("\"c)
                Dim targetFolder = Path.Combine(destPath, rootFolderName, relative)

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

            If worker.CancellationPending Then
                e.Cancel = True
                Return
            End If

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




```

---

### Key points for beginners

- **What blocks:** any long synchronous call on the UI thread (file I/O, heavy loops, network calls).  
- **Why non‑blocking matters:** the UI thread must keep processing messages so the app remains responsive.  
- **How to avoid blocking:** run long work on background threads or use asynchronous APIs; report progress via events or callbacks; marshal UI updates back to the UI thread with `Invoke`/`BeginInvoke`.  
- **Where decisions belong:** do quick validations and user confirmations on the UI thread before starting the engine; let the engine handle runtime conflicts and report them via events.  
- **Cancellation:** provide a `Cancel` method and check for cancellation inside the engine so long operations stop cooperatively.

---

### Small checklist to convert a blocking operation to non‑blocking

1. **Identify** the blocking call(s) on the UI thread.  
2. **Extract** the work into a background worker, `Task.Run`, or a dedicated engine class.  
3. **Raise events** for progress, errors, and completion.  
4. **Subscribe** to those events from the UI and marshal updates with `Invoke`.  
5. **Add cancellation** support and cooperative checks inside the worker.  
6. **Keep pre‑copy prompts synchronous** (overwrite/merge) so the engine never blocks waiting for user input.

---

## Quick Start
1. **Open the solution** in Visual Studio.  
2. Build the project (target .NET Framework compatible with Windows Forms).  
3. Run the app.  
4. Select a **Source** (file or folder) and a **Destination folder**.  
5. Click **Start**.  
6. Respond to any pre‑copy dialogs (overwrite or merge). The copy runs in the background and the UI remains responsive.

---

## Usage Notes for Beginners
- **What blocks** the UI: any long synchronous work on the UI thread (file I/O, heavy loops).  
- **How this app avoids blocking**: `CopyEngine` runs on a background thread and raises `ProgressChanged`, `ErrorOccurred`, and `Completed` events; the UI subscribes and marshals updates to the UI thread.  
- **Where decisions belong**: quick validation and overwrite/merge confirmations happen on the UI thread before the engine starts; runtime conflict resolution is handled by the engine via events.  
- **If you modify code**: keep UI updates on the UI thread (use `Invoke`/`BeginInvoke`) and avoid calling `.Wait()` or `.Result` on long operations.

---

## Architecture Overview
- **Form1** handles user input, validation, and pre‑copy dialogs.  
- **Protected path list** centralizes system and kernel‑locked paths and is checked by `IsProtectedPath(path As String)`.  
- **CopyEngine** performs scanning, creates folders, copies files in buffered chunks, reports progress, and raises events for errors and completion.  
- **CopyDialog** subscribes to engine events and displays progress without blocking the UI.

---

## Project origin  
I started this project intending to implement the simplest possible copy operation. That first implementation worked but it blocked the UI. After asking Copilot how to avoid freezing the app, I was pointed toward an event‑driven, non‑blocking design. I created a new repository and project called Event‑Driven Non‑Blocking Copy Engine, scaffolded the classes, pasted in the generated code, and ran the first copy test. What began as a tiny feature turned into a small subsystem: a background copy engine that reports progress via events, validates protected paths, and keeps the UI responsive.



## License
**MIT License**  
Copyright (c) 2026 Joseph W. Lumbley

Permission is granted to use, copy, modify, and distribute this software under the terms of the MIT License. See the LICENSE file for full text.

---


## Clones 







<img width="1920" height="1080" alt="013" src="https://github.com/user-attachments/assets/8aad5a1e-bcb2-422d-a7d5-ace4e1acd904" />






<img width="1920" height="1080" alt="008" src="https://github.com/user-attachments/assets/32ed786c-da2d-4aac-9d60-31fb1f5250ca" />




<img width="1920" height="1080" alt="007" src="https://github.com/user-attachments/assets/a648f5c9-e7db-4ee4-a250-601726350154" />



























