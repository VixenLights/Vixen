namespace VixenModules.Output.DmxUsbPro
{
	internal enum MessageType
	{
		ReprogramFirmware = 1,
		ProgramFlashPageRequest,
		ProgramFlagPageReply = 2,
		GetWidgetParametersRequest,
		GetWidgetParametersReply = 3,
		SetWidgetParametersRequest,
		ReceivedDMXPacket,
		OutputOnlySendDMXPacketRequest,
		SendRDMPacketRequest,
		ReceiveDMXOnChange,
		ReceiveDMXChangeOfStatePacket,
		GetWidgetSerialNumberRequest,
		GetWidgetSerialNumberReply = 10,
		SendRDMDiscoveryRequest
	};
}