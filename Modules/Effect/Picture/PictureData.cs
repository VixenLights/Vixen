using System;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Picture
{
	[DataContract]
	public class PictureData : EffectTypeModuleData
	{
		public PictureData()
		{
			EffectType = EffectType.RenderPictureNone;
			Speed = 1;
			FileName = String.Empty;
			Orientation=StringOrientation.Vertical;
			XOffset = 0;
			YOffset = 0;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Colors = new ColorGradient(Color.DodgerBlue);
			ColorEffect = ColorEffect.None;
			ScalePercent = 50;
			MovementRate = 4;
			Direction = 0;
			IncreaseBrightness = 10;
			ScaleToGrid = true;
			TilePictures = TilePictures.BlueGlowDots;
			GifSpeed = 1;
		}

		[DataMember]
		public ColorEffect ColorEffect { get; set; }

		[DataMember]
		public EffectType EffectType { get; set; }

		[DataMember]
		public TilePictures TilePictures { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int MovementRate { get; set; }

		[DataMember]
		public int GifSpeed { get; set; }

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

		[DataMember]
		public ColorGradient Colors { get; set; }

		[DataMember]
		public int Direction { get; set; }

		[DataMember]
		public int IncreaseBrightness { get; set; }

		[DataMember]
		public bool FitToTime { get; set; }

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
				MovementRate = MovementRate,
				ColorEffect = ColorEffect,
				TilePictures = TilePictures,
				Direction = Direction,
				GifSpeed = GifSpeed,
				IncreaseBrightness = IncreaseBrightness,
				Colors = Colors
			};
			return result;
		}
	}
}
