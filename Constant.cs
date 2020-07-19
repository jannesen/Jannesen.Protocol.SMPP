using System;

namespace Jannesen.Protocol.SMPP
{
    public enum ConnectionState
    {
        Unknown             = -1,
        Closed              = 0,
        Connecting,
        SslHandshake,
        StreamConnected,
        Binding,
        Connected,
        Unbinding,
        Stopped,
        Failed              = 90,
    }

    public enum BindingType : UInt32
    {
        Receiver                    = CommandSet.BindReceiver,
        Transmitter                 = CommandSet.BindTransmitter,
        Transceiver                 = CommandSet.BindTransceiver
    }

    public enum CommandSet: UInt32
    {
        Unknown                     = 0x70000000,
        Response                    = 0x80000000,
        GenericNack                 = 0x80000000,
        BindReceiver                = 0x00000001,
        BindReceiverResp            = 0x80000001,
        BindTransmitter             = 0x00000002,
        BindTransmitterResp         = 0x80000002,
        QuerySm                     = 0x00000003,
        QuerySmResp                 = 0x80000003,
        SubmitSm                    = 0x00000004,
        SubmitSmResp                = 0x80000004,
        DeliverSm                   = 0x00000005,
        DeliverSmResp               = 0x80000005,
        Unbind                      = 0x00000006,
        UnbindResp                  = 0x80000006,
        ReplaceSm                   = 0x00000007,
        ReplaceSmResp               = 0x80000007,
        CancelSm                    = 0x00000008,
        CancelSmResp                = 0x80000008,
        BindTransceiver             = 0x00000009,
        BindTransceiverResp         = 0x80000009,
        Outbind                     = 0x0000000B,
        EnquireLink                 = 0x00000015,
        EnquireLinkResp             = 0x80000015,
        SubmitMultiSm               = 0x00000021,
        SubmitMultiSmResp           = 0x80000021,
        AlertNotification           = 0x00000102,
        DataSm                      = 0x00000103,
        DataSmResp                  = 0x80000103
    }

    public enum CommandStatus: UInt32
    {
        Unknown                     = 0x70000000,
        ESME_ROK                    = 0x00000000,
        ESME_RINVMSGLEN             = 0x00000001,
        ESME_RINVCMDLEN             = 0x00000002,
        ESME_RINVCMDID              = 0x00000003,
        ESME_RINVBNDSTS             = 0x00000004,
        ESME_RALYBND                = 0x00000005,
        ESME_RINVPRTFLG             = 0x00000006,
        ESME_RINVREGDLVFLG          = 0x00000007,
        ESME_RSYSERR                = 0x00000008,
        ESME_RINVSRCADR             = 0x0000000A,
        ESME_RINVDSTADR             = 0x0000000B,
        ESME_RINVMSGID              = 0x0000000C,
        ESME_RBINDFAIL              = 0x0000000D,
        ESME_RINVPASWD              = 0x0000000E,
        ESME_RINVSYSID              = 0x0000000F,
        ESME_RCANCELFAIL            = 0x00000011,
        ESME_RREPLACEFAIL           = 0x00000013,
        ESME_RMSGQFUL               = 0x00000014,
        ESME_RINVSERTYP             = 0x00000015,
        ESME_RINVNUMDESTS           = 0x00000033,
        ESME_RINVDLNAME             = 0x00000034,
        ESME_RINVDESTFLAG           = 0x00000040,
        ESME_RINVSUBREP             = 0x00000042,
        ESME_RINVESMCLASS           = 0x00000043,
        ESME_RCNTSUBDL              = 0x00000044,
        ESME_RSUBMITFAIL            = 0x00000045,
        ESME_RINVSRCTON             = 0x00000048,
        ESME_RINVSRCNPI             = 0x00000049,
        ESME_RINVDSTTON             = 0x00000050,
        ESME_RINVDSTNPI             = 0x00000051,
        ESME_RINVSYSTYP             = 0x00000053,
        ESME_RINVREPFLAG            = 0x00000054,
        ESME_RINVNUMMSGS            = 0x00000055,
        ESME_RTHROTTLED             = 0x00000058,
        ESME_RINVSCHED              = 0x00000061,
        ESME_RINVEXPIRY             = 0x00000062,
        ESME_RINVDFTMSGID           = 0x00000063,
        ESME_RX_T_APPN              = 0x00000064,
        ESME_RX_P_APPN              = 0x00000065,
        ESME_RX_R_APPN              = 0x00000066,
        ESME_RQUERYFAIL             = 0x00000067,
        ESME_RINVOPTPARSTREAM       = 0x000000C0,
        ESME_ROPTPARNOTALLWD        = 0x000000C1,
        ESME_RINVPARLEN             = 0x000000C2,
        ESME_RMISSINGOPTPARAM       = 0x000000C3,
        ESME_RINVOPTPARAMVAL        = 0x000000C4,
        ESME_RDELIVERYFAILURE       = 0x000000FE,
        ESME_RUNKNOWNERR            = 0x000000FF
    }

