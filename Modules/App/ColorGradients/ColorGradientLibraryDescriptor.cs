using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	internal class ColorGradientLibraryDescriptor : AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public override string TypeName
		{
			get { return "ColorGradients"; }
		}

		public override Guid TypeId
		{
			get { return _id; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorGradientLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (ColorGradientLibraryStaticData); }
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
					"Provides a data type which represents an arbitrary color gradient/transition over time, and additional functionality (such as a gradient editor and library).";
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