using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Serialized data for a Morph polygon.
	/// </summary>
	[DataContract]
	class MorphPolygonData
	{
		#region Public Properties

		[DataMember]
		public PolygonFillType FillType { get; set; }

		[DataMember]
		public int HeadLength { get; set; }

		[DataMember]
		public int HeadDuration { get; set; }

		[DataMember]
		public int Acceleration { get; set; }

		[DataMember]
		public ColorGradient HeadColor { get; set; }

		[DataMember]
		public ColorGradient TailColor { get; set; }

		[DataMember]
		public ColorGradient FillColor { get; set; }

		[DataMember]
		public double Time { get; set; }

		[DataMember]
		public Polygon Polygon { get; set; }

		[DataMember]
		public Line Line { get; set; }

		[DataMember]
		public Ellipse Ellipse { get; set; }

		[DataMember]
		public string Label { get; set; }

		[DataMember]
		public int StartOffset { get; set; }

		[DataMember]
		public Curve TailBrightness { get; set; }

		[DataMember]
		public Curve HeadBrightness { get; set; }

		[DataMember]
		public Curve FillBrightness { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the Morph Polygon data.
		/// </summary>		
		public MorphPolygonData CreateInstanceForClone()
		{
			MorphPolygonData result = new MorphPolygonData
			{
				FillType = FillType,
				HeadLength = HeadLength,
				HeadDuration = HeadDuration,
				Acceleration = Acceleration,
				HeadColor = new ColorGradient(HeadColor),
				TailColor = new ColorGradient(TailColor),
				FillColor = new ColorGradient(FillColor),
				Time = Time,								
				Label = Label,
				StartOffset = StartOffset,
				TailBrightness = new Curve(TailBrightness),
				HeadBrightness = new Curve(HeadBrightness),
				FillBrightness =  new Curve(FillBrightness),
				Polygon = Polygon?.Clone(),
				Line = Line?.Clone(),
				Ellipse = Ellipse?.Clone()
			};

			return result;
		}

		#endregion
	}
}
