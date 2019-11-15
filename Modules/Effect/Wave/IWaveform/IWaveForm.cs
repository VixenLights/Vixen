using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Defines the supported wave types.
	/// </summary>
	public enum WaveType
	{
		[Description("Sine")]
		Sine,

		[Description("Triangle")]
		Triangle,

		[Description("Square")]
		Square,

		[Description("Decaying Sine")]
		DecayingSine,

		[Description("Fractal Ivey")]
		FractalIvey,

		[Description("Sawtooth")]
		Sawtooth,
	};

	/// <summary>
	/// Defines the direction of the wave.
	/// </summary>
	public enum DirectionType
	{
		[Description("Left To Right")]
		LeftToRight,

		[Description("Right To Left")]
		RightToLeft,

		[Description("Top To Bottom")]
		TopToBottom,

		[Description("Bottom To Top")]
		BottomToTop,
	};

	/// <summary>
	/// Defines the movement type of the wave.
	/// </summary>
	public enum WaveMovementType
	{
		[Description("Continuous")]
		Continuous,

		[Description("Snake")]
		Snake,

		[Description("Grow and Shrink")]
		GrowAndShrink,
	}

	/// <summary>
	/// Defines the color handling of the wave.
	/// </summary>
	public enum WaveColorHandling
	{
		[Description("Along Wave")]
		Along,

		[Description("Across Wave")]
		Across,		
	}

	/// <summary>
	/// Maintains the properties of a waveform.
	/// </summary>
	public interface IWaveform : ICloneable, IMarkCollectionExpandoObject
	{
		/// <summary>
		/// Speed of the waveform.
		/// </summary>
		Curve Speed { get; set; }

		/// <summary>
		/// Thickness of the waveform.
		/// </summary>
		Curve Thickness { get; set; }

		/// <summary>
		/// Type of wave.
		/// </summary>
		WaveType WaveType { get; set; }

		/// <summary>
		/// Frequency of the wave.
		/// </summary>
		Curve Frequency { get; set; }

		/// <summary>
		/// Height of the wave.
		/// </summary>
		Curve Height { get; set; }

		/// <summary>
		/// Determine whether the wave is mirrored along the Y-axis.
		/// </summary>
		bool Mirror { get; set; }

		/// <summary>
		/// Y offset of the waveform.  Shifts the waveform up and down along the y axis.
		/// </summary>
		Curve YOffset { get; set; }

		/// <summary>
		/// Shifts the waveform along the x axis.
		/// </summary>
		int PhaseShift { get; set; }

		/// <summary>
		/// Direction of movement of the waveform.
		/// </summary>
		DirectionType Direction { get; set; }

		/// <summary>
		/// Color of the waveform.
		/// </summary>
		ColorGradient Color { get; set; }

		/// <summary>
		/// How the color gradient is applied to the wave.
		/// </summary>
		WaveColorHandling ColorHandling { get; set; }

		/// <summary>
		/// Stores the Y positions of the pixels on the waveform.
		/// </summary>
		Queue<List<Tuple<Color, int>>> Pixels { get; }

		/// <summary>
		/// When true the waveform is primed to cover the entire display element.
		/// </summary>
		bool PrimeWave { get; set; }

		/// <summary>
		/// Controls the movement of the waveform. 
		/// </summary>
		WaveMovementType MovementType { get; set; }

		/// <summary>
		/// Defines the percentage of the display area that is used for the snake mode window.
		/// </summary>
		int WindowPercentage { get; set; }

		// The following properties support rendering but are not persisted.
		
		/// <summary>
		/// Defines the height of the display element.
		/// This property allows for each wave to be oriented independently.
		/// </summary>
		int BufferHt { get; set; }
		
		/// <summary>
		/// Defines the width of the display element.
		/// This property allows for each wave to be oriented independently.
		/// </summary>
		int BufferWi { get; set; }

		/// <summary>
		/// Stores Y values to support the Ivey wave type.
		/// </summary>
		List<int> IveyYValues { get; }

		/// <summary>
		/// Variable used to support the decaying sine wave type.
		/// </summary>
		int DecayFactor { get; set; }

		/// <summary>
		/// Keeps track of the X position on the sawtooth slanted line.
		/// </summary>
		int SawToothX { get; set; }

		/// <summary>
		/// Keeps track of the maximum SawTooth Y value.
		/// </summary>
		int SawToothMaxY { get; set; }

		/// <summary>
		/// Keeps track of the minimum SawTooth Y value.
		/// </summary>
		int SawToothMinY { get; set; }

		/// <summary>
		/// Previous Sawtooth Y position.
		/// </summary>
		int LastSawToothY { get; set; }

		/// <summary>
		/// Previous, previous Sawtooth Y position.
		/// </summary>
		int LastLastSawToothY { get; set; }

		/// <summary>
		/// Current degree position on the waveform.
		/// </summary>
		double Degrees { get; set; }

		/// <summary>
		/// Flag to control whether the wave is shrinking or expanding.
		/// </summary>
		bool Shrink { get; set; }

		/// <summary>
		/// Start position of the view window when the wave is in snake mode.
		/// </summary>
		int WindowStart { get; set; }

		/// <summary>
		/// End position of the view window when the wave is in snake mode.
		/// </summary>
		int WindowStop { get; set; }

		/// <summary>
		/// Current x position of the triangle waveform.
		/// </summary>
		int TriangleCounter { get; set; }

		/// <summary>
		/// Centered Y position of the wave.		
		/// </summary>
		double YC { get; }

		/// <summary>
		/// Gets the degrees per x coordinate.
		/// </summary>		
		double GetDegreesPerX(int numberOfWaves);		

		/// <summary>
		/// Thickness of the wave's previous column.		
		/// </summary>
		/// <remarks>Used to determine how thick to make square and sawtooth vertical lines</remarks>
		int LastWaveThickness { get; set; }
		
		/// <summary>
		/// Determines if a mark collection is used to determine when the decaying sine bounces back to full amplitude.
		/// </summary>
		bool UseMarks { get; set; }

		/// <summary>
		/// Name of the selected mark collection.
		/// </summary>
		string MarkCollectionName { get; set; }

		/// <summary>
		/// Guid of the selected mark collection.
		/// </summary>
		Guid MarkCollectionId { get; set; }				
	}
}
