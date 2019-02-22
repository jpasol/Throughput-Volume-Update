
Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator
Public Class MTDMonthlyThroughputVolume
    Inherits MonthlyThroughputVolume

    Public Sub New(Month As Integer, Year As Integer)
        MyBase.New(Month, Year)

        VesselLines = New Dictionary(Of String, String())
        VesselLines = ReadVesselLinesCSV()

        FullNames = New Dictionary(Of String, String)
        FullNames = ReadFullNamesCSV()

        MonthlyThroughputData = New ThroughputVolumeDatabase
        FormatMonthlyThroughputReport(VesselVolumes)

        FormatFullNames(FullNames)

        Report = New MTDMonthlyThroughputReport
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
        For Each registry As DataRow In MyBase.RegistryList(Month, Year).Rows
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

    Private ReadOnly VesselLines As Dictionary(Of String, String())
    Private ReadOnly FullNames As Dictionary(Of String, String)
    Private MonthlyThroughputData As ThroughputVolumeDatabase
    Public ReadOnly Property Report As MonthlyThroughputReport

End Class
