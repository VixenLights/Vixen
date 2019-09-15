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
			PositionCurve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 100.0, 0.0, 100.0 }));
			MovementType = MovementType.Position;
			IntensityPerIteration = false;
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

		[DataMember]
		public bool IntensityPerIteration { get; set; }
		
		[DataMember]
		public MovementType MovementType { get; set; }

		[DataMember]
		public Curve PositionCurve { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (PositionCurve == null)
			{
				PositionCurve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 100.0, 0.0, 100.0 }));
			}
		}

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
				LevelCurve = new Curve(LevelCurve),
				PositionCurve = new Curve(PositionCurve),
				MovementType = MovementType,
				IntensityPerIteration = IntensityPerIteration
			};
			return result;
		}
	}
}
