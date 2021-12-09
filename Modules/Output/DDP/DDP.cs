using System;
using System.Collections.Generic;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;

namespace VixenModules.Output.DDP
{
	internal class DDP : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Dictionary<int, byte> _lastValues;
		private Dictionary<int, int> _nullCommands;
		private DDPData _data;
		private UdpClient _udpClient;
		//private NetworkStream _networkStream;
		private Stopwatch _timeoutStopwatch;
		private int _outputCount;

		private const int DDP_HEADER_LEN		= 10;
		private const int DDP_SYNCPACKET_LEN	= 10;

		private const byte DDP_FLAGS1_VER		= 0xc0;  // version mask
		private const byte DDP_FLAGS1_VER1		= 0x40;  // version=1
		private const byte DDP_FLAGS1_PUSH		= 0x01;
		private const byte DDP_FLAGS1_QUERY		= 0x02;
		private const byte DDP_FLAGS1_REPLY		= 0x04;
		private const byte DDP_FLAGS1_STORAGE	= 0x08;
		private const byte DDP_FLAGS1_TIME		= 0x10;

		private const int DDP_PORT			= 4048;
		private const int DDP_ID_DISPLAY	= 1;
		private const int DDP_ID_CONFIG		= 250;
		private const int DDP_ID_STATUS		= 251;
		
		private const int DDP_CHANNELS_PER_PACKET = 1440; //1440 channels per packet
		private const int DDP_PACKET_LEN = (DDP_HEADER_LEN + DDP_CHANNELS_PER_PACKET);

		public DDP()
		{
			Logging.Trace("Constructor()");
			_lastValues = new Dictionary<int, byte>();
			_nullCommands = new Dictionary<int, int>();
			_data = new DDPData();
			_timeoutStopwatch = new Stopwatch();
			DataPolicyFactory = new DataPolicyFactory();
		}

		private void _setupDataBuffers()
		{
			Logging.Trace(LogTag + "_setupDataBuffers()");
			for (int i = 0; i < this.OutputCount; i++) {
				if (!_lastValues.ContainsKey(i))
					_lastValues[i] = 0;
				if (!_nullCommands.ContainsKey(i))
					_nullCommands[i] = 0;
			}
		}

		public override int OutputCount
		{
			get { return _outputCount; }
			set
			{
				_outputCount = value;
				_setupDataBuffers();
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as DDPData;
				CloseConnection();
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			Logging.Trace(LogTag + "Setup()");
			DDPSetup setup = new DDPSetup(_data);
			if (setup.ShowDialog() == DialogResult.OK) {
				if (setup.Address != null)
					_data.Address = setup.Address;
				CloseConnection();
				return true;
			}

			return false;
		}

		private bool FakingIt()
		{
			return _data.Address.ToString().Equals("1.1.1.1");
		}

		private string HostInfo()
		{
			return _data.Address + ":";
		}

		private string LogTag
		{
			get { return "[" + HostInfo() + "]: "; }
		}

		private bool OpenConnection()
		{
			Logging.Trace(LogTag + "OpenConnection()");

			// start off by closing the connection
			CloseConnection();

			if (_data.Address == null) {
				Logging.Warn(LogTag + "Trying to connect with a null IP address.");
				return false;
				}

			if( FakingIt())
				return true;

			try {
				_udpClient = new UdpClient();
				_udpClient.Connect(_data.Address, DDP_PORT);
				//_udpClient.Connect(_data Hostname, DDP_PORT);  //Switch to add Hostname support
				}
			catch (Exception ex) {
				Logging.Warn(LogTag + "DDP: Failed connect to host", ex);
				return false;
				}

			// reset the last values. That means that *any* values that come in will be 'new', and be sent out.
			_lastValues = new Dictionary<int, byte>();
			//_nullCommands = new Dictionary<int, int>();
			_setupDataBuffers();

			_timeoutStopwatch.Reset();
			_timeoutStopwatch.Start();

			return true;
		}

		private void CloseConnection()
		{
			Logging.Trace(LogTag + "CloseConnection()");

			if (FakingIt())
				return;

			if (_udpClient != null) {
				Logging.Trace(LogTag + "Closing UDP client...");
				_udpClient.Close();
				Logging.Trace(LogTag + "UDP client closed.");
				_udpClient = null;
			}

			_timeoutStopwatch.Reset();
		}

		public override void Start()
		{
			Logging.Trace(LogTag + "Start()");
			_setupDataBuffers();
			base.Start();
		}

		public override void Stop()
		{
			Logging.Trace(LogTag + "Stop()");
			base.Stop();
		}

		public override void Pause()
		{
			Logging.Trace(LogTag + "Pause()");
			base.Pause();
		}

		public override void Resume()
		{
			Logging.Trace(LogTag + "Resume()");
			base.Resume();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (!FakingIt()) {
				bool success = OpenConnection();
				if (!success) {
					Logging.Warn(LogTag + "failed to connect to device, not updating state");
					return;
				}
			}
			int dataOffset = 0;  //need to incorporate offset counter.
			int pktChanCtr = 0;
			byte[] packetData = new byte[DDP_CHANNELS_PER_PACKET+10];
			//************this is my 2nd attempt at the output drive
			for(int output = 0; output < _outputCount; output++)
				{				
				if(pktChanCtr < DDP_CHANNELS_PER_PACKET)
					{
					pktChanCtr++;
					dataOffset++;
					if (outputStates[output] is _8BitCommand command)
                        packetData[pktChanCtr+10] = (byte)outputStates[output].CommandValue;
					else
						packetData[pktChanCtr+10] = 0; //not a command, send a zero
					}
				else
					{
					//build header
					if(output < _outputCount)
						packetData[0] = DDP_FLAGS1_VER1;
					else
						packetData[0] = DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH;
					packetData[1] = 0; //reserved for future use
					packetData[2] = 1; //Custom Data = 1
					packetData[3] = DDP_ID_DISPLAY;

					//Header Bytes 4-7	Data offset in bytes
					//					(in units based on data-type.  ie: RGB=3 bytes=1 unit) or bytes??  32-bit number, MSB first
					packetData[4] = (byte)((dataOffset & 0xFF000000) >> 24);
					packetData[5] = (byte)((dataOffset & 0xFF0000) >> 16);
					packetData[6] = (byte)((dataOffset & 0xFF00) >> 8);
					packetData[7] = (byte)((dataOffset & 0xFF));

					//Header Bytes 8-9	Packet Size
					packetData[8] = (byte)(pktChanCtr & 0xFF00 >> 8);
					packetData[9] = (byte)(pktChanCtr & 0xFF);

					pktChanCtr = 0; //reset packet channel counter

					//Send Packet Now.
					_udpClient.Send(packetData, packetData.Length);
				}
			CloseConnection();
			}
		}
	}
}