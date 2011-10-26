using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class TimedSequenceData : ModuleDataModelBase
	{
		[DataMember]
		public List<MarkCollection> MarkCollections { get; set; }

		public TimedSequenceData()
		{
			MarkCollections = new List<MarkCollection>();
		}

		public override IModuleDataModel Clone()
		{
			TimedSequenceData result = new TimedSequenceData();
			result.MarkCollections.AddRange(MarkCollections);
			return result;
		}
	}
}
