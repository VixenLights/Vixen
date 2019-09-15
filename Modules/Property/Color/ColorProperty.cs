using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module.Property;
using System.Linq;
using Vixen.Module;
using Vixen.Sys;

namespace VixenModules.Property.Color
{
	public class ColorModule : PropertyModuleInstanceBase
	{
		private ColorData _data;

		public ColorModule()
		{
			ColorStaticData.ColorSetChanged += ColorSetChangedHandler;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ColorStaticData.ColorSetChanged -= ColorSetChangedHandler;	
			}
			base.Dispose(disposing);
		}

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		// if the color sets in the static data ("library") change, clear our reference to it,
		// and re-get it the next time we need it.
		private void ColorSetChangedHandler(object sender, StringEventArgs e)
		{
			ColorData instanceData = (ModuleData as ColorData);

			if (e.Value == instanceData.ColorSetName)
				_cachedColorSetReference = null;
		}

		public override void SetDefaultValues()
		{
			_data.ElementColorType = ElementColorType.FullColor;
		}

		public override void CloneValues(IProperty sourceProperty)
		{
			ColorModule source = sourceProperty as ColorModule;
			if (source == null) {
				Logging.Error(
					"ColorModule: trying to CloneValues from another property, but it's not a ColorModule!");
				return;
			}

			ColorType = source.ColorType;
			SingleColor = source.SingleColor;
			ColorSetName = source.ColorSetName;
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (ColorSetupForm colorSetupForm = new ColorSetupForm()) {
				colorSetupForm.ColorModule = this;
				colorSetupForm.ShowDialog();
			}

			return true;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = (ColorData) value; }
		}


		public ElementColorType ColorType
		{
			get { return _data.ElementColorType; }
			set { _data.ElementColorType = value; }
		}

		public System.Drawing.Color SingleColor
		{
			get { return _data.SingleColor; }
			set
			{
				_data.SingleColor = value;
				_cachedColorSetReference = null;
			}
		}

		public string ColorSetName
		{
			get { return _data.ColorSetName; }
			set
			{
				_data.ColorSetName = value;
				_cachedColorSetReference = null;
			}
		}


		private ColorSet _cachedColorSetReference = null;

		public ColorSet Colors
		{
			get
			{
				if (_cachedColorSetReference == null) {
					// get the color set from the static module data and cache it
					ColorStaticData staticData = (StaticModuleData as ColorStaticData);
					ColorData instanceData = (ModuleData as ColorData);

					if (staticData == null || instanceData == null) {
						Logging.Error("ColorProperty: null static data or instance data! That shouldn't happen!");
						return null;
					}

					if (staticData.ContainsColorSet(instanceData.ColorSetName)) {
						_cachedColorSetReference = staticData.GetColorSet(instanceData.ColorSetName);
					}
					else {
						Logging.Error("ColorProperty: can't find '" + instanceData.ColorSetName +
						                                    "' in the static data color sets. That's.... unexpected. Maybe the color set was deleted?");
						_cachedColorSetReference = new ColorSet();
					}
				}
				return _cachedColorSetReference;
			}
		}

		// static 'helper' methods in the color property
		// gets the color type for a given element node. If no color properties are defined, it's assumed to be "full color".
		public static ElementColorType getColorTypeForElementNode(IElementNode element)
		{
			ColorModule colorModule = element.Properties.Get(ColorDescriptor.ModuleId) as ColorModule;
			if (colorModule != null) {
				return colorModule.ColorType;
			}
			return ElementColorType.FullColor;
		}

		// static 'helper' methods in the color property
		// a simpler version of the getColorTypeForElementNode call -- often, we only care if an element should be discretely colored or not.
		public static bool isElementNodeDiscreteColored(IElementNode element)
		{
			ElementColorType type = getColorTypeForElementNode(element);
			if (type == ElementColorType.MultipleDiscreteColors || type == ElementColorType.SingleColor)
				return true;
			return false;
		}

		public static bool isElementNodeTreeDiscreteColored(IElementNode element)
		{
			return element.GetLeafEnumerator().Any(x => isElementNodeDiscreteColored(x));
		}

		// gets a enumerable of valid colors for the given element node. If the element is full color, an empty enumeration
		// will be returned; otherwise one of more colors will be returned (for single or multiple discrete colors).
		// wIt will recurse the children to collect from all parts of the element
		public static IEnumerable<System.Drawing.Color> getValidColorsForElementNode(IElementNode element, bool includeChildren)
		{
			HashSet<System.Drawing.Color> result = new HashSet<System.Drawing.Color>();
			ColorModule colorModule = element.Properties.Get(ColorDescriptor.ModuleId) as ColorModule;
			if (colorModule != null) {
				switch (colorModule.ColorType)
				{
					case ElementColorType.FullColor:
						break;

					case ElementColorType.MultipleDiscreteColors:
						colorModule.Colors.ForEach(x => result.Add(x));
						break;

					case ElementColorType.SingleColor:
						result.Add(colorModule.SingleColor);
						break;
				}
			}
			
			//recurse the children
			if (includeChildren)
			{
				if (element.Children.Any())
				{
					result.AddRange(element.Children.SelectMany(x => getValidColorsForElementNode(x, true)));
				}
			}
			

			return result;
		}
	}


	public class ColorDescriptor : PropertyModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{BFF34727-6B88-4F87-82B7-68424498C725}");

		public override string TypeName
		{
			get { return "Color"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public static Guid ModuleId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (ColorData); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (ColorStaticData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Defines the element to have color, and the particular color(s) it can produce"; }
		}

		public override string Version
		{
			get { return "0.1"; }
		}
	}


	[DataContract]
	public class ColorData : ModuleDataModelBase
	{
		[DataMember]
		public ElementColorType ElementColorType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public System.Drawing.Color SingleColor { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ColorSetName { get; set; }

		public ColorData()
		{
			ElementColorType = ElementColorType.FullColor;
			SingleColor = System.Drawing.Color.Empty;
			ColorSetName = null;
		}

		public override IModuleDataModel Clone()
		{
			return (ColorData) MemberwiseClone();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if(ElementColorType == ElementColorType.FullColor)
			{
				//Set any previous ones to the default so they won't be serialized needlessly
				SingleColor = System.Drawing.Color.Empty;
			}
			if (ColorSetName == String.Empty)
			{
				ColorSetName = null;
			}
		}

	}


	// contains all available options that are configured for the element color property.
	// by default, will contain some common formats -- RGBW, RGB, etc.
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

	public class ColorSet : List<System.Drawing.Color>
	{
	}

	public class StringEventArgs : EventArgs
	{
		public StringEventArgs(string value)
		{
			Value = value;
		}

		public string Value { get; private set; }
	}

	public enum ElementColorType
	{
		SingleColor,
		MultipleDiscreteColors,
		FullColor
	}
}
