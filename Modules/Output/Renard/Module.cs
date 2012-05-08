using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using Vixen.Sys;

namespace VixenModules.Output.Renard
{
	public class Module : ControllerModuleInstanceBase {
		private Data _moduleData;
		private SerialPort _port;
		private IDataPolicy _dataPolicy;
		private CommandHandler _commandHandler;
		private IRenardProtocolFormatter _protocolFormatter;

		private const int DEFAULT_WRITE_TIMEOUT = 500;

		public Module() {
			_dataPolicy = new RenardDataPolicy();
			_commandHandler = new CommandHandler();
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(CommonElements.SerialPortConfig serialPortConfig = new CommonElements.SerialPortConfig(_port)) {
				if(serialPortConfig.ShowDialog() == DialogResult.OK) {
					SerialPort port = serialPortConfig.SelectedPort;
					if(port != null) {
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

		public override IModuleDataModel ModuleData {
			get { return _moduleData; }
			set {
				_moduleData = (Data)value;
				if(_moduleData.WriteTimeout == 0) _moduleData.WriteTimeout = DEFAULT_WRITE_TIMEOUT;
				if(_moduleData.ProtocolVersion == 0) _moduleData.ProtocolVersion = 1;
				_UpdateFromData();
			}
		}

		public override void Start() {
			base.Start();
			if(_port != null && !_port.IsOpen) {
				_port.Open();
			}
		}

		public override void Stop() {
			if(_port != null && _port.IsOpen) {
				_port.Close();
			}
			base.Stop();
		}

		protected override void _SetOutputCount(int outputCount) { 
		}

		public override void UpdateState(ICommand[] outputStates) {
			if(_port != null && _port.IsOpen) {
				_protocolFormatter.StartPacket(OutputCount, ChainIndex);

				for(int i = 0; i < outputStates.Length && IsRunning; i++) {
					_commandHandler.Reset();
					ICommand command = outputStates[i];
					if(command != null) {
						command.Dispatch(_commandHandler);
					}
					_protocolFormatter.Add(_commandHandler.Value);
				}
				_WaitForBufferRoom(_PacketSize);
				byte[] packet = _GetPacket();
				_port.Write(packet, 0, _PacketSize);
			}
		}

		private void _WaitForBufferRoom(int bytesToWrite) {
			while(_port.WriteBufferSize - _port.BytesToWrite <= bytesToWrite) {
				System.Threading.Thread.Sleep(10);
			}
		}

		private void _UpdateFromData() {
			_CreatePort();
			_SetProtocolFormatter();
		}

		private void _CreatePort() {
			if(_port != null) {
				_port.Dispose();
				_port = null;
			}

			if(_moduleData.IsValid) {
				_port = new SerialPort(_moduleData.PortName, _moduleData.BaudRate, _moduleData.Parity, _moduleData.DataBits, _moduleData.StopBits);
				_port.WriteTimeout = _moduleData.WriteTimeout;
				_port.Handshake = Handshake.None;
				_port.Encoding = Encoding.UTF8;
				_port.RtsEnable = true;
				_port.DtrEnable = true;

				if(IsRunning) _port.Open();
			} else {
				_port = null;
			}
		}

		private void _SetProtocolFormatter() {
			_protocolFormatter = ProtocolFormatterService.Instance.FindFormatter(_moduleData.ProtocolVersion);
		}

		private byte[] _GetPacket() {
			return _protocolFormatter.FinishPacket();
		}

		private int _PacketSize {
			get { return _protocolFormatter.PacketSize; }
		}

		public override IDataPolicy DataPolicy {
			get { return _dataPolicy; }
		}
	}
}
