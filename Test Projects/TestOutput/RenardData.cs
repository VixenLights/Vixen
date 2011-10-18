using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestOutput
{
	[DataContract]
	class RenardData : ModuleDataModelBase
	{
		[DataMember]
		public RenardRenderStyle RenderStyle { get; set; }

		public RenardData()
		{
			RenderStyle = RenardRenderStyle.Monochrome;
		}

		public override IModuleDataModel Clone()
		{
			RenardData result = new RenardData();
			result.RenderStyle = RenderStyle;
			return result;
		}
	}
}
