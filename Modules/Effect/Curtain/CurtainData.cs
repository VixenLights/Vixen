using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Curtain
{
	[DataContract]
	public class CurtainData : EffectTypeModuleData
	{

		public CurtainData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime)};
			Direction = CurtainDirection.CurtainOpen;
			Speed = 1;
			Edge = CurtainEdge.Center;
			Swag = 1;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public CurtainDirection Direction { get; set; }

		[DataMember]
		public CurtainEdge Edge { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Swag { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			CurtainData result = new CurtainData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				Edge = Edge,
				Swag = Swag,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
