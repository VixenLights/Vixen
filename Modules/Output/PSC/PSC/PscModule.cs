using System.IO.Ports;
using System.Windows.Forms;
using CommonElements;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;

namespace PSC {
	public class PscModule : ControllerModuleInstanceBase {
		private SerialPort _port;
		private PscData _data;
		private PSC _psc;

		public PscModule() {
			_psc = new PSC();
		}

		protected override void _SetOutputCount(int outputCount) {
		}

		protected override void _UpdateState(Command[] outputStates) {
			byte index = 0;
			foreach(Command command in outputStates) {
				Animatronics.BasicPositioning.SetPosition setPositionCommand = command as Animatronics.BasicPositioning.SetPosition;
				
				if(setPositionCommand != null) {
					_psc.SetPosition(index, setPositionCommand.Position);
				}
				
				index++;
			}
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(SerialPortConfig serialPortConfig = new SerialPortConfig(_port, allowBaudEdit: false)) {
				if(serialPortConfig.ShowDialog() == DialogResult.OK) {
					SerialPort port = serialPortConfig.SelectedPort;
					if(port != null) {
						_data.PortName = port.PortName;
						_data.BaudRate = port.BaudRate;
						_data.DataBits = port.DataBits;
						_data.Parity = port.Parity;
						_data.StopBits = port.StopBits;
						_UpdateFromData();
						return true;
					}
				}
			}
			return false;
		}

		public override void Start() {
			if(!IsRunning) {
				base.Start();
				if(_port != null && !_port.IsOpen) {
					_port.Open();
				}

				int pscBaudRate = _psc.BaudRate;
				//if(pscBaudRate == 0) {
				//    throw new Exception("Could not determine the baud rate for the PSC unit.");
				//}

				if(pscBaudRate != 38400) {
					_psc.BaudRate = 38400;
				}
			}
		}

		public override void Stop() {
			if(IsRunning) {
				if(_port != null && _port.IsOpen) {
					_port.Close();
				}
				base.Stop();
			}
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { 
				_data = value as PscData;
				_UpdateFromData();
			}
		}

		private void _UpdateFromData() {
			_CreatePort();
			_psc.SerialPort = _port;
		}

		private void _CreatePort() {
			if(_port != null) {
				_port.Dispose();
				_port = null;
			}

			if(_data.IsValid) {
				_port = new SerialPort(_data.PortName, _data.BaudRate, _data.Parity, _data.DataBits, _data.StopBits);
				//_port.WriteTimeout = _data.WriteTimeout;
				//_port.Handshake = Handshake.None;
				//_port.Encoding = Encoding.UTF8;
				//_port.RtsEnable = true;
				//_port.DtrEnable = true;

				if(IsRunning) _port.Open();
			} else {
				_port = null;
			}
		}
	}
}
