using System;
using System.Collections.Generic;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace VixenModules.Output.DDP
{
	internal class DDP : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private DDPData _data;
		private UdpClient _udpClient;
		private int _outputCount = 0;
		private List<int> _packetSize = new List<int>();
		private byte[] _ddpPacket;
		private bool _isRunning = false;

		private const int DDP_HEADER_LEN = 10;
		private const int DDP_SYNCPACKET_LEN = 10;

		private const byte DDP_FLAGS1_VER = 0xc0;  // version mask
		private const byte DDP_FLAGS1_VER1 = 0x40;  // version=1
		private const byte DDP_FLAGS1_PUSH = 0x01;
		private const byte DDP_FLAGS1_QUERY = 0x02;
		private const byte DDP_FLAGS1_REPLY = 0x04;
		private const byte DDP_FLAGS1_STORAGE = 0x08;
		private const byte DDP_FLAGS1_TIME = 0x10;

		private const int DDP_PORT = 4048;
		private const int DDP_ID_DISPLAY = 1;
		private const int DDP_ID_CONFIG = 250;
		private const int DDP_ID_STATUS = 251;

		private const int DDP_CHANNELS_PER_PACKET = 1440; //1440 channels per packet
		private const int DDP_PACKET_LEN = (DDP_HEADER_LEN + DDP_CHANNELS_PER_PACKET);

		public DDP()
		{
			Logging.Trace("Constructor()");
			_data = new DDPData();
			_ddpPacket = new byte[DDP_PACKET_LEN];
			DataPolicyFactory = new DataPolicyFactory();
			SupportsNetwork = true;
			SetupPackets();
			OpenConnection();
		}

		public override int OutputCount
		{
			get { return _outputCount; }
			set {				
				_outputCount = value;
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set {
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
			_isRunning = false;
			DDPSetup setup = new DDPSetup(_data);
			if (setup.ShowDialog() == DialogResult.OK) {
				if (setup.Address != null)
					_data.Address = setup.Address;
				OpenConnection();
				return true;
			}
			_isRunning = true;
			return false;
		}

		public override void Start()
		{
			Logging.Trace(LogTag + "DDP: Start()");
			SetupPackets();
			OpenConnection();
			base.Start();
			_isRunning = true;
		}

		public override void Stop()
		{
			Logging.Trace(LogTag + "DDP: Stop()");
			_isRunning = false;
			CloseConnection();
			base.Stop();
		}

		public override void Pause()
		{
			Logging.Trace(LogTag + "DDP: Pause()");
			_isRunning = false;
			CloseConnection();
			base.Pause();
		}

		public override void Resume()
		{
			Logging.Trace(LogTag + "DDP: Resume()");
			OpenConnection();
			base.Resume();
			_isRunning = true;
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (!_isRunning)
				return;

			if ((outputStates is null) || (_outputCount == 0))  //added to prevent exceptions while loading profile
				return;

			if (outputStates.Length != _outputCount)
				SetupPackets();
			
			int dataStart = 0;
			int packet = 0;
			int channel = 0;
			foreach (int packetSize in _packetSize)
			{
				//copy bytes from state buffer to packet
				for (int i = 0; i < packetSize; i++)
				{
					if (outputStates[channel] is _8BitCommand cmd)
						_ddpPacket[i + 10] = cmd.CommandValue;
					else
						_ddpPacket[i + 10] = 0;
					channel++;
				}
				SendPacket(packet);

				dataStart += packetSize;
				packet++;
			}
		}

		private void SetupPackets()  //creates an indexed list of the sizes of each packet
		{
			if (_outputCount == 0)
				return;

			int numPackets = _outputCount / DDP_CHANNELS_PER_PACKET; //numPackets is zero based
			int lastPacketSize = _outputCount % DDP_CHANNELS_PER_PACKET;

			//Build a list containing the size of each packet.
			_packetSize.Clear();
			if(numPackets > 0)
            {
				for (int i = 0; i < numPackets; i++)
					{ _packetSize.Add( DDP_CHANNELS_PER_PACKET ); }
				_packetSize.Add( lastPacketSize );
			}
			else
				_packetSize.Add( lastPacketSize);

			//Prefill Header Info
			_ddpPacket[0] = DDP_FLAGS1_VER1;
			_ddpPacket[1] = 0; //reserved for future use
			_ddpPacket[2] = 1; //Custom Data = 1
			_ddpPacket[3] = DDP_ID_DISPLAY;
		}

		private void SendPacket( int packetNumber )
		{
			// When this is called, _ddpPacket should already contain the fixed 
			// header bytes and the channel payload starting at index 10
			// We still need to calculate offset

			//Header Bytes 4-7:	Data offset in bytes - 32-bit number, MSB first
			int offset = packetNumber * DDP_CHANNELS_PER_PACKET;

			_ddpPacket[4] = (byte)((offset & 0xFF000000) >> 24);
			_ddpPacket[5] = (byte)((offset & 0xFF0000) >> 16);
			_ddpPacket[6] = (byte)((offset & 0xFF00) >> 8);
			_ddpPacket[7] = (byte)((offset & 0xFF));

			//Header Bytes 8-9:	Payload Length
			_ddpPacket[8] = (byte)((_packetSize[packetNumber] & 0xFF00) >> 8);
			_ddpPacket[9] = (byte)(_packetSize[packetNumber] & 0xFF);

			if ( !(packetNumber == _packetSize.Count - 1) )  //if not last packet
				_ddpPacket[0] = DDP_FLAGS1_VER1;  //No push flag
			else
				_ddpPacket[0] = DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH; //Set push flag on last packet
			_udpClient.Send(_ddpPacket, _packetSize[packetNumber]+10);
		}

		private bool OpenConnection()
		{
			Logging.Trace(LogTag + "DDP: OpenConnection()");

			// start off by closing the connection
			CloseConnection();

			if (_data.Address == null) {
				Logging.Warn(LogTag + "DDP: Trying to connect with a null IP address.");
				return false;
			}

			try {
				_udpClient = new UdpClient(); //doesn't work when in class constructor
				_udpClient.Connect(_data.Address, DDP_PORT);
				//_udpClient.Connect(_data Hostname, DDP_PORT);  //Switch to add Hostname support
			}
			catch {
				Logging.Warn(LogTag + "DDP: Failed connect to host");
				return false;
			}

			return true;
		}

		private void CloseConnection()
		{
			Logging.Trace(LogTag + "DDP: CloseConnection()");

			if (_udpClient != null)
			{
				Logging.Trace(LogTag + "DDP: Closing UDP client...");
				_udpClient.Close();
				Logging.Trace(LogTag + "DDP: UDP client closed.");
				_udpClient = null;
			}
		}

		private string HostInfo()
		{
			return _data.Address + ":";
		}

		private string LogTag
		{
			get { return "[" + HostInfo() + "]: "; }
		}
		/*****************************************
		 *    Support for FPP Universe Export
		 *****************************************/
		public override ControllerNetworkConfiguration GetNetworkConfiguration()
		{
			var config = new ControllerNetworkConfiguration();
			config.SupportsUniverses = true;
			config.IpAddress = _data.Address;
			config.ProtocolType = ProtocolTypes.DDP;
			config.TransmissionMethod = TransmissionMethods.Unicast;
			var universes = new List<UniverseConfiguration>(1);
			
			var uc = new UniverseConfiguration();
			uc.UniverseNumber = 1; //not needed for DDP
			uc.Start = 1; //not needed for DDP
			uc.Size = _outputCount;
			uc.Active = true;
			universes.Add(uc);
			
			config.Universes = universes;
			return config;
		}
	}
}