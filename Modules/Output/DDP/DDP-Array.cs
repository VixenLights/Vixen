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
		private byte[] _channelBuffer;
		private byte[] _ddpPacket;
		private byte[] _ddpLastPacket;


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
			_ddpPacket = new byte[DDP_PACKET_LEN];
			DataPolicyFactory = new DataPolicyFactory();
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
			if ( (outputStates is null) || (_outputCount ==0) )  //added to prevent exceptions while loading profile
				return;

			if (outputStates.Length != _channelBuffer.Length)
				SetupPackets();

			//Copy outputStates to channelBuffer in Byte form
			for (int i = 0; i < outputStates.Length; i++)
			{
				if (outputStates[i] is null)
				_channelBuffer[i] = 0;
				else
					_channelBuffer[i] = (byte)outputStates[i].CommandValue;
			}

			//Move blocks from buffer to packet and send packets
			int dataStart = 0;
			int packet = 0;
			foreach (int packetSize in _packetSize)
            {
				Array.Copy(_channelBuffer, packet*DDP_CHANNELS_PER_PACKET, _ddpPacket, 10, packetSize);
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
			_channelBuffer = new byte[_outputCount];
			_ddpLastPacket = new byte[lastPacketSize+DDP_HEADER_LEN];

			_packetSize.Clear();
			if(numPackets > 0)
            {
				for (int i = 0; i < numPackets; i++)
					{ _packetSize.Add( DDP_CHANNELS_PER_PACKET ); }
				_packetSize.Add( lastPacketSize );
			}
			else
				{ _packetSize.Add( lastPacketSize); }

			//Prefill Header Info
			_ddpPacket[0] = DDP_FLAGS1_VER1;
			_ddpPacket[1] = 0; //reserved for future use
			_ddpPacket[2] = 1; //Custom Data = 1
			_ddpPacket[3] = DDP_ID_DISPLAY;
			Array.Copy(_ddpPacket, _ddpLastPacket, 4);  //duplicate to lastPacket
			_ddpLastPacket[0] = DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH; //Set last packet to push

			//Header Bytes 8-9:	Payload Length
			_ddpPacket[8] = (byte)((DDP_CHANNELS_PER_PACKET & 0xFF00) >> 8);
			_ddpPacket[9] = (byte)(DDP_CHANNELS_PER_PACKET & 0xFF);

			_ddpLastPacket[8] = (byte)((lastPacketSize & 0xFF00) >> 8);
			_ddpLastPacket[9] = (byte)(lastPacketSize & 0xFF);
		}

		private void SendPacket( int packetNumber )
		{
			// When this is called, _ddpPacket or _ddpLastPacket should already contain
			// the fixed header and channel payload starting at index 10
			// still need to calculate offset
			
			//Add Offset to Header.  Then Send.
			//Header Bytes 4-7:	Data offset in bytes - 32-bit number, MSB first
			int offset = packetNumber * DDP_CHANNELS_PER_PACKET;
			if (!(packetNumber == _packetSize.Count - 1))  //if not last packet
			{
				_ddpPacket[4] = (byte)((offset & 0xFF000000) >> 24);
				_ddpPacket[5] = (byte)((offset & 0xFF0000) >> 16);
				_ddpPacket[6] = (byte)((offset & 0xFF00) >> 8);
				_ddpPacket[7] = (byte)((offset & 0xFF));

				_udpClient.Send(_ddpPacket, _ddpPacket.Length);
			}
			else
			{
				_ddpLastPacket[4] = (byte)((offset & 0xFF000000) >> 24);
				_ddpLastPacket[5] = (byte)((offset & 0xFF0000) >> 16);
				_ddpLastPacket[6] = (byte)((offset & 0xFF00) >> 8);
				_ddpLastPacket[7] = (byte)((offset & 0xFF));

				_udpClient.Send(_ddpLastPacket, _ddpLastPacket.Length);
			}
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