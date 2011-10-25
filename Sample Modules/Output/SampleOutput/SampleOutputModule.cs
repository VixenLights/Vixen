using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;

namespace SampleOutput {
	public class SampleOutputModule : OutputModuleInstanceBase {
		private SampleOutputData _data;

		protected override void _SetOutputCount(int outputCount) {
			throw new NotImplementedException();
		}

		protected override void _UpdateState(Command[] outputStates) {
			for(int i = 0; i < outputStates.Length; i++) {
				Command command = outputStates[i];
				if(command is Lighting.Monochrome.SetLevel) {
					Lighting.Monochrome.SetLevel setLevelCommand = command as Lighting.Monochrome.SetLevel;
					//***
					setLevelCommand.Level
				}
			}
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as SampleOutputData; }
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			bool result;

			using(SampleOutputSetupForm sampleOutputSetupForm = new SampleOutputSetupForm(_data)) {
				result = sampleOutputSetupForm.ShowDialog() == DialogResult.OK;
			}

			return result;
		}
	}
}
