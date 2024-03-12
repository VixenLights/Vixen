using System.Runtime.Serialization;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Border;
using VixenModules.Effect.Effect;

using ZedGraph;

namespace VixenModules.Effect.Borders
{
	[DataContract]
	public class BorderData: EffectTypeModuleData
	{

		public BorderData()
		{
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red,0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, 1.0));
			ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			TopThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			BottomThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			LeftThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			RightThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BorderSizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BorderType = BorderType.Single;
			GradientMode = GradientMode.OverTime;
			BorderMode = BorderMode.Simple;
			Orientation=StringOrientation.Vertical;
			SimpleBorderWidth = 1;
			BorderHeightCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));

			// Marquee Specific Data Member Defaults 
			DefaultMarqueeDataMembers();
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember]
		public int SimpleBorderWidth { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public BorderMode BorderMode { get; set; }

		[DataMember]
		public BorderType BorderType { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve BorderSizeCurve { get; set; }

		[DataMember]
		public Curve ThicknessCurve { get; set; }

		[DataMember]
		public Curve TopThicknessCurve { get; set; }

		[DataMember]
		public Curve BottomThicknessCurve { get; set; }

		[DataMember]
		public Curve LeftThicknessCurve { get; set; }

		[DataMember]
		public Curve RightThicknessCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve BorderHeightCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		// Marquee Specific Data Members
		[DataMember]
		public RenderLevel RenderLevel { get; set; }

		[DataMember]
		public int BandSize { get; set; }

		[DataMember]
		public int SkipSize { get; set; }
		
		[DataMember]
		public int Thickness { get; set; }

		[DataMember]
		public int Stagger { get; set; }

		[DataMember]
		public Curve Speed { get; set; }

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public int XScale { get; set; }

		[DataMember]
		public int YScale { get; set; }

		[DataMember]
		public bool Reverse { get; set; }

		[DataMember]
		public bool WrapX { get; set; }

		[DataMember]
		public bool WrapY { get; set; }

		[DataMember]
		public int RenderScaleFactor { get; set; }

		[DataMember]
		public bool UsePercent { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (BorderHeightCurve == null)
			{
				BorderHeightCurve = BorderSizeCurve;

				if (XOffsetCurve == null)
				{
					XOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {50.0, 50.0}));
				}

				if (YOffsetCurve == null)
				{
					YOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {50.0, 50.0}));
				}
			}

			// If the speed is null this effect was created prior to introducing in the Marquee mode
			if (Speed == null)
			{
				DefaultMarqueeDataMembers();
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			BorderData result = new BorderData
			{
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				ThicknessCurve = new Curve(ThicknessCurve),
				TopThicknessCurve = new Curve(TopThicknessCurve),
				BottomThicknessCurve = new Curve(BottomThicknessCurve),
				LeftThicknessCurve = new Curve(LeftThicknessCurve),
				RightThicknessCurve = new Curve(RightThicknessCurve),
				BorderSizeCurve = new Curve(BorderSizeCurve),
				BorderType = BorderType,
				GradientMode = GradientMode,
				BorderMode = BorderMode,
				SimpleBorderWidth = SimpleBorderWidth,
				Gradient = Gradient,
				BorderHeightCurve = new Curve(BorderHeightCurve),
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),

				// Marquee Specific Properties
				RenderLevel = RenderLevel,
				BandSize = BandSize,
				SkipSize = SkipSize,
				Thickness = Thickness,
				Stagger = Stagger,
				Speed = new Curve(Speed),
				Colors = Colors.ToList(),
				XScale = XScale,
				YScale = YScale,
				Reverse = Reverse,
				WrapX = WrapX,
				WrapY = WrapY,
				RenderScaleFactor = RenderScaleFactor,
				UsePercent = UsePercent,
			};

			return result;
		}

		/// <summary>
		/// Defaults the marquee border data members.
		/// </summary>
		private void DefaultMarqueeDataMembers()
		{
			RenderLevel = RenderLevel.Level0;
			BandSize = 3;
			SkipSize = 0;
			Thickness = 1;
			Stagger = 0;
			Speed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			XScale = 100;
			YScale = 100;
			Reverse = false;
			WrapX = false;
			WrapY = false;
			UsePercent = false;

			// Initialize the scale factor to 1/4
			RenderScaleFactor = 4;

			// Default Color Bands for Marquee Border
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.White) };
		}
	}
}
