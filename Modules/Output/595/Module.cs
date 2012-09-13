using System.Linq;
using System.Runtime.InteropServices;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;

namespace VixenModules.Output.Olsen595
{
	public class Module : ControllerModuleInstanceBase {
		[DllImport("inpout32", EntryPoint = "Out32")]
		private static extern void Out(ushort port, short data);

		private Data _moduleData;
		private CommandHandler _commandHandler;

		public Module() {
			_commandHandler = new CommandHandler();
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override bool Setup() {
			using(SetupDialog setupDialog = new SetupDialog(_moduleData)) {
				setupDialog.ShowDialog();
			}
			return true;
		}

		public override IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as Data; }
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates) {
			if(_moduleData.Port != 0) {
				// The first bit clocked will end up on the last channel, so the channels 
				// need to be traversed backwards, from high to low.
				outputStates = outputStates.Reverse().ToArray();
				ushort controlPort = (ushort)(_moduleData.Port + 2);

				foreach (ICommand command in outputStates) {
					_commandHandler.Reset();
					if(command != null) {
						command.Dispatch(_commandHandler);
					}

					Out(_moduleData.Port, _commandHandler.Value);
					Out(controlPort, 2);
					Out(controlPort, 3);
				}

				Out(controlPort, 1);
				Out(controlPort, 3);
			}
		}
	}
}
