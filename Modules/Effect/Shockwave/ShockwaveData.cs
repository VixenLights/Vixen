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
			WidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 2.0, 4.0 }));
			RadiusCurve = new Curve(CurveType.RampUp);
			CenterXCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CenterYCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AccelerationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 55.0, 55.0 }));
			BlendEdges = true;
			Orientation=StringOrientation.Vertical;
			ScaledRadius = true;
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
		public Curve RadiusCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int EndRadius { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int StartWidth { get; set; }

		[DataMember]
		public Curve WidthCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int EndWidth { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Acceleration { get; set; }

		[DataMember]
		public Curve AccelerationCurve { get; set; }

		[DataMember]
		public bool BlendEdges { get; set; }

		[DataMember]
		public bool ScaledRadius { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (WidthCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(StartWidth, 255, 0);
				double endValue = PixelEffectBase.ScaleValueToCurve(EndWidth, 255, 0);
				WidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, endValue }));
				StartWidth = 0;
				EndWidth = 0;

				if (CenterYCurve == null)
				{
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						CenterY = 100 - CenterY;
					}
					else if(Orientation == StringOrientation.Vertical)
					{
						//fix the backwards x y adjustment when strings are vertical
						var tempX = CenterX;
						CenterX = CenterY;
						CenterY = tempX;
					}
					CenterYCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new double[] { CenterY, CenterY }));
					CenterY = 0;
				}

				if (CenterXCurve == null)
				{
					CenterXCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new double[] { CenterX, CenterX }));
					CenterX = 0;
				}

				if (AccelerationCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Acceleration, 10, -10);
					AccelerationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Acceleration = 0;
				}
				if (RadiusCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(StartRadius, 750, 0);
					endValue = PixelEffectBase.ScaleValueToCurve(EndRadius, 750, 0);
					RadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, endValue }));
					EndRadius = 0;
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
				WidthCurve = new Curve(WidthCurve),
				RadiusCurve = new Curve(RadiusCurve),
				CenterXCurve = new Curve(CenterXCurve),
				CenterYCurve = new Curve(CenterYCurve),
				AccelerationCurve = new Curve(AccelerationCurve),
				BlendEdges = BlendEdges
			};
			return result;
		}
	}
}
