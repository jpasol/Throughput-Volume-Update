Public Interface IVesselVolume
    ReadOnly Property Units(Bound As String, FreightKind As String, Size As Integer) As Integer
    ReadOnly Property TEU(Bound As String, FreightKind As String) As Integer
    ReadOnly Property HatchCover As Integer
    ReadOnly Property Shifting(FreightKind As String) As Integer
    ReadOnly Property Gearbox(Size As Integer) As Integer
    ReadOnly Property VesselName As String
    ReadOnly Property Voyage As String
    ReadOnly Property Registry As String
    ReadOnly Property ATA As Date
    ReadOnly Property ATD As Date
    ReadOnly Property Remarks As String 'enum
    ReadOnly Property Line As String
    ReadOnly Property BerthWindow As String 'enum
    ReadOnly Property OwnerVessel As String
    ReadOnly Property Service As String
    ReadOnly Property LOA As Integer
    ReadOnly Property Month As Integer
    ReadOnly Property Year As Integer
    ReadOnly Property WindowState As String 'enum

    Enum Bound
        Inbound
        Outbound
    End Enum

    Enum FreightKind
        FCL
        MTY
    End Enum

    Enum Sizes
        Twenty = 20
        Forty = 40
        FortyFive = 45
    End Enum

    Enum State
        OnWindow = 1
        OffWindow = 2
    End Enum

    Enum Terminals
        SBITC = 1
        ISI = 2
    End Enum

    Enum OwnerRemarks
        Own
        CoLoad
    End Enum

End Interface
