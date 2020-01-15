using System;
using System.Collections.Generic;
using Vixen.Module.Controller;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Text;
using System.IO;
using Vixen.Commands;


namespace VixenModules.Output.ElexolUSBIO
{
	public class ElexolUSBModule : ControllerModuleInstanceBase
	{
		private ElexolUSBData _data;
		private SerialPort _serialPort;
		private ElexolUSBCommandHandler _commandHandler;
		private System.Timers.Timer _retryTimer;

		private int _retryCounter;
		private int _minIntensity = 1;

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public ElexolUSBModule()
		{
			_commandHandler = new ElexolUSBCommandHandler();
			DataPolicyFactory = new ElexolUSBDataPolicyFactory();

			//set 2 minute timer before retrying to access com port
			_retryTimer = new System.Timers.Timer(120000);
			_retryTimer.Elapsed += new ElapsedEventHandler(_retryTimer_Elapsed);
			_retryCounter = 0;
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			int chan = 0; // Current channel being processed.
			byte[] buf = new byte[2];   // The serial data buffer.

			if (serialPortIsValid && _serialPort.IsOpen)
			{
				for (char port = 'A'; port <= 'C'; ++port)
				{
					buf[0] = (byte)port;
					buf[1] = 0;
					for (int bit = 0; (bit < 8 && chan < outputStates.Length && IsRunning); ++bit, ++chan)
					{
						_commandHandler.Reset();
						ICommand command = outputStates[chan];
						if (command != null)
						{
							command.Dispatch(_commandHandler);
						}
						buf[1] |= (byte)(((_commandHandler.Value > _minIntensity) ? 0x01 : 0x00) << bit);
					}
					_serialPort.Write(buf, 0, 2);
				}
			}
		}

		public override bool HasSetup
		{
			get
			{
				return true;
			}
		}

		public override bool Setup()
		{
			using (SetupDialog setup = new SetupDialog(_data))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					SerialPort port = setup.Port;
					if (port != null)
					{
						initModule();
						return _data.IsValid;
					}
				}
			}

			return false;
		}

		public override void Start()
		{
			base.Start();
			if (serialPortIsValid && !_serialPort.IsOpen)
			{
				_OpenComPort();

				_serialPort.Write("!A" + (char)0);
				_serialPort.Write("!B" + (char)0);
				_serialPort.Write("!C" + (char)0);
			}
		}

		public override void Stop()
		{
			if (serialPortIsValid && _serialPort.IsOpen)
			{
				_serialPort.Write("A" + (char)0);
				_serialPort.Write("B" + (char)0);
				_serialPort.Write("C" + (char)0);
				_serialPort.Close();
			}
			base.Stop();
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (ElexolUSBData)value;
				initModule();
			}
		}

		private void initModule()
		{
			dropExistingSerialPort();
			createSerialPortFromData();

			_minIntensity = _data.MinimumIntensity;

		}

		private void dropExistingSerialPort()
		{
			if (_serialPort != null)
			{
				_serialPort.Dispose();
				_serialPort = null;
			}
		}

		private void createSerialPortFromData()
		{
			if (_data.IsValid)
			{
				_serialPort = new SerialPort(
					_data.PortName,
					_data.BaudRate,
					_data.Parity,
					_data.DataBits,
					_data.StopBits);

				_serialPort.Handshake = Handshake.None;
				_serialPort.Encoding = Encoding.UTF8;

				if (IsRunning)
				{
					_OpenComPort();
				}
			}
			else
			{
				_serialPort = null;
			}
		}

		private bool serialPortIsValid
		{
			get { return _serialPort != null; }
		}

		//use this to always open the port so it is in one place and we can 
		//check if we have an access violation
		private void _OpenComPort()
		{
			try
			{
				_serialPort.Open();

				//if successfull 
				//stop the retry counter and log that we got this going
				//start the controller back up

				if (_serialPort.IsOpen && _retryTimer.Enabled)
				{
					_retryCounter = 0;
					_retryTimer.Stop();

					Logging.Info(
						string.Format("Serial Port conflict has been corrected, starting controller {0} on port {1}.", _data.ModuleTypeId,
									  _serialPort.PortName));
				}
			}
			catch (Exception ex)
			{
				if (ex is UnauthorizedAccessException ||
					ex is InvalidOperationException ||
					ex is IOException)
				{
					Logging.Error(String.Format("{0} is in use.  Starting controller retry timer for {1}",
																	  _serialPort.PortName, _data.ModuleTypeId));
					Stop();
					//lets set our retry timer
					if (_retryCounter < 3)
					{
						_retryCounter++;
						_retryTimer.Start();
						Logging.Info("Starting retry counter for com port access. Retry count is " + _retryCounter);
					}
					else
					{
						Logging.Info(
							"Retry counter for com port access has exceeded max tries.  Controller has been stopped.");
						_retryTimer.Stop();
						_retryCounter = 0;
					}
				}
			}
		}

		public void _retryTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			Logging.Info("Attempting to start controller.");
			Start();
		}

	}

}
