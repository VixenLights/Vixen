using System.IO.Ports;
using System.Text;
using Vixen.Commands;

namespace VixenModules.Output.GenericSerial
{
    /// <summary>
    /// This class contains implementations specific to Generic Serial output
    /// </summary>
    public class GenericSerial
    {

        private SerialPort _SerialPort = null;


        protected internal void initModule(Data _Data)
        {
            dropExistingSerialPort();
            createSerialPortFromData(_Data);
        }

        private void dropExistingSerialPort()
        {
            if (serialPortIsValid)
            {
                _SerialPort.Dispose();
                _SerialPort = null;
            }
        }

        private void createSerialPortFromData(Data _Data)
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

        protected internal bool serialPortIsValid
        {
            get { return _SerialPort != null; }
        }

        protected internal bool isSerialPortOpen
        {
            get { return _SerialPort.IsOpen; }
        }

        protected internal bool isSerialPortClosed
        {
            get { return !isSerialPortOpen; }
        }

        protected internal void closeSerialPort()
        {
            if (serialPortIsValid && isSerialPortOpen)
            {
                _SerialPort.Close();
            }
        }

        protected internal void openSerialPort()
        {
            if (serialPortIsValid && isSerialPortClosed)
            {
                _SerialPort.Open();
            }
        }

        internal void SendUpdate(ICommand[] outputStates, Data _Data, CommandHandler _commandHandler)
        {
            if (serialPortIsValid && isSerialPortOpen)
            {
                byte[] _header = Encoding.ASCII.GetBytes(_Data.Header == null ? string.Empty : _Data.Header);
                var headerLen = _header.Length; // compute this here so we don't have to compute it in the loop
                
                byte[] _footer = Encoding.ASCII.GetBytes(_Data.Footer == null ? string.Empty : _Data.Footer);
                
                byte[] _packet = new byte[headerLen + outputStates.Length + _footer.Length];

                _header.CopyTo(_packet, 0);
                _footer.CopyTo(_packet, _packet.Length - _footer.Length);

                ICommand command = null;

                // Why do we use the command handler?  For this module, it just seems like unnecessary overhead.
                for (int i = 0; i < outputStates.Length; i++)
                {
                    _commandHandler.Reset();
                    command = outputStates[i];
                    if (command != null)
                    {
                        command.Dispatch(_commandHandler);
                    }
                    _packet[i + headerLen] = _commandHandler.Value;
                }

                if (_packet.Length > 0)
                {
                    _SerialPort.Write(_packet, 0, _packet.Length);
                }
            }


        }
    }
}