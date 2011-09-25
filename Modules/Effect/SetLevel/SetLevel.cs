using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using VixenModules.Property.RGB;
using CommandStandard;
using CommandStandard.Types;
using CommonElements.ColorManagement.ColorModels;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevel : EffectModuleInstanceBase
	{
		static private CommandIdentifier _setLevelCommandId = Standard.Lighting.Monochrome.SetLevel.Id;
		private SetLevelData _data;
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
					Level = (Level)value[0];
					Color = (RGB)value[1];
				}
			}
		}

		public Level Level
		{
			get { return _data.level; }
			set { _data.level = value; }
		}

		public RGB Color
		{
			get { return _data.color; }
			set { _data.color = value; }
		}

		// renders the given node to the internal ChannelData dictionary. If the given node is
		// not a channel, will recursively descend until we render its channels.
		private void RenderNode(ChannelNode node)
		{
			// if this node is an RGB node, then it will know what to do with it (might render directly,
			// might be broken down into sub-channels, etc.) So just pass it off to that instead.
			if (node.Properties.Contains(SetLevelModuleDescriptor._rgbProperty)) {
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
				Command data = new Command(TimeSpan.Zero, TimeSpan, _setLevelCommandId, new object[] { Level } );
				_channelData[channel.Id] = new[] { data };
			}
		}

		private void RenderRGB(ChannelNode node)
		{
			// get the RGB property for the channel, and get it to render the data for us
			RGBModule rgbProperty = node.Properties.Get(SetLevelModuleDescriptor._rgbProperty) as RGBModule;
			ChannelData rgbData = rgbProperty.RenderColorToCommands(Color, Level);

			// iterate through the rendered commands, adjust them to fit our times, and add them to our rendered data
			foreach (KeyValuePair<Guid, Command[]> kvp in rgbData) {
				foreach (Command c in kvp.Value) {
					c.StartTime = TimeSpan.Zero;
					c.EndTime = TimeSpan;
					_channelData.AddCommandForChannel(kvp.Key, c);
				}
			}
		}
	}
}
