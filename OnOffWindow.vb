Public Class OnOffWindow
    Public Sub New(StatelessRegistries As String())

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim connections As New Reports.Connections
        OPConnection = connections.OPConnection
        For Each registry As String In StatelessRegistries
            OnOffInterface.Rows.Add({registry})
        Next
    End Sub

    Private Function InsertRegistry(registry As String, windowstate As String, datenow As DateTime) As Integer
        Dim saveRegistry As New ADODB.Command
        saveRegistry.ActiveConnection = OPConnection
        saveRegistry.CommandText = $"
INSERT INTO [opreports].[dbo].[tvr_onoffwindow]
           ([registry]
           ,[windowstate]
           ,[created])
     VALUES
           ('{registry}'
           ,'{windowstate}'
           ,'{datenow}'
           )
"
        saveRegistry.Execute()
    End Function

    Private OPConnection As ADODB.Connection
    Private OnOffKeys As Dictionary(Of Integer,String)
    Private Function RetrieveData(statelessRegistries() As String) As DataTable
        Dim getRegistrywithKey As New ADODB.Command

    End Function

    Private Registries As DataTable

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        OPConnection.Open()
        OPConnection.BeginTrans()
        Try
            Dim datenow As DateTime = Date.Now
            For Each row As DataGridViewRow In OnOffInterface.Rows
                InsertRegistry(row.Cells(0).Value, row.Cells(1).Value, datenow)
            Next
            OPConnection.CommitTrans()
        Catch ex As Exception
            MsgBox(ex.Message)
            OPConnection.RollbackTrans()
        End Try
        OPConnection.Close()

        Me.Dispose()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.Dispose()
    End Sub
End Class