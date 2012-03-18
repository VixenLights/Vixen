using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.Property;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace TestScriptModule {
	static internal class NestedBehavior {
		static public ChannelData Render(ChannelNode[] nodes, TimeSpan timeSpan, Level level) {
			ChannelData channelData = new ChannelData();
			Level startLevel = level;
			SetLevel setLevelEffect = _GetSetLevelEffect();

			foreach(ChannelNode node in nodes) {
				// We have dependencies on the effect and property, so they will exist.
				// But the property may not exist on the node.
				TestProperty.RGB rgbProperty = _GetRgbProperty(node);
				if(rgbProperty == null) continue;

				// Split the time over the three channels in RGB order.
				Channel[] allChannels = node.GetChannelEnumerator().ToArray();
				Channel redChannel = allChannels.First(x => x.Id == rgbProperty.RedChannelId);
				Channel greenChannel = allChannels.First(x => x.Id == rgbProperty.GreenChannelId);
				Channel blueChannel = allChannels.First(x => x.Id == rgbProperty.BlueChannelId);
				Channel[] orderedChannels = {
												  redChannel,
												  greenChannel,
												  blueChannel
											  };
				for(int i = 0; i < orderedChannels.Length; i++) {
					// Call the SetLevel effect 
					TimeSpan startTime = TimeSpan.FromTicks(timeSpan.Ticks / orderedChannels.Length * i);
					TimeSpan channelTimeSpan = TimeSpan.FromTicks(timeSpan.Ticks / orderedChannels.Length);
					Level setLevelParam = (i + 1) * (startLevel / orderedChannels.Length);
					setLevelEffect.TargetNodes = new[] { node };
					setLevelEffect.TimeSpan = channelTimeSpan;
					setLevelEffect.ParameterValues = new object[] { setLevelParam };
					ChannelData setLevelData = setLevelEffect.Render();
					// The data timing coming back from the effect is relative to that effect,
					// so it needs to be offset to be relative to this effect.
					Guid channelId = orderedChannels[i].Id;
					CommandNode[] data = setLevelData[channelId].Select(x => new CommandNode(x.Command, x.StartTime + startTime, x.TimeSpan)).ToArray();
					channelData[channelId] = data;
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
