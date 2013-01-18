

using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;

namespace VixenModules.Output.GenericSerial
{
    public class Module : ControllerModuleInstanceBase {

        private GenericSerial gs = new GenericSerial();
        private Data _ModuleData;
        private CommandHandler _commandHandler;

        public Module()
        {
            _commandHandler = new CommandHandler();
            DataPolicyFactory = new DataPolicyFactory();
        }

        public override void UpdateState(int chainIndex, ICommand[] outputStates)
        {
            if (IsRunning)
            {
                gs.SendUpdate(outputStates, _ModuleData, _commandHandler);
            }
        }

        public override bool HasSetup
        {
            get { return true; }
        }

        public override bool Setup()
        {
            SetupDialog setup = new SetupDialog(_ModuleData);
            setup.ShowDialog();
            setup.Close();

            return _ModuleData.IsValid;
        }

        public override IModuleDataModel ModuleData
        {
            get { return _ModuleData; }
            set
            {
                _ModuleData = (Data)value;

                gs.initModule(_ModuleData);
                
                if (gs.serialPortIsValid && IsRunning)
                {
                    gs.openSerialPort();
                }
            }
        }

        public override void Start()
        {
            base.Start();
            gs.openSerialPort();
        }

        public override void Stop()
        {
            gs.closeSerialPort();
            base.Stop();
        }





    }
}
