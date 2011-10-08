using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	class ColorGradientLibraryModule : AppModuleInstanceBase
	{
		private ColorGradientLibraryStaticData _data;

		public override void Loading() { }

		public override void Unloading() { }

		public override Vixen.Sys.IApplication Application { set { } }

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as ColorGradientLibraryStaticData; }
		}

	}

}
