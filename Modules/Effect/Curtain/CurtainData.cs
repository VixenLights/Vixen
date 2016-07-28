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
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red, 0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, 1.0));
			Direction = CurtainDirection.CurtainOpen;
			Speed = 1;
			Edge = CurtainEdge.Center;
			Swag = 1;
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

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
				Gradient = new ColorGradient(Gradient),
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
