namespace VixenModules.Output.DmxUsbPro
{
	using System;

	internal class Message
	{
		private readonly MessageType _messageType;

		private byte[] _data;

		private ushort _dataLength; // 0 - 600

		public Message(MessageType type)
		{
			this._messageType = type;
		}

		public byte[] Data
		{
			set
			{
				this._data = value;
				this._dataLength = (ushort) Math.Min(value == null ? 0 : value.Length, 600);
			}
		}

		public byte[] Packet
		{
			get
			{
				var packet = new byte[5 + this._dataLength];
				packet[0] = 0x7e;
				packet[1] = (byte) this._messageType;
				packet[2] = (byte) this._dataLength;
				packet[3] = (byte) (this._dataLength >> 8);
				packet[4 + this._dataLength] = 0xe7;
				if (this._data != null) {
					this._data.CopyTo(packet, 4);
				}

				return packet;
			}
		}
	}
}