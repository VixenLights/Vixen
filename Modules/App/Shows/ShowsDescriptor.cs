using System;
using Vixen.Module.App;

namespace VixenModules.App.Shows
{
	public class ShowsDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{67A6DFDB-DDEC-4A5F-A158-129421E2366F}");

		public override string TypeName
		{
			get { return "Shows"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Derek Backus"; }
		}

		public override string Description
		{
			get { return "Create and edit a massive show extravaganza"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(ShowsModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(ShowsData); }
		}
	}
}