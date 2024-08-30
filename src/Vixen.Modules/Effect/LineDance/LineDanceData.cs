using System.Runtime.Serialization;

using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

using ZedGraph;

namespace VixenModules.Effect.LineDance
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
		
			// Default the pan increment to 80%
			PanIncrement = 80;

			// Default the speed to 40%
			PanSpeed = 40;

			// Default the hold time to 10%
			HoldTime = 10;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public LineDanceModes Mode { get; set; }

		[DataMember]
		public FanModes FanMode { get; set; }

		[DataMember]
		public FanDirections FanDirection { get; set; }

		[DataMember]
		public bool InvertPan { get; set; }

		[DataMember]
		public bool AdvancedOverrides { get; set; }

		[DataMember]
		public Curve IncrementAngle { get; set; }
		
		[DataMember]
		public int PanStartAngle { get; set; }
		
		[DataMember]
		public int PanIncrement { get; set; }

		[DataMember]
		public int PanSpeed { get; set; }

		[DataMember]
		public FanCenterOptions CenterHandling { get; set; }

		[DataMember]
		public int HoldTime { get; set; }

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
				FanMode = FanMode,	
				FanDirection = FanDirection,
				IncrementAngle = new Curve(IncrementAngle),
				PanStartAngle = PanStartAngle,
				CenterHandling = CenterHandling,
				AdvancedOverrides = AdvancedOverrides,	
				PanIncrement = PanIncrement,
				PanSpeed = PanSpeed,
				InvertPan = InvertPan,	
				HoldTime = HoldTime,
			};
			return result;
		}

		#endregion
	}
}
