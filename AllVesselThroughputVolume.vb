Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator
Imports System.Threading.Tasks

Public Class AllVesselThroughputVolume
    'Inherits MonthlyThroughputVolume
    Public Sub New()

        'MyBase.New(Month, Year)

        AllVesselThroughputVolumeDatabase = New ThroughputVolumeDatabase

        Dim tempVMRDates As List(Of Date) = GetVMRDates()
        MonthlyThroughputVolumeList = New List(Of MonthlyThroughputVolume)

        Dim maxCores As New ParallelOptions
        maxCores.MaxDegreeOfParallelism = Environment.ProcessorCount

        Parallel.ForEach(tempVMRDates.AsEnumerable.Select(Function(dte) dte.Year).Distinct,
                         maxCores,
        Sub(vmrYear)
            Parallel.ForEach(tempVMRDates.AsEnumerable.Where(Function(dte) dte.Year = vmrYear).
                                                Select(Function(dte) dte.Month).Distinct,
                             maxCores,
                             Sub(vmrMonth)
                                 MonthlyThroughputVolumeList.Add(New MonthlyThroughputVolume(vmrMonth, vmrYear))
                             End Sub)
        End Sub
        )

        'For Each vmrYear As Integer In tempVMRDates.AsEnumerable.Select(Function(dte) dte.Year).Distinct
        '        For Each vmrMonth As Integer In tempVMRDates.AsEnumerable.Where(Function(dte) dte.Year = vmrYear).
        '                                        Select(Function(dte) dte.Month).Distinct

        '            MonthlyThroughputVolumeList.Add(New MonthlyThroughputVolume(vmrMonth, vmrYear))
        '        Next
        '    Next


        '    FormatReport(MyBase.VesselVolumes)

        Parallel.ForEach(MonthlyThroughputVolumeList,
                         maxCores,
                         Sub(volume)
                             FormatReport(volume.VesselVolumes)
                         End Sub)

        'For Each volume As MonthlyThroughputVolume In MonthlyThroughputVolumeList
        '        FormatReport(volume.VesselVolumes)
        '    Next

        '    Report = New AllVesselThroughputVolumeReport
        '    Report.SetDataSource(AllVesselThroughputVolumeDatabase)
        '    Report.SetParameterValue("month", Month)
        '    Report.SetParameterValue("year", Year)

    End Sub


    Private Sub FormatReport(vesselVolumes As List(Of VesselVolume))
        Dim freightkinds As String() = {"FCL", "MTY"}
        Dim bounds As String() = {"Inbound", "Outbound"}
        Dim sizes As Integer() = {20, 40, 45}

        For Each volume As VesselVolume In vesselVolumes
            Dim tempRow As ThroughputVolumeDatabase.AllVesselThroughputVolumeDataRow
            tempRow = AllVesselThroughputVolumeDatabase.AllVesselThroughputVolumeData.NewRow
            tempRow.TEU = 0 'initialize
            Dim counter As Integer = 0
            For Each bound In bounds
                For Each freight In freightkinds
                    tempRow.TEU += volume.TEU(bound, freight)
                    For Each size In sizes
                        tempRow.Item(counter) = volume.Units(bound, freight, size)
                        counter += 1
                    Next
                Next
            Next
            tempRow.Gearbox20 = volume.Gearbox(20)
            tempRow.Gearbox40 = volume.Gearbox(20)
            tempRow.Hatchcover = volume.HatchCover
            tempRow.ShiftEmpty = volume.Shifting("MTY")
            tempRow.ShiftFull = volume.Shifting("FCL")
            tempRow.VesselName = volume.VesselName
            tempRow.Voyage = volume.Voyage
            tempRow.Registry = volume.Registry
            tempRow.ATA = volume.ATA
            tempRow.ATD = volume.ATD
            tempRow.WindowState = volume.WindowState
            tempRow.Remarks = volume.Remarks
            tempRow.Line = volume.Line
            tempRow.BerthWindow = volume.BerthWindow
            tempRow.VesselVolume = volume.OwnerVessel
            tempRow.Service = volume.Service
            tempRow.LOA = volume.LOA
            tempRow.Month = volume.Month
            tempRow.Year = volume.Year
            tempRow.Seguido = volume.Seguido

            AllVesselThroughputVolumeDatabase.AllVesselThroughputVolumeData.AddAllVesselThroughputVolumeDataRow(tempRow)
        Next
    End Sub

    'Public Property Report As AllVesselThroughputVolumeReport

    Private Function GetVMRDates() As List(Of Date)
        Dim getVMRDatesCommand As New ADODB.Command
        Dim tempconnection As New Reports.Connections
        tempconnection.OPConnection.Open()
        getVMRDatesCommand.ActiveConnection = tempconnection.OPConnection
        getVMRDatesCommand.CommandText = $"
SELECT [ata]
  FROM [opreports].[dbo].[reports_vmr]
"
        Dim tempDataadapter As New OleDb.OleDbDataAdapter
        Dim tempDatatable As New DataTable
        tempDataadapter.Fill(tempDatatable, getVMRDatesCommand.Execute)

        Return tempDatatable.AsEnumerable.Select(Of Date)(Function(row) (row("ata"))).ToList


    End Function
    Public AllVesselThroughputVolumeDatabase As ThroughputVolumeDatabase
    Private MonthlyThroughputVolumeList As List(Of MonthlyThroughputVolume)
End Class
