# Event‑Driven Non‑Blocking Copy Engine


 <img width="640" height="320" alt="003" src="https://github.com/user-attachments/assets/7462800a-a680-47a7-8fec-d282c3947518" />

**Event‑Driven Non‑Blocking Copy Engine** is a small Windows Forms project that demonstrates how to build a responsive file/folder copy subsystem. It validates sources, blocks protected system paths, shows Explorer‑style overwrite and folder‑merge dialogs, and performs the copy work on a background thread while reporting progress via events.

---
<img width="640" height="320" alt="006" src="https://github.com/user-attachments/assets/75dfede5-9597-40ed-8e73-2ce2b5103dd5" />

## Features
- **Non‑blocking copy** using a background worker engine and events.  
- **Protected path policy** that prevents copying system and kernel‑locked files.  
- **Explorer‑style UX**: file overwrite confirmation and Windows‑7‑style folder merge dialog.  
- **Progress reporting** with bytes, percent, ETA, and file counts.  
- **Error handling** with Skip, Skip All, and Cancel semantics exposed via events.  
- **Cancellation support** so long operations stop cooperatively.

---

## Quick Start
1. **Open the solution** in Visual Studio.  
2. Build the project (target .NET Framework compatible with Windows Forms).  
3. Run the app.  
4. Select a **Source** (file or folder) and a **Destination folder**.  
5. Click **Start**.  
6. Respond to any pre‑copy dialogs (overwrite or merge). The copy runs in the background and the UI remains responsive.

---

### Usage Notes for Beginners
- **What blocks** the UI: any long synchronous work on the UI thread (file I/O, heavy loops).  
- **How this app avoids blocking**: `CopyEngine` runs on a background thread and raises `ProgressChanged`, `ErrorOccurred`, and `Completed` events; the UI subscribes and marshals updates to the UI thread.  
- **Where decisions belong**: quick validation and overwrite/merge confirmations happen on the UI thread before the engine starts; runtime conflict resolution is handled by the engine via events.  
- **If you modify code**: keep UI updates on the UI thread (use `Invoke`/`BeginInvoke`) and avoid calling `.Wait()` or `.Result` on long operations.

---

### Architecture Overview
- **Form1** handles user input, validation, and pre‑copy dialogs.  
- **Protected path list** centralizes system and kernel‑locked paths and is checked by `IsProtectedPath(path As String)`.  
- **CopyEngine** performs scanning, creates folders, copies files in buffered chunks, reports progress, and raises events for errors and completion.  
- **CopyDialog** subscribes to engine events and displays progress without blocking the UI.

---

### License
**MIT License**  
Copyright (c) 2026 Joseph W. Lumbley

Permission is granted to use, copy, modify, and distribute this software under the terms of the MIT License. See the LICENSE file for full text.

---
