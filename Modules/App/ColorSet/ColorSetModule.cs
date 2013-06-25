using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

using System.Drawing;
using System.Runtime.Serialization;


namespace VixenModules.App.ColorSets
{
	class ColorSetDescriptor : AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{8E7A1A79-CEC9-483B-98EB-6AC21F90E11E}");

		override public string TypeName
		{
			get { return "ColorSets"; }
		}

		override public Guid TypeId
		{
			get { return _id; }
		}

		override public Type ModuleClass
		{
			get { return typeof(ColorSetModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(ColorSetStaticData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string Description
		{
			get { return "Provides an application-wide way to create a 'set' of colors to be used anywhere in the application (eg. 'RGBW', or 'RGB', or possibly some sequence-specific color sets)."; }
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



	[DataContract]
	class ColorSetStaticData : ModuleDataModelBase
	{
		public ColorSetStaticData()
		{
			Library = new Dictionary<string, ColorSet>();
		}

		[DataMember]
		private Dictionary<string, ColorSet> _library;
		public Dictionary<string, ColorSet> Library
		{
			get
			{
				if (_library == null)
					_library = new Dictionary<string, ColorSet>();

				return _library;
			}
			set
			{
				_library = value;
			}
		}

		public override IModuleDataModel Clone()
		{
			ColorSetStaticData result = new ColorSetStaticData();
			result.Library = new Dictionary<string, ColorSet>(Library);
			return result;
		}
	}





	class ColorSetModule : AppModuleInstanceBase, IEnumerable<KeyValuePair<string, ColorSet>>
	{
		private ColorSetStaticData _data;

		public override void Loading() { }

		public override void Unloading() { }

		public override Vixen.Sys.IApplication Application { set { } }

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as ColorSetStaticData; }
		}


		public Dictionary<string, ColorSet> ColorSetLibrary
		{
			get { return _data.Library; }
		}

		public List<string> ColorSetNames
		{
			get { return new List<string>(ColorSetLibrary.Keys.AsEnumerable()); }
		}

		public bool Contains(string name)
		{
			return ColorSetLibrary.ContainsKey(name);
		}

		public ColorSet GetColorSet(string name)
		{
			if (ColorSetLibrary.ContainsKey(name))
				return ColorSetLibrary[name];
			else
				return null;
		}

		public bool AddColorSet(string name, ColorSet cs)
		{
			if (name == "")
				return false;

			bool inLibrary = Contains(name);
			ColorSetLibrary[name] = cs;
			return inLibrary;
		}

		public bool RemoveColorGradient(string name)
		{
			if (!Contains(name))
				return false;

			ColorSetLibrary.Remove(name);
			return true;
		}

		public IEnumerator<KeyValuePair<string, ColorSet>> GetEnumerator()
		{
			return ColorSetLibrary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ColorSetLibrary.GetEnumerator();
		}

	}



	public class ColorSet
	{
		public List<Color> ColorList;
	}
}
