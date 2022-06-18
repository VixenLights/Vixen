using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Prism
{
	/// <summary>	
	/// Maintains the prism effect data.
	/// </summary>	
	[DataContract]
	public class PrismData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PrismData()
		{
			// Default the prism rotation curve to ramp up
			PrismCurve = new Curve(CurveType.RampUp);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Prism function name.
		/// </summary>
		[DataMember]
		public string PrismFunctionName { get; set; }

		/// <summary>
		/// Prism index name.
		/// </summary>
		[DataMember]
		public string PrismIndexValue { get; set; }

		/// <summary>
		/// Curve for the prism position or rotation.
		/// </summary>
		[DataMember]
		public Curve PrismCurve { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the prism data.
		/// </summary>
		/// <returns>Clone of the prism data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new prism data instance
			PrismData result = new PrismData
			{
				// Copy the prism function name
				PrismFunctionName = PrismFunctionName,
				
				// Copy the prism index value
				PrismIndexValue = PrismIndexValue,

				// Copy the prism curve
				PrismCurve = new Curve(PrismCurve),				
			};

			return result;
		}

		#endregion
	}
}