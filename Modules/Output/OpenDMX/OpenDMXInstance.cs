using System;
using System.Windows.Forms;
using Vixen.Module.Controller;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
    public class VixenOpenDMXInstance : ControllerModuleInstanceBase
    {
        private byte[] _data;
        private FTDI _dmxPort = new FTDI();
        private int _outputCount;

        public VixenOpenDMXInstance() {
            DataPolicyFactory = new DataPolicyFactory();
        }

        public override int OutputCount
        {
            get {
                return _outputCount;}

            set
            {
                if (value > 512)
                {
                    throw new Exception("Output count greater than 512");
                }
                else
                {
                    _outputCount = value;
                    this._data = new byte[value + 1];
                    this._data[0] = 0;
                }
            }
        }

        public override void UpdateState(int chainInex, ICommand[] outputStates)
        {

            for (int i = 0; i < _outputCount; i++)
                {
                    if ((outputStates[i] as _8BitCommand) == null)
                    {
                        this._data[i + 1] = 0; // account for the null case representing 0
                    }else{
                        this._data[i+1] = (byte)((byte.MaxValue/100.0)*((outputStates[i] as _8BitCommand).CommandValue)); //convert to byte
                    }
                }


             _dmxPort.writeData(_data);


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
            MessageBox.Show("Nothing to Setup");
            return base.Setup();
        }

        public override void Start()
        {
            base.Start();
            //Open up FTDI interface
            _dmxPort.start();
        }

        public override void Stop()
        {
            base.Stop();
            //Close FTDI interface
           _dmxPort.stop();
        }
    }
}
