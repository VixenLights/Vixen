using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public class ChromaKeyModuleDescriptor: LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{2D467733-1A64-45AD-BCDC-84BF074747E6}");

		public override string TypeName
		{
			get { return "Chroma Key"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ChromaKeyModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ChromaKeyData); }
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
					"Creates a Chroma mask based on the selected brightness and fills with the higher layer.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
