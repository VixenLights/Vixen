using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.LipSyncApp
{
	public class LipSyncMapDescriptor : AppModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("A475C535-73FA-4D31-A1BC-ED2B50AD21CF");

		public override string TypeName
		{
			get { return "LipSync App"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public static Guid ModuleID
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(LipSyncMapLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(LipSyncMapStaticData); }
		}

		public override string Author
		{
			get { return "Ed Brady"; }
		}

		public override string Description
		{
			get { return "Breaks down Phoneme mappings into discrete animation components."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
