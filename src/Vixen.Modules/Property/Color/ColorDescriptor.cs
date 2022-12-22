
using Vixen.Module.Property;

namespace VixenModules.Property.Color
{
	public class ColorDescriptor : PropertyModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{BFF34727-6B88-4F87-82B7-68424498C725}");

		public override string TypeName
		{
			get { return "Color"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public static Guid ModuleId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (ColorData); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (ColorStaticData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Defines the element to have color, and the particular color(s) it can produce"; }
		}

		public override string Version
		{
			get { return "0.1"; }
		}
	}
}