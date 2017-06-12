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
				Gradient = Gradient
			};
			return result;
		}
	}
}
