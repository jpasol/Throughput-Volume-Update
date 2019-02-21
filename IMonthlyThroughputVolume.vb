Public Interface IMonthlyThroughputVolume

    ReadOnly Property Month As Integer
    ReadOnly Property Year As Integer
    ReadOnly Property VesselMovementReports As List(Of Vessel_Movement_Report_Creator.VMRClass)
    ReadOnly Property VesselVolumes As List(Of VesselVolume)

    Sub RetrieveVesselMovementReports(Month As Integer, Year As Integer)

End Interface
