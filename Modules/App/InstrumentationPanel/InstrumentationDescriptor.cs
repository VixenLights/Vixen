using System;
using Vixen.Module.App;

namespace VixenModules.App.InstrumentationPanel
{
	public class InstrumentationDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{CBC07BF2-7E1B-4a29-AEED-9EE235CB0DEE}");

		public override string TypeName
		{
			get { return "Instrumentation Panel"; }
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
			get { return "Text display of a few stats."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (InstrumentationModule); }
		}
	}
}