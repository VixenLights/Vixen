using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.Mask
{
	public class MaskModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{ACE0F097-7982-4EDC-B5DD-8989DCE60E1D}");

		public override string TypeName
		{
			get { return "Mask"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (MaskModule); }
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
					"Uses the higher layer to mask the next lower layer.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
