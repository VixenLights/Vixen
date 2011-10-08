using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Runtime.Serialization;

namespace VixenModules.App.Curves
{
	[DataContract]
	class CurveLibraryStaticData : ModuleDataModelBase, IEnumerable<KeyValuePair<string, Curve>>
	{
		[DataMember]
		private Dictionary<string, Curve> library;

		public CurveLibraryStaticData()
		{
			library = new Dictionary<string, Curve>();
		}

		public override IModuleDataModel Clone()
		{
			CurveLibraryStaticData result = new CurveLibraryStaticData();
			result.library = new Dictionary<string, Curve>(library);
			return result;
		}

		public bool Contains(string name)
		{
			return library.ContainsKey(name);
		}

		public Curve GetCurve(string name)
		{
			if (library.ContainsKey(name))
				return library[name];
			else
				return null;
		}

		public bool SetCurve(string name, Curve curve)
		{
			bool result = Contains(name);
			library[name] = curve;
			return result;
		}

		public IEnumerator<KeyValuePair<string, Curve>> GetEnumerator()
		{
			return library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return library.GetEnumerator();
		}
	}
}
