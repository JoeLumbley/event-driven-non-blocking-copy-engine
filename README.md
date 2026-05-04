# Event‑Driven Non‑Blocking Copy Engine



<img width="1254" height="635" alt="002" src="https://github.com/user-attachments/assets/1d97a189-325b-434f-afc2-632d4cfd68b8" />

**Event‑Driven Non‑Blocking Copy Engine** is a small Windows Forms project that demonstrates how to build a responsive file/folder copy subsystem. It validates sources, blocks protected system paths, shows Explorer‑style overwrite and folder‑merge dialogs, and performs the copy work on a background thread while reporting progress via events.

---

<img width="1280" height="640" alt="005" src="https://github.com/user-attachments/assets/387a8922-8da1-49af-9f41-119c920bca8a" />



## Features
- **Non‑blocking copy** using a background worker engine and events.  
- **Protected path policy** that prevents copying system and kernel‑locked files.  
- **Explorer‑style UX**: file overwrite confirmation and Windows‑7‑style folder merge dialog.  
- **Progress reporting** with bytes, percent, ETA, and file counts.  
- **Error handling** with Skip, Skip All, and Cancel semantics exposed via events.  
- **Cancellation support** so long operations stop cooperatively.

---


### Before — blocking copy (what freezes the UI)

**Problem:** this runs on the UI thread and blocks message processing while files copy.

```vbnet
' Synchronous, blocking example inside a button click handler
Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
    Dim src = txtSource.Text.Trim()
    Dim dst = txtDest.Text.Trim()

    ' Quick validation omitted for brevity

    If File.Exists(src) Then
        ' This call blocks the UI until the copy finishes
        File.Copy(src, Path.Combine(dst, Path.GetFileName(src)), True)
    ElseIf Directory.Exists(src) Then
        ' Recursive synchronous copy — also blocks the UI
        DirectoryCopy(src, Path.Combine(dst, Path.GetFileName(src)))
    End If

    MessageBox.Show("Copy finished") ' UI was frozen until this point
End Sub

Private Sub DirectoryCopy(sourceDir As String, destDir As String)
    Directory.CreateDirectory(destDir)
    For Each file In Directory.GetFiles(sourceDir)
        File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), True)
    Next
    For Each dir In Directory.GetDirectories(sourceDir)
        DirectoryCopy(dir, Path.Combine(destDir, Path.GetFileName(dir)))
    Next
End Sub
```

---

### After — non‑blocking, event‑driven copy (UI stays responsive)

**Approach:** start the copy engine from the UI thread, subscribe to its events, and update the UI only from the UI thread. The engine does the heavy work on a background thread.

```vbnet
' Start the engine and subscribe to events
Private engine As CopyEngine

Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
    Dim src = txtSource.Text.Trim()
    Dim dst = txtDest.Text.Trim()

    ' Perform quick, synchronous validation here (exists, protected paths, overwrite prompts)
    ' If validation passes, start the engine

    engine = New CopyEngine()
    AddHandler engine.ProgressChanged, AddressOf Engine_ProgressChanged
    AddHandler engine.ErrorOccurred, AddressOf Engine_ErrorOccurred
    AddHandler engine.Completed, AddressOf Engine_Completed

    engine.StartCopy(src, dst)

    ' Optionally show a non-blocking progress dialog that listens to the same events
    Dim dlg As New CopyDialog(engine)
    dlg.Show(Me)
End Sub

' Event handlers marshal updates to the UI thread
Private Sub Engine_ProgressChanged(info As CopyProgressInfo)
    If Me.InvokeRequired Then
        Me.BeginInvoke(New Action(Of CopyProgressInfo)(AddressOf Engine_ProgressChanged), info)
        Return
    End If

    ' Update progress bar and labels on UI thread
    progressBar.Value = Math.Min(100, info.Percent)
    lblStatus.Text = $"{info.CurrentFile} — {info.FilesDone}/{info.TotalFiles} ({info.Percent}%)"
    lblEta.Text = info.Eta.ToString("hh\:mm\:ss")
End Sub

Private Sub Engine_ErrorOccurred(file As String, ex As Exception)
    If Me.InvokeRequired Then
        Me.BeginInvoke(New Action(Of String, Exception)(AddressOf Engine_ErrorOccurred), file, ex)
        Return
    End If

    ' Show non-blocking error UI or queue user decision dialog
    Using errDlg As New CopyErrorDialog(file, ex)
        Dim action = errDlg.ShowDialog(Me)
        Select Case action
            Case DialogResult.Yes
                engine.SetErrorAction(CopyErrorAction.Skip)
            Case DialogResult.No
                engine.SetErrorAction(CopyErrorAction.SkipAll)
            Case DialogResult.Cancel
                engine.SetErrorAction(CopyErrorAction.Cancel)
        End Select
    End Using
End Sub

Private Sub Engine_Completed(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)
    If Me.InvokeRequired Then
        Me.BeginInvoke(New Action(Of Boolean, Boolean, Boolean)(AddressOf Engine_Completed), success, hadSkips, hadErrors)
        Return
    End If

    MessageBox.Show(If(success, "Copy completed", "Copy cancelled or failed"))
End Sub

' Cancel button example
Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
    engine?.Cancel()
End Sub
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
