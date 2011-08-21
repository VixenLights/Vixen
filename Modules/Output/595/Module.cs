using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Common;
using CommandStandard;
using CommandStandard.Types;

namespace _595 {
	public class Module : OutputModuleInstanceBase {
		[DllImport("inpout32", EntryPoint = "Inp32")]
		private static extern short In(ushort port);

		[DllImport("inpout32", EntryPoint = "Out32")]
		private static extern void Out(ushort port, short data);

		private Data _moduleData;
		private int _outputCount;
		private byte _ourCategory;
		private byte _ourPlatform;
		private byte _ourCommand;

		public Module() {
			_ourPlatform = CommandStandard.Standard.Lighting.Value;
			_ourCategory = CommandStandard.Standard.Lighting.Monochrome.Value;
			_ourCommand = CommandStandard.Standard.Lighting.Monochrome.SetLevel.Value;
		}

		public override bool Setup() {
			using(SetupDialog setupDialog = new SetupDialog(_moduleData)) {
				setupDialog.ShowDialog();
			}
			return true;
		}

		protected override void _SetOutputCount(int outputCount) {
			_outputCount = outputCount;
		}

		public override IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as Data; }
		}
		protected override void _UpdateState(CommandData[] outputStates) {
			if(_moduleData.Port != 0) {
				// The first bit clocked will end up on the last channel, so the channels 
				// need to be traversed backwards, from high to low.
				outputStates = outputStates.Reverse().ToArray();
				ushort controlPort = (ushort)(_moduleData.Port + 2);

				foreach(CommandData commandData in outputStates) {
					if(commandData.CommandIdentifier.Platform == _ourPlatform &&
						commandData.CommandIdentifier.Category == _ourCategory &&
						commandData.CommandIdentifier.CommandIndex == _ourCommand) {
						Level level = (Level)commandData.ParameterValues[0];
						Out(_moduleData.Port, level > 0 ? (short)1 : (short)0);
						Out(controlPort, 2);
						Out(controlPort, 3);
					}
				}

				Out(controlPort, 1);
				Out(controlPort, 3);
			}
		}
	}
}
