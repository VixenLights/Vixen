using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Runtime.Serialization;

namespace VixenModules.App.ColorGradients
{
	[DataContract]
	class ColorGradientLibraryStaticData : ModuleDataModelBase
	{
		public ColorGradientLibraryStaticData()
		{
			Library = new Dictionary<string, ColorGradient>();
		}

		[DataMember]
		private Dictionary<string, ColorGradient> _library;
		public Dictionary<string, ColorGradient> Library
		{
			get
			{
				if (_library == null)
					_library = new Dictionary<string, ColorGradient>();

				return _library;
			}
			set
			{
				_library = value;
			}
		}

		[DataMember]
		public Rectangle SelectorWindowBounds { get; set; }

		public override IModuleDataModel Clone()
		{
			ColorGradientLibraryStaticData result = new ColorGradientLibraryStaticData();
			result.Library = new Dictionary<string, ColorGradient>(Library);
			return result;
		}
	}
}
