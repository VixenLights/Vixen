using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using Vixen.Module.Controller;
using System.Net;

namespace VixenModules.Output.BlinkyLinky
{
	internal class BlinkyLinkyDescriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{b9d5d3a1-c746-4395-8e19-a0c975ced438}");

		public override string Author
		{
			get { return "Michael Sallaway"; }
		}

		public override string Description
		{
			get { return "Blinky-Linky hardware controller module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (BlinkyLinky); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (BlinkyLinkyData); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "BlinkyLinky"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override int UpdateInterval
		{
			get
			{
				return 20;
			}
		}
	}
}