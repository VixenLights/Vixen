
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.Controller.E131;

namespace VixenModules.Output.E131
{
	[DataContract]
	public class E131ModuleDataModel : ModuleDataModelBase
	{
		public E131ModuleDataModel() {
			Universes = new List<UniverseEntry>();
		}
         
		[DataMember]
		public int OutputCount { get; set; }

		[DataMember]
		public bool Warnings { get; set; }

		[DataMember]
		public bool Statistics { get; set; }

		[DataMember]
		public int EventRepeatCount { get; set; }

		[DataMember]
		public int EventSuppressCount { get; set; }

		[DataMember]
		public List<UniverseEntry> Universes { get; set; }

		public override IModuleDataModel Clone()
		{
			return new E131ModuleDataModel
			{
				OutputCount = OutputCount,
				Universes = Universes,

				EventRepeatCount = EventRepeatCount,
				EventSuppressCount = EventSuppressCount,
				Statistics = Statistics,
				Warnings = Warnings
			};
		}
	}

   
}