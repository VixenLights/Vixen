using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Module.SequenceType;

namespace VixenModules.Sequence.Vixen2x
{
	[DataContract]
	public class Vixen2xSequenceData : ModuleDataModelBase
	{
		//[DataMember]

		public Vixen2xSequenceData()
		{
			
		}

		public override IModuleDataModel Clone()
		{
			Vixen2xSequenceData result = new Vixen2xSequenceData();
			return result;
		}
	}
}
