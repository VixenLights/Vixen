using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Gobo
{
	/// <summary>	
	/// Maintains the gobo effect data.
	/// </summary>	
	[DataContract]
	public class GoboData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public GoboData()
		{
			// Default the Gobo rotation curve to ramp up
			GoboCurve = new Curve(CurveType.RampUp);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gobo function name.
		/// </summary>
		[DataMember]
		public string GoboFunctionName { get; set; }

		/// <summary>
		/// Selected Gobo name.
		/// </summary>
		[DataMember]
		public string GoboIndexValue { get; set; }

		/// <summary>
		/// Curve for rotating the Gobowheel at a certain speed.
		/// </summary>
		[DataMember]
		public Curve GoboCurve { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the gobo data.
		/// </summary>
		/// <returns>Clone of the gobo data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new gobo data instance
			GoboData result = new GoboData
			{
				// Copy the Gobo function name
				GoboFunctionName = GoboFunctionName,	

				// Copy the selected gobo value
				GoboIndexValue = GoboIndexValue,		
				
				// Curve for rotating the gobo at certain speed
				GoboCurve = new Curve(GoboCurve),
			};

			return result;
		}

		#endregion
	}
}