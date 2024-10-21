using System;
using System.Collections.Generic;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPTLV
    {
        public          OptionalTags    Tag         { get; }
        public          byte[]          Value       { get; set; }

        public          byte            ValueByte
        {
            get {
                if (Value.Length != 1)
                    throw new InvalidOperationException("Invalid byte value");

                return  Value[0];
            }
            set {
                Value = new byte[1];
                Value[0] = value;
            }
        }
        public          UInt16          ValueUInt16
        {
            get {
                if (Value.Length != 2)
                    throw new InvalidOperationException("Invalid short value");

                return  (UInt16)((Value[0] << 8) | (Value[0]));
            }
            set {
                Value = new byte[2];
                Value[0] = (byte)(value >> 8);
                Value[1] = (byte)(value     );
            }
        }
        public          UInt32          ValueUInt32
        {
            get {
                if (Value.Length != 4)
                    throw new InvalidOperationException("Invalid int value");

                return  (UInt32)((Value[0] << 24) | (Value[0] << 16) | (Value[0] << 8) | (Value[0]));
            }
            set {
                Value = new byte[4];
                Value[0] = (byte)(value >> 24);
                Value[1] = (byte)(value >> 16);
                Value[2] = (byte)(value >>  8);
                Value[3] = (byte)(value      );
            }
        }
        public          string          ValueString
        {
            get {
                var size = Value.Length;

                if (size > 0 && Value[size - 1] == 0)
                    --size;

                return System.Text.ASCIIEncoding.ASCII.GetString(Value, 0, size);
            }
            set {
                Value = System.Text.ASCIIEncoding.ASCII.GetBytes(value + "\0");
            }
        }

        internal                        SMPPTLV(OptionalTags tag)
        {
            Tag = tag;
        }
        internal                        SMPPTLV(PduReader reader)
        {
            Tag   = (OptionalTags)(reader.ReadInteger16());
            Value = reader.ReadBytes(reader.ReadInteger16());
        }
    }

    public class SMPPTLVCollection: List<SMPPTLV>
    {
        public          SMPPTLV         this[OptionalTags tag]
        {
            get {
                var tlv = Find(tag);

                if (tlv == null)
                    base.Add(tlv = new SMPPTLV(tag));

                return tlv;
            }
        }

        public          byte?           DestAddrSubunit             { get { return _getByte  (OptionalTags.DestAddrSubunit);            } set { _setByte  (OptionalTags.DestAddrSubunit,            value); } }
        public          byte?           DestNetworkType             { get { return _getByte  (OptionalTags.DestNetworkType);            } set { _setByte  (OptionalTags.DestNetworkType,            value); } }
        public          byte?           DestBearerType              { get { return _getByte  (OptionalTags.DestBearerType);             } set { _setByte  (OptionalTags.DestBearerType,             value); } }
        public          UInt16?         DestTelematicsId            { get { return _getUInt16(OptionalTags.DestTelematicsId);           } set { _setUInt16(OptionalTags.DestTelematicsId,           value); } }
        public          byte?           SourceAddrSubunit           { get { return _getByte  (OptionalTags.SourceAddrSubunit);          } set { _setByte  (OptionalTags.SourceAddrSubunit,          value); } }
        public          byte?           SourceNetworkType           { get { return _getByte  (OptionalTags.SourceNetworkType);          } set { _setByte  (OptionalTags.SourceNetworkType,          value); } }
        public          byte?           SourceBearerType            { get { return _getByte  (OptionalTags.SourceBearerType);           } set { _setByte  (OptionalTags.SourceBearerType,           value); } }
        public          byte?           SourceTelematicsId          { get { return _getByte  (OptionalTags.SourceTelematicsId);         } set { _setByte  (OptionalTags.SourceTelematicsId,         value); } }
        public          UInt32?         QosTimeToLive               { get { return _getUInt32(OptionalTags.QosTimeToLive);              } set { _setUInt32(OptionalTags.QosTimeToLive,              value); } }
        public          byte?           PayloadType                 { get { return _getByte  (OptionalTags.PayloadType);                } set { _setByte(OptionalTags.PayloadType,                  value); } }
        public          string          AdditionalStatusInfoText    { get { return _getString(OptionalTags.AdditionalStatusInfoText);   } set { _setString(OptionalTags.AdditionalStatusInfoText,   value); } }
        public          string          ReceiptedMessageId          { get { return _getString(OptionalTags.ReceiptedMessageId);         } set { _setString(OptionalTags.ReceiptedMessageId,         value); } }
        public          byte?           MsMsgWaitFacilities         { get { return _getByte  (OptionalTags.MsMsgWaitFacilities);        } set { _setByte  (OptionalTags.MsMsgWaitFacilities,        value); } }
        public          byte?           PrivacyIndicator            { get { return _getByte  (OptionalTags.PrivacyIndicator);           } set { _setByte  (OptionalTags.PrivacyIndicator,           value); } }
        public          byte?           SourceSubaddress            { get { return _getByte  (OptionalTags.SourceSubaddress);           } set { _setByte  (OptionalTags.SourceSubaddress,           value); } }
        public          string          DestSubaddress              { get { return _getString(OptionalTags.DestSubaddress);             } set { _setString(OptionalTags.DestSubaddress,             value); } }
        public          UInt16?         UserMessageReference        { get { return _getUInt16(OptionalTags.UserMessageReference);       } set { _setUInt16(OptionalTags.UserMessageReference,       value); } }
        public          byte?           UserResponseCode            { get { return _getByte  (OptionalTags.UserResponseCode);           } set { _setByte  (OptionalTags.UserResponseCode,           value); } }
        public          UInt16?         SourcePort                  { get { return _getUInt16(OptionalTags.SourcePort);                 } set { _setUInt16(OptionalTags.SourcePort,                 value); } }
        public          UInt16?         DestinationPort             { get { return _getUInt16(OptionalTags.DestinationPort);            } set { _setUInt16(OptionalTags.DestinationPort,            value); } }
        public          UInt16?         SarMsgRefNum                { get { return _getUInt16(OptionalTags.SarMsgRefNum);               } set { _setUInt16(OptionalTags.SarMsgRefNum,               value); } }
        public          byte?           LanguageIndicator           { get { return _getByte  (OptionalTags.LanguageIndicator);          } set { _setByte  (OptionalTags.LanguageIndicator,          value); } }
        public          byte?           SarTotalSegments            { get { return _getByte  (OptionalTags.SarTotalSegments);           } set { _setByte  (OptionalTags.SarTotalSegments,           value); } }
        public          byte?           SarSegmentSeqnum            { get { return _getByte  (OptionalTags.SarSegmentSeqnum);           } set { _setByte  (OptionalTags.SarSegmentSeqnum,           value); } }
        public          byte?           SCInterfaceVersion          { get { return _getByte  (OptionalTags.SCInterfaceVersion);         } set { _setByte  (OptionalTags.SCInterfaceVersion,         value); } }
        public          byte?           CallbackNumPresInd          { get { return _getByte  (OptionalTags.CallbackNumPresInd);         } set { _setByte  (OptionalTags.CallbackNumPresInd,         value); } }
        public          string          CallbackNumAtag             { get { return _getString(OptionalTags.CallbackNumAtag);            } set { _setString(OptionalTags.CallbackNumAtag,            value); } }
        public          byte?           NumberOfMessages            { get { return _getByte  (OptionalTags.NumberOfMessages);           } set { _setByte  (OptionalTags.NumberOfMessages,           value); } }
        public          string          CallbackNum                 { get { return _getString(OptionalTags.CallbackNum);                } set { _setString(OptionalTags.CallbackNum,                value); } }
        public          byte?           DpfResult                   { get { return _getByte  (OptionalTags.DpfResult);                  } set { _setByte  (OptionalTags.DpfResult,                  value); } }
        public          byte?           SetDpf                      { get { return _getByte  (OptionalTags.SetDpf);                     } set { _setByte  (OptionalTags.SetDpf,                     value); } }
        public          byte?           MsAvailabilityStatus        { get { return _getByte  (OptionalTags.MsAvailabilityStatus);       } set { _setByte  (OptionalTags.MsAvailabilityStatus,       value); } }
        public          byte[]          NetworkErrorCode            { get { return _getBytes (OptionalTags.NetworkErrorCode);           } set { _setBytes (OptionalTags.NetworkErrorCode,           value); } }
        public          string          MessagePayload              { get { return _getString(OptionalTags.MessagePayload);             } set { _setString(OptionalTags.MessagePayload,             value); } }
        public          byte?           DeliveryFailureReason       { get { return _getByte  (OptionalTags.DeliveryFailureReason);      } set { _setByte  (OptionalTags.DeliveryFailureReason,      value); } }
        public          byte?           MoreMessagesToSend          { get { return _getByte  (OptionalTags.MoreMessagesToSend);         } set { _setByte  (OptionalTags.MoreMessagesToSend,         value); } }
        public          byte?           MessageState                { get { return _getByte  (OptionalTags.MessageState);               } set { _setByte  (OptionalTags.MessageState,               value); } }
        public          byte?           UssdServiceOp               { get { return _getByte  (OptionalTags.UssdServiceOp);              } set { _setByte  (OptionalTags.UssdServiceOp,              value); } }
        public          byte?           DisplayTime                 { get { return _getByte  (OptionalTags.DisplayTime);                } set { _setByte  (OptionalTags.DisplayTime,                value); } }
        public          UInt16?         SmsSignal                   { get { return _getUInt16(OptionalTags.SmsSignal);                  } set { _setUInt16(OptionalTags.SmsSignal,                  value); } }
        public          byte?           MsValidity                  { get { return _getByte  (OptionalTags.MsValidity);                 } set { _setByte  (OptionalTags.MsValidity,                 value); } }
        public          bool            AlertOnMessageDelivery      { get { return _getBool  (OptionalTags.AlertOnMessageDelivery);     } set { _setBool  (OptionalTags.AlertOnMessageDelivery,     value); } }
        public          byte?           ItsReplyType                { get { return _getByte  (OptionalTags.ItsReplyType);               } set { _setByte  (OptionalTags.ItsReplyType,               value); } }
        public          UInt16?         ItsSessionInfo              { get { return _getUInt16(OptionalTags.ItsSessionInfo);             } set { _setUInt16(OptionalTags.ItsSessionInfo,             value); } }

        public                          SMPPTLVCollection()
        {
        }

        private         SMPPTLV         Find(OptionalTags tag)
        {
            for(var i = 0 ; i < base.Count ; ++i) {
                if (base[i].Tag == tag)
                    return base[i];
            }

            return null;
        }
        private         void            Remove(OptionalTags tag)
        {
            for(var i = 0 ; i < base.Count ; ++i) {
                if (base[i].Tag == tag)
                    base.RemoveAt(i);
            }
        }
        private         bool            _getBool(OptionalTags tag)
        {
            var tlv = Find(tag);
            return (tlv != null);
        }
        private         byte?           _getByte(OptionalTags tag)
        {
            var tlv = Find(tag);
            return (tlv != null) ? (byte?)tlv.ValueByte : (byte?)null;
        }
        private         UInt16?         _getUInt16(OptionalTags tag)
        {
            var tlv = Find(tag);
            return (tlv != null) ? (UInt16?)tlv.ValueUInt16 : (UInt16?)null;
        }
        private         UInt32?         _getUInt32(OptionalTags tag)
        {
            var tlv = Find(tag);
            return (tlv != null) ? (UInt32?)tlv.ValueUInt32 : (UInt32?)null;
        }
        private         string          _getString(OptionalTags tag)
        {
            return Find(tag).ValueString;
        }
        private         byte[]          _getBytes(OptionalTags tag)
        {
            return Find(tag).Value;
        }
        private         void            _setBool(OptionalTags tag, bool value)
        {
            if (value)
                this[tag].Value = null;
            else
                Remove(tag);
        }
        private         void            _setByte(OptionalTags tag, byte? value)
        {
            if (value.HasValue)
                this[tag].ValueByte = value.Value;
            else
                Remove(tag);
        }
        private         void            _setUInt16(OptionalTags tag, UInt16? value)
        {
            if (value.HasValue)
                this[tag].ValueUInt16 = value.Value;
            else
                Remove(tag);
        }
        private         void            _setUInt32(OptionalTags tag, UInt32? value)
        {
            if (value.HasValue)
                this[tag].ValueUInt32 = value.Value;
            else
                Remove(tag);
        }
        private         void            _setString(OptionalTags tag, string value)
        {
            if (value != null)
                this[tag].ValueString = value;
            else
                Remove(tag);
        }
        private         void            _setBytes(OptionalTags tag, byte[] value)
        {
            if (value != null)
                this[tag].Value = value;
            else
                Remove(tag);
        }
    }
}
