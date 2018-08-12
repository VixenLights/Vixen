using System;
using Vixen.Module.Property;

namespace VixenModules.Property.Orientation {
	public	class OrientationDescriptor  : PropertyModuleDescriptorBase
	{
		public static Guid _typeId = new Guid("{DB0FC9FC-CDD5-4F88-BD66-5E4CF08FEAE4}");

		public override string TypeName
		{
			get { return "Orientation"; }
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
			get { return typeof (OrientationModule); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Provides Orientation information about display elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (OrientationData); }
		}
	}
}

