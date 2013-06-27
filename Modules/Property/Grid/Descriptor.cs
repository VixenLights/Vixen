using System;
using Vixen.Module.Property;

namespace VixenModules.Property.Grid
{
	public class Descriptor : PropertyModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{DE3B4EC9-4E7F-4690-B0FE-3780B17A85EC}");

		public override string TypeName
		{
			get { return "Grid"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Data); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Marks the attributed node as a grid"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}