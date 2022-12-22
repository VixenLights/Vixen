using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Color
{
	[DataContract]
	public class ColorStaticData : ModuleDataModelBase
	{
		public static event EventHandler<StringEventArgs> ColorSetChanged;

		[DataMember]
		private Dictionary<string, ColorSet> ColorSets { get; set; }

		public ColorStaticData()
		{
			ColorSets = new Dictionary<string, ColorSet>();

			ColorSet newcs;

			newcs = new ColorSet();
			newcs.Add(System.Drawing.Color.FromArgb(255, 0, 0));
			newcs.Add(System.Drawing.Color.FromArgb(0, 255, 0));
			newcs.Add(System.Drawing.Color.FromArgb(0, 0, 255));
			newcs.Add(System.Drawing.Color.FromArgb(255, 255, 255));
			ColorSets.Add("RGBW", newcs);

			newcs = new ColorSet();
			newcs.Add(System.Drawing.Color.FromArgb(255, 0, 0));
			newcs.Add(System.Drawing.Color.FromArgb(0, 255, 0));
			newcs.Add(System.Drawing.Color.FromArgb(0, 0, 255));
			ColorSets.Add("RGB", newcs);
		}

		public bool ContainsColorSet(string name)
		{
			return ColorSets.ContainsKey(name);
		}

		public ColorSet GetColorSet(string name)
		{
			return ColorSets[name];
		}

		public void SetColorSet(string name, ColorSet value)
		{
			if (!ColorSets.ContainsKey(name))
				ColorSets.Add(name, value);
			ColorSets[name] = value;
			if (ColorSetChanged != null)
				ColorSetChanged(this, new StringEventArgs(name));
		}

		public bool RemoveColorSet(string name)
		{
			return ColorSets.Remove(name);
		}

		public List<string> GetColorSetNames()
		{
			return ColorSets.Keys.ToList();
		}


		public override IModuleDataModel Clone()
		{
			return (ColorStaticData) MemberwiseClone();
		}
	}
}