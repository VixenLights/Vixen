using System.IO.Ports;
using System.Text;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System.Timers;
using System;
using System.Windows.Forms;
using System.IO;

namespace VixenModules.Output.GenericSerial
{
	public class Module : ControllerModuleInstanceBase
	{
		private Data _Data;
		private SerialPort _SerialPort = null;
		private CommandHandler _commandHandler;
		private byte[] _packet;
		private byte[] _header;
		private byte[] _footer;
		private int headerLen = 0;
		private int footerLen = 0;
		private System.Timers.Timer _retryTimer;
		private int _retryCounter;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public Module()
		{
			_commandHandler = new CommandHandler();
			DataPolicyFactory = new DataPolicyFactory();

			//set 2 minute timer before retrying to access com port
			_retryTimer = new System.Timers.Timer(120000);
			_retryTimer.Elapsed += new ElapsedEventHandler(_retryTimer_Elapsed);
			_retryCounter = 0;
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (serialPortIsValid && _SerialPort.IsOpen) {
				_packet = new byte[headerLen + OutputCount + footerLen];
				var packetLen = _packet.Length;

				_header.CopyTo(_packet, 0);
				_footer.CopyTo(_packet, packetLen - footerLen);

				for (int i = 0; i < outputStates.Length && IsRunning; i++) {
					_commandHandler.Reset();
					ICommand command = outputStates[i];
					if (command != null) {
						command.Dispatch(_commandHandler);
					}
					_packet[i + headerLen] = _commandHandler.Value;
				}

				if (outputStates.Length > headerLen + footerLen) {
					_SerialPort.Write(_packet, 0, packetLen);
				}
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (SetupDialog setup = new SetupDialog(_Data)) {
				if (setup.ShowDialog() == DialogResult.OK) {
					SerialPort port = setup.Port;
					if (port != null) {
						initModule();
						return _Data.IsValid;
					}
				}
			}

			return false;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _Data; }
			set
			{
				_Data = (Data) value;
				initModule();
			}
		}

		public override void Start()
		{
			base.Start();
			if (serialPortIsValid && !_SerialPort.IsOpen) {
				_OpenComPort();
			}
		}

		public override void Stop()
		{
			if (serialPortIsValid && _SerialPort.IsOpen) {
				_SerialPort.Close();
			}
			base.Stop();
		}

		private void initModule()
		{
			dropExistingSerialPort();
			createSerialPortFromData();
			_header = Encoding.ASCII.GetBytes(_Data.Header == null ? string.Empty : _Data.Header);
			headerLen = _header.Length;
			_footer = Encoding.ASCII.GetBytes(_Data.Footer == null ? string.Empty : _Data.Footer);
			footerLen = _footer.Length;
		}

		private void dropExistingSerialPort()
		{
			if (_SerialPort != null) {
				_SerialPort.Dispose();
				_SerialPort = null;
			}
		}

		private void createSerialPortFromData()
		{
			if (_Data.IsValid) {
				_SerialPort = new SerialPort(
					_Data.PortName,
					_Data.BaudRate,
					_Data.Parity,
					_Data.DataBits,
					_Data.StopBits);

				_SerialPort.Handshake = Handshake.None;
				_SerialPort.Encoding = Encoding.UTF8;

				if (IsRunning) {
					_OpenComPort();
				}
			}
			else {
				_SerialPort = null;
			}
		}

		private bool serialPortIsValid
		{
			get { return _SerialPort != null; }
		}

		//use this to always open the port so it is in one place and we can 
		//check if we have an access violation
		private void _OpenComPort()
		{
			try {
				_SerialPort.Open();

				//if successfull 
				//stop the retry counter and log that we got this going
				//start the controller back up

				if (_SerialPort.IsOpen && _retryTimer.Enabled) {
					_retryCounter = 0;
					_retryTimer.Stop();

					Logging.Info("Serial Port conflict has been corrected, starting controller type {0} on port {1}.", _Data.ModuleTypeId, _SerialPort.PortName);
				}
			}
			catch (Exception ex) {
				if (ex is UnauthorizedAccessException ||
				    ex is InvalidOperationException ||
				    ex is IOException)
				{
					Logging.Error("{0} could not be opened because {1}.  Starting controller retry timer for controller type {2}",
						_SerialPort.PortName, ex.Message, _Data.ModuleTypeId);
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
				else
				{
					Logging.Error(String.Format("{0} could not be opened.", _SerialPort.PortName), ex);
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