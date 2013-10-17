using System;
using Vixen.Module.Controller;

namespace VixenModules.Output.DummyLighting
{
	public class DummyLightingDescriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{b6ee6308-189c-4268-8996-32a4bab8ab5f}");

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (DummyLighting); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string TypeName
		{
			get { return "Dummy Lighting"; }
		}

		public override string Description
		{
			get { return "A dummy/test output module that will display lighting commands in a new window."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (DummyLightingData); }
		}

	}
}