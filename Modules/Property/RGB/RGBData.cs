using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.RGB
{
	[DataContract]
	public class RGBData : ModuleDataModelBase
	{
		[DataMember]
		public Guid RedChannelNode { get; set; }

		[DataMember]
		public Guid GreenChannelNode { get; set; }

		[DataMember]
		public Guid BlueChannelNode { get; set; }

		[DataMember]
		public RGBModelType RGBType { get; set; }

		public override IModuleDataModel Clone()
		{
			RGBData result = new RGBData();
			result.RedChannelNode = RedChannelNode;
			result.GreenChannelNode = GreenChannelNode;
			result.BlueChannelNode = BlueChannelNode;
			return result;
		}
	}

	public enum RGBModelType
	{
		// in this mode, the RGB color is made up of individual color channels; one
		// each for R, G and B. The property should intelligently break it down to those groups.
		eIndividualRGBChannels,

		// in this mode, the RGB color is in a single channel, and whole 'set color' commands
		// will be sent to the controller for that channel. 
		eSingleRGBChannel,
	}
}
