using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using CommandStandard;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevel : EffectModuleInstanceBase
	{
		static private CommandIdentifier _setLevelCommandId = Standard.Lighting.Monochrome.SetLevel.Id;

		private ChannelData _channelData = null;

		protected override void _PreRender()
		{
			_channelData = new ChannelData();

			foreach (ChannelNode node in TargetNodes) {
				if (node.Properties.Contains(SetLevelModuleDescriptor._rgbProperty)) {
					RenderRGB(node);
				} else {
					RenderMonochrome(node);
				}
			}
		}

		protected override ChannelData _Render()
		{
			return _channelData;
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
				Command data = new Command(TimeSpan.Zero, TimeSpan, _setLevelCommandId, ParameterValues);
				_channelData[channel.Id] = new[] { data };
			}
		}

		private void RenderRGB(ChannelNode node)
		{
			//TODO: need to do after we've implemented the RGB module
		}
	}
}
