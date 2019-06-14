namespace VixenModules.Output.DmxUsbPro
{
	using System;
	using System.IO.Ports;
	using Vixen.Commands;

	internal class DmxUsbProSender : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger(); 
		
		private readonly Message _dmxPacketMessage;

		private readonly byte[] _statePacket;

		private SerialPort _serialPort;

		public DmxUsbProSender(SerialPort serialPort)
		{
			this._serialPort = serialPort;
			this._statePacket = new byte[513];
			this._dmxPacketMessage = new Message(MessageType.OutputOnlySendDMXPacketRequest)
			                         	{
			                         		Data = this._statePacket
			                         	};
		}

		~DmxUsbProSender()
		{
			this.Dispose();
		}

		public void SendDmxPacket(ICommand[] outputStates)
		{
			if (outputStates == null || this._statePacket == null || _serialPort == null) {
				return;
			}

			var channelValues = new byte[outputStates.Length];
			for (int index = 0; index < outputStates.Length; index++) {
				_8BitCommand command = outputStates[index] as _8BitCommand;
				if (command == null) {
					// State reset
					channelValues[index] = 0;
					continue;
				}

				channelValues[index] = command.CommandValue;
			}

			if (!this._serialPort.IsOpen) {
				//this._serialPort.Open();
				return;
			}

			this._statePacket[0] = 0; // Start code
			Array.Copy(channelValues, 0, this._statePacket, 1, Math.Min(512, channelValues.Length));
			byte[] packet = this._dmxPacketMessage.Packet;
			if (packet != null) {
				this._serialPort.Write(packet, 0, packet.Length);
			}
		}

		public void Start()
		{
			try {
				if (_serialPort != null && !this._serialPort.IsOpen) {
					this._serialPort.Open();
				}
			}
			catch (Exception ex) {
				Logging.Error(ex, "Serial Port Open failed");
			}
		}

		public void Stop()
		{
			if (_serialPort != null && this._serialPort.IsOpen) {
				this._serialPort.Close();
			}
		}

		public void Dispose()
		{
			if (this._serialPort != null && this._serialPort.IsOpen) {
				this._serialPort.Close();
			}
			if (this._serialPort != null) {
				this._serialPort.Dispose();
				this._serialPort = null;
			}

			GC.SuppressFinalize(this);
		}
	}
}