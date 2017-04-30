using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.PinWheel
{
	[DataContract]
	public class PinWheelData: EffectTypeModuleData
	{

		public PinWheelData()
		{
			Colors = new List<GradientLevelPair> { new GradientLevelPair(Color.Red, CurveType.Flat100), new GradientLevelPair(Color.Lime, CurveType.Flat100), new GradientLevelPair(Color.Blue, CurveType.Flat100) };
			ColorType = PinWheelColorType.Standard;
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.0, 1.0 }));
			Arms = 8;
			TwistCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 56.0, 56.0 }));
			ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 15.0, 15.0 }));
			Rotation = RotationType.Forward;
			SizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 23.0, 23.0 }));
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CenterStart = 0;
			CenterHubCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			PinWheel3D = false;
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
			PinWheelBladeType = PinWheelBladeType.Flat;
		}

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public PinWheelColorType ColorType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int XOffset { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int YOffset { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int CenterStart { get; set; }

		[DataMember]
		public Curve CenterHubCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Twist { get; set; }

		[DataMember]
		public Curve TwistCurve { get; set; }

		[DataMember]
		public int Arms { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Thickness { get; set; }

		[DataMember]
		public Curve ThicknessCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Size { get; set; }

		[DataMember]
		public Curve SizeCurve { get; set; }

		[DataMember]
		public RotationType Rotation { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool PinWheel3D { get; set; } //Deprecated

		[DataMember]
		public PinWheelBladeType PinWheelBladeType { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			if (PinWheel3D)
			{
				PinWheelBladeType = PinWheelBladeType.ThreeD;
				PinWheel3D = false;
			}


			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (TwistCurve == null) 
			{
				double value = PixelEffectBase.ScaleValueToCurve(Twist, 500d, -500d);
				TwistCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				Twist = 0;

				if (ThicknessCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Thickness, 100, 1);
					ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Thickness = 0;
				}
				if (SizeCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Size, 400, 1);
					SizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Size = 0;
				}
				if (SpeedCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Speed, 50, 1);
					SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Speed = 0;
				}
				if (XOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(XOffset, 100, -100);
					XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					XOffset = 0;
				}
				if (YOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(YOffset, 100, -100);
					YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					YOffset = 0;
				}
				if (CenterHubCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(CenterStart, 100, 0);
					CenterHubCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					CenterStart = 0;
				}
			}
			

		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var gradientLevelList = Colors.Select(glp => new GradientLevelPair(new ColorGradient(glp.ColorGradient), new Curve(glp.Curve))).ToList();
			PinWheelData result = new PinWheelData
			{
				Colors = gradientLevelList,
				ColorType = ColorType,
				SpeedCurve = new Curve(SpeedCurve),
				PinWheel3D = PinWheel3D,
				Orientation = Orientation,
				Arms = Arms,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				CenterHubCurve = new Curve(CenterHubCurve),
				TwistCurve = new Curve(TwistCurve),
				ThicknessCurve = new Curve(ThicknessCurve),
				Rotation = Rotation,
				SizeCurve = new Curve(SizeCurve),
				LevelCurve = new Curve(LevelCurve),
				PinWheelBladeType = PinWheelBladeType
			};
			return result;
		}
	}
}
