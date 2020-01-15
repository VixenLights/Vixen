using System;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using ZedGraph;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Serialized data for a waveform.
	/// </summary>
	[DataContract]
	class WaveformData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WaveformData()
		{
			WaveType = WaveType.Sine;
			Speed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			Thickness = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			Frequency = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 26.0, 26.0 }));
			Height = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffset = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Direction = DirectionType.LeftToRight;
			Color = new ColorGradient(System.Drawing.Color.Blue);
			PhaseShift = 0;			
			PrimeWave = false;			
			MovementType = WaveMovementType.Continuous;
			WindowPercentage = 25;
			MarkCollectionId = Guid.Empty;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the waveform data.
		/// </summary>		
		public WaveformData CreateInstanceForClone()
		{
			WaveformData result = new WaveformData
			{
				WaveType = WaveType.Sine,
				UseMarks = UseMarks,
				Speed = new Curve(Speed),
				Thickness = new Curve(Thickness),
				Frequency = new Curve(Frequency),
				Height = new Curve(Height),
				YOffset = new Curve(YOffset),
				Direction = Direction,
				Color = new ColorGradient(Color),
				ColorHandling = ColorHandling,
				PhaseShift = PhaseShift,				
				PrimeWave = PrimeWave,				
				MovementType = MovementType,
				WindowPercentage = WindowPercentage,
				MarkCollectionId = MarkCollectionId,
			};

			return result;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public Curve Speed { get; set; }

		[DataMember]
		public Curve Thickness { get; set; }

		[DataMember]
		public WaveType WaveType { get; set; }

		[DataMember]
		public bool UseMarks { get; set; }

		[DataMember]
		public Curve Frequency { get; set; }

		[DataMember]
		public Curve Height { get; set; }

		[DataMember]
		public bool Mirror { get; set; }

		[DataMember]
		public Curve YOffset { get; set; }

		[DataMember]
		public DirectionType Direction { get; set; }

		[DataMember]
		public ColorGradient Color { get; set; }

		[DataMember]
		public WaveColorHandling ColorHandling { get; set; }

		[DataMember]
		public bool PrimeWave { get; set; }
		
		[DataMember]
		public WaveMovementType MovementType { get; set; }

		[DataMember]
		public int WindowPercentage { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		[DataMember]
		public int PhaseShift { get; set; }

		#endregion
	}
}
