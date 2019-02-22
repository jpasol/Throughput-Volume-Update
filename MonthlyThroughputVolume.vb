Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator

Public Class MonthlyThroughputVolume
    Implements IMonthlyThroughputVolume


    Public Sub New(Month As Integer, Year As Integer)
        Me.Month = Month
        Me.Year = Year

        Dim connections As New Reports.Connections
        Me.OPConnection = connections.OPConnection
        Me.N4Connection = connections.N4Connection

        VesselMovementReports = New List(Of VMRClass)
        RetrieveVesselMovementReports(Month, Year)

        VesselLines = New Dictionary(Of String, String())
        VesselLines = ReadVesselLinesCSV()

        FullNames = New Dictionary(Of String, String)
        FullNames = ReadFullNamesCSV()

        VesselVolumes = New List(Of VesselVolume)
        CreateVesselVolumes(VesselMovementReports)

        CheckWindowStates(VesselVolumes)

        MonthlyThroughputData = New ThroughputVolumeDatabase
        FormatMonthlyThroughputReport(VesselVolumes)

        FormatFullNames(FullNames)

        Report = New MonthlyThroughputReport
        Report.SetDataSource(MonthlyThroughputData)
        Report.SetParameterValue("month", Month)
        Report.SetParameterValue("year", Year)
    End Sub

    Private Sub FormatFullNames(fullNames As Dictionary(Of String, String))
        For Each key As String In fullNames.Keys
            MonthlyThroughputData.FullNames.AddFullNamesRow(key, fullNames(key))
        Next
    End Sub

    Private Sub FormatMonthlyThroughputReport(vesselVolumes As List(Of VesselVolume))
        Dim freightkinds As String() = {"FCL", "MTY"}
        Dim bounds As String() = {"Inbound", "Outbound"}
        Dim sizes As Integer() = {20, 40, 45}
        Dim tempRow As DataRow
        For Each registry As DataRow In RegistryList(Month, Year).Rows
            With vesselVolumes.Where(Function(vol) vol.Registry = registry(0))
                tempRow = MonthlyThroughputData.MonthlyThroughputVolume.NewRow
                Dim counter As Integer = 0
                For Each bound In bounds
                    For Each freight In freightkinds
                        For Each size In sizes

                            tempRow.Item(counter) = .Sum(Function(vol) vol.Units(bound, freight, size))
                            counter += 1
                        Next
                    Next

                Next
                tempRow("Gearbox20") = .Sum(Function(vol) vol.Gearbox(20))
                tempRow("Gearbox40") = .Sum(Function(vol) vol.Gearbox(40))
                tempRow("Hatchcover") = .Sum(Function(vol) vol.HatchCover)
                tempRow("ShiftFull") = .Sum(Function(vol) vol.Shifting("FCL"))
                tempRow("ShiftEmpty") = .Sum(Function(vol) vol.Shifting("MTY"))

                With .First
                    tempRow("VesselName") = .VesselName
                    tempRow("Voyage") = .Voyage
                    tempRow("Registry") = .Registry
                    tempRow("ATA") = .ATA
                    tempRow("ATD") = .ATD
                    tempRow("WindowState") = .WindowState
                    tempRow("BerthWindow") = .BerthWindow
                    tempRow("Owner") = TranslateOwner(.OwnerVessel)
                End With

                MonthlyThroughputData.MonthlyThroughputVolume.AddMonthlyThroughputVolumeRow(tempRow)
            End With
        Next

    End Sub

    Private Function TranslateOwner(ownerVessel As String) As Object
        For Each key As String In VesselLines.Keys
            If VesselLines.Item(key).Contains(ownerVessel) Then Return key
        Next
        Return "OTHERS"
    End Function

    Private Function ReadFullNamesCSV() As Dictionary(Of String, String)
        Dim lines As New Dictionary(Of String, String)
        Using Reader As New Microsoft.VisualBasic.FileIO.TextFieldParser($"{Application.StartupPath}/VesselLines.csv")
            Reader.TextFieldType = FileIO.FieldType.Delimited
            Reader.SetDelimiters(",")
            While Not Reader.EndOfData
                Dim current As String() = Reader.ReadFields()
                lines.Add(current(1), current(0))
            End While
        End Using
        Return lines
    End Function

    Private Sub CheckWindowStates(vesselVolumes As List(Of VesselVolume))
        Dim noWindowStates As List(Of String)
        noWindowStates = vesselVolumes.AsEnumerable.Where(Function(volume) volume.WindowState = "").Select(Function(volume) volume.Registry).Distinct.ToList

        If noWindowStates.Count > 0 Then
            Dim result As Integer = MsgBox($"There are Registries that has no On/Off Window, {vbNewLine}Would you like to update?", vbYesNo)
            If result = vbYes Then
                Dim onoffwindow As New OnOffWindow(noWindowStates.ToArray)
                onoffwindow.ShowDialog()
            End If
        End If

        UpdateWindowStates(noWindowStates.ToArray, vesselVolumes)
    End Sub

    Private Sub UpdateWindowStates(toArray() As String, vesselVolumes As List(Of VesselVolume))
        For Each registry In toArray
            Dim windowState As String = GetWindowState(registry)
            For Each volume As VesselVolume In vesselVolumes.AsEnumerable.Where(Function(vol) vol.Registry = registry)
                volume.WindowState = windowState
            Next
        Next
    End Sub

    Private Function GetWindowState(registry As String) As String
        Dim getState As New ADODB.Command
        OPConnection.Open()
        getState.ActiveConnection = OPConnection
        getState.CommandText = $"
