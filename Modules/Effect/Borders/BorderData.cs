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
			BorderWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			BorderType = BorderType.Single;
			GradientMode = GradientMode.OverTime;
			RoundEdges = false;
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public BorderType BorderType { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve BorderWidthCurve { get; set; }

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
		public bool RoundEdges { get; set; }

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
				BorderWidthCurve = new Curve(BorderWidthCurve),
				BorderType = BorderType,
				GradientMode = GradientMode,
				RoundEdges =RoundEdges,
				Gradient = Gradient
			};
			return result;
		}
	}
}
