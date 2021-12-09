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
		private NetworkStream _networkStream;
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


		/* FROM BLINKYLINKY
		 * public static byte HEADER_1 = 0xDE;
		public static byte HEADER_2 = 0xAD;
		public static byte HEADER_3 = 0xBE;
		public static byte HEADER_4 = 0xEF;

		public static byte COMMAND_SET_VALUES = 0x01;
		*/

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

			// start off closing the connection
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

			/*
			try {
				_networkStream = _udpClient.GetStream();
				Logging.Debug(LogTag + "New connection to  host");
				Logging.Debug(LogTag + "(WriteTimeout default is " + _networkStream.WriteTimeout + ")");
			}
			catch (Exception ex) {
				Logging.Warn(LogTag + "Failed stream for host", ex);
				_udpClient.Close();
				return false;
			}
			*/
			

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
			Logging.Trace(LogTag + "CloseConnection()");

			if (FakingIt())
				return;

			if (_networkStream != null) {
				Logging.Trace(LogTag + "Closing network stream...");
				_networkStream.Close();
				Logging.Trace(LogTag + "Network stream closed.");
				_networkStream = null;
			}

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
			if (_networkStream == null && !FakingIt()) {
				bool success = OpenConnection();
				if (!success) {
					Logging.Warn(LogTag + "failed to connect to device, not updating state");
					return;
				}
			}

			//How many DDP packets do we need per frame?
			int pktCount = _outputCount / DDP_CHANNELS_PER_PACKET;
			if ((_outputCount % DDP_CHANNELS_PER_PACKET)>0){
				pktCount++;
				}

			// build up transmission packet
			byte[,] ddpPackets = new byte[pktCount,DDP_PACKET_LEN];
			int totalPacketLength = 0;

			for (int packet = 0; packet < pktCount; packet++)
			{
				//ddpPackets[packet] = (unsigned char*)calloc(1, DDP_HEADER_LEN);

			// use scatter/gather for the packet.   One IOV will contain
			// the header, the second will point into the raw channel data
			// and will be set at output time.   This avoids any memcpy.
				//ddpIovecs[packet * 2].iov_base = ddpBuffers[packet];
				//ddpIovecs[packet * 2].iov_len = DDP_HEADER_LEN;
				//ddpIovecs[packet * 2 + 1].iov_base = nullptr;

			ddpPackets[packet,0] = DDP_FLAGS1_VER1;
			ddpPackets[packet,1] = 0; //reserved for future use
			ddpPackets[packet,2] = 1;
			ddpPackets[packet,3] = DDP_ID_DISPLAY;
			
			int pktSize = DDP_CHANNELS_PER_PACKET;

			if (packet == (pktCount - 1))  //this is the last packet
				{
				ddpPackets[packet,0] = DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH;  //set push flag
				if ( (_outputCount % DDP_CHANNELS_PER_PACKET) >0 )
					{
					pktSize = _outputCount % DDP_CHANNELS_PER_PACKET;
					}
				}
			
			//THIS IS WHERE I LEFT OFF....
			
			ddpIovecs[packet * 2 + 1].iov_len = pktSize;

			//Header Bytes 4-7	Data offset in bytes
			//					(in units based on data-type.
			//    				ie: RGB=3 bytes=1 unit) or bytes??  32-bit number, MSB first
			ddpPackets[packet,4] = (chan & 0xFF000000) >> 24;
			ddpPackets[packet,5] = (chan & 0xFF0000) >> 16;
			ddpPackets[packet,6] = (chan & 0xFF00) >> 8;
			ddpPackets[packet,7] = (chan & 0xFF);

			//Header Bytes 8-9	Packet Size
			ddpPackets[packet,8] = (byte)(pktSize & 0xFF00 >> 8);
			ddpPackets[packet,9] = (byte)(pktSize & 0xFF);

			chan += pktSize;

			//FROM ORIGINAL BlinkyLinky 
			int lengthPosH = totalPacketLength++;
			int lengthPosL = totalPacketLength++;

			bool changed = false;
			for (int i = 0; i < outputStates.Length; i++) {
				byte newValue = 0;

				if (outputStates[i] != null) {
					_8BitCommand command = outputStates[i] as _8BitCommand;
						
						if (command == null)
						continue;
					newValue = command.CommandValue;
					_nullCommands[i] = 0;
				}
				/*  I'm going to ignore this eror checking for now - JC
				 * else {
					// it was a null command. We should turn it off; however, to avoid some potentially nasty flickering,
					// we will keep track of the null commands for this output, and ignore the first one. Any after that will
					// actually be sent through.
					if (_nullCommands[i] == 0) {
						_nullCommands[i] = 1;
						newValue = _lastValues[i];
					}
				*/
				}

				if (_lastValues[i] != newValue) {
					changed = true;
					data[totalPacketLength++] = (byte) i;
					data[totalPacketLength++] = newValue;
					_lastValues[i] = newValue;
				}
			}

			int totalData = totalPacketLength - lengthPosL - 1;
			data[lengthPosH] = (byte) ((totalData >> 8) & 0xFF);
			data[lengthPosL] = (byte) (totalData & 0xFF);

			// don't bother writing anything if we haven't acutally *changed* any values...
			// (also, send at least a 'null' update command every 10 seconds. I think there's a bug in the micro
			// firmware; it doesn't seem to close network connections properly. Need to diagnose more, later.)
			if (changed || _timeoutStopwatch.ElapsedMilliseconds >= 10000) {
				try {
					_timeoutStopwatch.Restart();
					if (FakingIt()) {
						System.Threading.Thread.Sleep(1);
					}
					else {
						_networkStream.Write(data, 0, totalPacketLength);
						_networkStream.Flush();
					}
				}
				catch (Exception ex) {
					Logging.Warn(LogTag + "failed to write data to device during update", ex);
					CloseConnection();
				}
			}
		}
	}
}


