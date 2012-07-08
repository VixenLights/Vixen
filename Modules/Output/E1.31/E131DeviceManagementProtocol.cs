namespace VixenModules.Controller.E131
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
            this.FlagsLength = (ushort)(0x7000 | (PDU_BASE + 1 + slots));
            this.Vector = 0x02;
            this.AddrTypeDataType = 0xa1;
            this.FirstPropertyAddr = 0x0000;
            this.AddrIncrement = 0x0001;
            this.PropertyValueCnt = (ushort)(slots + 1);
            this.PropertyValues = new byte[slots + 1];
            this.PropertyValues[0] = 0;
            Array.Copy(values, offset, this.PropertyValues, 1, slots);
            this.Malformed = true;
        }

        public E131DeviceManagementProtocol(byte[] bfr, int offset)
        {
            this.FromBfr(bfr, offset);
        }

        public ushort Length
        {
            get
            {
                return (ushort)(this.FlagsLength & 0x0fff);
            }
        }

        public bool Malformed { get; private set; }

        public override byte[] PhyBuffer
        {
            get
            {
                var bfr = new byte[this.PhyLength];

                this.ToBfr(bfr, 0);

                return bfr;
            }

            set
            {
                this.FromBfr(value, 0);
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
                return (ushort)(PHYBUFFER_BASE + this.PropertyValueCnt);
            }
        }

        private ushort PropertyValueCnt { get; set; }

        private byte[] PropertyValues { get; set; }

        private byte Vector { get; set; }

        public void ToBfr(byte[] bfr, int offset)
        {
            Extensions.UInt16ToBfrSwapped(this.FlagsLength, bfr, offset + FLAGSLENGTH_OFFSET);
            bfr[offset + VECTOR_OFFSET] = this.Vector;
            bfr[offset + ADDRTYPEDATATYPE_OFFSET] = this.AddrTypeDataType;
            Extensions.UInt16ToBfrSwapped(this.FirstPropertyAddr, bfr, offset + FIRSTPROPERTYADDR_OFFSET);
            Extensions.UInt16ToBfrSwapped(this.AddrIncrement, bfr, offset + ADDRINCREMENT_OFFSET);
            Extensions.UInt16ToBfrSwapped(this.PropertyValueCnt, bfr, offset + PROPERTYVALUECNT_OFFSET);
            Array.Copy(this.PropertyValues, 0, bfr, offset + PROPERTYVALUES_OFFSET, this.PropertyValueCnt);
        }

        private void FromBfr(byte[] bfr, int offset)
        {
            this.FlagsLength = Extensions.BfrToUInt16Swapped(bfr, offset + FLAGSLENGTH_OFFSET);
            this.Vector = bfr[offset + VECTOR_OFFSET];
            this.AddrTypeDataType = bfr[offset + ADDRTYPEDATATYPE_OFFSET];
            this.FirstPropertyAddr = Extensions.BfrToUInt16Swapped(bfr, offset + FIRSTPROPERTYADDR_OFFSET);
            this.AddrIncrement = Extensions.BfrToUInt16Swapped(bfr, offset + ADDRINCREMENT_OFFSET);
            this.PropertyValueCnt = Extensions.BfrToUInt16Swapped(bfr, offset + PROPERTYVALUECNT_OFFSET);
            this.PropertyValues = new byte[this.PropertyValueCnt];

            this.Malformed = true;

            Array.Copy(bfr, offset + PROPERTYVALUES_OFFSET, this.PropertyValues, 0, this.PropertyValueCnt);

            this.Malformed = false;
        }
    }
}