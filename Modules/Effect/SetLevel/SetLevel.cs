using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Module.Intent;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using VixenModules.Property.RGB;
using System.Drawing;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevel : EffectModuleInstanceBase
	{
		private SetLevelData _data;
		private EffectIntents _channelData = null;
		private readonly Guid _levelIntentId = new Guid("{0DFDF022-B1C4-49b9-9D65-2568A372FE28}");

		public SetLevel()
		{
			_data = new SetLevelData();
		}

		protected override void _PreRender()
		{
			_channelData = new EffectIntents();

			foreach (ChannelNode node in TargetNodes) {
				RenderNode(node);
			}
		}

		protected override EffectIntents _Render()
		{
			return _channelData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as SetLevelData; }
		}

		public override object[] ParameterValues
		{
			get
			{
				return new object[] { Level, Color };
			}
			set
			{
				if (value.Length != 2) {
					VixenSystem.Logging.Error("SetLevel parameters set with " + value.Length + " parameters!");
				} else {
					Level = value[0].DynamicCast<Level>();
					Color = (Color)value[1];
				}
			}
		}

		public Level Level
		{
			get { return _data.level; }
			set { _data.level = value; IsDirty = true; }
		}

		public Color Color
		{
			get { return _data.color; }
			set { _data.color = value; IsDirty = true; }
		}

		// renders the given node to the internal ChannelData dictionary. If the given node is
		// not a channel, will recursively descend until we render its channels.
		private void RenderNode(ChannelNode node)
		{
			// if this node is an RGB node, then it will know what to do with it (might render directly,
			// might be broken down into sub-channels, etc.) So just pass it off to that instead.
			if (node.Properties.Contains(SetLevelDescriptor._RGBPropertyId)) {
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
				//Command setLevelCommand = new Lighting.Monochrome.SetLevel(Level);
				//CommandNode data = new CommandNode(setLevelCommand, TimeSpan.Zero, TimeSpan);

				IIntentModuleInstance intent = ApplicationServices.Get<IIntentModuleInstance>(_levelIntentId);
				intent.TimeSpan = TimeSpan;
				IntentNode data = new IntentNode(intent, TimeSpan.Zero);
				if (channel != null)
					_channelData[channel.Id] = new[] { data };
			}
		}

		private void RenderRGB(ChannelNode node)
		{
			// get the RGB property for the channel, and get it to render the data for us
			RGBModule rgbProperty = node.Properties.Get(SetLevelDescriptor._RGBPropertyId) as RGBModule;
			ChannelCommands rgbData = rgbProperty.RenderColorToCommands(Color, Level);

			// iterate through the rendered commands, adjust them to fit our times, and add them to our rendered data
			foreach (KeyValuePair<Guid, Command[]> kvp in rgbData) {
				foreach (Command c in kvp.Value) {
					//TODO
					//CommandNode newCommandNode = new CommandNode(c, TimeSpan.Zero, TimeSpan);
					//_channelData.AddCommandNodeForChannel(kvp.Key, newCommandNode);
				}
			}
		}
	}
}
