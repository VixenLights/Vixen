namespace VixenModules.Controller.E131
{
	using System;

	/// <summary>
	///   The E1.31 packet.
	/// </summary>
	public class E131Packet : E131Base
	{
		private const int DMP_OFFSET = E131Root.PHYBUFFER_SIZE + E131Framing.PHYBUFFER_SIZE;

		private const int FRAMING_OFFSET = E131Root.PHYBUFFER_SIZE;

		private const int ROOT_OFFSET = 0;

		public E131Packet(
			Guid guid, string source, byte sequence, ushort universe, byte[] values, int offset, int slots, int priority, bool blind)
		{
			this.E131DeviceManagementProtocol = new E131DeviceManagementProtocol(values, offset, slots);
			this.E131Framing = new E131Framing(
				(ushort) (E131Framing.PHYBUFFER_SIZE + this.E131DeviceManagementProtocol.Length), source, sequence, universe, priority, blind);
			this.E131Root = new E131Root((ushort) (E131Root.PDU_SIZE + this.E131Framing.Length), guid);
		}

		public E131Packet(byte[] bfr)
		{
			this.SetBuffer(bfr);
		}

		public override byte[] PhyBuffer
		{
			get
			{
				var bfr = new byte[this.PhyLength];

				this.E131Root.ToBuffer(bfr, ROOT_OFFSET);
				this.E131Framing.ToBfr(bfr, FRAMING_OFFSET);
				this.E131DeviceManagementProtocol.ToBfr(bfr, DMP_OFFSET);

				return bfr;
			}

			set
			{
				if (value.Length <
				    E131Root.PHYBUFFER_SIZE + E131Framing.PHYBUFFER_SIZE + E131DeviceManagementProtocol.PHYBUFFER_BASE) {
					return;
				}

				this.E131Root = new E131Root(value, ROOT_OFFSET);
				if (this.E131Root.IsMalformed) {
					return;
				}

				this.E131Framing = new E131Framing(value, FRAMING_OFFSET);
				if (this.E131Root.IsMalformed) {
					return;
				}

				this.E131DeviceManagementProtocol = new E131DeviceManagementProtocol(value, DMP_OFFSET);
				if (this.E131DeviceManagementProtocol.Malformed) {
					return;
				}
			}
		}

		public ushort PhyLength
		{
			get { return (ushort) (E131Root.PHYBUFFER_SIZE + this.E131Framing.Length); }
		}

		public E131DeviceManagementProtocol E131DeviceManagementProtocol { get; set; }

		public E131Framing E131Framing { get; set; }

		public E131Root E131Root { get; set; }

		// -------------------------------------------------------------
		// 	CompareSlots() - compare a new event buffer against current
		// 					 slots
		// 		this is a static function to work on prebuilt packets.
		// 		it is embedded in the E131Packet class to keep it with
		// 		the constants and rules that were used to build the
		// 		original packet.
		// -------------------------------------------------------------
		public static bool CompareSlots(byte[] phyBuffer, byte[] values, int offset, int slots)
		{
			var idx = E131Root.PHYBUFFER_SIZE + E131Framing.PHYBUFFER_SIZE + E131DeviceManagementProtocol.PROPERTYVALUES_OFFSET +
			          1;

			while (slots-- > 0) {
				if (phyBuffer[idx++] != values[offset++]) {
					return false;
				}
			}

			return true;
		}

		// -------------------------------------------------------------
		// 	CopySlotsSeqNum() - copy a new sequence # and slots into
		// 						an existing packet buffer
		// 		this is a static function to work on prebuilt packets.
		// 		it is embedded in the E131Packet class to keep it with
		// 		the constants and rules that were used to build the
		// 		original packet.
		// -------------------------------------------------------------
		public static void CopySeqNumSlots(byte[] phyBuffer, byte[] values, int offset, int slots, byte seqNum)
		{
			if (values.Length > 0) {
				const int INDEX =
					E131Root.PHYBUFFER_SIZE + E131Framing.PHYBUFFER_SIZE + E131DeviceManagementProtocol.PROPERTYVALUES_OFFSET + 1;
				Array.Copy(values, offset, phyBuffer, INDEX, slots);
				phyBuffer[E131Root.PHYBUFFER_SIZE + E131Framing.SEQUENCENUMBER_OFFSET] = seqNum;
			}
		}

		private void SetBuffer(byte[] bfr)
		{
			this.PhyBuffer = bfr;
		}
	}
}