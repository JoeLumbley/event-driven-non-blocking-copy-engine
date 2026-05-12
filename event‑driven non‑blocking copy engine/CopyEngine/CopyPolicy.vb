'===============================
'  COPY POLICY
'===============================
Imports System.Threading

Public Class CopyPolicy
    Private pendingAction As CopyErrorAction?
    Private ReadOnly actionEvent As New AutoResetEvent(False)

    Public Sub SetAction(action As CopyErrorAction)
        SyncLock Me
            pendingAction = action
        End SyncLock
        actionEvent.Set()
    End Sub

    Public Function WaitForAction() As CopyErrorAction?
        actionEvent.WaitOne()
        SyncLock Me
            Return pendingAction
        End SyncLock
    End Function
End Class


