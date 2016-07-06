using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.IntensityOverlay
{
	public class IntensityOverlayModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{43F48BF5-4088-403F-AD7A-D0FF2114CA31}");

		public override string TypeName
		{
			get { return "Intensity Overlay"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (IntensityOverlay); }
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
					"Uses the higher layer intensity to apply to the lower layer.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
