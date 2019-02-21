Imports Throughput_Volume_Update

Public Class BerthWindowReport
    Implements IBerthWindowReport

    Public Sub New(ShippingLine As String)
        CreateShippingVolumes(ShippingLine)
        FormatShippingVolumesToReport()
        Report.SetDataSource(Throughput)
    End Sub

    Private Sub FormatShippingVolumesToReport()
        For Each volume As ShippingVolume In ShippingVolumes
            Dim tempRow As DataRow
            tempRow = Throughput.CummulativeReport.NewCummulativeReportRow
            tempRow.ItemArray = VolumeEntry(volume)
            Throughput.CummulativeReport.AddCummulativeReportRow(tempRow)
        Next
    End Sub

    Private Function VolumeEntry(volume As ShippingVolume) As String()
        With volume
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

    Private Sub CreateShippingVolumes(shippingLine As String)
        Dim berthWindowLines As Dictionary(Of String, String()) = BerthWindowVolume(shippingLine)
        For Each line As String In berthWindowLines.Keys
            For month As Integer = 1 To 12
                For berth As Integer = 1 To 2
                    ShippingVolumes.Add(New ShippingVolume(line, berthWindowLines.Item(line), month, Date.Now.Year, 1))
                Next
            Next
        Next
    End Sub

    Private Function BerthWindowVolume(shippingLine As String) As Dictionary(Of String, String())
        If shippingLine = "" Then
            Return ShippingLines()
        Else
            Dim tempDictionary As New Dictionary(Of String, String())
            tempDictionary.Add(shippingLine, ShippingLines.Item(shippingLine))
            Return tempDictionary
        End If

    End Function

    Public Function ShippingLines() As Dictionary(Of String, String())
        Dim lines As New Dictionary(Of String, String())

        With RecognizedLines()
            For Each key As String In .Keys
                lines.Add(key, .Item(key))
            Next
        End With

        With UnRecognizedLines(RecognizedLines)
            For Each key As String In .Keys
                lines.Add(key, .Item(key))
            Next
        End With

        Return lines
    End Function

    Private Function UnRecognizedLines(recognizedLines As Dictionary(Of String, String())) As Object
        Dim Lines As New ADODB.Command
        Dim Connection As New Reports.Connections
        Connection.OPConnection.Open()

        Lines.ActiveConnection = Connection.OPConnection
        Lines.CommandText = $"
SELECT DISTINCT LINE_OP from VMR_UNITS

union

SELECT DISTINCT [OWNER] from REPORTS_VMR

"
        Dim OtherLineOP As New List(Of String)
        With Lines.Execute()
            Dim lineop As String = .Fields(0).Value
            While Not .EOF
                If Not recognizedLines.ContainsKey(lineop) Or Not CheckValuesinArrays(recognizedLines, lineop) Then
                    OtherLineOP.Add(.Fields(0).Value)
                End If
                .MoveNext()
            End While
        End With

        Dim UnrecognizedRegistries As New Dictionary(Of String, String())
        UnrecognizedRegistries.Add("Other Shipping Lines", OtherLineOP.ToArray)

        Return UnrecognizedRegistries
    End Function

    Private Function CheckValuesinArrays(recognizedLines As Dictionary(Of String, String()), lineop As String) As Boolean
        For Each lines As String() In recognizedLines.Values
            If lines.Contains(lineop) Then
                Return True
            End If
        Next
        Return false
    End Function

    Private Function RecognizedLines() As Dictionary(Of String, String())
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

    Private ReadOnly Property Throughput As New Throughput
    Public ReadOnly Property ShippingVolumes As New List(Of ShippingVolume) Implements IBerthWindowReport.ShippingVolumes
    Public ReadOnly Property Report As New BerthWindowVolume Implements IBerthWindowReport.Report


End Class
