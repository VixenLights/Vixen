using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.Property;
using CommandStandard.Types;

namespace TestScriptModule {
	static internal class NestedBehavior {
		static public ChannelData Render(ChannelNode[] nodes, long timeSpan, object[] parameterValues) {
			ChannelData channelData = new ChannelData();
			Level startLevel = (Level)parameterValues[0];
			SetLevel setLevelEffect = _GetSetLevelEffect();

			foreach(ChannelNode node in nodes) {
				// We have dependencies on the effect and property, so they will exist.
				// But the property may not exist on the node.
				TestProperty.RGB rgbProperty = _GetRgbProperty(node);
				if(rgbProperty == null) continue;

				// Split the time over the three channels in RGB order.
				OutputChannel[] allChannels = node.GetChannelEnumerator().ToArray();
				OutputChannel redChannel = allChannels.First(x => x.Id == rgbProperty.RedChannelId);
				OutputChannel greenChannel = allChannels.First(x => x.Id == rgbProperty.GreenChannelId);
				OutputChannel blueChannel = allChannels.First(x => x.Id == rgbProperty.BlueChannelId);
				OutputChannel[] orderedChannels = {
												  redChannel,
												  greenChannel,
												  blueChannel
											  };
				for(int i = 0; i < orderedChannels.Length; i++) {
					// Call the SetLevel effect 
					long startTime = timeSpan / orderedChannels.Length * i;
					long channelTimeSpan = timeSpan / orderedChannels.Length;
					Level level = (i + 1) * (startLevel / orderedChannels.Length);
					ChannelData setLevelData = setLevelEffect.Render(new[] { node }, channelTimeSpan, new object[] { level });
					// The data timing coming back from the effect is relative to that effect,
					// so it needs to be offset to be relative to this effect.
					Guid channelId = orderedChannels[i].Id;
					CommandData[] data = setLevelData[channelId];
					foreach(CommandData commandData in data) {
						commandData.StartTime += startTime;
						commandData.EndTime += startTime;
					}
					channelData[channelId] = setLevelData[channelId];
				}
			}

			return channelData;
		}

		static private TestProperty.RGB _GetRgbProperty(ChannelNode node) {
			return node.Properties.Get(SetLevelModule._rgbProperty) as TestProperty.RGB;
		}

		static private SetLevel _GetSetLevelEffect() {
			return ApplicationServices.Get<IEffectModuleInstance>(NestedModule._setLevelEffect) as SetLevel;
		}
	}
}
