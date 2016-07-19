using System;
using System.CodeDom;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Glediator
{
	[DataContract]
	public class GlediatorData : EffectTypeModuleData
	{
		public GlediatorData()
		{
			Speed = 2;
			FileName = String.Empty;
			MovementType = MovementType.Iterations;
			Iterations = 1;
			Orientation=StringOrientation.Vertical;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Iterations { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public MovementType MovementType { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			GlediatorData result = new GlediatorData
			{
				Speed = Speed,
				Iterations = Iterations,
				Orientation = Orientation,
				MovementType = MovementType,
				LevelCurve = new Curve(LevelCurve),
				FileName = FileName
			};
			return result;
		}
	}
}
