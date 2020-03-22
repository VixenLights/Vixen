using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VixenModules.Output.FGDimmer
{
	[DataContract]
	public class FGDimmerControlModule
	{
		public FGDimmerControlModule(int id)
		{
			ID = id;
			Enabled = false;
			StartChannel = 1;
		}

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public int StartChannel { get; set; }

		[DataMember]
		public int ID { get; set; }
	}
}