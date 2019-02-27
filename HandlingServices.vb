Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator
Imports Reports

Public Class HandlingServices
    Inherits HandlingServicesReport
    Public Sub New(year As Integer)
        Me.Year = year

        Dim tempConnections As New Connections
        N4Connection = tempConnections.N4Connection
        OPConnection = tempConnections.OPConnection

        VesselVolumes = GetVesselVolumes(year)
        FormatHandlingServiceData(VesselVolumes)

        MyBase.SetDataSource(HandlingServicesData)
        For Each subreport As CrystalDecisions.CrystalReports.Engine.ReportDocument In MyBase.Subreports
            subreport.SetDataSource(HandlingServicesData)
        Next
        MyBase.SetParameterValue("berth1", "SBITC")
        MyBase.SetParameterValue("berth2", "ISI")
        MyBase.SetParameterValue("Year", year)


    End Sub

    Private Function FormatHandlingServiceData(vesselVolumes As List(Of VesselVolume))
        HandlingServicesData = New HandlingServicesData
        With HandlingServicesData
            For month As Integer = 1 To 12
                .ChargeableHandling.AddChargeableHandlingRow(ChargeableHandlingRow(month, "SBITC"))
                .ChargeableHandling.AddChargeableHandlingRow(ChargeableHandlingRow(month, "ISI"))
                .VesselHandling.AddVesselHandlingRow(VesselHandlingRow(month))
                Next
        End With
    End Function

    Private Function VesselHandlingRow(month As Integer) As HandlingServicesData.VesselHandlingRow
        VesselHandlingRow = HandlingServicesData.VesselHandling.NewRow
        With VesselVolumes.AsEnumerable 'kahit wala ng year kasi prefiltered na siya
            VesselHandlingRow.Month = month
            VesselHandlingRow.VesselsNCT1 = .Where(Function(vol) vol.Month = month And vol.BerthWindow = "SBITC").Select(Function(vol) vol.Registry).Distinct.Count
            VesselHandlingRow.VesselsNCT2 = .Where(Function(vol) vol.Month = month And vol.BerthWindow = "ISI").Select(Function(vol) vol.Registry).Distinct.Count

        End With
    End Function

    Private Function ChargeableHandlingRow(month As Integer, v As String) As HandlingServicesData.ChargeableHandlingRow
        ChargeableHandlingRow = HandlingServicesData.ChargeableHandling.NewRow
        With VesselVolumes.AsEnumerable.Where(Function(vol) vol.Month = month) 'kahit wala ng year kasi prefiltered na siya
            ChargeableHandlingRow.Month = month
            ChargeableHandlingRow.Gearbox20 = .Sum(Function(vol) vol.Gearbox(20))
            ChargeableHandlingRow.Gearbox40 = .Sum(Function(vol) vol.Gearbox(40))
            ChargeableHandlingRow.HatchCover = .Sum(Function(vol) vol.HatchCover)
            ChargeableHandlingRow.ShiftingFull = .Sum(Function(vol) vol.Shifting("FCL"))
            ChargeableHandlingRow.ShiftingEmpty = .Sum(Function(vol) vol.Shifting("MTY"))
            ChargeableHandlingRow.BerthWindow = v

        End With
    End Function

    Private Function GetVesselVolumes(year As Integer) As List(Of VesselVolume)
        GetVesselVolumes = New List(Of VesselVolume)
        For month As Integer = 1 To 12
            For Each vmr As VMRClass In Registries(month, year)
                For Each line As String In Lines(vmr)
                    GetVesselVolumes.Add(New VesselVolume(vmr, line, month, year))
                Next
            Next
        Next
    End Function

    Private Function Lines(vmr As VMRClass) As List(Of String)
        Dim tempLines = New List(Of String)
        Dim ConsolidatedUnits As DataTable = ConsolidateUnits(vmr)

        tempLines.AddRange(ConsolidatedUnits.AsEnumerable.Select(Function(units) units("line_op").ToString).Distinct)
        tempLines.AddRange(vmr.GetvesselMovementReportData.dtDG.AsEnumerable.Select(Function(units) units("Line").ToString).Distinct)
        tempLines.AddRange(vmr.GetvesselMovementReportData.dtCMU.AsEnumerable.Select(Function(units) units("Line").ToString).Distinct)

        Return tempLines.Distinct.ToList
    End Function


    Private Function ConsolidateUnits(vmr As VMRClass) As DataTable
        ConsolidateUnits = New DataTable
        With vmr.GetvesselMovementReportData
            ConsolidateUnits.Merge(.dtInboundFCL)
            ConsolidateUnits.Merge(.dtInboundMTY)
            ConsolidateUnits.Merge(.dtOutboundFCL)
            ConsolidateUnits.Merge(.dtOutboundMTY)
        End With
    End Function

    Private Function Registries(month As Integer, year As Integer) As List(Of VMRClass)
        Registries = New List(Of VMRClass)
        For Each registry As String In CreatedVMR(month, year)
            Registries.Add(New VMRClass(registry, N4Connection, OPConnection, ""))
        Next
    End Function

    Private Function CreatedVMR(month As Integer, year As Integer) As List(Of String)
        CreatedVMR = New List(Of String)
        For Each registryRow As DataRow In recordedVMR(month, year)
            CreatedVMR.Add(registryRow("registry"))
        Next
    End Function

    Private Function recordedVMR(month As Integer, year As Integer) As List(Of DataRow)
        Dim getVMR As New ADODB.Command
        OPConnection.Open()
        getVMR.ActiveConnection = OPConnection
        getVMR.CommandText = $"
select registry from reports_vmr where
datepart(month,ata) = {month}
and
datepart(year,ata) = {year}
and
(status <> 'VOID' or status IS NULL)
"
        Dim tempTable As New DataTable
        Dim adapter As New OleDb.OleDbDataAdapter
        adapter.Fill(tempTable, getVMR.Execute)
        OPConnection.Close()
        Return tempTable.AsEnumerable.ToList
    End Function

    Private VesselVolumes As List(Of VesselVolume)
    Private HandlingServicesData As HandlingServicesData
    Private OPConnection As ADODB.Connection
    Private N4Connection As ADODB.Connection
    Public Year As Integer

End Class
