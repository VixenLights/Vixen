namespace VixenModules.Controller.E131
{
	using System;

	/// <summary>
	///   E1.31 Root Layer.
	/// </summary>
	public class E131Root : E131Base
	{
		public const int PDU_SIZE = PHYBUFFER_SIZE - FLAGSLENGTH_OFFSET;
		public const int PHYBUFFER_SIZE = 38;
		private const int ACNPACKETID_OFFSET = 4;
		private const int ACNPACKETID_SIZE = 12;
		private const int FLAGSLENGTH_OFFSET = 16;
		private const int POSTAMBLESIZE_OFFSET = 2;
		private const int PREAMBLESIZE_OFFSET = 0;
		private const int SENDERCID_OFFSET = 22;
		private const int VECTOR_OFFSET = 18;

		/// <summary>
		///   Identifies Packet (ASC-E1.17)
		/// </summary>
		private string _acnPacketId;

		/// <summary>
		///   PDU Flags/Length
		/// </summary>
		private ushort _flagsLength;

		/// <summary>
		///   RLP Postamble Size (0x0000)
		/// </summary>
		private ushort _postambleSize;

		/// <summary>
		///   RLP Preamble Size (0x0010)
		/// </summary>
		private ushort _preambleSize;

		/// <summary>
		///   Sender's CID
		/// </summary>
		private Guid _sendCid;

		/// <summary>
		///   The Vector (0x00000004)
		/// </summary>
		private uint _vector;

		public E131Root(ushort length, Guid guid)
		{
			_preambleSize = 0x0010;
			_postambleSize = 0x0000;
			_acnPacketId = "ASC-E1.17";
			_flagsLength = (ushort) (0x7000 | length);
			_vector = 0x00000004;
			_sendCid = guid;
			IsMalformed = true;
		}

		public E131Root(byte[] bfr, int offset)
		{
			FromBuffer(bfr, offset);
		}

		/// <summary>
		///   Gets a value indicating whether the packet malformed (length error)?
		/// </summary>
		public bool IsMalformed { get; private set; }

		public override byte[] PhyBuffer
		{
			get
			{
				var bfr = new byte[PHYBUFFER_SIZE];

				ToBuffer(bfr, 0);

				return bfr;
			}

			set { FromBuffer(value, 0); }
		}

		private ushort Length
		{
			get { return (ushort) (_flagsLength & 0x0fff); }
		}

		public void ToBuffer(byte[] buffer, int offset)
		{
			Extensions.UInt16ToBfrSwapped(_preambleSize, buffer, offset + PREAMBLESIZE_OFFSET);
			Extensions.UInt16ToBfrSwapped(_postambleSize, buffer, offset + POSTAMBLESIZE_OFFSET);
			Extensions.StringToBfr(_acnPacketId, buffer, offset + ACNPACKETID_OFFSET, ACNPACKETID_SIZE);
			Extensions.UInt16ToBfrSwapped(_flagsLength, buffer, offset + FLAGSLENGTH_OFFSET);
			Extensions.UInt32ToBfrSwapped(_vector, buffer, offset + VECTOR_OFFSET);
			Extensions.GuidToBfr(_sendCid, buffer, offset + SENDERCID_OFFSET);
		}

		private void FromBuffer(byte[] buffer, int offset)
		{
			_preambleSize = Extensions.BfrToUInt16Swapped(buffer, offset + PREAMBLESIZE_OFFSET);
			_postambleSize = Extensions.BfrToUInt16Swapped(buffer, offset + POSTAMBLESIZE_OFFSET);
			_acnPacketId = Extensions.BfrToString(buffer, offset + ACNPACKETID_OFFSET, ACNPACKETID_SIZE);
			_flagsLength = Extensions.BfrToUInt16Swapped(buffer, offset + FLAGSLENGTH_OFFSET);
			_vector = Extensions.BfrToUInt32Swapped(buffer, offset + VECTOR_OFFSET);
			_sendCid = Extensions.BufferToGuid(buffer, offset + SENDERCID_OFFSET);

			IsMalformed = true;

			if (buffer == null || Length != buffer.Length - FLAGSLENGTH_OFFSET) {
				return;
			}

			IsMalformed = false;
		}
	}
}