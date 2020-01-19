using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using Vixen.Module.Timing;

namespace VixenModules.Timing.Generic
{
	public class Descriptor : TimingModuleDescriptorBase
	{
		private Guid _typeId = Guid.Empty;

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Stopwatch-based timing module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return null; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "Generic timing"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}