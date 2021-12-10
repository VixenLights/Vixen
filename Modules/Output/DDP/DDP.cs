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
		private DDPData _data;
		private UdpClient _udpClient;
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
			_data = new DDPData();
			_timeoutStopwatch = new Stopwatch();
			DataPolicyFactory = new DataPolicyFactory();
			OpenConnection();
		}

		public override int OutputCount
		{
			get { return _outputCount; }
			set { _outputCount = value;	}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set	{
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

		public override void Start()
		{
			Logging.Trace(LogTag + "Start()");
			OpenConnection();
			base.Start();
		}

		public override void Stop()
		{
			Logging.Trace(LogTag + "Stop()");
			CloseConnection();
			base.Stop();
		}

		public override void Pause()
		{
			Logging.Trace(LogTag + "Pause()");
			CloseConnection();
			base.Pause();
		}

		public override void Resume()
		{
			Logging.Trace(LogTag + "Resume()");
			OpenConnection();
			base.Resume();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			/*bool success = OpenConnection();
			if (!success) {
				Logging.Warn(LogTag + "failed to connect to device, not updating state");
				return;
				}*/
			int pktChanCtr = 0;
			byte[] packetData = new byte[DDP_CHANNELS_PER_PACKET];
			int packetNumber = 0;  //zero based packet per frame counter
			for (int output = 0; output < _outputCount; output++)

				{
				if (outputStates[output] is _8BitCommand command)
					packetData[pktChanCtr] = (byte)outputStates[output].CommandValue;
				else
					packetData[pktChanCtr] = 0; //not a command, send a zero

				if (pktChanCtr == DDP_CHANNELS_PER_PACKET - 1)
					{
					SendPacket(packetData, false, packetNumber);
					pktChanCtr = 0;
					packetNumber++;
				}
				else
					pktChanCtr++;				
				}
			//send last packet here
			SendPacket(packetData, true, packetNumber);
			//CloseConnection();			
		}

		private void SendPacket(byte[] packetPayload, bool isLastPacket, int packetNumber)
		{
			byte[] ddpPacket = new byte[DDP_CHANNELS_PER_PACKET + 10];
			Array.Copy(packetPayload, 0, ddpPacket, 10, packetPayload.Length);
			//build header
			if (!isLastPacket)
				ddpPacket[0] = DDP_FLAGS1_VER1;
			else
				ddpPacket[0] = DDP_FLAGS1_VER1 | DDP_FLAGS1_PUSH;
			ddpPacket[1] = 0; //reserved for future use
			ddpPacket[2] = 1; //Custom Data = 1
			ddpPacket[3] = DDP_ID_DISPLAY;

			//Header Bytes 4-7	Data offset in bytes
			//					(in units based on data-type.  ie: RGB=3 bytes=1 unit) or bytes??  32-bit number, MSB first
			int offset = 0;
			offset = packetNumber * DDP_CHANNELS_PER_PACKET;
			ddpPacket[4] = (byte)((offset & 0xFF000000) >> 24);
			ddpPacket[5] = (byte)((offset & 0xFF0000) >> 16);
			ddpPacket[6] = (byte)((offset & 0xFF00) >> 8);
			ddpPacket[7] = (byte)((offset & 0xFF));

			//Header Bytes 8-9	Packet Size
			ddpPacket[8] = (byte)(packetPayload.Length & 0xFF00 >> 8);
			ddpPacket[9] = (byte)(packetPayload.Length & 0xFF);

			//Send Packet Now.
			_udpClient.Send(ddpPacket, ddpPacket.Length);
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

			try {
				_udpClient = new UdpClient();
				_udpClient.Connect(_data.Address, DDP_PORT);
				//_udpClient.Connect(_data Hostname, DDP_PORT);  //Switch to add Hostname support
			}
			catch (Exception ex) {
				Logging.Warn(LogTag + "DDP: Failed connect to host", ex);
				return false;
			}

			return true;
		}

		private void CloseConnection()
		{
			Logging.Trace(LogTag + "CloseConnection()");

			if (_udpClient != null)
			{
				Logging.Trace(LogTag + "Closing UDP client...");
				_udpClient.Close();
				Logging.Trace(LogTag + "UDP client closed.");
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