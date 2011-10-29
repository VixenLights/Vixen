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
			ChannelData channelData = new ChannelData();

			// We are given a collection of ChannelNodes.
			// A ChannelNode may be a single channel or it may be a tree of channels.
			// Ultimately, we have an ordered collection of channels to generate
			// data for.
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
