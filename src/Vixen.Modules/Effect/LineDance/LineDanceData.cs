using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Fan
{
	/// <summary>	
	/// Maintains the Line Dance effect data.
	/// </summary>	
	[DataContract]
	public class LineDanceData: EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LineDanceData()
		{
			// Default the middle beam to centered
			CenterHandling = FanCenterOptions.Centered;

			// Default the Increment Angle to start at 20% and increase to 80%
			IncrementAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 80.0 }));
			
			// Set the Pan Start Angle to -1 to cause the effect to calculate a default value
			PanStartAngle = -1;			
		}

		#endregion

		#region Public Properties

		[DataMember]
		public LineDanceModes Mode { get; set; }

		[DataMember]
		public bool AdvancedOverrides { get; set; }

		[DataMember]
		public Curve IncrementAngle { get; set; }
		
		[DataMember]
		public int PanStartAngle { get; set; }
		
		[DataMember]
		public FanCenterOptions CenterHandling { get; set; }
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the fan data.
		/// </summary>
		/// <returns>Clone of the fan data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			LineDanceData result = new LineDanceData
			{
				Mode = Mode,
				IncrementAngle = new Curve(IncrementAngle),
				PanStartAngle = PanStartAngle,
				CenterHandling = CenterHandling,
				AdvancedOverrides = AdvancedOverrides,	

			};
			return result;
		}

		#endregion
	}
}
