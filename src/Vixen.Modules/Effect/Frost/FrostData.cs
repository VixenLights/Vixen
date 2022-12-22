using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Frost
{
	/// <summary>	
	/// Maintains the frost effect data.
	/// </summary>	
	[DataContract]
	public class FrostData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FrostData()
		{
			// Default the Frost curve to ramp up
			Frost = new Curve(CurveType.RampUp);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Frost curve data.
		/// </summary>
		[DataMember]
		public Curve Frost { get; set; }

		/// <summary>
		/// Frost function name.
		/// </summary>
		[DataMember]
		public string FrostFunctionName { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the frost data.
		/// </summary>
		/// <returns>Clone of the frost data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new frost data instance
			FrostData result = new FrostData
			{
				// Copy the frost curve
				Frost = new Curve(Frost),		

				// Copy the frost function name
				FrostFunctionName = FrostFunctionName,
			};

			return result;
		}

		#endregion
	}
}