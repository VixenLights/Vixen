using System;
using Vixen.Module.App;

namespace VixenModules.App.InputEffectRouter
{
	public class InputEffectRouterDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{1F27C318-F5DA-45b2-B138-EA9A4054288D}");

		public override string TypeName
		{
			get { return "Input effect router"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Routes input effect data to nodes, live."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (InputEffectRouterModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (InputEffectRouterData); }
		}
	}
}