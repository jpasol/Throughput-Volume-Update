Imports Throughput_Volume_Update

Public Class VolumeBreakdown
    Inherits VolumeBreakdownReport

    Private ThroughputVolumeData As New Throughput
    Private BerthWindowVolumeData As New Throughput

    Public Sub New()
        MyBase.New

        FormatThroughputVolumes(ThroughputVolumes)
        FormatBerthWindowVolumes(BerthWindowVolumes)

        MyBase.Subreports.Item("ThroughputVolume.rpt").SetDataSource(ThroughputVolumeData)
        MyBase.Subreports.Item("TotalBerthWindowVolume.rpt").SetDataSource(BerthWindowVolumeData)

    End Sub

    Private Function BerthWindowVolumes() As List(Of ShippingVolume)
        BerthWindowVolumes = New List(Of ShippingVolume)
        For month As Integer = 1 To 12
            For berthwindow As Integer = 1 To 2
                BerthWindowVolumes.Add(New ShippingVolume("All",
                                                      {""},
                                                      month,
                                                      2018,
                                                      berthwindow,
                                                      Remarks:="",
                                                      UnitRemarks:="Throughput",
                                                      GetAll:=True))
            Next
        Next
    End Function

    Private Function ThroughputVolumes() As List(Of ShippingVolume)
        ThroughputVolumes = New List(Of ShippingVolume)
        For month As Integer = 1 To 12
            ThroughputVolumes.Add(New ShippingVolume("All",
                                                      {""},
                                                      month,
                                                      2018,
                                                      0,
                                                      Remarks:="",
                                                      UnitRemarks:="Throughput",
                                                      GetAll:=True))
        Next
    End Function

    Private Sub FormatBerthWindowVolumes(berthWindowVolumes As List(Of ShippingVolume))
        For Each berthvolume As ShippingVolume In berthWindowVolumes
            Dim tempRow As DataRow
            tempRow = BerthWindowVolumeData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(berthvolume)
            BerthWindowVolumeData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub

    Private Function VolumeEntry(shippingVolume As ShippingVolume) As Object()
        With shippingVolume
            Dim entry As New List(Of String)
            entry.Add(.Name)
            entry.Add(.BerthWindow)
            entry.Add(.Month)
            entry.Add(.Year)

            For Each bound As String In {"Inbound", "Outbound"}
                For Each freight As String In {"FCL", "MTY"}
                    For Each size As Integer In {20, 40, 45}
                        entry.Add(.Units(bound, freight, size))
                    Next
                Next
            Next

            Return entry.ToArray
        End With
    End Function

    Private Sub FormatThroughputVolumes(throughputVolumes As List(Of ShippingVolume))
        For Each throughputVolume As ShippingVolume In throughputVolumes
            Dim tempRow As DataRow
            tempRow = ThroughputVolumeData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(throughputVolume)
            ThroughputVolumeData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub
End Class
