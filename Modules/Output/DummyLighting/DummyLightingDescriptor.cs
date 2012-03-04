using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace VixenModules.Output.DummyLighting
{
	public class DummyLightingDescriptor : OutputModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{b6ee6308-189c-4268-8996-32a4bab8ab5f}");

		override public Guid TypeId
		{
			get { return _typeId; }
		}

		override public Type ModuleClass
		{
			get { return typeof(DummyLighting); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string TypeName
		{
			get { return "Dummy Lighting"; }
		}

		override public string Description
		{
			get { return "A dummy/test output module that will display lighting commands in a new window."; }
		}

		override public string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(DummyLightingData); }
		}

		public override int UpdateInterval {
			get {
				return 20;
			}
		}
	}
}
