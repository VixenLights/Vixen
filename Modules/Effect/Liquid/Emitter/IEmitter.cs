using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vixen.Marks;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Liquid
{
	public enum ParticleType
	{
		Elastic,
		Powder,
		Tensile,
		Spring,
		Viscous,
		[Description("Static Pressure")]
		StaticPressure,
		Water,
		Reactive,
		Repulsive,
	}

	public enum EdgeHandling
	{
		[Description("Bounce")]
		Bounce,
		[Description("Wrap")]
		Wrap
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FlowControl
	{
		[Description("Continuous")]
		Continuous,
		[Description("Pulsating")]
		Pulsating,
		[Description("Use Marks")]
		UseMarks,
		[Description("Musical")]
		Musical,
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum NozzleMovement
	{
		[Description("Fixed Angle")]
		FixedAngle,
		[Description("Oscillate")]
		Oscillate,
		[Description("Spin Clockwise")]
		SpinClockwise,
		[Description("Spin Counter Clockwise")]
		SpinCounterClockwise
	}

    /// <summary>
    /// Maintains the properties and methods of an emitter.
    /// </summary>
	public interface IEmitter: ICloneable
	{
		/// <summary>
      /// Parent effect of the emitter.
      /// This property is used to register for mark events.
      /// </summary>
		BaseEffect Parent { get; set; }
		
      /// <summary>
      /// Returns a clone of the emitter.
      /// </summary>        
		IEmitter CreateInstanceForClone();

		/// <summary>
		/// Initializes the browseable state of all the emitter properties.
		/// </summary>
		void InitAllAttributes();

		/// <summary>
		/// Updates the selected mark name collection for the emitter.
		/// This method handles when a IMarkCollection is renamed.
		/// </summary>
		void UpdateSelectedMarkCollectionName();

		/// <summary>
		/// Type of particles emitted by the emitter.
		/// </summary>
		ParticleType ParticleType { get; set; }

		/// <summary>
		/// Controls whether the color array is used to color the emitter particles.
		/// </summary>
		bool UseColorArray { get; set; }

		/// <summary>
		/// Controls how many frames of each color in the color array are emitted.
		/// </summary>
		int FramesPerColor { get; set; }

		/// <summary>
		/// Color of the emitter.
		/// </summary>
		ColorGradient Color { get; set; }

		/// <summary>
		/// Brightness of the emitter color.
		/// </summary>
		Curve Brightness { get; set; }

		/// <summary>
		/// Lifetime of the emitter particles.
		/// </summary>
		Curve Lifetime { get; set; }

		/// <summary>
		/// Velocity of the particles.
		/// </summary>
		Curve ParticleVelocity { get; set; }
        
      /// <summary>
      /// Whether the emitter is animated.
      /// </summary>
		bool Animate { get; set; }

		/// <summary>
		/// Whether the starting position of an animated emitter is random.
		/// </summary>
		bool RandomStartingPosition { get; set; }

		/// <summary>
		/// Starting X position of the emitter.  Only applicable to animated emitters.
		/// </summary>
		int AnimateXStart { get; set; }

		/// <summary>
		/// Starting Y position of the emitter.  Only applicable to animated emitters.
		/// </summary>
		int AnimateYStart { get; set; }

		/// <summary>
		/// How the emitter behaves when animated and it reachs the edge of the element.
		/// </summary>
		EdgeHandling EdgeHandling { get; set; }

      /// <summary>
      /// Velocity of the emitter in the X direction.
      /// </summary>
		Curve VelocityX { get; set; }

      /// <summary>
      /// Velocity of the emittter in the Y direction.
      /// </summary>
		Curve VelocityY { get; set; }

      /// <summary>
      /// Manual position of the emitter in the X direction.
      /// </summary>
		Curve X { get; set; }

      /// <summary>
      /// Manual position of the emitter in the Y direction.
      /// </summary>
		Curve Y { get; set; }
      
      /// <summary>
      /// Manual flow of the emitter.
      /// </summary>
		Curve Flow { get; set; }

      /// <summary>
      /// Source size of the emitter.
      /// </summary>
		Curve SourceSize { get; set; }

      /// <summary>
      /// Controls how the nozzle (angle) is positioned.
      /// </summary>
		NozzleMovement NozzleMovement { get; set; }

      /// <summary>
      /// Angle of the emitter's nozzle.
      /// </summary>
		Curve NozzleAngle { get; set; }
      	          		
      /// <summary>
      /// Start angle in degrees of the emitter oscillation.
      /// </summary>
		int OscillateStartAngle { get; set; }

      /// <summary>
      /// End angle in degrees of the emitter oscillation.
      /// </summary>
		int OscillateEndAngle { get; set; }

      /// <summary>
      /// Speed of rotation of the emitter nozzle when rotating.
      /// </summary>
		Curve OscillationSpeed { get; set; }

      /// <summary>
      /// Determines how the flow is controlled for the emitter (Continuous, Pulsating, Use Marks).
      /// </summary>
		FlowControl FlowControl { get; set; }
                 
      /// <summary>
      /// Name of the selected mark collection.
      /// </summary>
		string MarkCollectionName { get; set; }

      /// <summary>
      /// Guid of the selected mark collection.
      /// </summary>
		Guid MarkCollectionId { get; set; }

      /// <summary>
      /// Collection of the mark collection names.
      /// </summary>
		ObservableCollection<string> MarkNameCollection { get; set; }

      /// <summary>
      /// Collection of the mark collections.
      /// </summary>
		ObservableCollection<IMarkCollection> MarkCollections { get; set; }

		// The following properties are not persisted but are used during rendering.

		/// <summary>
		/// On Time for the emitter in seconds.
		/// </summary>
		int OnTime { get; set; }

      /// <summary>
      /// Off time for the emitter in seconds.
      /// </summary>
		int OffTime { get; set; }
		
      /// <summary>
      /// Frame counter for the color array.
      /// This counter determines when it is time to move to the next color in the array.
      /// </summary>
		int FrameColorCounter { get; set; }

      /// <summary>
      /// Current index into the color array.
      /// </summary>
		int ColorArrayIndex { get; set; }
			
      /// <summary>
      /// Current X location of the emitter.
      /// </summary>
		double LocationX { get; set; }

      /// <summary>
      /// Current Y location of the emitter.
      /// </summary>
		double LocationY { get; set; }

      /// <summary>
      /// Velocity of the emitter in the X axis.
      /// </summary>
		double VelX { get; set; }

      /// <summary>
      /// Velocity of the emitter in the Y axis.
      /// </summary>
		double VelY { get; set; }

      /// <summary>
      /// Number frames the emitter has been On.
      /// </summary>
		int OnTimer { get; set; }

      /// <summary>
      /// Number of frames the emitter has been Off.
      /// </summary>
		int OffTimer { get; set; }		
	
      /// <summary>
      /// True when the emitter is On.
      /// </summary>
		bool On { get; set; }

      /// <summary>
      /// Degrees to move when the emitter is rotating.
      /// </summary>
		int AngleOscillationDelta { get; set; }

      /// <summary>
      /// True when the emitter is rotating clockwise.
      /// </summary>
		bool OscillationClockwise { get; set; }

      /// <summary>
      /// Angle of the emitter in integer precision.
      /// </summary>
		int IntAngle { get; set; }
	}
}
