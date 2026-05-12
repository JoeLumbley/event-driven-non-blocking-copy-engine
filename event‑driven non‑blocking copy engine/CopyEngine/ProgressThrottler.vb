'===============================
'  PROGRESS THROTTLER
'===============================
Public Class ProgressThrottler
    Private lastUpdate As DateTime = DateTime.MinValue
    Private ReadOnly intervalMs As Integer

    Public Sub New(Optional intervalMilliseconds As Integer = 200)
        intervalMs = Math.Max(50, intervalMilliseconds)
    End Sub

    Public Function ShouldReport() As Boolean
        Dim now = DateTime.UtcNow
        If (now - lastUpdate).TotalMilliseconds >= intervalMs Then
            lastUpdate = now
            Return True
        End If
        Return False
    End Function
End Class
