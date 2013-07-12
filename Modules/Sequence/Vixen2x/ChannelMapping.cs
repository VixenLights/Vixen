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
		public string ChannelName;
		public Color ChannelColor;
		public string ChannelOutput;
		public string ChannelNumber;
		public Color DestinationColor;
		public Guid ElementNodeId;


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