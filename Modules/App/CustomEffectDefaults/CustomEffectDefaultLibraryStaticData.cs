using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
//using VixenModules.Effect.CustomValue;


namespace VixenModules.App.CustomEffectDefaults
{

	[DataContract]
	[KnownType(typeof(System.Drawing.Color))]
	[KnownType(typeof(ColorGradient))]
	[KnownType(typeof(Curve))]
	[KnownType(typeof(VixenModules.Effect.CustomValue.CustomValueType))]
	[KnownType(typeof(VixenModules.Effect.Nutcracker.NutcrackerData))]


	internal class CustomEffectDefaultLibraryStaticData : ModuleDataModelBase
	{
		public CustomEffectDefaultLibraryStaticData()
		{
			Library = new Dictionary<string, object[]>();
		}

		[DataMember]
		private Dictionary<string, object[]> _library;

		public Dictionary<string, object[]> Library
		{
			get
			{
				if (_library == null)
					_library = new Dictionary<string, object[]>();

				return _library;
			}
			set { _library = value; }
		}

		//[DataMember]
		//public Rectangle SelectorWindowBounds { get; set; }

		public override IModuleDataModel Clone()
		{
			CustomEffectDefaultLibraryStaticData result = new CustomEffectDefaultLibraryStaticData();
			result.Library = new Dictionary<string, object[]>(Library);
			return result;
		}
	}
}
