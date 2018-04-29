using System;
using Vixen.Module.Property;

namespace VixenModules.Property.Order {
	public	class OrderDescriptor  : PropertyModuleDescriptorBase
	{
		public static Guid Id = new Guid("{47D4A43B-5407-44BD-8BE1-4974B148504F}");

		public override string TypeName
		{
			get { return "Order"; }
		}

		public override Guid TypeId
		{
			get { return Id; }
		}

		public static Guid ModuleId
		{
			get { return Id; }
		}

		public override Type ModuleClass
		{
			get { return typeof (OrderModule); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Provides order mechanism for patching of display elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (OrderData); }
		}
	}
}

