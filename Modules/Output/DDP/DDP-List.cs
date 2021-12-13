using System;
using System.Collections.Generic;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System.Net.Sockets;
using System.Windows.Forms;

namespace VixenModules.Output.DDP
{
	internal class DDP : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private DDPData _data;
		private UdpClient _udpClient;
		private int _outputCount;
		private List<int> _packetSize;
		private List<byte> _channelBuffer;
		private List<byte> _ddpPacket;


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
			//_udpClient = new UdpClient();  //this doesn't work when I initialize it here
			_outputCount = 0;
			_packetSize = new List<int>();
			_channelBuffer = new List<byte>();
			_ddpPacket = new List<byte>();
			DataPolicyFactory = new DataPolicyFactory();
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
			DDPSetup setup = new DDPSetup(_data);
			if (setup.ShowDialog() == DialogResult.OK) {
				if (setup.Address != null)
					_data.Address = setup.Address;
				OpenConnection();
				return true;
			}

			return false;
		}

		public override void Start()
		{
			Logging.Trace(LogTag + "DDP: Start()");
			SetupPackets();
			OpenConnection();
			base.Start();
		}

		public override void Stop()
		{
			Logging.Trace(LogTag + "DDP: Stop()");
			CloseConnection();
			base.Stop();
		}

		public override void Pause()
		{
			Logging.Trace(LogTag + "DDP: Pause()");
			CloseConnection();
			base.Pause();
		}

		public override void Resume()
		{
			Logging.Trace(LogTag + "DDP: Resume()");
			OpenConnection();
			base.Resume();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (outputStates is null)  //added to prevent exceptions while loading profile
				return; 

			if (outputStates.Length != _channelBuffer.Count)
				SetupPackets();

			//Copy outputStates to channelBuffer in Byte form
			_channelBuffer.Clear();
			foreach (ICommand outputState in outputStates)
            {
				if (outputState is null)
					_channelBuffer.Add(0);
				else
					_channelBuffer.Add((byte)outputState.CommandValue);
			}

			//Move blocks from buffer to packet and send packets
			int dataStart = 0;
			int i = 0;
			foreach (int packetSize in _packetSize)
            {
				_ddpPacket.Clear(); 
				_ddpPacket.AddRange(_channelBuffer.GetRange(dataStart, packetSize));
				SendPacket( i );
				dataStart += packetSize;
				i++;
            }
		}

		private void SetupPackets()  //creates an indexed list of the sizes of each packet
		{
			int numPackets = _outputCount / DDP_CHANNELS_PER_PACKET; //numPackets is zero based
			int lastPacketSize = _outputCount % DDP_CHANNELS_PER_PACKET;
			_packetSize.Clear();
			if(numPackets > 0)
            {
				for (int i = 0; i < numPackets; i++)
					{ _packetSize.Add( DDP_CHANNELS_PER_PACKET ); }
				_packetSize.Add( lastPacketSize );
			}
			else
				{ _packetSize.Add( lastPacketSize); }
		}

		private void SendPacket( int packetNumber )
		{
			// When this is called, ddpPacket list should already contain
			// only the channel payload starting at index 0
			bool isLastPacket = packetNumber == _packetSize.Count - 1;
			//Build Header
			if (!isLastPacket)
				_ddpPacket.Insert( 0, DDP_FLAGS1_VER1 );
			else
				_ddpPacket.Insert( 0, DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH );
			_ddpPacket.Insert( 1, 0 ); //reserved for future use
			_ddpPacket.Insert( 2, 1 ); //Custom Data = 1
			_ddpPacket.Insert( 3, DDP_ID_DISPLAY );

			//Header Bytes 4-7:	Data offset in bytes - 32-bit number, MSB first
			int offset = packetNumber * DDP_CHANNELS_PER_PACKET;
			_ddpPacket.Insert( 4, (byte) ( (offset & 0xFF000000) >> 24 ) );
			_ddpPacket.Insert( 5, (byte) ( (offset & 0xFF0000) >> 16 ) );
			_ddpPacket.Insert( 6, (byte) ( (offset & 0xFF00) >> 8 ) );
			_ddpPacket.Insert( 7, (byte) ( (offset & 0xFF) ) );

			//Header Bytes 8-9:	Payload Length
			_ddpPacket.Insert( 8, (byte)((_packetSize[packetNumber] & 0xFF00) >> 8));
			_ddpPacket.Insert( 9, (byte)(_packetSize[packetNumber] & 0xFF) );

			//Open Fire!!
			_udpClient.Send(_ddpPacket.ToArray(), _ddpPacket.Count);
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
	}
}