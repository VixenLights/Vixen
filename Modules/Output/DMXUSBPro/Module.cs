namespace VixenModules.Output.DmxUsbPro
{
    using System.IO.Ports;
    using System.Text;
    using System.Windows.Forms;
    using CommonElements;
    using Vixen.Commands;
    using Vixen.Module.Output;

    public class Module : OutputModuleInstanceBase
    {
        private DmxUsbProSender _dmxUsbProSender;
        private SerialPort _serialPort;

        public override bool HasSetup
        {
            get
            {
                return true;
            }
        }

        public override void Dispose()
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

            base.Dispose();
        }

        public override bool Setup()
        {
            using (var portConfig = new SerialPortConfig(_serialPort))
            {
                if (portConfig.ShowDialog()
                    == DialogResult.OK)
                {
                    _serialPort = portConfig.SelectedPort;
                    if (_serialPort != null)
                    {
                        _serialPort.Handshake = Handshake.None;
                        _serialPort.Encoding = Encoding.UTF8;

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
            if (_dmxUsbProSender != null)
            {
                _dmxUsbProSender.Dispose();
            }

            InitializePort();
            _dmxUsbProSender = new DmxUsbProSender(_serialPort);
            _dmxUsbProSender.Start();
            base.Start();
        }

        public override void Stop()
        {
            _dmxUsbProSender.Stop();
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }

            base.Stop();
        }

        protected override void _SetOutputCount(int outputCount)
        {
        }

        protected override void _UpdateState(Command[] outputStates)
        {
            _dmxUsbProSender.SendDmxPacket(outputStates);
        }

        private Data GetModuleData()
        {
            return (Data)ModuleData;
        }

        private void InitializePort()
        {
            // Recreate serial port based on setup data
            if (_serialPort != null
                && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }

            var data = GetModuleData();
            if (data.PortName != null)
            {
                _serialPort = new SerialPort(data.PortName, data.BaudRate, data.Partity, data.DataBits, data.StopBits)
                              {
                                 Handshake = Handshake.None, Encoding = Encoding.UTF8 
                              };
            }
        }
    }
}
