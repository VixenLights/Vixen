using Vixen.Sys;

namespace VixenModules.Output.DmxUsbPro
{
	using System.IO.Ports;
	using System.Text;
	using System.Windows.Forms;
	using Common.Controls;
	using Vixen.Commands;
	using Vixen.Module.Controller;

	public class Module : ControllerModuleInstanceBase
	{
		private SerialPort _serialPort;

		private DmxUsbProSender _dmxUsbProSender;

		public Module()
		{
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (var portConfig = new SerialPortConfig(this._serialPort)) {
				if (portConfig.ShowDialog() == DialogResult.OK) {
					this._serialPort = portConfig.SelectedPort;
					if (_serialPort != null) {
						this._serialPort.Handshake = Handshake.None;
						this._serialPort.Encoding = Encoding.UTF8;

						// Write back to setup
						var data = GetModuleData();
						data.PortName = _serialPort.PortName;
						data.BaudRate = _serialPort.BaudRate;
						data.Partity = _serialPort.Parity;
						data.DataBits = _serialPort.DataBits;
						data.StopBits = _serialPort.StopBits;
						return true;
					}
				}

				return false;
			}
		}

		public override void Start()
		{
			if (this._dmxUsbProSender != null) {
				this._dmxUsbProSender.Dispose();
			}

			this.InitializePort();
			this._dmxUsbProSender = new DmxUsbProSender(this._serialPort);
			this._dmxUsbProSender.Start();
			base.Start();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_serialPort != null)
				{
					if (_serialPort.IsOpen)
					{
						_serialPort.Close();
						_serialPort.Dispose();
						_serialPort = null;
					}
				}	
			}
			
			base.Dispose(disposing);
		}

		public override void Stop()
		{
			this._dmxUsbProSender.Stop();
			if (this._serialPort != null) {
				if (this._serialPort.IsOpen) {
					this._serialPort.Close();
					this._serialPort.Dispose();
					this._serialPort = null;
				}
			}

			base.Stop();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			this._dmxUsbProSender.SendDmxPacket(outputStates);
		}

		private Data GetModuleData()
		{
			return (Data) this.ModuleData;
		}

		private void InitializePort()
		{
			// Recreate serial port based on setup data
			if (this._serialPort != null && this._serialPort.IsOpen) {
				this._serialPort.Close();
				this._serialPort.Dispose();
			}

			var data = this.GetModuleData();
			if (data.PortName != null) {
				this._serialPort = new SerialPort(data.PortName, data.BaudRate, data.Partity, data.DataBits, data.StopBits)
				                   	{
				                   		Handshake = Handshake.None,
				                   		Encoding = Encoding.UTF8
				                   	};
			}
		}
	}
}