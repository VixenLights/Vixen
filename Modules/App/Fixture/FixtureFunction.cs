using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Vixen.Data.Value;
using Vixen.TypeConverters;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Defines the types of functions supported.
	/// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FixtureFunctionType
	{
		[Description("Range")]
		Range,

		[Description("Indexed")]
		Indexed,

		[Description("Color Wheel")]
		ColorWheel,

		[Description("RGB Color")]
		RGBColor,
		
		[Description("RGBW Color")]
		RGBWColor,
		
		[Description("None")]
		None
	}

	/// <summary>
	/// Defines the direction of the zoom.
	/// </summary>
	public enum FixtureZoomType
	{
		[Description("Narrow To Wide")]
		NarrowToWide,

		[Description("Wide To Narrow")]
		WideToNarrow
	}

	/// <summary>
	/// Maintains a fixture function.
	/// </summary>
	[DataContract]
	public class FixtureFunction : FixtureItem
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunction()
		{
			// Initialize the collection of color wheel data
			ColorWheelData = new List<FixtureColorWheel>();

			// Initialize the collection of index (enumeration) data
			IndexData = new List<FixtureIndex>();

			// Initialize the rotation limits of the function
			RotationLimits = new FixtureRotationLimits();

			// Default the function identity to custom
			FunctionIdentity = FunctionIdentity.Custom;

			// Default the timeline color to a random color
			Random rnd = new Random();
			KnownColor[] colors = (KnownColor[])Enum.GetValues(typeof(KnownColor));			
			int colorIndex = rnd.Next(colors.Length - 1);				
			TimelineColor = Color.FromKnownColor(colors[colorIndex]);							
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Type of the function.
		/// </summary>
		[DataMember]
		public FixtureFunctionType FunctionType { get; set; }

		/// <summary>
		/// Function identity for support of the preview.
		/// </summary>
		[DataMember]
		public FunctionIdentity FunctionIdentity { get; set; }
		
		/// <summary>
		/// Index (enumeration) data associated with the function.
		/// </summary>
		[DataMember]
		public List<FixtureIndex> IndexData { get; set; }

		/// <summary>
		/// Color wheel data associated with the function.
		/// </summary>
		[DataMember]
		public List<FixtureColorWheel> ColorWheelData { get; set; }

		/// <summary>
		/// Label associated with the function.
		/// </summary>
		[DataMember]
		public string Label { get; set; }

		/// <summary>
		/// Maximum rotation.  This property only applies to pan and tilt functions.
		/// </summary>
		[DataMember]
		public FixtureRotationLimits RotationLimits { get; set; }

		/// <summary>
		/// Indicates how the fixture zooms.
		/// </summary>
		[DataMember]
		public FixtureZoomType ZoomType { get; set; }

		private Color _timelineColor;

		/// <summary>
		/// Color to use on timeline for effect graphical representation.
		/// </summary>
		[XmlIgnoreAttribute]
		public Color TimelineColor
		{	
			get
			{
				return _timelineColor;	
			}
			set
			{
				_timelineColor = value;	
			}
		}

		/// <summary>
		/// Persists the Color1 in HTML format.
		/// </summary>
		[DataMember]
		public string TimelineColor1Html
		{
			get { return ColorTranslator.ToHtml(TimelineColor); }
			set { TimelineColor = ColorTranslator.FromHtml(value); }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves normalized index data for the function.
		/// This method is supported by both index functions and color wheel functions.
		/// </summary>
		/// <returns>Collection of index data</returns>
		public List<FixtureIndexBase> GetIndexDataBase()
		{
			// Initialize the return collection to empty
			List<FixtureIndexBase> indexData = new List<FixtureIndexBase>();

			// If the function is an index function then...
			if (FunctionType == FixtureFunctionType.Indexed)
			{
				// Retrieve the fixture index data
				indexData = IndexData.Cast<FixtureIndexBase>().ToList();
			}
			// Otherwise if the function is a color wheel function then...
			else if (FunctionType == FixtureFunctionType.ColorWheel)
			{
				// Retrieve the function color wheel index data
				indexData = ColorWheelData.Cast<FixtureIndexBase>().ToList();
			}

			return indexData;
		}

		/// <summary>
		/// Creates a clone of the fixture function.
		/// </summary>
		/// <returns>Clone of the fixture function</returns>
		public FixtureFunction CreateInstanceForClone()
		{
			// Create a clone of the fixture function
			FixtureFunction result = new FixtureFunction
			{
				Name = Name,
				FunctionType = FunctionType,
				Label = Label,
				FunctionIdentity = FunctionIdentity,
				ZoomType = ZoomType,
				TimelineColor = TimelineColor,	
			};

			// If rotation limits are defined then...
			if (RotationLimits != null)
			{
				// Clone the rotation limits
				result.RotationLimits = RotationLimits.CreateInstanceForClone();
			}

			// Loop over the fixture index data
			foreach (FixtureIndex fixtureIndex in IndexData)
			{
				// Clone the index entry
				result.IndexData.Add(fixtureIndex.CreateInstanceForClone());
			}

			// Loop over the color wheel data
			foreach (FixtureColorWheel colorWheel in ColorWheelData)
			{
				// Clone the color wheel entry
				result.ColorWheelData.Add(colorWheel.CreateInstanceForClone());
			}

			return result;
		}

		#endregion
	}
}
