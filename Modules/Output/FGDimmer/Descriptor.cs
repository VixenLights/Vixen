using System;
using System.Collections.Generic;
using Vixen.Module.Controller;

namespace VixenModules.Output.FGDimmer
{
	public class Descriptor : ControllerModuleDescriptorBase
	{
		private readonly Guid _typeId = new Guid("{1AD81BB5-CB3E-4479-B758-7BC1EDBECC12}");

		public override string Author
		{
			get { return "Tony Eberle"; }
		}

		public override string Description
		{
			get { return "FireGod Dimmer hardware module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (FGDimmer); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (FGDimmerData); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "Firegod"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}