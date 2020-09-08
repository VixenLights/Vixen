using System;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using ZedGraph;

namespace Liquid
{
	[DataContract]
	public class EmitterData
	{
		#region Public Types 

		public enum ParticleSerializationType
		{
			Elastic,
			Powder,
			Tensile,
			Spring,
			Viscous,
			StaticPressure,
			Water,
			Reactive,
			Repulsive,
		};

		public enum EdgeHandlingSerializationType
		{
			Bounce,
			Wrap
		}

		public enum NozzleMovementSerializationType
		{
			FixedAngle,
			Oscillate,
			SpinClockwise,
			SpinCounterClockwise
		}

		public enum FlowControlSerializationType
		{			
			Continuous,		
			Pulsating,			
			UseMarks,
			Musical
		}

		#endregion

		#region Constructor

		public EmitterData()
		{
			ParticleType = ParticleSerializationType.Water;
			UseColorArray = false;
			FramesPerColor = 3;
			Color = new ColorGradient(System.Drawing.Color.Blue);
			Lifetime = new Curve(CurveType.Flat100);
			ParticleVelocity = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Animate = false;
			RandomStartingPosition = true;
			AnimateXStart = 0;
			AnimateYStart = 0;
			EdgeHandling = EdgeHandlingSerializationType.Bounce;
			VelocityX = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			VelocityY = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			// X and Y are used when the emitter is not animated
			X = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Y = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 90.0, 90.0 }));

			ManualFlow = true;
			Flow = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			SourceSize = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			NozzleMovement = NozzleMovementSerializationType.FixedAngle;
			NozzleAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 75.0, 75.0 }));			
			OscillateStartAngle = 0;
			OscillateEndAngle = 90;
			OscillationSpeed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));

			FlowControl = FlowControlSerializationType.Continuous;			
			MarkCollectionId = Guid.Empty;

			OnTime = 2000;  // Milliseconds
			OffTime = 2000; // Milliseconds	

			// Default the brightness to 100%
			Brightness = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		#endregion

		#region Public Methods

		public EmitterData CreateInstanceForClone()
		{
			EmitterData result = new EmitterData
			{
				ParticleType = ParticleType,
				UseColorArray = UseColorArray,
				FramesPerColor = FramesPerColor,
				Color = new ColorGradient(Color),
				Lifetime = new Curve(Lifetime),
				ParticleVelocity = new Curve(ParticleVelocity),
				Animate = Animate,
				RandomStartingPosition = RandomStartingPosition,
				AnimateXStart = AnimateXStart,
				AnimateYStart = AnimateYStart,
				EdgeHandling = EdgeHandling,
				VelocityX = new Curve(VelocityX),
				VelocityY = new Curve(VelocityY),
				X = new Curve(X),
				Y = new Curve(Y),
				ManualFlow = ManualFlow,
				Flow = new Curve(Flow),
				SourceSize = new Curve(SourceSize),
				NozzleMovement = NozzleMovement,
				NozzleAngle = new Curve(NozzleAngle),				
				OscillateStartAngle = OscillateStartAngle,
				OscillateEndAngle = OscillateEndAngle,
				OscillationSpeed = OscillationSpeed,

				FlowControl = FlowControl,
				MarkCollectionId = MarkCollectionId,
				OnTime = OnTime,
				OffTime = OffTime,
				Brightness = new Curve(Brightness),
			};

			return result;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public ParticleSerializationType ParticleType { get; set; }

		[DataMember]
		public bool UseColorArray { get; set; }

		[DataMember]
		public int FramesPerColor { get; set; }

		[DataMember]
		public ColorGradient Color { get; set; }

		[DataMember]
		public Curve Lifetime { get; set; }

		[DataMember]
		public Curve ParticleVelocity { get; set; }

		[DataMember]
		public bool Animate { get; set; }

		[DataMember]
		public bool RandomStartingPosition { get; set; }
		
		[DataMember]
		public int AnimateXStart { get; set; }

		[DataMember]
		public int AnimateYStart { get; set; }

		[DataMember]
		public EdgeHandlingSerializationType EdgeHandling { get; set; }

		[DataMember]
		public Curve VelocityX { get; set; }

		[DataMember]
		public Curve VelocityY { get; set; }

		[DataMember]
		public Curve X { get; set; }

		[DataMember]
		public Curve Y { get; set; }

		[DataMember]
		public bool ManualFlow { get; set; }

		[DataMember]
		public Curve Flow { get; set; }

		[DataMember]
		public Curve SourceSize { get; set; }

		[DataMember]
		public NozzleMovementSerializationType NozzleMovement { get; set; }

		[DataMember]
		public Curve NozzleAngle { get; set; }
				
		[DataMember]
		public int OscillateStartAngle { get; set; }

		[DataMember]
		public int OscillateEndAngle { get; set; }

		[DataMember]
		public Curve OscillationSpeed { get; set; }

		[DataMember]
		public FlowControlSerializationType FlowControl { get; set; }
								
		[DataMember]
		public Guid MarkCollectionId { get; set; }
				
		[DataMember]
		public int OnTime { get; set; }
		
		[DataMember]
		public int OffTime { get; set; }

		[DataMember]
		public Curve Brightness { get; set; }

		#endregion
	}
}

