using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;

namespace VixenModules.Output.BlinkyLinky
{
	class BlinkyLinky : ControllerModuleInstanceBase
	{
		private Dictionary<int, byte> _lastValues;
		private Dictionary<int, int> _nullCommands;
		private BlinkyLinkyData _data;
		private TcpClient _tcpClient;
		private NetworkStream _networkStream;
		private Stopwatch _timeoutStopwatch;
		private IDataPolicy _dataPolicy;

		public static byte HEADER_1 = 0xDE;
		public static byte HEADER_2 = 0xAD;
		public static byte HEADER_3 = 0xBE;
		public static byte HEADER_4 = 0xEF;

		public static byte COMMAND_SET_VALUES = 0x01;

	
		public BlinkyLinky()
		{
			_lastValues = new Dictionary<int, byte>();
			_nullCommands = new Dictionary<int, int>();
			_data = new BlinkyLinkyData();
			_timeoutStopwatch = new Stopwatch();
			_dataPolicy = new DataPolicy();
		}

		private void _setupDataBuffers()
		{
			for (int i = 0; i < this.OutputCount; i++) {
				if (!_lastValues.ContainsKey(i))
					_lastValues[i] = 0;
				if (!_nullCommands.ContainsKey(i))
					_nullCommands[i] = 0;
			}
		}

		protected override void _SetOutputCount(int outputCount)
		{
			_setupDataBuffers();
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as BlinkyLinkyData; CloseConnection(); }
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			BlinkyLinkySetup setup = new BlinkyLinkySetup(_data);
			if (setup.ShowDialog() == DialogResult.OK) {
				if (setup.Address != null)
					_data.Address = setup.Address;
				_data.Port = setup.Port;
				_data.Stream = setup.Stream;
				CloseConnection();
				return true;
			}

			return false;
		}

		private bool OpenConnection()
		{
			// start off closing the connection
			CloseConnection();

			if (_data.Address == null) {
				VixenSystem.Logging.Warning("BlinkyLinky: Trying to connect with a null IP address.");
				return false;
			}

			try {
				_tcpClient = new TcpClient();
				_tcpClient.Connect(_data.Address, _data.Port);
			} catch (Exception ex) {
				VixenSystem.Logging.Warning("BlinkyLinky: Failed to connect to remote host " + _data.Address.ToString() + ", " + _data.Port, ex);
				return false;
			}

			try {
				_networkStream = _tcpClient.GetStream();
			} catch (Exception ex) {
				VixenSystem.Logging.Warning("BlinkyLinky: Failed to get stream to communicate with remote host " + _data.Address.ToString() + ", " + _data.Port, ex);
				_tcpClient.Close();
				return false;
			}

			// reset the last values. That means that *any* values that come in will be 'new', and be sent out.
			_lastValues = new Dictionary<int, byte>();
			_nullCommands = new Dictionary<int, int>();
			_setupDataBuffers();

			_timeoutStopwatch.Reset();
			_timeoutStopwatch.Start();

			return true;
		}

		private void CloseConnection()
		{
			if (_networkStream != null) {
				_networkStream.Close();
				_networkStream = null;
			}

			if (_tcpClient != null) {
				_tcpClient.Close();
				_tcpClient = null;
			}

			_timeoutStopwatch.Reset();
		}

		public override void Start()
		{
			_setupDataBuffers();
			base.Start();
		}


		public override void UpdateState(ICommand[] outputStates) {
			if(_networkStream == null) {
				bool success = OpenConnection();
				if (!success) {
					VixenSystem.Logging.Warning("BlinkyLinky: failed to connect to device, not updating the current state.");
					return;
				}
			}

			// build up transmission packet
			byte[] data = new byte[1024];		// overkill, but it'll do
			int totalPacketLength = 0;

			// protocol is:	4 bytes header
			//				1 byte command
			//
			// at the moment, only the 'Set Values' blinky-protocol command is supported.
			// Set Values:	1 byte for the stream (ie. 0-2, for which RS485 stream)
			//				2 bytes for the length of following data (high byte first) - ie. only the channel/value bytes
			//				2 bytes (repeated): channel, value.

			data[totalPacketLength++] = HEADER_1;
			data[totalPacketLength++] = HEADER_2;
			data[totalPacketLength++] = HEADER_3;
			data[totalPacketLength++] = HEADER_4;
			data[totalPacketLength++] = COMMAND_SET_VALUES;

			data[totalPacketLength++] = (byte)_data.Stream;
			int lengthPosH = totalPacketLength++;
			int lengthPosL = totalPacketLength++;

			bool changed = false;
			for (int i = 0; i < outputStates.Length; i++) {
				byte newValue = 0;

				if (outputStates[i] != null) {
					LightingValueCommand lightingCommand = outputStates[i] as LightingValueCommand;
					if (lightingCommand == null)
						continue;
					newValue = (byte)(lightingCommand.CommandValue.Intensity * Byte.MaxValue);
					_nullCommands[i] = 0;
				} else {
					// it was a null command. We should turn it off; however, to avoid some potentially nasty flickering,
					// we will keep track of the null commands for this output, and ignore the first one. Any after that will
					// actually be sent through.
					if (_nullCommands[i] == 0) {
						_nullCommands[i] = 1;
						newValue = _lastValues[i];
					}
				}

				if (_lastValues[i] != newValue) {
					changed = true;
					data[totalPacketLength++] = (byte)i;
					data[totalPacketLength++] = newValue;
					_lastValues[i] = newValue;
				}
			}

			int totalData = totalPacketLength - lengthPosL - 1;
			data[lengthPosH] = (byte)((totalData >> 8) & 0xFF);
			data[lengthPosL] = (byte)(totalData & 0xFF);

			// don't bother writing anything if we haven't acutally *changed* any values...
			// (also, send at least a 'null' update command every 10 seconds. I think there's a bug in the micro
			// firmware; it doesn't seem to close network connections properly. Need to diagnose more, later.)
			if (changed || _timeoutStopwatch.ElapsedMilliseconds >= 10000) {
				try {
					_timeoutStopwatch.Restart();
					_networkStream.Write(data, 0, totalPacketLength);
					_networkStream.Flush();
				} catch (Exception ex) {
					VixenSystem.Logging.Warning("BlinkyLinky: failed to write data to device, this update state may be lost. Closing connection.", ex);
					CloseConnection();
				}
			}
		}

		public override IDataPolicy DataPolicy {
			get { return _dataPolicy; }
		}
	}
}
