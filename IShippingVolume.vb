Public Interface IShippingVolume
    ReadOnly Property Name As String
    ReadOnly Property Lines As String()
    ReadOnly Property Month As Integer
    ReadOnly Property Year As Integer
    ReadOnly Property BerthWindow As Integer
    ReadOnly Property Units(ByVal Bound As String, ByVal Freight As String, ByVal Size As String)
    ReadOnly Property Remarks As String
End Interface
