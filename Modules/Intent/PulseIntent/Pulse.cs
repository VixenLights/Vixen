using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.Curves;
using System.Drawing;

namespace VixenModules.Intent.Pulse
{
	public class Pulse : IntentModuleInstanceBase
	{
		private PulseData _data;

		public Pulse()
		{
			_data = new PulseData();
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as PulseData; }
		}

		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set { _data.LevelCurve = value; }
		}

		// The minimum amount of time between successive generated commands. Currently, this 
		// is essentially a fixed value, but if we need to later we can add the ability to change this.
		// (note: this value is nothing more than a minimum interval. If the value doesn't change much,
		// the intervals may be much higher.)
		private TimeSpan MinimumRenderInterval { get { return TimeSpan.FromMilliseconds(10); } }

		// the minimum level change between successive generated commands. As above, currently fixed,
		// may change later.
		// This is only used for monochrome commands, as it will be hard to tell what to do with RGB
		// commands (as the RGB property module may render them off to subchannels, etc.)
		private double MinimumLevelChangeInterval { get { return 1.0; } }


		// renders the given node to the internal ChannelData dictionary. If the given node is
		// not a channel, will recursively descend until we render its channels.
		private void RenderNode(ChannelNode node)
		{
			foreach (ChannelNode renderableNode in RGBModule.FindAllRenderableChildren(node)) {
				RenderMonochrome(renderableNode);
			}
		}

		// renders a single command for each channel in the given node with the specified monochrome level.
		private void RenderMonochrome(ChannelNode node)
		{
			// this is probably always going to be a single channel for the given node, as
			// we have iterated down to leaf nodes in RenderNode() above. May as well do
			// it this way, though, in case something changes in future.
			foreach (Channel channel in node.GetChannelEnumerator()) {
				if (channel == null)
					continue;

				List<CommandNode> data = new List<CommandNode>();
				double currentLevel = double.MaxValue;
				TimeSpan currentTime = TimeSpan.Zero;

				// track the time that the last command started at and the level it was at. We need to actually create
				// a command after we've iterated past it (so we know how long it should be), so save these values for that.
				TimeSpan lastCommandTime = TimeSpan.Zero;
				double lastCommandLevel = 0.0;

				while (currentTime < TimeSpan) {
					double percentProgress = currentTime.TotalMilliseconds / TimeSpan.TotalMilliseconds * 100.0;
					double newLevel = LevelCurve.GetValue(percentProgress);

					// if the difference is big enough to change, then make a new command: for the period that just
					// finished! -- and start a new one. But only do that if it's not the initial mark point.
					if (Math.Abs(currentLevel - newLevel) > MinimumLevelChangeInterval) {
						if (currentTime > TimeSpan.Zero) {
							data.Add(new CommandNode(new Lighting.Monochrome.SetLevel(lastCommandLevel), lastCommandTime, currentTime - lastCommandTime));
						}
						lastCommandTime = currentTime;
						lastCommandLevel = newLevel;
					}

					currentTime += MinimumRenderInterval;
				}

				// there will be one last command to cover the end bit; so add that one too.
				if (currentTime > TimeSpan.Zero) {
					data.Add(new CommandNode(new Lighting.Monochrome.SetLevel(lastCommandLevel), lastCommandTime, TimeSpan - lastCommandTime));
				}

				// now take that list of commands, and whack it in the rendered channel data.
				_channelData.AddCommandNodesForChannel(channel.Id, data.ToArray());
			}
		}

		public override Command GetCurrentState(TimeSpan timeOffset) {
		}
	}
}
