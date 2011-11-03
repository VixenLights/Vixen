using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Sys;

namespace SampleEffect {
	public class SampleEffectModule : EffectModuleInstanceBase {
		private ChannelData _channelData = null;
		private SampleEffectData _data;
		private Guid _samplePropertyId = new Guid("{F7A1D96D-DC90-427f-927C-7A79DEABDCA7}");

		public override object[] ParameterValues {
			get { return new object[] { _data.Level }; }
			set {
				_data.Level = (Level)value[0];
				IsDirty = true;
			}
		}

		protected override void _PreRender() {
			_channelData = _Generate(TargetNodes, TimeSpan, _data.Level);
		}

		protected override ChannelData _Render() {
			return _channelData;
		}

		// Overriding this so that we can cast the value to
		// our specific data object's type one time instead
		// of every time we reference it.
		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as SampleEffectData; }
		}

		private ChannelData _Generate(ChannelNode[] nodes, TimeSpan timeSpan, Level level) {
			ChannelData channelData;

			// If applying to a single node and that node has the Sample Property property,
			// we're going to render differently.
			if(nodes.Length == 1 && nodes.First().Properties.Get(_samplePropertyId) != null) {
				channelData = _RenderSpecial(nodes, timeSpan, level);
			} else {
				channelData = _RenderNormal(nodes, timeSpan, level);
			}

			return channelData;
		}

		private ChannelData _RenderSpecial(ChannelNode[] nodes, TimeSpan timeSpan, Level level) {
			// We're going to render the normal data, but then tweak it a little bit
			// by having it fade out over a series of 10 steps.
			ChannelData normalData = _RenderNormal(nodes, timeSpan, level);
			ChannelData ourData = new ChannelData();
			int divisionCount = 10;

			if(normalData.Count > 0) {
				CommandNode[] commandNodes = normalData.Values.First();
				if(commandNodes.Length > 0) {
					foreach(Guid channelId in normalData.Keys) {
						List<CommandNode> commands = new List<CommandNode>();
						foreach(CommandNode commandNode in normalData[channelId]) {
							Level currentLevel = (commandNode.Command as Lighting.Monochrome.SetLevel).Level;
							double currentTime = commandNode.StartTime.TotalMilliseconds;
							double timeDelta = commandNode.TimeSpan.TotalMilliseconds / divisionCount;
							double levelDelta = (commandNode.Command as Lighting.Monochrome.SetLevel).Level / divisionCount;
							for(int i=0; i<divisionCount; i++) {
								commands.Add(new CommandNode(new Lighting.Monochrome.SetLevel(currentLevel), TimeSpan.FromMilliseconds(currentTime), TimeSpan.FromMilliseconds(timeDelta)));
								currentLevel -= levelDelta;
								currentTime += timeDelta;
							}
						}
						ourData.AddCommandNodesForChannel(channelId, commands);
					}
				}
			}

			return ourData;
		}

		private ChannelData _RenderNormal(ChannelNode[] nodes, TimeSpan timeSpan, Level level) {
			// We are given a collection of ChannelNodes.
			// A ChannelNode may be a single channel or it may be a tree of channels.
			// Ultimately, we have an ordered collection of channels to generate
			// data for.
			ChannelData channelData = new ChannelData();
			Channel[] channels = nodes.SelectMany(x => x.GetChannelEnumerator()).ToArray();

			foreach(Channel channel in channels) {
				CommandNode[] channelCommands = _GenerateChannelData(timeSpan, level);
				channelData[channel.Id] = channelCommands;
			}

			return channelData;
		}

		private CommandNode[] _GenerateChannelData(TimeSpan timeSpan, Level highLevel) {
			double timeLeft = timeSpan.TotalMilliseconds;
			double startTime = 0;
			bool on = true;
			List<CommandNode> commands = new List<CommandNode>();

			// We are going to make all of the channels blink on and off together every 500 ms.
			while(timeLeft > 0) {
				double time = timeLeft >= 500 ? 500 : timeLeft;

				CommandNode data = new CommandNode(new Lighting.Monochrome.SetLevel(on ? highLevel : 0), TimeSpan.FromMilliseconds(startTime), TimeSpan.FromMilliseconds(time));
				commands.Add(data);

				on = !on;
				timeLeft -= time;
				startTime += time;
			}

			return commands.ToArray();
		}
	}
}
