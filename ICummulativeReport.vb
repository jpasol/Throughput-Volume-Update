Public Interface ICummulativeReport


    Function CummulativeShippingVolumes() As List(Of ShippingVolume)
    Function CummulativeActualShippingVolumes() As List(Of ShippingVolume)
    Function CummulativeOwnedShippingVolumes() As List(Of ShippingVolume)
    Function CummulativeCoLoadShippingVolumes() As List(Of ShippingVolume)

    Sub FormatCummulativeShippingVolumes(CummulativeShippingVolumes As List(Of ShippingVolume))
    Sub FormatCummulativeActualShippingVolumes(CummulativeActualShippingVolumes As List(Of ShippingVolume))
    Sub FormatCummulativeOwnedShippingVolumes(CummulativeOwnedShippingVolumes As List(Of ShippingVolume))
    Sub FormatCummulativeCoLoadShippingVolumes(CummulativeCoLoadShippingVolumes As List(Of ShippingVolume))

    Property Report As CummulativeThroughputReport

End Interface
