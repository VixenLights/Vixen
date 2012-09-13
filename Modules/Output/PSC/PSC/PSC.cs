using System;
using System.IO.Ports;
using System.Threading;

namespace VixenModules.Controller.PSC {
	class PSC {
		private byte[] _packet;

		public const ushort RangeLow = 250;
		public const ushort RangeHigh = 1250;
		public const ushort RangeWidth = RangeHigh - RangeLow;
		public const ushort RangeHalf = RangeWidth >> 1;
		public const ushort RangeMid = RangeLow + RangeHalf;

		public PSC() {
			_packet = new byte[] { (byte)'!', (byte)'S', (byte)'C', 0, 0, 0, 0, 0x0D };
		}

		private SerialPort _serialPort;

		public SerialPort SerialPort {
			get { return _serialPort; }
			set {
				_serialPort = value;
				if(_serialPort != null) {
					_serialPort.ReadTimeout = 500;
				}
			}
		}

		public int BaudRate {
			get {
				if(SerialPort.BaudRate != 2400) {
					_SetBaudRate(2400);
				}
				if(Ping()) return SerialPort.BaudRate;
				_SetBaudRate(38400);
				if(Ping()) return SerialPort.BaudRate;
				return 0;
			}
			set {
				byte rate = value == 2400 ? (byte)0 : (byte)1;
				byte[] packet = new byte[] { (byte)'!', (byte)'S', (byte)'C', (byte)'S', (byte)'B', (byte)'R', rate, 0x0D };

				// Using the two step command-response does not do anything with the
				// port open state.
				if(!SerialPort.IsOpen) SerialPort.Open();

				SerialPort.DiscardInBuffer();
				_PSCCommand(packet);

				// Read the echo
				byte[] dump = new byte[8];
				SerialPort.Read(dump, 0, 8);

				// Update baud rate
				_SetBaudRate(value);

				// Going to ignore the response for now
				SerialPort.DiscardInBuffer();
			}
		}

		public bool Ping() {
			try {
				byte[] packet = new byte[] { (byte)'!', (byte)'S', (byte)'C', (byte)'V', (byte)'E', (byte)'R', (byte)'?', 0x0D };
				byte[] response = _PSCCommandAndResponse(packet);
				return
					char.IsDigit((char)response[packet.Length + 0]) &&
					response[packet.Length + 1] == '.' &&
					char.IsDigit((char)response[packet.Length + 2]);
			} catch {
				return false;
			}
		}

		public void SetPosition(byte index, ushort position) {
			_packet[3] = index;
			_packet[4] = 0;
			_packet[5] = (byte)position;
			_packet[6] = (byte)(position >> 8);
			SerialPort.Write(_packet, 0, _packet.Length);
			SerialPort.DiscardInBuffer();
			Thread.Sleep(10);
		}

		private byte[] _PSCCommandAndResponse(byte[] commandPacket) {
			bool wasOpen = SerialPort.IsOpen;
			// Response will be an echo plus a 3-byte reply
			byte[] response = new byte[commandPacket.Length + 3];

			if(!SerialPort.IsOpen) SerialPort.Open();

			SerialPort.Write(commandPacket, 0, commandPacket.Length);
			Thread.Sleep(400);

			// Need to wait for all expected bytes, or a timeout exception
			try {
				int index = 0;
				while(index < response.Length) {
					response[index++] = (byte)SerialPort.ReadByte();
				}
			} catch(TimeoutException) {
				// Sometimes, we just get the echo and no response?
				// Just be done.
			} catch {
				SerialPort.Close();
				throw;
			}

			if(SerialPort.IsOpen != wasOpen) {
				if(wasOpen) {
					SerialPort.Open();
				} else {
					SerialPort.Close();
				}
			}
			return response;
		}

		private void _PSCCommand(byte[] commandPacket) {
			if(!SerialPort.IsOpen) SerialPort.Open();

			SerialPort.Write(commandPacket, 0, commandPacket.Length);
		}

		private void _SetBaudRate(int baudRate) {
			bool wasOpen = SerialPort.IsOpen;
			if(wasOpen) {
				SerialPort.Close();
			}

			SerialPort.BaudRate = baudRate;

			if(wasOpen) {
				Thread.Sleep(100);
				SerialPort.Open();
			}
		}

	}
}
