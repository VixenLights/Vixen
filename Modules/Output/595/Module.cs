using System.Linq;
using System.Runtime.InteropServices;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;
using System;

namespace VixenModules.Output.Olsen595
{
	internal class LoadInpOutDLL
	{
		[DllImport("Common\\inpout32.dll", EntryPoint = "Out32")]
		private static extern short Out32(ushort port, short data);

		[DllImport("Common\\inpoutx64.dll", EntryPoint = "Out32")]
		private static extern short Out64(ushort port, short data);

		public static short outputData(ushort port, short data)
		{
#if WIN64
			return Out64(port, data);
#else
			return Out32(port, data);
#endif
			//if (Environment.Is64BitOperatingSystem) {
			//	return Out64(port, data);
			//}
			//else {
			//	return Out32(port, data);
			//}
		}
	}

	public class Module : ControllerModuleInstanceBase
	{
		private Data _moduleData;
		private CommandHandler _commandHandler;

		public Module()
		{
			_commandHandler = new CommandHandler();
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override bool Setup()
		{
			using (SetupDialog setupDialog = new SetupDialog(_moduleData)) {
				setupDialog.ShowDialog();
			}
			return true;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _moduleData; }
			set { _moduleData = value as Data; }
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (_moduleData.Port != 0) {
				// The first bit clocked will end up on the last channel, so the channels 
				// need to be traversed backwards, from high to low.
				outputStates = outputStates.Reverse().ToArray();
				ushort controlPort = (ushort) (_moduleData.Port + 2);

				foreach (ICommand command in outputStates) {
					_commandHandler.Reset();
					if (command != null) {
						command.Dispatch(_commandHandler);
					}

					LoadInpOutDLL.outputData(_moduleData.Port, _commandHandler.Value);
					LoadInpOutDLL.outputData(controlPort, 2);
					LoadInpOutDLL.outputData(controlPort, 3);
				}

				LoadInpOutDLL.outputData(controlPort, 1);
				LoadInpOutDLL.outputData(controlPort, 3);
			}
		}
	}
}