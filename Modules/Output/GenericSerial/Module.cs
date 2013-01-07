using System.IO.Ports;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Sys;
using System.Text;

namespace VixenModules.Output.GenericSerial
{
    public class Module : ControllerModuleInstanceBase {

        private Data _Data;
        private SerialPort _SerialPort = null;
        private CommandHandler _commandHandler;
        private byte[] _packet;
        private byte[] _header;
        private byte[] _footer;

        public Module()
        {
            _commandHandler = new CommandHandler();
            DataPolicyFactory = new DataPolicyFactory();
        }

        public override void UpdateState(int chainIndex, ICommand[] outputStates)
        {
            //if (serialPortIsValid && _SerialPort.IsOpen)
            {
                var headerLen = _header.Length;
                var footerLen = _footer.Length;

                _packet = new byte[headerLen + OutputCount + footerLen];
                var packetLen = _packet.Length; 
                
                _header.CopyTo(_packet, 0);
                _footer.CopyTo(_packet, packetLen - footerLen);

                for (int i = 0; i < outputStates.Length && IsRunning; i++)
                {
                    _commandHandler.Reset();
                    ICommand command = outputStates[i];
                    if (!command.Equals(null))
                    {
                        command.Dispatch(_commandHandler);
                    }
                    _packet[i + headerLen] = _commandHandler.Value;
                }

                _SerialPort.Write(_packet, 0, packetLen);
                VixenSystem.Logging.Info(_packet.ToString());
            }

        }

        public override bool HasSetup
        {
            get { return true; }
        }

        public override bool Setup()
        {
            SetupDialog setup = new SetupDialog(_Data);
            setup.ShowDialog();
            setup.Close();

            return _Data.IsValid;
        }

        public override IModuleDataModel ModuleData
        {
            get { return _Data; }
            set
            {
                _Data = (Data)value;
                initModule();
            }
        }

        public override void Start()
        {
            base.Start();
            if (serialPortIsValid && !_SerialPort.IsOpen)
            {
                _SerialPort.Open();
            }
        }

        public override void Stop()
        {
            if (serialPortIsValid && _SerialPort.IsOpen)
            {
                _SerialPort.Close();
            }
            base.Stop();
        }

        private void initModule()
        {
            dropExistingSerialPort();
            createSerialPortFromData();
            _header = Encoding.ASCII.GetBytes(_Data.Header);
            _footer = Encoding.ASCII.GetBytes(_Data.Footer);

            if (serialPortIsValid && IsRunning)
            {
                _SerialPort.Open();
            }
        }

        private void dropExistingSerialPort()
        {
            if (serialPortIsValid)
            {
                _SerialPort.Dispose();
                _SerialPort = null;
            }
        }
    
        private void createSerialPortFromData()
        {
            if (_Data.IsValid)
            {
                _SerialPort = new SerialPort(
                    _Data.PortName,
                    _Data.BaudRate,
                    _Data.Parity,
                    _Data.DataBits,
                    _Data.StopBits);

                _SerialPort.Handshake = Handshake.None;
                _SerialPort.Encoding = Encoding.UTF8;
            }
        }

        private bool serialPortIsValid
        {
            get { return !_SerialPort.Equals(null); }
        }
    }
}
