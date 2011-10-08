using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Runtime.Serialization;

namespace VixenModules.App.ColorGradients
{
	[DataContract]
	class ColorGradientLibraryStaticData : ModuleDataModelBase, IEnumerable<KeyValuePair<string, ColorGradient>>
	{
		[DataMember]
		private Dictionary<string, ColorGradient> library;

		public ColorGradientLibraryStaticData()
		{
			library = new Dictionary<string, ColorGradient>();
		}

		public override IModuleDataModel Clone()
		{
			ColorGradientLibraryStaticData result = new ColorGradientLibraryStaticData();
			result.library = new Dictionary<string, ColorGradient>(library);
			return result;
		}

		public bool Contains(string name)
		{
			return library.ContainsKey(name);
		}

		public ColorGradient GetColorGradient(string name)
		{
			if (library.ContainsKey(name))
				return library[name];
			else
				return null;
		}

		public bool SetCurve(string name, ColorGradient curve)
		{
			bool result = Contains(name);
			library[name] = curve;
			return result;
		}

		public IEnumerator<KeyValuePair<string, ColorGradient>> GetEnumerator()
		{
			return library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return library.GetEnumerator();
		}
	}
}
