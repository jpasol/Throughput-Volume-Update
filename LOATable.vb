Imports Throughput_Volume_Update

Public Class LOATable
    Inherits LOATableReport

    Private Structure Vessel
        Public Sub New(VesselName As String, LOA As Integer)
            Me.VesselName = VesselName
            Me.LOA = LOA
        End Sub
        Public Property VesselName As String
        Public Property LOA As Integer

    End Structure


    Public Sub New()
        MyBase.New

        Dim tempConnections As New Reports.Connections
        Me.N4Connection = tempConnections.N4Connection

        LOATable = New ThroughputVolumeDatabase
        LOATable = FormatLOATable()

        MyBase.SetDataSource(LOATable)


    End Sub

    Private Function FormatLOATable() As ThroughputVolumeDatabase
        Dim tempDataset As New ThroughputVolumeDatabase
        For Each vessel As Vessel In Vessels()
            tempDataset.LOA.AddLOARow(vessel.VesselName, vessel.LOA)
        Next

        Return tempDataset
    End Function

    Private Function Vessels() As List(Of Vessel)
        Vessels = New List(Of Vessel)
        For Each row As DataRow In getVesselandLOA.Rows
            Vessels.Add(New Vessel(row("VesselName").ToString.ToUpper, 0 & row("LOA")))
        Next
    End Function

    Private Function getVesselandLOA() As DataTable
        Dim vesselAndLoa As New ADODB.Command
        N4Connection.Open()
        vesselAndLoa.ActiveConnection = N4Connection
        vesselAndLoa.CommandText = $"
SELECT vsl.[name] as 'VesselName'
		,cast(round([loa_cm] / 100.0,0) as integer) as 'LOA'
  FROM [apex].[dbo].[vsl_vessels] vsl
 inner join vsl_Vessel_classes vcls
 on vcls.gkey = vsl.vesclass_gkey 

ORDER BY VesselName
"
        Dim tempDatatable As New DataTable
        Dim Adapter As New OleDb.OleDbDataAdapter
        Adapter.Fill(tempDatatable, vesselAndLoa.Execute)
        N4Connection.Close()
        Return tempDatatable
    End Function

    Private LOATable As ThroughputVolumeDatabase
    Private N4Connection As ADODB.Connection
End Class
