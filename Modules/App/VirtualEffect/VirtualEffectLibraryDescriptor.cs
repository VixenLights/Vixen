using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.VirtualEffect
{
	class VirtualEffectLibraryDescriptor:AppModuleDescriptorBase
	{
		private Guid _id = new Guid("{CEFA7ECE-B0CD-42F3-AD3F-42E38A6EEAD5}");
		public override string Author
		{
			get { return "Chris Maloney"; }
		}

		public override string Description
		{
			get { return "Implementation of Virtual Effects"; }
		}

		public override Guid TypeId
		{
			get { return _id; }
		}

		public override string TypeName
		{
			get { return "Virtual Effect"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(VirtualEffectLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get
			{
				return typeof(VirtualEffectLibraryData);
			}
		}
	}
}
