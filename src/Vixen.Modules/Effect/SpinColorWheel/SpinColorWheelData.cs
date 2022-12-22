using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>	
	/// Maintains the spin color wheel effect data.
	/// </summary>	
	[DataContract]
	public class SpinColorWheelData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SpinColorWheelData()
		{
			// Default the color wheel rotation curve to ramp up
			SpinColorWheelCurve = new Curve(CurveType.RampUp);

			// Default the intensity to full
			Intensity = new Curve(CurveType.Flat100);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Selected spin color wheel function name.
		/// </summary>
		[DataMember]
		public string SpinColorWheelFunctionName { get; set; }

		/// <summary>
		/// Selected Spin Color Wheel index value.
		/// </summary>
		[DataMember]
		public string SpinColorWheelIndexValue { get; set; }

		/// <summary>
		/// Curve for the position or rotation of the color wheel.
		/// </summary>
		[DataMember]
		public Curve SpinColorWheelCurve { get; set; }

		/// <summary>
		/// Curve for the intensity of the light.
		/// </summary>
		[DataMember]
		public Curve Intensity { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the spin color wheel data.
		/// </summary>
		/// <returns>Clone of the spin color wheel data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new spin color wheel data instance
			SpinColorWheelData result = new SpinColorWheelData
			{
				// Copy the spin color wheel function name
				SpinColorWheelFunctionName = SpinColorWheelFunctionName,
				
				// Copy the spin color wheel index value
				SpinColorWheelIndexValue = SpinColorWheelIndexValue,

				// Copy the spin color wheel curve
				SpinColorWheelCurve = new Curve(SpinColorWheelCurve),				

				// Copy the light intensity
				Intensity = new Curve(Intensity),
			};

			return result;
		}

		#endregion
	}
}