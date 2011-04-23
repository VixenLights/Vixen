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
	public class Spin : IEffectModuleInstance {
		private CommandIdentifier _setLevelCommandId;

		public Spin() {
			// Get the id of the primitive commands we're going to use
			_setLevelCommandId = Standard.Lighting.Monochrome.SetLevel.Id;
		}

		public CommandData[][] Generate(int channelCount, int intervalCount, params object[] parameterValues) {
			CommandData[][] commands = new CommandData[channelCount][];
			for(int i = 0; i < channelCount; i++) {
				commands[i] = new CommandData[intervalCount];
			}

			// If < 4 channels, do nothing.
			if(channelCount < 4) return commands;

			// Just for demonstration purposes, we're not taking any parameters and are
			// going to assume to split the entity into quadrants.
			//double channelsPerQuad = (double)channelCount / 4;
			int channelsPerQuad = channelCount / 4;
			// Constant rate of spin that is dependent on the interval length.
			// Every interval, we're going to advance by one channel.

			// The delta between adjacent channels will also be constant.
			// They will fade from 100% to 0%.
			// This is for demo purposes only, so it is featureless.
			double levelDelta = 100d / channelCount;
			levelDelta *= 4;
			int channelsToUse = 1; // Keep below channelsPerQuad

			for(int intervalIndex = 0; intervalIndex < intervalCount; intervalIndex++) {
				Level level = 100;
				int offset = intervalIndex;
				for(int channelIndex = 0; channelIndex < channelsPerQuad && channelIndex < channelsToUse; channelIndex++) {
					for(int quadrant = 0; quadrant < 4; quadrant++) {
						int startChannel = (int)(quadrant * channelsPerQuad);
						commands[(offset + channelsPerQuad) % channelsPerQuad + startChannel][intervalIndex] = new CommandData(_setLevelCommandId, level);
					}
					level -= levelDelta;
					offset--;
				}
				if(++channelsToUse > channelsPerQuad) {
					channelsToUse = (int)channelsPerQuad;
				}
			}

			return commands;
		}

		public string EffectName {
			get { return SpinModule._effectName; }
		}

		public CommandParameterSpecification[] Parameters {
			get { return SpinModule._parameters; }
		}

		public Guid TypeId {
			get { return SpinModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}
