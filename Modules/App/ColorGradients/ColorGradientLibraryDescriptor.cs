using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	class ColorGradientLibraryDescriptor : AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		override public string TypeName
		{
			get { return "ColorGradients"; }
		}

		override public Guid TypeId
		{
			get { return _id; }
		}

		override public Type ModuleClass
		{
			get { return typeof(ColorGradientLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(ColorGradientLibraryStaticData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string Description
		{
			get { return "Provides a data type which represents an arbitrary color gradient/transition over time, and additional functionality (such as a gradient editor and library)."; }
		}

		override public string Version
		{
			get { return "0.1"; }
		}

		public static Guid ModuleID
		{
			get { return _id; }
		}
	}
}
