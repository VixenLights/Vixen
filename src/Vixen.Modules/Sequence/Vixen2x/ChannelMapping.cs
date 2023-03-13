using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using System.Drawing;
using System.Xml.Serialization;

namespace VixenModules.SequenceType.Vixen2x
{
	[Serializable]
	public class ChannelMapping
	{
		public string ChannelName = string.Empty;
		public Color ChannelColor = new Color();
		public string ChannelOutput = string.Empty;
		public string ChannelNumber = string.Empty;
		public Color DestinationColor = new Color();
		public Guid ElementNodeId;
		public bool RgbPixel = false;


		public ChannelMapping(string channelName, Color channelColor, string channelNumber, string channelOutput, Guid nodeId,
							  Color destinationColor, bool rgbPixel)
		{
			ChannelName = channelName;
			ChannelColor = channelColor;
			ChannelOutput = channelOutput;
			ChannelNumber = channelNumber;
			ElementNodeId = nodeId;
			DestinationColor = destinationColor;
			RgbPixel = rgbPixel;
		}

		public ChannelMapping(string channelName, Color channelColor, string channelNumber, string channelOutput, Guid nodeId,
		                      Color destinationColor)
		{
			ChannelName = channelName;
			ChannelColor = channelColor;
			ChannelOutput = channelOutput;
			ChannelNumber = channelNumber;
			ElementNodeId = nodeId;
			DestinationColor = destinationColor;
		}

		public ChannelMapping(string channelName, Color channelColor, string channelNumber, string channelOutput)
		{
			ChannelName = channelName;
			ChannelColor = channelColor;
			ChannelOutput = channelOutput;
			ChannelNumber = channelNumber;
		}

		public ChannelMapping()
		{
		}
	}
}