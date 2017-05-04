using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Shockwave
{
	[DataContract]
	public class ShockwaveData: EffectTypeModuleData
	{

		public ShockwaveData()
		{
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red,0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, .5));
			Gradient.Colors.Add(new ColorPoint(Color.Blue, 1.0));
			StartWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 2.0, 2.0 }));
			EndWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 4.0, 4.0 }));
			StartRadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.2, 0.2 }));
			EndRadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.2, 1.2 }));
			CenterXCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CenterYCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AccelerationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			BlendEdges = true;
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int CenterX { get; set; }

		[DataMember]
		public Curve CenterXCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int CenterY { get; set; }

		[DataMember]
		public Curve CenterYCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int StartRadius { get; set; }

		[DataMember]
		public Curve StartRadiusCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int EndRadius { get; set; }

		[DataMember]
		public Curve EndRadiusCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int StartWidth { get; set; }

		[DataMember]
		public Curve StartWidthCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int EndWidth { get; set; }

		[DataMember]
		public Curve EndWidthCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Acceleration { get; set; }

		[DataMember]
		public Curve AccelerationCurve { get; set; }

		[DataMember]
		public bool BlendEdges { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (StartWidthCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(StartWidth, 255, 0);
				StartWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				StartWidth = 0;

				if (EndWidthCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(EndWidth, 255, 0);
					EndWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					EndWidth = 0;
				}
				if (CenterXCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(CenterX, 100, 0);
					CenterXCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					CenterX = 0;
				}
				if (CenterYCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(CenterY, 100, 0);
					CenterYCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					CenterY = 0;
				}
				if (AccelerationCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Acceleration, 10, -10);
					AccelerationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Acceleration = 0;
				}
				if (EndRadiusCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(EndRadius, 750, 0);
					EndRadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					EndRadius = 0;
				}
				if (StartRadiusCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(StartRadius, 750, 0);
					StartRadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					StartRadius = 0;
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ShockwaveData result = new ShockwaveData
			{
				
				Orientation = Orientation,
				Gradient = new ColorGradient(Gradient),
				StartWidthCurve = new Curve(StartWidthCurve),
				EndWidthCurve = new Curve(EndWidthCurve),
				StartRadiusCurve = new Curve(StartRadiusCurve),
				EndRadiusCurve = new Curve(EndRadiusCurve),
				CenterXCurve = new Curve(CenterXCurve),
				CenterYCurve = new Curve(CenterYCurve),
				AccelerationCurve = new Curve(AccelerationCurve),
				BlendEdges = BlendEdges
			};
			return result;
		}
	}
}
