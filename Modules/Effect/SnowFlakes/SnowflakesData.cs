using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using ZedGraph;

namespace VixenModules.Effect.Snowflakes
{
	[DataContract]
	public class SnowflakesData: ModuleDataModelBase
	{

		public SnowflakesData()
		{
			OuterColor = Color.White;
			CenterColor = Color.Blue;
			SnowflakeType = SnowflakeType.Random;
			Speed = 5;
			FlakeCount = 1;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public Color CenterColor { get; set; }

		[DataMember]
		public Color OuterColor { get; set; }

		[DataMember]
		public SnowflakeType SnowflakeType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int FlakeCount { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		public override IModuleDataModel Clone()
		{
			SnowflakesData result = new SnowflakesData
			{
				
				SnowflakeType = SnowflakeType,
				Speed = Speed,
				FlakeCount = FlakeCount,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				CenterColor = CenterColor,
				OuterColor = OuterColor
				
			};
			return result;
		}
	}
}
