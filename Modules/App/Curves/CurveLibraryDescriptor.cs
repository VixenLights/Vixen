using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.Curves
{
	class CurveLibraryDescriptor: AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");

		override public string TypeName
		{
			get { return "Curves"; }
		}

		override public Guid TypeId
		{
			get { return _id; }
		}

		override public Type ModuleClass
		{
			get { return typeof(CurveLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(CurveLibraryStaticData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string Description
		{
			get { return "Provides a data type which represents a curve and additional functionality (such as a curve editor and library)."; }
		}

		override public string Version
		{
			get { return "1.0"; }
		}

		public static Guid ModuleID
		{
			get { return _id; }
		}
	}
}
