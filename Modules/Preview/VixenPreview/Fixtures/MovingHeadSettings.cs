﻿using System.Drawing;

namespace VixenModules.Preview.VixenPreview.Fixtures
{
	/// <summary>
	/// This class exists for testing and preliminary integration into Vixen.
	/// This class allows for a fake effect to change settings on the moving head.
	/// Once the back end is in place this class will be removed and the IMovingHead interface will
	/// be implemented by the <c>MovingHeadOpenGL</c> and MovingHeadWPF classes.
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
			BeamColor = Color.Yellow;
			Intensity = 100;
			Focus = 100;
			Legend = "G:0 P:O";
			IncludeLegend = true;
			OptimizeOpenGLVertexData = true;

			// Default the legend color to red
			LegendColor = Color.Red;
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
		public Color BeamColor { get; set; }

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
		/// <returns></returns>
		public IMovingHead Clone()
		{
			// Return a clone of all the settings
			return new MovingHeadSettings()
			{
				PanAngle = PanAngle,
				TiltAngle = TiltAngle,
				BeamLength = BeamLength,
				OnOff = OnOff,
				BeamColor = BeamColor,
				Intensity = Intensity,
				Focus = Focus,
				Legend = Legend,
				IncludeLegend = IncludeLegend,
				LegendColor = LegendColor,
				EnableGDI = EnableGDI,
			};
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
				   movingHead.BeamColor == BeamColor &&
				   movingHead.Intensity == Intensity &&
				   movingHead.Focus == Focus &&
				   movingHead.Legend == Legend &&
				   movingHead.IncludeLegend == IncludeLegend &&
				   movingHead.LegendColor == LegendColor);
		}

		#endregion
	}
}