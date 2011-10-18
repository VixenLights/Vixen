using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Sys;
using CommonElements.ColorManagement.ColorModels;
using System.Drawing;

namespace VixenModules.Property.RGB
{
	public class RGBModule : PropertyModuleInstanceBase
	{
		private RGBData _data;

		public override void SetDefaultValues()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			if (channels.Count == 3) {
				_data.RGBType = RGBModelType.eIndividualRGBChannels;
				_data.RedChannelNode = channels[0].Id;
				_data.GreenChannelNode = channels[1].Id;
				_data.BlueChannelNode = channels[2].Id;
			} else {
				_data.RGBType = RGBModelType.eSingleRGBChannel;
			}
		}

		public override void Setup()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			using (RGBSetup form = new RGBSetup(_data, channels.ToArray())) {
				form.ShowDialog();
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as RGBData; }
		}

		public ChannelCommands RenderColorToCommands(Color color, Level level)
		{
			ChannelCommands result = new ChannelCommands();

			HSV finalColor = HSV.FromRGB(color);
			finalColor.V = level / 100.0;

			// if it's all getting dumped out each channel, just make 'smart' RGB commands for them
			if (_data.RGBType == RGBModelType.eSingleRGBChannel)
			{
				Color fullColorParameter = finalColor.ToRGB().ToArgb();
				result.AddCommandForChannel(Owner.Channel.Id, new Lighting.Polychrome.SetColor(fullColorParameter));
			}
			// otherwise, we're breaking it up by channel, so split the color up into discrete components
			else
			{
				// These need to be scaled: levels are 0-100, components are 0.0 - 1.0
				Level R = finalColor.ToRGB().R * 100.0;
				Level G = finalColor.ToRGB().G * 100.0;
				Level B = finalColor.ToRGB().B * 100.0;

				// populate the red channel(s) with a setlevel of the red value
				ChannelNode redNode = ChannelNode.GetChannelNode(_data.RedChannelNode);
				if (redNode != null) {
					result.AddCommandForChannel(redNode.Channel.Id, new Lighting.Monochrome.SetLevel(R));
				}
				
				// populate the green channel(s) with a setlevel of the green value
				ChannelNode greenNode = ChannelNode.GetChannelNode(_data.GreenChannelNode);
				if (greenNode != null) {
					result.AddCommandForChannel(greenNode.Channel.Id, new Lighting.Monochrome.SetLevel(G));
				}
				
				// populate the blue channel(s) with a setlevel of the blue value
				ChannelNode blueNode = ChannelNode.GetChannelNode(_data.BlueChannelNode);
				if (blueNode != null) {
					result.AddCommandForChannel(blueNode.Channel.Id, new Lighting.Monochrome.SetLevel(B));
				}
			}

			return result;
		}

		public static ChannelCommands RenderColorToCommandsForNode(ChannelNode node, CommonElements.ColorManagement.ColorModels.RGB color, Level level)
		{
			RGBModule instance = node.Properties.Get(RGBDescriptor.ModuleID) as RGBModule;
			if (instance == null)
			{
				VixenSystem.Logging.Warning("Invalid attempt to render RGB color on node '" + node.Name + "', ID " + node.Id);
				return null;
			}

			return instance.RenderColorToCommands(color, level);
		}
	}
}
