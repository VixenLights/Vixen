using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module.Property;

namespace VixenModules.Property.Location {
	public	class LocationDescriptor  : PropertyModuleDescriptorBase
	{
		public static Guid _typeId = new Guid("{3FB53423-DD2A-4719-B3E3-19AA6F062F62}");

		public override string TypeName
		{
			get { return "Location"; }
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
			get { return typeof (LocationModule); }
		}

		public override string Author
		{
			get { return "Darren McDaniel"; }
		}

		public override string Description
		{
			get { return "Provides XYZ positioning of display elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (LocationData); }
		}
	}
}

