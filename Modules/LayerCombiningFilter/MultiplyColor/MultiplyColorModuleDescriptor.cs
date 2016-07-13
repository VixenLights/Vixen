using System;
using Vixen.Module.MixingFilter;
using VixenModules.LayerMixingFilter.MultiplyColor;

namespace VixenModules.LayerCombiningFilter.Mask
{
    public class MultiplyColorModuleDescriptor : LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{0C9DFC0B-8E39-4167-8036-79408C6B0B35}");

		public override string TypeName
		{
			get { return "Multiply Color"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
            get { return typeof(MultiplyColor); }
		}

		public override string Author
		{
			get { return "Jon Chuchla"; }
		}

		public override string Description
		{
			get
			{
				return
					"Multiplies the colors between layers.  Useful for modifying a color or brightness with another layer as input.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
