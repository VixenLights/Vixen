using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SetZoom
{
	/// <summary>	
	/// Maintains the zoom effect data.
	/// </summary>	
	[DataContract]
	public class SetZoomData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SetZoomData()
		{
			// Default the Zoom curve to ramp up
			Zoom = new Curve(CurveType.RampUp);			
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Zoom curve data.
		/// </summary>
		[DataMember]
		public Curve Zoom { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the zoom data.
		/// </summary>
		/// <returns>Clone of the zoom data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a new zoom data instance
			SetZoomData result = new SetZoomData
			{
				// Copy the zoom curve
				Zoom = new Curve(Zoom),				
			};

			return result;
		}

		#endregion
	}
}