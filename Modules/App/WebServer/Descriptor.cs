using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.App;
using VixenModules.App.WebServer; 

namespace VixenModules.App.WebServer
{
	public class Descriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{8AC1CDF9-8D57-43B6-81DE-93003ED2F907}");

		public override string TypeName
		{
			get { return "Vixen WebServer"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Darren McDaniel"; }
		}

		public override string Description
		{
			get { return "Web interface to Vixen 3"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Module); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(Data); }
		}
	}
}
