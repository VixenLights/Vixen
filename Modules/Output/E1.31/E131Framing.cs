namespace VixenModules.Controller.E131
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

		public E131Framing(ushort length, string source, byte sequence, ushort univ, int priority, bool blind)
		{
			this.FlagsLength = (ushort) (0x7000 | length);
			this.Vector = 0x00000002;
			this.SourceName = source;
			this.Priority = (byte)priority;
			this.Reserved = 0;
			this.SequenceNumber = sequence;
            if (blind)
                this.Options = 128;
            else
                this.Options = 0;
			this.Universe = univ;
		}

		public E131Framing(byte[] bfr, int offset)
		{
			this.FromBfr(bfr, offset);
		}

		public ushort Length
		{
			get { return (ushort) (this.FlagsLength & 0x0fff); }
		}

		public override byte[] PhyBuffer
		{
			get
			{
				var bfr = new byte[PHYBUFFER_SIZE];

				this.ToBfr(bfr, 0);

				return bfr;
			}

			set { this.FromBfr(value, 0); }
		}

		/// <summary>
		///   Gets or sets the Vector (0x00000002)
		/// </summary>
		private uint Vector { get; set; }

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

		public void ToBfr(byte[] bfr, int offset)
		{
			Extensions.UInt16ToBfrSwapped(this.FlagsLength, bfr, offset + FLAGSLENGTH_OFFSET);
			Extensions.UInt32ToBfrSwapped(this.Vector, bfr, offset + VECTOR_OFFSET);
			Extensions.StringToBfr(this.SourceName, bfr, offset + SOURCENAME_OFFSET, SOURCENAME_SIZE);
			bfr[offset + PRIORITY_OFFSET] = this.Priority;
			Extensions.UInt16ToBfrSwapped(this.Reserved, bfr, offset + RESERVED_OFFSET);
			bfr[offset + SEQUENCENUMBER_OFFSET] = this.SequenceNumber;
			bfr[offset + OPTIONS_OFFSET] = this.Options;
			Extensions.UInt16ToBfrSwapped(this.Universe, bfr, offset + UNIVERSE_OFFSET);
		}

		private void FromBfr(byte[] bfr, int offset)
		{
			this.FlagsLength = Extensions.BfrToUInt16Swapped(bfr, offset + FLAGSLENGTH_OFFSET);
			this.Vector = Extensions.BfrToUInt32Swapped(bfr, offset + VECTOR_OFFSET);
			this.SourceName = Extensions.BfrToString(bfr, offset + SOURCENAME_OFFSET, SOURCENAME_SIZE);
			this.Priority = bfr[offset + PRIORITY_OFFSET];
			this.Reserved = Extensions.BfrToUInt16Swapped(bfr, offset + RESERVED_OFFSET);
			this.SequenceNumber = bfr[offset + SEQUENCENUMBER_OFFSET];
			this.Options = bfr[offset + OPTIONS_OFFSET];
			this.Universe = Extensions.BfrToUInt16Swapped(bfr, offset + UNIVERSE_OFFSET);
		}
	}
}