namespace VixenModules.Output.Renard
{
	[ProtocolVersion(1)]
	internal class Protocol1 : IRenardProtocolFormatter
	{
		private byte[] _packet;
		private int _dst_index;
		private byte[] _valueMap;
		private int _outputCount;

		// Assume clocks are accurate to 1%, so insert a pad byte every 100 bytes.     
		private const int PAD_DISTANCE = 100;
		private const byte FRAME_MARKER = 0x7E;
		private const byte PIC_INDEX_OFFSET = 0x80;
		private const byte PAD_BYTE = 0x7D;

		public Protocol1()
		{
			_valueMap = _BuildValueMap();
		}

		public void StartPacket(int outputCount, int chainIndex)
		{
			if (_packet == null || outputCount != _outputCount) {
				_outputCount = outputCount;
				int necessaryPacketLength = _CalcMaxPacketLength(_outputCount);
				_packet = new byte[necessaryPacketLength];
			}
			_dst_index = 0;
			_Add(_GetFrameMarkerValue());
			_Add(_GetChainIndexValue(chainIndex));
		}

		public void Add(byte value)
		{
			_Add(_GetValue(value));
		}

		public int PacketSize
		{
			get { return _dst_index; }
		}

		public byte[] FinishPacket()
		{
			return _packet;
		}

		private byte[] _BuildValueMap()
		{
			byte[] valueMap = new byte[256];
			for (int i = 0; i < valueMap.Length; i++) {
				valueMap[i] = (byte) i;
			}
			valueMap[0x7d] = 124;
			valueMap[0x7e] = 124;
			valueMap[0x7f] = 128;

			return valueMap;
		}

		private int _CalcMaxPacketLength(int outputCount)
		{
			int maxBytesPerOutput = 1;
			int preambleLength = 2;
			int maxUnpaddedPacketLength = preambleLength + outputCount*maxBytesPerOutput;
			int countOfPadsToInsert = _GetCountOfPadsToInsert(maxUnpaddedPacketLength);
			return maxUnpaddedPacketLength + countOfPadsToInsert;
		}

		private byte _GetValue(byte level)
		{
			return _valueMap[level];
		}

		private int _GetCountOfPadsToInsert(int packetLength)
		{
			return packetLength/PAD_DISTANCE;
		}

		private byte _GetFrameMarkerValue()
		{
			return FRAME_MARKER;
		}

		private byte _GetChainIndexValue(int chainIndex)
		{
			return (byte) (PIC_INDEX_OFFSET + chainIndex);
		}

		private void _Add(byte value)
		{
			_packet[_dst_index++] = value;
			if (_dst_index%PAD_DISTANCE == 0) {
				_packet[_dst_index++] = PAD_BYTE;
			}
		}
	}
}