SELECT [registry]

  FROM [opreports].[dbo].[tvr_onoffwindow] WHERE [registry] = '{registry}'
"
        Dim state As String = getState.Execute.Fields(0).Value
        OPConnection.Close()
        Return state
    End Function

    Private Sub CreateVesselVolumes(vesselMovementReport As List(Of VMRClass))
        For Each vmr As VMRClass In VesselMovementReports
            For Each line As String In VMRLines(vmr)
                VesselVolumes.Add(New VesselVolume(vmr, line, Month, Year))
            Next
        Next
    End Sub

    Private Function VMRLines(vmr As VMRClass) As List(Of String)
        Dim ContainerLines As List(Of String) = ConsolidatedContainers(vmr).AsEnumerable.Select(Function(ctn) ctn("line_op").ToString).ToList
        Dim CMULines As List(Of String) = vmr.GetvesselMovementReportData.dtCMU.AsEnumerable.Select(Function(ctn) ctn("Line").ToString).ToList
        Dim DGLines As List(Of String) = vmr.GetvesselMovementReportData.dtDG.AsEnumerable.Select(Function(ctn) ctn("Line").ToString).ToList

        Dim lines As New List(Of String)
        lines.AddRange(ContainerLines.ToArray)
        lines.AddRange(CMULines.ToArray)
        lines.AddRange(DGLines.ToArray)

        Return lines.Distinct.ToList
    End Function

    Private Function ConsolidatedContainers(vmr As VMRClass) As DataTable
        Dim consolidatedDatatable As New DataTable
        With vmr.GetvesselMovementReportData
            consolidatedDatatable.Merge(.dtInboundFCL)
            consolidatedDatatable.Merge(.dtInboundMTY)
            consolidatedDatatable.Merge(.dtOutboundFCL)
            consolidatedDatatable.Merge(.dtOutboundMTY)
        End With
        Return consolidatedDatatable
    End Function

    Private Function ReadVesselLinesCSV() As Dictionary(Of String, String())
        Dim lines As New Dictionary(Of String, String())
        Using Reader As New Microsoft.VisualBasic.FileIO.TextFieldParser($"{Application.StartupPath}/VesselLines.csv")
            Reader.TextFieldType = FileIO.FieldType.Delimited
            Reader.SetDelimiters(",")
            While Not Reader.EndOfData
                Dim current As String() = Reader.ReadFields()
                lines.Add(current(1), current.Skip(1).ToArray)
            End While
        End Using
        Return lines
    End Function

    Private OPConnection As ADODB.Connection
    Private N4Connection As ADODB.Connection
    Private ReadOnly VesselLines As Dictionary(Of String, String())
    Private ReadOnly FullNames As Dictionary(Of String, String)
    Private MonthlyThroughputData As ThroughputVolumeDatabase
    Public ReadOnly Property Month As Integer Implements IMonthlyThroughputVolume.Month
    Public ReadOnly Property Year As Integer Implements IMonthlyThroughputVolume.Year
    Public ReadOnly Property VesselMovementReports As List(Of VMRClass) Implements IMonthlyThroughputVolume.VesselMovementReports
    Public ReadOnly Property VesselVolumes As List(Of VesselVolume) Implements IMonthlyThroughputVolume.VesselVolumes
    Public ReadOnly Property Report As MonthlyThroughputReport


    Public Sub RetrieveVesselMovementReports(Month As Integer, Year As Integer) Implements IMonthlyThroughputVolume.RetrieveVesselMovementReports
        For Each registryRow As DataRow In RegistryList(Month, Year).Rows
            VesselMovementReports.Add(New VMRClass(registryRow("registry"), N4Connection, OPConnection, ""))
        Next

    End Sub

    Private Function RegistryList(month As Integer, year As Integer) As DataTable
        Dim retrieveRegistries As New ADODB.Command
        OPConnection.Open()
        retrieveRegistries.ActiveConnection = OPConnection
        retrieveRegistries.CommandText = $"
Select registry 
from reports_vmr 
where 
([status] is null 
or [status] <> 'VOID')
and 
datepart(yy,ata) = {year} 
and
datepart(mm,ata) = {month}
"
        Dim recordsetFiller As New OleDb.OleDbDataAdapter
        Dim tempRegistries As New DataTable
        recordsetFiller.Fill(tempRegistries, retrieveRegistries.Execute)
        OPConnection.Close()
        Return tempRegistries
    End Function
End Class
