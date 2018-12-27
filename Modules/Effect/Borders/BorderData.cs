using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
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
				XOffsetCurve = new Curve(XOffsetCurve)
			};
			return result;
		}
	}
}
