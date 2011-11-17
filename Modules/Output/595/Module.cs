using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Output.Olsen595
{
	public class Module : OutputModuleInstanceBase {
		[DllImport("inpout32", EntryPoint = "Inp32")]
		private static extern short In(ushort port);

		[DllImport("inpout32", EntryPoint = "Out32")]
		private static extern void Out(ushort port, short data);

		private Data _moduleData;
		private int _outputCount;

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

		protected override void _SetOutputCount(int outputCount) {
			_outputCount = outputCount;
		}

		protected override void _UpdateState(Command[] outputStates)
		{
			if(_moduleData.Port != 0) {
				// The first bit clocked will end up on the last channel, so the channels 
				// need to be traversed backwards, from high to low.
				outputStates = outputStates.Reverse().ToArray();
				ushort controlPort = (ushort)(_moduleData.Port + 2);

				foreach (Command command in outputStates) {
					if(command is Lighting.Monochrome.SetLevel) {
						Level level = (command as Lighting.Monochrome.SetLevel).Level;
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
