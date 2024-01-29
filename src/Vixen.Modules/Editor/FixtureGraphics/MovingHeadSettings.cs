using System.Drawing;

namespace VixenModules.Editor.FixtureGraphics
{
	/// <summary>	
	/// Maintains the settings of a intelligent fixture moving head.
	/// These settings are used to determine how to draw the moving head.
	/// </summary>
	public class MovingHeadSettings : IMovingHead
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MovingHeadSettings()
		{
			BeamLength = 1;
			OnOff = true;
			BeamColorLeft = Color.Yellow;
			BeamColorRight = Color.Yellow;
			Intensity = 100;
			Focus = 100;
			Legend = string.Empty;
			IncludeLegend = true;
			OptimizeOpenGLVertexData = true;
			FixtureIntensity = 255;

			// Default the legend color to red
			LegendColor = Color.Red;

			// Default to mounting on the bottom
			MountingPosition = MountingPositionType.Bottom;
		}

		#endregion

		#region IMovingHead

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public double PanAngle { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public double TiltAngle { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public double BeamLength { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool OnOff { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Color BeamColorLeft { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Color BeamColorRight { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public int Intensity { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public int Focus { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string Legend { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool IncludeLegend { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Color LegendColor { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool EnableGDI { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool OptimizeOpenGLVertexData { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public int FixtureIntensity { get; set; }

		/// <inheritdoc/>
		public int StrobeRate { get; set; }
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		/// <returns>Clone of the moving head settings.</returns>
		public IMovingHead Clone()
		{
			// Return a clone of all the settings
			return new MovingHeadSettings()
			{
				PanAngle = PanAngle,
				TiltAngle = TiltAngle,
				BeamLength = BeamLength,
				OnOff = OnOff,
				BeamColorLeft = BeamColorLeft,
				BeamColorRight = BeamColorRight,
				Intensity = Intensity,
				Focus = Focus,
				Legend = Legend,
				IncludeLegend = IncludeLegend,
				LegendColor = LegendColor,
				EnableGDI = EnableGDI,
				FixtureIntensity = FixtureIntensity,
				MountingPosition = MountingPosition,
				StrobeRate = StrobeRate,
			};
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public MountingPositionType MountingPosition { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public double GetOrientationSign()
		{
			// Default to a positive sign
			double orientationSign = 1.0;

			// If the fixture is mounted on the top (upside down) then...
			if (MountingPosition == MountingPositionType.Top)
			{
				// Use a negative sign
				orientationSign = -1;
			}

			return orientationSign;	
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to MSDN documentation.
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if the objects are equal</returns>
		public override bool Equals(object obj)
		{
			// Comparing all the individual settings to determine equality
			IMovingHead movingHead = (IMovingHead)obj;
			return (movingHead.PanAngle == PanAngle &&
				   movingHead.TiltAngle == TiltAngle &&
				   movingHead.BeamLength == BeamLength &&
				   movingHead.OnOff == OnOff &&
				   movingHead.BeamColorLeft == BeamColorLeft &&
				   movingHead.BeamColorRight == BeamColorRight &&
				   movingHead.Intensity == Intensity &&
				   movingHead.Focus == Focus &&
				   movingHead.Legend == Legend &&
				   movingHead.IncludeLegend == IncludeLegend &&
				   movingHead.LegendColor == LegendColor &&
				   movingHead.FixtureIntensity == FixtureIntensity &&
				   movingHead.MountingPosition == MountingPosition &&
				   movingHead.StrobeRate == StrobeRate);
		}

		#endregion
	}
}
