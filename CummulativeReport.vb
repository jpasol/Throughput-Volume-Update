Imports Throughput_Volume_Update

Public Class CummulativeReport
    Implements ICummulativeReport

    Public Sub New(ShippingLines As String)
        GenerateLines(ShippingLines)
        FormatCummulativeShippingVolumes(CummulativeShippingVolumes)
        FormatCummulativeActualShippingVolumes(CummulativeActualShippingVolumes)
        FormatCummulativeOwnedShippingVolumes(CummulativeOwnedShippingVolumes)
        FormatCummulativeCoLoadShippingVolumes(CummulativeCoLoadShippingVolumes)

        Report = New CummulativeThroughputReport
        Report.SetDataSource(CummulativeThroughputReport)
        Report.Subreports.Item("CummulativeVolumeReport.rpt").SetDataSource(CummulativeVolumeReportData)
        Report.Subreports.Item("CummulativeVolumeActualReport.rpt").SetDataSource(CummulativeVolumeReportActualData)
        Report.Subreports.Item("CummulativeVolumeOwnedReport.rpt").SetDataSource(CummulativeVolumeReportOwnedData)
        Report.Subreports.Item("CummulativeVolumeCoLoadReport.rpt").SetDataSource(CummulativeVolumeReportCoLoadData)

    End Sub

    Private Sub GenerateLines(shippingLines As String)
        If shippingLines = "" Then
            For Each line As String In OwnerShippingLines.Keys
                CummulativeThroughputReport.Lines.AddLinesRow(line)
            Next
        Else
            CummulativeThroughputReport.Lines.AddLinesRow(shippingLines)
        End If
    End Sub

    Private ReadOnly CummulativeThroughputReport As New Throughput
    Private ReadOnly CummulativeVolumeReportData As New Throughput
    Private ReadOnly CummulativeVolumeReportActualData As New Throughput
    Private ReadOnly CummulativeVolumeReportOwnedData As New Throughput
    Private ReadOnly CummulativeVolumeReportCoLoadData As New Throughput

    Public Property Report As CummulativeThroughputReport Implements ICummulativeReport.Report

    Public Function OwnerShippingLines() As Dictionary(Of String, String())
        Dim lines As New Dictionary(Of String, String())
        Using Reader As New Microsoft.VisualBasic.FileIO.TextFieldParser($"{Application.StartupPath}/ShippingLines.csv")
            Reader.TextFieldType = FileIO.FieldType.Delimited
            Reader.SetDelimiters(",")
            While Not Reader.EndOfData
                Dim current As String() = Reader.ReadFields()
                lines.Add(current(0), current)
            End While
        End Using
        Return lines
    End Function

    Public Function CummulativeShippingVolumes() As List(Of ShippingVolume) Implements ICummulativeReport.CummulativeShippingVolumes
        Dim shippingvolumes As New List(Of ShippingVolume)
        For Each line As String In OwnerShippingLines.Keys
            For month As Integer = 1 To 12
                shippingvolumes.Add(New ShippingVolume(line, OwnerShippingLines.Item(line), month, Date.Now.Year, Remarks:="Throughput"))
            Next
        Next
        Return shippingvolumes
    End Function

    Public Function CummulativeActualShippingVolumes() As List(Of ShippingVolume) Implements ICummulativeReport.CummulativeActualShippingVolumes
        Dim shippingvolumes As New List(Of ShippingVolume)
        For Each line As String In OwnerShippingLines.Keys
            For month As Integer = 1 To 12
                shippingvolumes.Add(New ShippingVolume(line, OwnerShippingLines.Item(line), month, Date.Now.Year))
            Next
        Next
        Return shippingvolumes
    End Function

    Public Function CummulativeOwnedShippingVolumes() As List(Of ShippingVolume) Implements ICummulativeReport.CummulativeOwnedShippingVolumes
        Dim shippingvolumes As New List(Of ShippingVolume)
        For Each line As String In OwnerShippingLines.Keys
            For month As Integer = 1 To 12
                shippingvolumes.Add(New ShippingVolume(line, OwnerShippingLines.Item(line), month, Date.Now.Year, Remarks:="Owner"))
            Next
        Next
        Return shippingvolumes
    End Function

    Public Function CummulativeCoLoadShippingVolumes() As List(Of ShippingVolume) Implements ICummulativeReport.CummulativeCoLoadShippingVolumes
        Dim shippingvolumes As New List(Of ShippingVolume)
        For Each line As String In OwnerShippingLines.Keys
            For month As Integer = 1 To 12
                shippingvolumes.Add(New ShippingVolume(line, OwnerShippingLines.Item(line), month, Date.Now.Year, Remarks:="Co-Load"))
            Next
        Next
        Return shippingvolumes
    End Function

    Public Sub FormatCummulativeShippingVolumes(CummulativeShippingVolumes As List(Of ShippingVolume)) Implements ICummulativeReport.FormatCummulativeShippingVolumes
        For Each shippingvolume As ShippingVolume In CummulativeShippingVolumes
            Dim tempRow As DataRow
            tempRow = CummulativeVolumeReportData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(shippingvolume)
            CummulativeVolumeReportData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub

    Private Function VolumeEntry(shippingvolume As ShippingVolume) As Object()
        With shippingvolume
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

    Public Sub FormatCummulativeActualShippingVolumes(CummulativeActualShippingVolumes As List(Of ShippingVolume)) Implements ICummulativeReport.FormatCummulativeActualShippingVolumes
        For Each shippingvolume As ShippingVolume In CummulativeActualShippingVolumes
            Dim tempRow As DataRow
            tempRow = CummulativeVolumeReportActualData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(shippingvolume)
            CummulativeVolumeReportActualData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub

    Public Sub FormatCummulativeOwnedShippingVolumes(CummulativeOwnedShippingVolumes As List(Of ShippingVolume)) Implements ICummulativeReport.FormatCummulativeOwnedShippingVolumes
        For Each shippingvolume As ShippingVolume In CummulativeOwnedShippingVolumes
            Dim tempRow As DataRow
            tempRow = CummulativeVolumeReportOwnedData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(shippingvolume)
            CummulativeVolumeReportOwnedData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub

    Public Sub FormatCummulativeCoLoadShippingVolumes(CummulativeCoLoadShippingVolumes As List(Of ShippingVolume)) Implements ICummulativeReport.FormatCummulativeCoLoadShippingVolumes
        For Each shippingvolume As ShippingVolume In CummulativeCoLoadShippingVolumes
            Dim tempRow As DataRow
            tempRow = CummulativeVolumeReportCoLoadData.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(shippingvolume)
            CummulativeVolumeReportCoLoadData.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub
End Class
