using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.ColorChange
{
	public class ColorChangeModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{93D4169E-5138-403A-B71C-A5A1AB089801}");

		public override string TypeName
		{
			get { return "Color Change"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorChangeModule); }
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
					"Replaces the current color with the effect color at the same intensity.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
