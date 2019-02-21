Imports Throughput_Volume_Update
Imports Vessel_Movement_Report_Creator
Public Class ShippingVolume
    Implements IShippingVolume

    Public Sub New(Name As String, Lines As String(), Month As Integer, Year As Integer,
                   Optional BerthWindow As Integer = 0, Optional Remarks As String = "",
                   Optional UnitRemarks As String = "",
                   Optional GetAll As Boolean = False)
        Me.Name = Name
        Me.Lines = Lines
        Me.Month = Month
        Me.Year = Year
        Me.BerthWindow = BerthWindow
        Me.Remarks = Remarks
        Me.UnitRemarks = UnitRemarks
        Me.GetAll = GetAll
        With New Reports.Connections
            OPConnection = .OPConnection
            N4Connection = .N4Connection
        End With

        RetrieveInfo()
    End Sub

    Private OPConnection As New ADODB.Connection
    Private N4Connection As New ADODB.Connection
    Private UnitRemarks As String
    Private GetAll As Boolean
    Private ReadOnly VesselMovementReports As New List(Of VMRClass)
    Public ReadOnly Property Name As String Implements IShippingVolume.Name
    Public ReadOnly Property Month As Integer Implements IShippingVolume.Month
    Public ReadOnly Property Year As Integer Implements IShippingVolume.Year
    Public ReadOnly Property Lines As String() Implements IShippingVolume.Lines
    Public ReadOnly Property BerthWindow As Integer Implements IShippingVolume.BerthWindow
    Public ReadOnly Property Remarks As String Implements IShippingVolume.Remarks

    Public ReadOnly Property Units(Bound As String, Freight As String, Size As String) As Object Implements IShippingVolume.Units
        Get
            With VesselMovementReports.AsEnumerable
                Return .Sum(Function(vmr) vmr.GetvesselMovementReportData.Units(Lines, $"dt{Bound}{Freight}", Size, UnitRemarks))
            End With
        End Get
    End Property

    Private Sub RetrieveInfo()
        For Each registry As String In RegistryList()
            VesselMovementReports.Add(New VMRClass(registry, N4Connection, OPConnection, ""))
        Next
    End Sub

    Private Function RegistryList() As IEnumerable(Of String)
        OPConnection.Open()
        Dim linesArray As String = $"'{String.Join(" ','", Lines.ToArray)}'"
        Dim berthwindow As String = IIf(Me.BerthWindow = 1, "SBITC", "ISI")
        Dim registryRecordset As New ADODB.Command
        registryRecordset.ActiveConnection = OPConnection
        registryRecordset.CommandText = $"
SELECT DISTINCT [registry]
  FROM [opreports].[dbo].[reports_vmr] where
    (datepart(yy,ata) = {Year}
and datepart(mm,ata) = {Month})
and (status <> 'VOID' OR status is null)
"
        If GetAll = False Then
            registryRecordset.CommandText &= $" and owner = '{Name}'"
        End If

        If Me.BerthWindow <> 0 Then
            registryRecordset.CommandText &= $" and berth = '{berthwindow}'"
        End If

        Select Case Remarks
            Case "Owner"
                registryRecordset.CommandText &= $" and owner in ({linesArray})"
            Case "Co-Load"
                registryRecordset.CommandText &= $" and owner not in ({linesArray})"
        End Select

        Dim registries As New List(Of String)
        With registryRecordset.Execute()
            While Not .EOF
                registries.Add(.Fields(0).Value)
                .MoveNext()
            End While
        End With
        OPConnection.Close()
        Return registries
    End Function
End Class
