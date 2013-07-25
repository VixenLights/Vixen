using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System;
using System.Timers;
using System.IO;

namespace VixenModules.Output.Renard
{
	public class Module : ControllerModuleInstanceBase
	{
		private Data _moduleData;
		private SerialPort _port;
		private CommandHandler _commandHandler;
		private IRenardProtocolFormatter _protocolFormatter;
		private System.Timers.Timer _retryTimer;
		private int _retryCounter;

		private const int DEFAULT_WRITE_TIMEOUT = 500;

		public Module()
		{
			_commandHandler = new CommandHandler();
			DataPolicyFactory = new RenardDataPolicyFactory();

			//set 2 minute timer before retrying to access com port
			_retryTimer = new System.Timers.Timer(120000);
			_retryTimer.Elapsed += new ElapsedEventHandler(_retryTimer_Elapsed);
			_retryCounter = 0;
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (Common.Controls.SerialPortConfig serialPortConfig = new Common.Controls.SerialPortConfig(_port)) {
				if (serialPortConfig.ShowDialog() == DialogResult.OK) {
					SerialPort port = serialPortConfig.SelectedPort;
					if (port != null) {
						_moduleData.PortName = port.PortName;
						_moduleData.BaudRate = port.BaudRate;
						_moduleData.DataBits = port.DataBits;
						_moduleData.Parity = port.Parity;
						_moduleData.StopBits = port.StopBits;
						_UpdateFromData();
						return true;
					}
				}
			}
			return false;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _moduleData; }
			set
			{
				_moduleData = (Data) value;
				if (_moduleData.WriteTimeout == 0) _moduleData.WriteTimeout = DEFAULT_WRITE_TIMEOUT;
				if (_moduleData.ProtocolVersion == 0) _moduleData.ProtocolVersion = 1;
				_UpdateFromData();
			}
		}

		public override void Start()
		{
			base.Start();
			if (_port != null && !_port.IsOpen) {
				_OpenComPort();
			}
		}

		public override void Stop()
		{
			if (_port != null && _port.IsOpen) {
				_port.Close();
			}
			base.Stop();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (_port != null && _port.IsOpen) {
				_protocolFormatter.StartPacket(OutputCount, chainIndex);

				for (int i = 0; i < outputStates.Length && IsRunning; i++) {
					_commandHandler.Reset();
					ICommand command = outputStates[i];
					if (command != null) {
						command.Dispatch(_commandHandler);
					}
					_protocolFormatter.Add(_commandHandler.Value);
				}
				_WaitForBufferRoom(_PacketSize);
				byte[] packet = _GetPacket();
				_port.Write(packet, 0, _PacketSize);
			}
		}

		private void _WaitForBufferRoom(int bytesToWrite)
		{
			while (_port.WriteBufferSize - _port.BytesToWrite <= bytesToWrite) {
				System.Threading.Thread.Sleep(10);
			}
		}

		private void _UpdateFromData()
		{
			_CreatePort();
			_SetProtocolFormatter();
		}

		private void _CreatePort()
		{
			if (_port != null) {
				_port.Dispose();
				_port = null;
			}

			if (_moduleData.IsValid) {
				_port = new SerialPort(_moduleData.PortName, _moduleData.BaudRate, _moduleData.Parity, _moduleData.DataBits,
				                       _moduleData.StopBits);
				_port.WriteTimeout = _moduleData.WriteTimeout;
				_port.Handshake = Handshake.None;
				_port.Encoding = Encoding.UTF8;
				_port.RtsEnable = true;
				_port.DtrEnable = true;

				if (IsRunning) {
					_OpenComPort();
				}
			}
			else {
				_port = null;
			}
		}

		//use this to always open the port so it is in one place and we can 
		//check if we have an access violation
		private void _OpenComPort()
		{
			try {
				_port.Open();

				//if successfull 
				//stop the retry counter and log that we got this going
				//start the controller back up

				if (_port.IsOpen && _retryTimer.Enabled) {
					_retryCounter = 0;
					_retryTimer.Stop();

					Vixen.Sys.VixenSystem.Logging.Info(
						string.Format("Serial Port conflict has been corrected, starting controller {0} on port {1}.",
						              _moduleData.ModuleTypeId, _port.PortName));
				}
			}
			catch (Exception ex) {
				if (ex is UnauthorizedAccessException ||
				    ex is InvalidOperationException ||
				    ex is IOException) {
					Vixen.Sys.VixenSystem.Logging.Error(string.Format("{0} is in use.  Starting controller retry timer for {1}",
					                                                  _port.PortName, _moduleData.ModuleTypeId));
					Stop();
					//lets set our retry timer
					if (_retryCounter < 3) {
						_retryCounter++;
						_retryTimer.Start();
						Vixen.Sys.VixenSystem.Logging.Info("Starting retry counter for com port access. Retry count is " + _retryCounter);
					}
					else {
						Vixen.Sys.VixenSystem.Logging.Info(
							"Retry counter for com port access has exceeded max tries.  Controller has been stopped.");
						_retryTimer.Stop();
						_retryCounter = 0;
					}
				}
			}
		}

		private void _SetProtocolFormatter()
		{
			_protocolFormatter = ProtocolFormatterService.Instance.FindFormatter(_moduleData.ProtocolVersion);
		}

		private byte[] _GetPacket()
		{
			return _protocolFormatter.FinishPacket();
		}

		private int _PacketSize
		{
			get { return _protocolFormatter.PacketSize; }
		}

		public void _retryTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			Vixen.Sys.VixenSystem.Logging.Info("Attempting to start controller.");
			Start();
		}
	}
}