using System;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Picture
{
	[DataContract]
	public class PictureData: EffectTypeModuleData
	{
		public PictureData()
		{
			EffectType = EffectType.RenderPictureNone;
			ScalePercent = 100;
			ScaleToGrid = false;
			Speed = 1;
			FileName = String.Empty;
			Orientation=StringOrientation.Vertical;
			XOffset = 0;
			YOffset = 0;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		[DataMember]
		public EffectType EffectType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool FitToTime { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public bool ScaleToGrid { get; set; }

		[DataMember]
		public int ScalePercent { get; set; }

		[DataMember]
		public int XOffset { get; set; }

		[DataMember]
		public int YOffset { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			PictureData result = new PictureData
			{
				EffectType = EffectType,
				FitToTime = FitToTime,
				XOffset = XOffset,
				YOffset = YOffset,
				Speed = Speed,
				ScalePercent = ScalePercent,
				ScaleToGrid = ScaleToGrid,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				FileName = FileName,
			};
			return result;
		}
	}
}
