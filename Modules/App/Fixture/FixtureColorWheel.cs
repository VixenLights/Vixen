using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains a color wheel entry.
	/// </summary>
    [DataContract]
	public class FixtureColorWheel : FixtureItem
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
		/// DMX start value of the color entry.
		/// </summary>
		[DataMember]
		public int StartValue { get; set; }
		
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
		/// Indicates that this color wheel selection needs to be represented by a curve.
		/// </summary>
		[DataMember]
		public bool UseCurve { get; set; }

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
				HalfStep = HalfStep,
				Color1 = Color1,
				Color2 = Color2,
				UseCurve = UseCurve,
			};

			return colorWheelEntry;
		}

		#endregion
	}
}
