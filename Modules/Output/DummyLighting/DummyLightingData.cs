using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.DummyLighting
{
	[DataContract]
	class DummyLightingData : ModuleDataModelBase
	{
		[DataMember]
		public RenderStyle RenderStyle { get; set; }

		public DummyLightingData()
		{
			RenderStyle = RenderStyle.Monochrome;
		}

		public override IModuleDataModel Clone()
		{
			DummyLightingData result = new DummyLightingData();
			result.RenderStyle = RenderStyle;
			return result;
		}
	}
}
