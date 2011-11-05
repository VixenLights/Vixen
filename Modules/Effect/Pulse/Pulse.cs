using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using CommonElements.ColorManagement.ColorModels;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Property.RGB;
using System.Drawing;

namespace VixenModules.Effect.Pulse
{
	public class Pulse : EffectModuleInstanceBase
	{
		private PulseData _data;
		private ChannelData _channelData = null;

		protected override void _PreRender()
		{
			_channelData = new ChannelData();

			foreach (ChannelNode node in TargetNodes) {
				RenderNode(node);
			}
		}

		protected override ChannelData _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as PulseData; }
		}

		public override object[] ParameterValues
		{
			get
			{
				return new object[] { LevelCurve, ColorGradient };
			}
			set
			{
				if (value.Length != 2) {
					VixenSystem.Logging.Error("Pulse parameters set with " + value.Length + " parameters!");
				} else {
					LevelCurve = (Curve)value[0];
					ColorGradient = (ColorGradient)value[1];
				}
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (!LevelCurve.CheckLibraryReference())
					return true;

				if (!ColorGradient.CheckLibraryReference())
					return true;

				return base.IsDirty;
			}
			protected set
			{
				base.IsDirty = value;
			}
		}


		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set { _data.LevelCurve = value; IsDirty = true; }
		}

		public ColorGradient ColorGradient
		{
			get { return _data.ColorGradient; }
			set { _data.ColorGradient = value; IsDirty = true; }
		}

		// The minimum amount of time between successive generated commands. Currently, this 
		// is essentially a fixed value, but if we need to later we can add the ability to change this.
		// (note: this value is nothing more than a minimum interval. If the value doesn't change much,
		// the intervals may be much higher.)
		private TimeSpan MinimumRenderInterval { get { return TimeSpan.FromMilliseconds(5); } }

		// the minimum level change between successive generated commands. As above, currently fixed,
		// may change later.
		// This is only used for monochrome commands, as it will be hard to tell what to do with RGB
		// commands (as the RGB property module may render them off to subchannels, etc.)
		private double MinimumLevelChangeInterval { get { return 1.0; } }


		// renders the given node to the internal ChannelData dictionary. If the given node is
		// not a channel, will recursively descend until we render its channels.
		private void RenderNode(ChannelNode node)
		{
			// if this node is an RGB node, then it will know what to do with it (might render directly,
			// might be broken down into sub-channels, etc.) So just pass it off to that instead.
			if (node.Properties.Contains(PulseDescriptor._RGBPropertyId)) {
				RenderRGB(node);
			} else {
				if (node.IsLeaf) {
					RenderMonochrome(node);
				} else {
					foreach (ChannelNode child in node.Children)
						RenderNode(child);
				}
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


		private void RenderRGB(ChannelNode node)
		{
			// for the given Channel, render a bunch of color commands (using the RGB property helpers)
			// for individual time slices of the effect. There's no real easy way (that's obvious after
			// 5 minutes of thought) to intelligently generate commands more efficently (ie. slower if we
			// can get away with it), so for now, just blat it out at full speed. TODO here for that.
			TimeSpan currentTime = TimeSpan.Zero;
			RGBModule rgbProperty = node.Properties.Get(PulseDescriptor._RGBPropertyId) as RGBModule;

			while (currentTime < TimeSpan) {
				double fractionalProgress = currentTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
				double percentProgress = fractionalProgress * 100.0;
				Level currentLevel = LevelCurve.GetValue(percentProgress);
				Color currentColor = ColorGradient.GetColorAt(fractionalProgress);

				TimeSpan sliceDuration = (currentTime + MinimumRenderInterval < TimeSpan) ? MinimumRenderInterval : TimeSpan - currentTime;
				ChannelCommands rgbData = rgbProperty.RenderColorToCommands(currentColor, currentLevel);
				foreach (KeyValuePair<Guid, Command[]> kvp in rgbData) {
					foreach (Command c in kvp.Value) {
						CommandNode newCommandNode = new CommandNode(c, currentTime, sliceDuration);
						_channelData.AddCommandNodeForChannel(kvp.Key, newCommandNode);
					}
				}

				currentTime += MinimumRenderInterval;
			}
		}
	}
}
