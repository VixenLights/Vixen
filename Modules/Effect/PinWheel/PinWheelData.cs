using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.PinWheel
{
	[DataContract]
	public class PinWheelData: EffectTypeModuleData
	{

		public PinWheelData()
		{
			Colors = new List<GradientLevelPair> { new GradientLevelPair(Color.Red, CurveType.Flat100), new GradientLevelPair(Color.Lime, CurveType.Flat100), new GradientLevelPair(Color.Blue, CurveType.Flat100) };
			ColorType = PinWheelColorType.Standard;
			Speed = 1;
			Arms = 8;
			Twist = 60;
			Rotation = RotationType.Forward;
			Size = 90;
			Thickness = 15;
			XOffset = 0;
			YOffset = 0;
			CenterStart = 0;
			PinWheel3D = false;
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public PinWheelColorType ColorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int XOffset { get; set; }

		[DataMember]
		public int YOffset { get; set; }

		[DataMember]
		public int CenterStart { get; set; }

		[DataMember]
		public int Twist { get; set; }

		[DataMember]
		public int Arms { get; set; }

		[DataMember]
		public int Thickness { get; set; }

		[DataMember]
		public int Size { get; set; }

		[DataMember]
		public RotationType Rotation { get; set; }

		[DataMember]
		public bool PinWheel3D { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var gradientLevelList = Colors.Select(glp => new GradientLevelPair(new ColorGradient(glp.ColorGradient), new Curve(glp.Curve))).ToList();
			PinWheelData result = new PinWheelData
			{
				Colors = gradientLevelList,
				ColorType = ColorType,
				Speed = Speed,
				PinWheel3D = PinWheel3D,
				Orientation = Orientation,
				Arms = Arms,
				YOffset = YOffset,
				XOffset = XOffset,
				CenterStart = CenterStart,
				Twist = Twist,
				Thickness = Thickness,
				Rotation = Rotation,
				Size = Size,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
