' Event-Driven Non-Blocking Copy Engine is a sample application demonstrating
' how to implement a file copy operation that is both non-blocking and
' event-driven.
' The main features include:
' - Non-blocking file copy
' - Event-driven architecture
' - Validation of source and destination paths
' - Overwrite and merge prompts

' MIT License
' Copyright(c) 2026 Joseph W. Lumbley

' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:

' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.

' Form1 is the main window where the user chooses a source and destination,
' runs validation, and then starts the non-blocking copy engine.

Public Class Form1

    ' Central copy engine instance so we can cancel or inspect it later.
    Private engine As CopyEngine

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

        Dim sourceDirectory As String = txtSource.Text
        Dim destinationDirectory As String = txtDest.Text

        ' Centralized validation
        Dim validation = Validator.Validate(Me, sourceDirectory, destinationDirectory)
        If Not validation.Success Then
            Return
        End If

        '===============================
        '  VALIDATION PASSED
        '===============================

        ' At this point:
        ' - The source and destination are valid.
        ' - The user has confirmed any overwrites/merges.
        ' - We can safely start the non-blocking copy engine.

        sourceDirectory = validation.SourcePath
        destinationDirectory = validation.DestinationPath

        ' Disable the Start button and show the Cancel button.
        btnStart.Enabled = False
        btnCancel.Visible = True

        ' Create the engine and keep a reference so we can cancel later.
        engine = New CopyEngine()

        AddHandler engine.Completed, AddressOf OnCompleted

        ' Start the copy on a background thread.
        engine.StartCopy(sourceDirectory, destinationDirectory)

        ' Show a modeless progress dialog that listens to engine events.
        Dim copyDialog As New CopyDialog(engine)
        copyDialog.Show(Me)

    End Sub

    '===============================
    '  CANCEL BUTTON
    '===============================
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        engine?.Cancel()
    End Sub

    Private Sub OnCompleted(success As Boolean, hadSkips As Boolean, hadErrors As Boolean)

        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of Boolean, Boolean, Boolean)(AddressOf OnCompleted), success, hadSkips, hadErrors)
            Return
        End If

        If Not btnStart.Enabled Then btnStart.Enabled = True
        If btnCancel.Visible Then btnCancel.Visible = False

    End Sub

End Class