    public enum OptionalTags: UInt16
    {
        DestAddrSubunit             = 0x0005,
        DestNetworkType             = 0x0006,
        DestBearerType              = 0x0007,
        DestTelematicsId            = 0x0008,
        SourceAddrSubunit           = 0x000D,
        SourceNetworkType           = 0x000E,
        SourceBearerType            = 0x000F,
        SourceTelematicsId          = 0x0010,
        QosTimeToLive               = 0x0017,
        PayloadType                 = 0x0019,
        AdditionalStatusInfoText    = 0x001D,
        ReceiptedMessageId          = 0x001E,
        MsMsgWaitFacilities         = 0x0030,
        PrivacyIndicator            = 0x0201,
        SourceSubaddress            = 0x0202,
        DestSubaddress              = 0x0203,
        UserMessageReference        = 0x0204,
        UserResponseCode            = 0x0205,
        SourcePort                  = 0x020A,
        DestinationPort             = 0x020B,
        SarMsgRefNum                = 0x020C,
        LanguageIndicator           = 0x020D,
        SarTotalSegments            = 0x020E,
        SarSegmentSeqnum            = 0x020F,
        SCInterfaceVersion          = 0x0210,
        CallbackNumPresInd          = 0x0302,
        CallbackNumAtag             = 0x0303,
        NumberOfMessages            = 0x0304,
        CallbackNum                 = 0x0381,
        DpfResult                   = 0x0420,
        SetDpf                      = 0x0421,
        MsAvailabilityStatus        = 0x0422,
        NetworkErrorCode            = 0x0423,
        MessagePayload              = 0x0424,
        DeliveryFailureReason       = 0x0425,
        MoreMessagesToSend          = 0x0426,
        MessageState                = 0x0427,
        UssdServiceOp               = 0x0501,
        DisplayTime                 = 0x1201,
        SmsSignal                   = 0x1203,
        MsValidity                  = 0x1204,
        AlertOnMessageDelivery      = 0x130C,
        ItsReplyType                = 0x1380,
        ItsSessionInfo              = 0x1383
    }

    public enum SmppVersionType: byte
    {
        Version3_3                  = 0x33,
        Version3_4                  = 0x34
    }

    public enum TonType: byte
    {
        Unknown                     = 0x00,
        International               = 0x01,
        National                    = 0x02,
        NetworkSpecific             = 0x03,
        SubscriberNumber            = 0x04,
        Alphanumeric                = 0x05,
        Abbreviated                 = 0x06
    }

    public enum NpiType: byte
    {
        Unknown                     = 0x00,
        ISDN                        = 0x01,
        Data                        = 0x03,
        Telex                       = 0x04,
        LandMobile                  = 0x06,
        National                    = 0x08,
        Private                     = 0x09,
        ERMES                       = 0x0A,
        Internet                    = 0x0E
    }

    public enum DataCodings: byte
    {
        Default                     = 0x00,
        ASCII                       = 0x01,
        Octets                      = 0x02,
        Latin1                      = 0x03,
        OctetUnspecified            = 0x04,
        Cyrllic                     = 0x06,
        LatinHebrew                 = 0x07,
        UCS2                        = 0x08,
        DefaultFlashSMS             = 0x10,
        UnicodeFlashSMS             = 0x18,
        Latin1Escape                = 0xC0,
        Class0                      = 0xF0,
        Class1                      = 0xF1,
        Class2                      = 0xF2,
        Class3                      = 0xF3,
        Class0Alert8Bit             = 0xF4,
        Class1ME8Bit                = 0xF5
    }

    [FlagsAttribute]
    public enum EmsClass: byte
    {
        MessagingMode                       = 0x03,
        Default                             = 0x00,
        Datagram                            = 0x01,
        Forward                             = 0x02,
        StoreForward                        = 0x03,
        MessageType                         = 0x3C,
        SMEDeliveryAcknowledgement          = 0x08,
        SMEManualAcknowledgement            = 0x10,
        SMSCDeliveryReceipt                 = 0x04,
        ConversationAbort                   = 0x18,
        IntermediateDeliveryNotification    = 0x20,
        GSMNetworkSpecificFeatures          = 0xC0,
        No                                  = 0x00,
        UDHI                                = 0x40,
        ReplyPath                           = 0x80,
        UDHIandReplyPath                    = 0xC0
    }

    public enum PriorityFlags: byte
    {
        Lowest                              = 0x00,
        Level1                              = 0x01,
        Level2                              = 0x02,
        Highest                             = 0x03
    }

    [FlagsAttribute]
    public enum RegisteredDeliveryFlags: byte
    {
        SMSCDeliveryReceipt                 = 0x03,
        NotRequested                        = 0x00,
        SuccessOrFailure                    = 0x01,
        Failure                             = 0x02,
        SMEOriginatedAcknowledgement        = 0x0C,
        Delivery                            = 0x04,
        Manual                              = 0x08,
        DeliveryManual                      = 0x0C,
        IntermediateNotification            = 0x10
    }
}
