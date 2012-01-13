namespace VixenModules.Output.E131.Model
{
    using System;

    /// <summary>
    ///   E1.31 Device Management Protocol Layer
    /// </summary>
    public class E131DeviceManagementProtocol : E131Base
    {
        public const int PHYBUFFER_BASE = 10;

        public const int PROPERTYVALUES_OFFSET = 10;

        private const int ADDRINCREMENT_OFFSET = 6;

        private const int ADDRTYPEDATATYPE_OFFSET = 3;

        private const int FIRSTPROPERTYADDR_OFFSET = 4;

        private const int FLAGSLENGTH_OFFSET = 0;

        private const int PDU_BASE = PHYBUFFER_BASE;

        private const int PROPERTYVALUECNT_OFFSET = 8;

        private const int VECTOR_OFFSET = 2;

        public E131DeviceManagementProtocol(byte[] values, int offset, int slots)
        {
            FlagsLength = (ushort)(0x7000 | (PDU_BASE + 1 + slots));
            Vector = 0x02;
            AddrTypeDataType = 0xa1;
            FirstPropertyAddr = 0x0000;
            AddrIncrement = 0x0001;
            PropertyValueCnt = (ushort)(slots + 1);
            PropertyValues = new byte[slots + 1];
            PropertyValues[0] = 0;
            Array.Copy(values, offset, PropertyValues, 1, slots);
            Malformed = true;
        }

        public E131DeviceManagementProtocol(byte[] bfr, int offset)
        {
            FromBfr(bfr, offset);
        }

        public ushort Length
        {
            get
            {
                return (ushort)(FlagsLength & 0x0fff);
            }
        }

        public bool Malformed { get; private set; }

        public override byte[] PhyBuffer
        {
            get
            {
                var bfr = new byte[PhyLength];

                ToBfr(bfr, 0);

                return bfr;
            }

            set
            {
                FromBfr(value, 0);
            }
        }

        private ushort AddrIncrement { get; set; }

        private byte AddrTypeDataType { get; set; }

        private ushort FirstPropertyAddr { get; set; }

        private ushort FlagsLength { get; set; }

        private ushort PhyLength
        {
            get
            {
                return (ushort)(PHYBUFFER_BASE + PropertyValueCnt);
            }
        }

        private ushort PropertyValueCnt { get; set; }

        private byte[] PropertyValues { get; set; }

        private byte Vector { get; set; }

        public void ToBfr(byte[] bfr, int offset)
        {
            Extensions.UInt16ToBfrSwapped(FlagsLength, bfr, offset + FLAGSLENGTH_OFFSET);
            bfr[offset + VECTOR_OFFSET] = Vector;
            bfr[offset + ADDRTYPEDATATYPE_OFFSET] = AddrTypeDataType;
            Extensions.UInt16ToBfrSwapped(FirstPropertyAddr, bfr, offset + FIRSTPROPERTYADDR_OFFSET);
            Extensions.UInt16ToBfrSwapped(AddrIncrement, bfr, offset + ADDRINCREMENT_OFFSET);
            Extensions.UInt16ToBfrSwapped(PropertyValueCnt, bfr, offset + PROPERTYVALUECNT_OFFSET);
            Array.Copy(PropertyValues, 0, bfr, offset + PROPERTYVALUES_OFFSET, PropertyValueCnt);
        }

        private void FromBfr(byte[] bfr, int offset)
        {
            FlagsLength = Extensions.BfrToUInt16Swapped(bfr, offset + FLAGSLENGTH_OFFSET);
            Vector = bfr[offset + VECTOR_OFFSET];
            AddrTypeDataType = bfr[offset + ADDRTYPEDATATYPE_OFFSET];
            FirstPropertyAddr = Extensions.BfrToUInt16Swapped(bfr, offset + FIRSTPROPERTYADDR_OFFSET);
            AddrIncrement = Extensions.BfrToUInt16Swapped(bfr, offset + ADDRINCREMENT_OFFSET);
            PropertyValueCnt = Extensions.BfrToUInt16Swapped(bfr, offset + PROPERTYVALUECNT_OFFSET);
            PropertyValues = new byte[PropertyValueCnt];

            Malformed = true;

            Array.Copy(bfr, offset + PROPERTYVALUES_OFFSET, PropertyValues, 0, PropertyValueCnt);

            Malformed = false;
        }
    }
}
