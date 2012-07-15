namespace VixenModules.Output.Renard {
	interface IRenardProtocolFormatter {
		void StartPacket(int outputCount, int chainIndex);
		byte[] FinishPacket();
		int PacketSize { get; }
		void Add(byte value);
	}
}
