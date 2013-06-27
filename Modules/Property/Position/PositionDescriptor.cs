using System;
using Vixen.Module.Property;

namespace VixenModules.Property.Position
{
	public class PositionDescriptor : PropertyModuleDescriptorBase
	{
		internal static Guid _typeId = new Guid("{47156247-62F7-401d-A5A4-1F1C547DAEF9}");

		public override string TypeName
		{
			get { return "Position"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (PositionModule); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Provides display-relative positioning of display elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (PositionData); }
		}
	}
}