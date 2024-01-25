using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SetPosition
{
	/// <summary>	
	/// Maintains the Set Position effect data.
	/// </summary>	
	[DataContract]
	public class SetPositionData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SetPositionData()
		{
			Pan = new Curve(CurveType.RampUp);
			Tilt = new Curve(CurveType.RampDown);

			// Default both Pan and Tilt control to enabled
			EnablePan = true;
			EnableTilt = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets pan curve data.
		/// </summary>
		[DataMember]
		public Curve Pan { get; set; }

		/// <summary>
		/// Gets or sets tilt curve data.
		/// </summary>
		[DataMember]
		public Curve Tilt { get; set; }

		/// <summary>
		/// Gets or sets whether the effect is controlling the fixture's pan position.
		/// </summary>
		[DataMember]
		public bool EnablePan { get; set; }

		/// <summary>
		/// Gets or sets whether the effect is controlling the fixture's tilt position.
		/// </summary>
		[DataMember]
		public bool EnableTilt { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the Set Position data.
		/// </summary>
		/// <returns>Clone of the Set Position data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new Set Position data instance
			SetPositionData result = new SetPositionData
			{
				// Copy the curve data
				Pan = new Curve(Pan),
				Tilt = new Curve(Tilt),
				EnablePan = EnablePan,
				EnableTilt = EnableTilt,
			};
			return result;
		}

		#endregion
	}
}