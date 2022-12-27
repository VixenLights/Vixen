using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>	
	/// Maintains the fixture strobe effect data.
	/// </summary>	
	[DataContract]
	public class FixtureStrobeData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureStrobeData()
		{
			// Default the strobe rotation curve to ramp up
			FixtureStrobeCurve = new Curve(CurveType.RampUp);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Fixture strobe function name.
		/// </summary>
		[DataMember]
		public string FixtureStrobeFunctionName { get; set; }

		/// <summary>
		/// Fixture strobe index name.
		/// </summary>
		[DataMember]
		public string FixtureStrobeIndexValue { get; set; }

		/// <summary>
		/// Curve for the fixture strobe.
		/// </summary>
		[DataMember]
		public Curve FixtureStrobeCurve { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the fixture strobe data.
		/// </summary>
		/// <returns>Clone of the fixture strobe data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new fixture strobe data instance
			FixtureStrobeData result = new FixtureStrobeData
			{
				// Copy the fixture strobe function name
				FixtureStrobeFunctionName = FixtureStrobeFunctionName,
				
				// Copy the fixture strobe index value
				FixtureStrobeIndexValue = FixtureStrobeIndexValue,

				// Copy the curve
				FixtureStrobeCurve = new Curve(FixtureStrobeCurve),				
			};

			return result;
		}

		#endregion
	}
}