using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace VixenModules.Output.HelixController
{
	[DataContract]
	public class HelixData : ModuleDataModelBase
	{
		[DataMember]
		private int SequenceEventPeriod { get; set; }

		public int EventPeriod
		{
			get
			{
				if (SequenceEventPeriod == 0)
				{
					SequenceEventPeriod = 50;
				}
				return SequenceEventPeriod;
			}
			set { SequenceEventPeriod = value; }
		}

		override public IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}
