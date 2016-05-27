using System;
using Vixen.Module.MixingFilter;
using VixenModules.LayerCombiningFilter.InverseProportion;

namespace VixenModules.OutputFilter.DimmingCurve
{
	public class InverseProportionModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{1F16D67A-1FFF-485C-8E76-27FF7E7D072B}");

		public override string TypeName
		{
			get { return "Inverse Proportion"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (InverseProportionModule); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get
			{
				return
					"Combines two layers by using an inverse proportion of the intensity of the higher layer applied to the lower layer";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
