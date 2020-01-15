using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System.Diagnostics;

namespace VixenModules.Output.Hill320
{
	internal class LoadInpOutDLL
	{
		[DllImport("Common\\inpout32.dll", EntryPoint = "Out32")]
		private static extern short Out32(ushort port, ushort data);

		[DllImport("Common\\inpoutx64.dll", EntryPoint = "Out32")]
		private static extern short Out64(ushort port, ushort data);

		public static short outputData(ushort port, ushort data)
		{
#if WIN64
			//if (Environment.Is64BitOperatingSystem) {
				return Out64(port, data);
#else
			//}
			//else {
				return Out32(port, data);
#endif
			//}
		}
	}

	public class Module : ControllerModuleInstanceBase
	{
		private Data _moduleData;
		private ushort _portAddress;
		private CommandHandler _commandHandler;

		public Module()
		{
			_commandHandler = new CommandHandler();
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override bool Setup()
		{
			using (Common.Controls.ParallelPortConfig parallelPortConfig = new Common.Controls.ParallelPortConfig(_portAddress)) {
				if (parallelPortConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_portAddress = parallelPortConfig.PortAddress;
					_moduleData.PortAddress = _portAddress;
					_moduleData.StatusPort = (ushort) (_portAddress + 1);
					_moduleData.ControlPort = (ushort) (_portAddress + 2);
					return true;
				}
			}
			return false;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _moduleData; }
			set { _moduleData = value as Data; }
		}

		public override int OutputCount { get; set; }

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			int valueIndex = 0;
			int bitCount;
			byte value;
			byte bankBox, bank;

			int loopCount = outputStates.Length >> 3;

			for (int box = 0; box < loopCount; box++) {
				value = 0;
				for (bitCount = 0; bitCount < 8; bitCount++) {
					_commandHandler.Reset();

					ICommand command = outputStates[valueIndex++];

					if (command != null) {
						command.Dispatch(_commandHandler);

						value >>= 1;
						if (_commandHandler.Value > 0)
							value |= (byte) 0x80;
						else
							value |= (byte) 0;
					}
					else {
						value >>= 1;
						value |= (byte) 0;
					}
				}

				bank = (byte) (8 << (box >> 3));
				bankBox = (byte) (box%8);

				LoadInpOutDLL.outputData(_moduleData.PortAddress, value);
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 0);
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 1);
				LoadInpOutDLL.outputData(_moduleData.PortAddress, (ushort) (bank | bankBox));
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 3);
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 1);

				Debug.WriteLine(string.Format("{0} {1} {2}", _moduleData.PortAddress, value, ((ushort) (bank | bankBox))));
			}
		}
	}
}