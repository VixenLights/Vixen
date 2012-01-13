namespace VixenModules.Output.E131.Model
{
    /// <summary>
    ///   E1.31 Framing Layer.
    /// </summary>
    public class E131Framing : E131Base
    {
        public const int PHYBUFFER_SIZE = 77;

        public const int SEQUENCENUMBER_OFFSET = 73;

        private const int FLAGSLENGTH_OFFSET = 0;

        private const int OPTIONS_OFFSET = 74;

        private const int PRIORITY_OFFSET = 70;

        private const int RESERVED_OFFSET = 71;

        private const int SOURCENAME_OFFSET = 6;

        private const int SOURCENAME_SIZE = 64;

        private const int UNIVERSE_OFFSET = 75;

        private const int VECTOR_OFFSET = 2;

        public E131Framing(ushort length, string source, byte sequence, ushort univ)
        {
            FlagsLength = (ushort)(0x7000 | length);
            Vector = 0x00000002;
            SourceName = source;
            Priority = 100;
            Reserved = 0;
            SequenceNumber = sequence;
            Options = 0;
            Universe = univ;
        }

        public E131Framing(byte[] bfr, int offset)
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

        public override byte[] PhyBuffer
        {
            get
            {
                var bfr = new byte[PHYBUFFER_SIZE];

                ToBfr(bfr, 0);

                return bfr;
            }

            set
            {
                FromBfr(value, 0);
            }
        }

        /// <summary>
        ///   Gets or sets the PDU Flags/Length
        /// </summary>
        private ushort FlagsLength { get; set; }

        private byte Options { get; set; }

        private byte Priority { get; set; }

        private ushort Reserved { get; set; }

        private byte SequenceNumber { get; set; }

        private string SourceName { get; set; }

        private ushort Universe { get; set; }

        /// <summary>
        ///   Gets or sets the Vector (0x00000002)
        /// </summary>
        private uint Vector { get; set; }

        public void ToBfr(byte[] bfr, int offset)
        {
            Extensions.UInt16ToBfrSwapped(FlagsLength, bfr, offset + FLAGSLENGTH_OFFSET);
            Extensions.UInt32ToBfrSwapped(Vector, bfr, offset + VECTOR_OFFSET);
            Extensions.StringToBfr(SourceName, bfr, offset + SOURCENAME_OFFSET, SOURCENAME_SIZE);
            bfr[offset + PRIORITY_OFFSET] = Priority;
            Extensions.UInt16ToBfrSwapped(Reserved, bfr, offset + RESERVED_OFFSET);
            bfr[offset + SEQUENCENUMBER_OFFSET] = SequenceNumber;
            bfr[offset + OPTIONS_OFFSET] = Options;
            Extensions.UInt16ToBfrSwapped(Universe, bfr, offset + UNIVERSE_OFFSET);
        }

        private void FromBfr(byte[] bfr, int offset)
        {
            FlagsLength = Extensions.BfrToUInt16Swapped(bfr, offset + FLAGSLENGTH_OFFSET);
            Vector = Extensions.BfrToUInt32Swapped(bfr, offset + VECTOR_OFFSET);
            SourceName = Extensions.BfrToString(bfr, offset + SOURCENAME_OFFSET, SOURCENAME_SIZE);
            Priority = bfr[offset + PRIORITY_OFFSET];
            Reserved = Extensions.BfrToUInt16Swapped(bfr, offset + RESERVED_OFFSET);
            SequenceNumber = bfr[offset + SEQUENCENUMBER_OFFSET];
            Options = bfr[offset + OPTIONS_OFFSET];
            Universe = Extensions.BfrToUInt16Swapped(bfr, offset + UNIVERSE_OFFSET);
        }
    }
}
