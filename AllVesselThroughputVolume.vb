﻿Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator

Public Class AllVesselThroughputVolume
    Inherits MonthlyThroughputVolume
    Public Sub New(Month As Integer, Year As Integer)
        MyBase.New(Month, Year)

        Report = New AllVesselThroughputVolumeReport
        AllVesselThroughputVolumeDatabase = New ThroughputVolumeDatabase

        FormatReport(MyBase.VesselVolumes)
        Report.SetDataSource(AllVesselThroughputVolumeDatabase)
        Report.SetParameterValue("month", Month)
        Report.SetParameterValue("year", Year)

    End Sub

    Private Sub FormatReport(vesselVolumes As List(Of VesselVolume))
        Dim freightkinds As String() = {"FCL", "MTY"}
        Dim bounds As String() = {"Inbound", "Outbound"}
        Dim sizes As Integer() = {20, 40, 45}
        For Each volume As VesselVolume In vesselVolumes
            Dim tempRow As DataRow
            tempRow = AllVesselThroughputVolumeDatabase.AllVesselThroughputVolumeData.NewRow
            Dim counter As Integer = 0
            For Each bound In bounds
                    For Each freight In freightkinds
                        For Each size In sizes

                        tempRow.Item(counter) = volume.Units(bound, freight, size)
                        counter += 1
                    Next
                    Next

            Next

            tempRow("VesselName") = volume.VesselName
            tempRow("Voyage") = volume.Voyage
            tempRow("Registry") = volume.Registry
            tempRow("ATA") = volume.ATA
            tempRow("ATD") = volume.ATD
            tempRow("WindowState") = volume.WindowState
            tempRow("Remarks") = volume.Remarks
            tempRow("Line") = volume.Line
            tempRow("BerthWindow") = volume.BerthWindow
            tempRow("VesselVolume") = volume.OwnerVessel
            tempRow("Service") = volume.Service
            tempRow("LOA") = volume.LOA
            tempRow("Month") = volume.Month

            AllVesselThroughputVolumeDatabase.AllVesselThroughputVolumeData.AddAllVesselThroughputVolumeDataRow(tempRow)
        Next
    End Sub

    Public Property Report As AllVesselThroughputVolumeReport
    Private AllVesselThroughputVolumeDatabase As ThroughputVolumeDatabase
End Class