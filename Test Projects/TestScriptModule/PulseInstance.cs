using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.Effect;

namespace TestScriptModule {
	public class PulseInstance : IEffectModuleInstance {
		private CommandIdentifier _setLevelCommandId;

		public PulseInstance() {
			// Get the id of the primitive commands we're going to use
			_setLevelCommandId = Standard.Lighting.Monochrome.SetLevel.Id;
		}

		public Guid TypeId {
			get { return PulseModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }

		public CommandData[][] Generate(int channelCount, int intervalCount, params object[] parameterValues) {
			Level startLevel = (Level)parameterValues[0];
			Level onLevel;
			Level offLevel = 0;
			CommandData[][] commands = new CommandData[channelCount][];
			CommandData[] channelCommands;
			// This will round it down so that the level will not go < 0.
			double fadeAmount;
			//fadeAmount = (1.0d / (intervalCount / 2)) * onLevel;
			//if(intervalCount % 2 == 0) {
			//    fadeAmount = (1.0d / (intervalCount / 2)) * onLevel;
			//} else {
			//    // Smaller fade delta so it will not go below 0.
			//    fadeAmount = (1.0d / ((intervalCount + 1) / 2)) * onLevel;
			//}
			fadeAmount = (1.0d / intervalCount) * startLevel;

			for(int i = 0; i < channelCount; i++) {
				commands[i] = channelCommands = new CommandData[intervalCount];
				onLevel = startLevel;
				for(int j = 0; j < intervalCount; j++) {
					//// Alternate on and off.
					//channelCommands[j] = new CommandData(_setLevelCommandId, (j % 2 == 0) ? onLevel : offLevel);
					//// Alternate on and off, fading.
					//channelCommands[j] = new CommandData(_setLevelCommandId, (j % 2 == 0) ? onLevel : offLevel);
					//if(j % 2 == 0) {
					//    onLevel -= fadeAmount;
					//}
					// All on, fading.
					channelCommands[j] = new CommandData(_setLevelCommandId, onLevel);
					onLevel -= fadeAmount;
				}
			}

			return commands;
		}

		public string EffectName {
			get { return PulseModule._effectName; }
		}

		public CommandParameterSpecification[] Parameters {
			get {
				//return new CommandParameterSpecification[] { 
				//    new CommandParameterSpecification("level", typeof(Level))
				//};
				return PulseModule._parameters;
			}
		}

		public override string ToString() {
			return EffectName;
		}
	}
}
