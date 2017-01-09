using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.Curves
{
	public class CurveLibraryDescriptor : AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");

		public override string TypeName
		{
			get { return "Curves"; }
		}

		public override Guid TypeId
		{
			get { return _id; }
		}

		public override Type ModuleClass
		{
			get { return typeof (CurveLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (CurveLibraryStaticData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get
			{
				return
					"Provides a data type which represents a curve and additional functionality (such as a curve editor and library).";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public static Guid ModuleID
		{
			get { return _id; }
		}
	}
}