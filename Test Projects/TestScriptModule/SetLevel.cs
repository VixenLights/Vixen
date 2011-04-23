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
	public class SetLevel : IEffectModuleInstance {
		private CommandIdentifier _setLevelCommandId = Standard.Lighting.Monochrome.SetLevel.Id;

		public CommandData[][] Generate(int channelCount, int intervalCount, params object[] parameterValues) {
			Level level = (Level)parameterValues[0];
			CommandData[][] commands = new CommandData[channelCount][];
			CommandData[] channelCommands;

			for(int i = 0; i < channelCount; i++) {
				commands[i] = channelCommands = new CommandData[intervalCount];
				for(int j = 0; j < intervalCount; j++) {
					channelCommands[j] = new CommandData(_setLevelCommandId, level);
				}
			}

			return commands;
		}

		public string EffectName {
			get { return SetLevelModule._commandName; }
		}

		public CommandParameterSpecification[] Parameters {
			get { return SetLevelModule._parameters; }
		}

		public Guid TypeId {
			get { return SetLevelModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() {
		}

		public override string ToString() {
			return EffectName;
		}
	}
}
