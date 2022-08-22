using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Vixen.Commands;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains a color wheel entry.
	/// </summary>
    [DataContract]
	public class FixtureColorWheel : FixtureIndexBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureColorWheel()
        {
			// Initialize the colors to white
			Color1 = Color.White;
			Color2 = Color.White;
		}

		#endregion

		#region Public Properties
						
		/// <summary>
		/// Indicates the color wheel entry represents a half stop in between two colors.
		/// </summary>
		[DataMember]
		public bool HalfStep { get; set; }

		/// <summary>
		/// Persists the Color1 in HTML format.
		/// </summary>
		[DataMember]
		public string Color1Html
		{
			get { return ColorTranslator.ToHtml(Color1); }

			set { Color1 = ColorTranslator.FromHtml(value); }
		}

		/// <summary>
		/// Persists the Color2 in HTML format.
		/// </summary>
		[DataMember]
		public string Color2Html
		{
			get { return ColorTranslator.ToHtml(Color2); }
			
			set { Color2 = ColorTranslator.FromHtml(value); }			
		}

		/// <summary>
		/// First color of the entry.
		/// </summary>
		[XmlIgnoreAttribute]
		public Color Color1 { get; set; }

		/// <summary>
		/// Second color of the entry.
		/// </summary>
		[XmlIgnoreAttribute]
		public Color Color2 { get; set; }

		/// <summary>
		/// Backing field for the UseCurve property.
		/// </summary>
		private bool _useCurve;

		/// <summary>
		/// Indicates that this index entry selection needs to be represented by a curve.
		/// </summary>
		[DataMember]
		public override bool UseCurve 
		{ 
			get
			{
				return _useCurve;
			}
			set
			{
				_useCurve = value;

				// If the index uses a curve then...
				if (_useCurve)
				{
					// Indicate the index is color wheel type
					// Note this tag helps with opening the shutter when spinning the color wheel is active.
					IndexType = FixtureIndexType.ColorWheel;
				}
			}
		}

		/// <summary>
		/// Backing field for the ExcludeColorProperty property.
		/// </summary>
		private bool _excludeColorProperty;

		/// <summary>
		/// Indicates if the color entry should be included in the color property for the fixture.
		/// </summary>
		[DataMember]
		public bool ExcludeColorProperty
		{
			get
			{
				return _excludeColorProperty;
			}
			set
			{
				_excludeColorProperty = value;				
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the color wheel entry.
		/// </summary>
		/// <returns>Clone of the color wheel entry</returns>
		public FixtureColorWheel CreateInstanceForClone()
		{
			// Clone the color wheel entry
			FixtureColorWheel colorWheelEntry = new FixtureColorWheel
			{
				Name = Name,
				StartValue = StartValue,				
				EndValue = EndValue,
				HalfStep = HalfStep,
				Color1 = Color1,
				Color2 = Color2,
				UseCurve = UseCurve,
				ExcludeColorProperty = ExcludeColorProperty,
			};

			return colorWheelEntry;
		}

		#endregion
	}
}
