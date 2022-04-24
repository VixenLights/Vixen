using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
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

			// Default the function identity to custom
			FunctionIdentity = FunctionIdentity.Custom;
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

        #endregion

        #region Public Properties

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
			};

			// If rotation limits are defined then...
			if (RotationLimits != null)
            {
				// Clone the rotation limits
				result.RotationLimits = RotationLimits.CreateInstanceForClone();
            }

			// Loop over the fixture index data
			foreach(FixtureIndex fixtureIndex in IndexData)
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
