Partial Class dsThroughput
    Public Function Units(lines As String(), datatable As String, Size As Integer, Remarks As String)
        Dim enumerable As EnumerableRowCollection(Of DataRow)
        With Me.Tables(datatable).AsEnumerable
            Select Case Remarks
                Case ""
                    enumerable = .Where(Function(unit) lines.Contains(unit("line_op")))
                Case "Throughput"
                    enumerable = .Select(Function(unit) unit)
            End Select
        End With
        With enumerable
            Select Case Size
                Case 20
                    Return .Sum(Function(unit) unit("Dry20") + unit("Rfr20") + unit("FRk20") + unit("Tnk20") + unit("Opn20"))
                Case 40
                    Return .Sum(Function(unit) unit("Dry40") + unit("Rfr40") + unit("FRk40") + unit("Opn40"))
                Case 45
                    Return .Sum(Function(unit) unit("Dry45"))
            End Select
        End With
    End Function

End Class
