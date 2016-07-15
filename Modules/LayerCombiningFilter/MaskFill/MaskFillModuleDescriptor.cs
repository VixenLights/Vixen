using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.MaskFill
{
	public class MaskFillModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{24878B18-14F8-458E-ABAA-3A9C9A34931C}");

		public override string TypeName
		{
			get { return "Mask And Fill"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (MaskFillModule); }
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
					"Uses the higher layer to mask and fill in the next lower layer.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
