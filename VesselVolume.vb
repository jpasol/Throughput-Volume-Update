Imports Throughput_Volume_Update
Imports Reports
Imports Vessel_Movement_Report_Creator
Public Class VesselVolume
    Implements IVesselVolume

    Public Sub New(VesselMovementReport As VMRClass, Line As String, Month As Integer, Year As Integer)
        Dim initializeConnection As New Connections
        OPConnection = initializeConnection.OPConnection

        Me.VMR = VesselMovementReport
        Me.Line = Line
        Me.Month = Month
        Me.Year = Year
        Me.WindowState = GetOnOffWindow(Me.Registry)
        Me.ShippingLines = ReadCSV()

    End Sub

    Private Function GetOnOffWindow(registry As String) As String
        Dim state As String = ""
        Dim windowState As New ADODB.Command
        OPConnection.Open()

        With windowState
            .ActiveConnection = OPConnection
            .CommandText = $"
Select windowstate from tvr_onoffwindow where registry = '{Me.Registry}'
"
            With .Execute
                If Not .EOF Then state = .Fields(0).Value
            End With

        End With
        OPConnection.Close()
        Return state
    End Function

    Private Function ReadCSV() As Dictionary(Of String, String())
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

    Private VMR As VMRClass
    Private OPConnection As ADODB.Connection
    Private ShippingLines As Dictionary(Of String, String())


    Public ReadOnly Property Units(Bound As String, FreightKind As String, Size As Integer) As Integer Implements IVesselVolume.Units
        Get
            Dim line As String() = {Me.Line} 'convert line to string array
            Return VMR.GetvesselMovementReportData.Units(line, $"dt{Bound}{FreightKind}", Size, "")
        End Get
    End Property

    Public ReadOnly Property TEU(Bound As String, FreightKind As String) As Integer Implements IVesselVolume.TEU
        Get
            Dim line As String() = {Me.Line} 'convert line to string array
            Dim sizeTwenty As Integer = VMR.GetvesselMovementReportData.Units(line, $"dt{Bound}{FreightKind}", 20, "")
            Dim sizeForty As Integer = VMR.GetvesselMovementReportData.Units(line, $"dt{Bound}{FreightKind}", 40, "")
            Dim sizeFortyFive As Integer = VMR.GetvesselMovementReportData.Units(line, $"dt{Bound}{FreightKind}", 40, "")

            Return sizeTwenty * 1 +
                    sizeForty * 2 +
                    sizeFortyFive * 2.25
        End Get
    End Property

    Public ReadOnly Property HatchCover As Integer Implements IVesselVolume.HatchCover
        Get
            With VMR.GetvesselMovementReportData.dtCMU.AsEnumerable.Where(Function(cmu) cmu("Description").ToString.Contains("HC"))
                Return .Sum(Function(desc) desc.ToString.Substring(0, desc.ToString.IndexOf("HC")))
            End With
        End Get
    End Property

    Public ReadOnly Property Shifting(FreightKind As String) As Integer Implements IVesselVolume.Shifting
        Get
            Dim freight As String
            Select Case FreightKind
                Case "FCL"
                    freight = "FULL"
                Case "MTY"
                    freight = "EMPTY"
            End Select

            With VMR.GetvesselMovementReportData.dtCMU.AsEnumerable.
                Where(Function(cmu) cmu("Description").ToString.Contains(freight))

                .Sum(Function(desc) desc.ToString.Substring(0, desc.ToString.IndexOf("X")))
            End With
        End Get
    End Property

    Public ReadOnly Property Gearbox(Size As Integer) As Integer Implements IVesselVolume.Gearbox
        Get
            With VMR.GetvesselMovementReportData.dtCMU.AsEnumerable.
                Where(Function(cmu) cmu("Description").ToString.Contains("GB") And cmu("Description").ToString.Contains($"{Size}"))

                Return .Sum(Function(desc) desc.ToString.Substring(0, desc.ToString.IndexOf("X")))
            End With
        End Get
    End Property

    Public ReadOnly Property VesselName As String Implements IVesselVolume.VesselName
        Get
            Return VMR.vmrVessel.Name
        End Get
    End Property

    Public ReadOnly Property Voyage As String Implements IVesselVolume.Voyage
        Get
            Dim inboundVoyage As String = VMR.vmrVessel.InboundVoyage
            Dim outboundVoyage As String = VMR.vmrVessel.OutboundVoyage

            Return $"{inboundVoyage} - {outboundVoyage}"
        End Get
    End Property

    Public ReadOnly Property Registry As String Implements IVesselVolume.Registry
        Get
            Return VMR.vmrVessel.Registry
        End Get
    End Property

    Public ReadOnly Property ATA As Date Implements IVesselVolume.ATA
        Get
            Return VMR.vmrVessel.ATA
        End Get
    End Property

    Public ReadOnly Property ATD As Date Implements IVesselVolume.ATD
        Get
            Return VMR.vmrVessel.ATD
        End Get
    End Property

    Public ReadOnly Property Remarks As String Implements IVesselVolume.Remarks
        Get
            Dim lines As String() = ShippingLines.Item(OwnerVessel)
            Dim OwnerRemark As IVesselVolume.OwnerRemarks
            Select Case lines.Contains(Me.Line)
                Case True
                    OwnerRemark = IVesselVolume.OwnerRemarks.Own
                Case False
                    OwnerRemark = IVesselVolume.OwnerRemarks.CoLoad
            End Select

            Return GetType(IVesselVolume.OwnerRemarks).GetEnumName(OwnerRemark)
        End Get
    End Property

    Public ReadOnly Property Line As String Implements IVesselVolume.Line

    Public ReadOnly Property BerthWindow As String Implements IVesselVolume.BerthWindow
        Get
            Return VMR.vmrVessel.BerthWindow
        End Get
    End Property

    Public ReadOnly Property OwnerVessel As String Implements IVesselVolume.OwnerVessel
        Get
            Return VMR.vmrVessel.Owner
        End Get
    End Property

    Public ReadOnly Property Service As String Implements IVesselVolume.Service
        Get
            Return VMR.vmrVessel.Service
        End Get
    End Property

    Public ReadOnly Property LOA As Integer Implements IVesselVolume.LOA
        Get
            Return VMR.vmrVessel.LOA
        End Get
    End Property

    Public ReadOnly Property Month As Integer Implements IVesselVolume.Month
    Public Property WindowState As String Implements IVesselVolume.WindowState
    Public ReadOnly Property Year As Integer Implements IVesselVolume.Year
End Class
