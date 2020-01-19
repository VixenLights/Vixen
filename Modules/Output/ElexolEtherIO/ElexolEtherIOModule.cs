using System;
using System.Collections.Generic;
using Vixen.Module.Controller;
using System.Net.Sockets;
using System.Net;
using Vixen.Commands;
using System.Windows.Forms;

namespace VixenModules.Output.ElexolEtherIO
{
	public class ElexolEtherIOModule : ControllerModuleInstanceBase
	{
		private readonly static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private int _minIntensity = 1;
		private int _remotePort = 2424;
		private UdpClient _socket = new UdpClient();
		private IPAddress _remoteIPAddr;
		private ElexolEtherIOData _data;
		private ElexolEtherIOCommandHandler _commandHandler;
		

		public ElexolEtherIOModule()
		{
			_commandHandler = new ElexolEtherIOCommandHandler();
			DataPolicyFactory = new ElexolEtherIODataPolicyFactory();
			
			if (_data != null && _data.Port <= 0)
			{
				_data.Port = _remotePort;
			}
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (_socket.Available == 0)
			{
				if (!OpenConnection())
				{
					Logging.Warn("Elexol Ether I/O: failed to connect to device, not updating the current state.");
					return;
				}
			}
			int chan = 0;               // Current channel being processed.
			int i = 0;                  // Buffer iterator
			byte[] buf = new byte[6];   // The data buffer. ("A[byte_val]B[byte_val]C[byte_val]" = 6bytes)

			for (char port = 'A'; port <= 'C'; ++port)
			{
				buf[i++] = (byte)port;  // Port specification
				buf[i] = 0;             // Initialize value to zero
				for (int bit = 0; (bit < 8 && chan < outputStates.Length); ++bit, ++chan)
				{
					_commandHandler.Reset();
					ICommand command = outputStates[chan];
					if (command != null)
					{
						command.Dispatch(_commandHandler);
					}
					// If this channel's value is greater than minIntensity, turn on its bit for this port
					buf[i] |= (byte)(((_commandHandler.Value > _minIntensity) ? 0x01 : 0x00) << bit);
				}
				i++;
			}
			_socket.Send(buf, buf.Length);
		}

		public override bool Setup()
		{
			using (SetupDialog setup = new SetupDialog(_data))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					if(setup.IPAddr != null)
					{
						_data.MinimumIntensity = setup.MinIntensity;
						_data.Address = setup.IPAddr;
						_data.Port = setup.DataPort;
						initModule();
						OpenConnection();
						return true;
					}
				}
				return false;
			}
		}

		public override void Start()
		{
			OpenConnection();
			base.Start();
		}

		public override void Stop()
		{
			CloseConnection();
			base.Stop();
		}

		private void initModule()
		{
			if (_data.MinimumIntensity > 1)
			{

				_minIntensity = _data.MinimumIntensity;
			}
			if (_data.Port > 0)
			{
				_remotePort = _data.Port;
			}

			if (_data.Address != null)
			{
				_remoteIPAddr = _data.Address;
			}

		}

		private bool OpenConnection()
		{
			if (_remoteIPAddr != null)
			{
				try
				{
					_socket.Connect(_remoteIPAddr, _remotePort);

					// Set the ports to output mode ("!A\0"), and initialize each of them to zero ("A\0"):
					//"!A\0A\0..."
					_socket.Send(new byte[]{  (byte)'!', (byte)'A', 0,    // Set port A to output mode    (3 bytes)
                                        (byte)'A', 0,               // Initialize port A to 0       (2 bytes)
                                        (byte)'!', (byte)'B', 0,    // Set port B to output mode    (3 bytes)
                                        (byte)'B', 0,               // Initialize port B to 0       (2 bytes)
                                        (byte)'!', (byte)'C', 0,    // Set port C to output mode    (3 bytes)
                                        (byte)'C', 0 },             // Initialize port C to 0       (2 bytes)
									15);
					return true;
				}
				catch
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		private void CloseConnection()
		{
			_socket.Send(new byte[] { (byte)'A', 0, (byte)'B', 0, (byte)'C', 0 }, 6);
			_socket.Close();
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get	{ return _data; }
			set
			{
				_data = (ElexolEtherIOData)value;
				initModule();
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

	}

}
