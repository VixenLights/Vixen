using System;
using Vixen.Module.Property;

namespace VixenModules.Property.Face {
	public	class FaceDescriptor  : PropertyModuleDescriptorBase
	{
		public static Guid Id = new Guid("{EDA73851-8350-4D3F-9DF2-0D3B34E70BB6}");

		public override string TypeName => "Face";

		public override Guid TypeId => Id;

		public static Guid ModuleId => Id;

		public override Type ModuleClass => typeof (FaceModule);

		public override string Author => "Jeff Uchitjil";

		public override string Description => "Provides mechanism for assigning Phoneme mapping of display elements";

		public override string Version => "1.0";

		public override Type ModuleStaticDataClass => typeof (FaceData);
	}
}

