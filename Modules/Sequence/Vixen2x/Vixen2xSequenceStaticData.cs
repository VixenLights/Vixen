using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module;
using System.Runtime.Serialization;

namespace VixenModules.SequenceType.Vixen2x
{
	[DataContract]
	public class Vixen2xSequenceStaticData : ModuleDataModelBase
	{
		[DataMember] private Dictionary<string, List<ChannelMapping>> vixen2xMappings;

		public Dictionary<string, List<ChannelMapping>> Vixen2xMappings
		{
			get
			{
				if (vixen2xMappings == null)
					vixen2xMappings = new Dictionary<string, List<ChannelMapping>>();
				return vixen2xMappings;
			}
			set { vixen2xMappings = value; }
		}

		public Vixen2xSequenceStaticData()
		{
			Vixen2xMappings = new Dictionary<string, List<ChannelMapping>>();
		}

		public override IModuleDataModel Clone()
		{
			Vixen2xSequenceStaticData data = new Vixen2xSequenceStaticData();
			data.Vixen2xMappings = new Dictionary<string, List<ChannelMapping>>(Vixen2xMappings);
			return data;
		}
	}
}