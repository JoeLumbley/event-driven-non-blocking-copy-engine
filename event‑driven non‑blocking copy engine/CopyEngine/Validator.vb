Imports System.IO

Public Class ValidatorResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property SourcePath As String
    Public Property DestinationPath As String
End Class

Public Class Validator

    Public Shared Function Validate(owner As IWin32Window,
                                    rawSource As String,
                                    rawDest As String) As ValidatorResult

        ' Run pure logic (no UI)
        Dim outcome = ValidationEngine.ValidatePaths(rawSource, rawDest)

        Dim result As New ValidatorResult With {
            .Success = False,
            .SourcePath = outcome.NormalizedSource,
            .DestinationPath = outcome.NormalizedDestination
        }

        ' HARD FAIL (no prompts allowed)
        If Not outcome.IsValid Then
            If Not String.IsNullOrEmpty(outcome.ErrorMessage) Then
                Using dlg As New ValidationErrorDialog(outcome.ErrorMessage)
                    dlg.ShowDialog(owner)
                End Using
            End If
            Return result
        End If

        ' FILE OVERWRITE PROMPT
        If outcome.RequiresFileOverwritePrompt Then
            Dim fileName = Path.GetFileName(outcome.NormalizedSource)
            Dim msg =
                "The file '" & fileName & "' already exists in the destination folder." &
                Environment.NewLine &
                "Do you want to overwrite it?"

            Dim overwrite = MessageBox.Show(owner,
                                            msg,
                                            "Confirm File Replace",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning)

            If overwrite = DialogResult.No Then
                Return result
            End If
        End If

        ' FOLDER MERGE PROMPT
        If outcome.RequiresFolderMergePrompt Then
            Dim folderName = Path.GetFileName(outcome.NormalizedSource.TrimEnd("\"c))

            Using dlg As New FolderMergeDialog(folderName)
                If dlg.ShowDialog(owner) = DialogResult.No Then
                    Return result
                End If
            End Using
        End If

        ' SUCCESS
        result.Success = True
        Return result

    End Function

End Class
