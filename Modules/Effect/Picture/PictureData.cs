using System;
using System.Drawing;
using System.Runtime.Serialization;
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
			Orientation = StringOrientation.Vertical;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Colors = new ColorGradient(Color.DodgerBlue);
			ColorEffect = ColorEffect.None;
			ScalePercent = 50;
			Source = PictureSource.Embedded;
			MovementRate = 4;
			Direction = 0;
			IncreaseBrightnessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			ScaleToGrid = true;
			TilePictures = TilePictures.BlueGlowDots;
			GifSpeed = 1;
			StretchToGrid = false;
			CenterStop = false;
		}

		[DataMember]
		public ColorEffect ColorEffect { get; set; }

		[DataMember]
		public EffectType EffectType { get; set; }

		[DataMember]
		public PictureSource Source { get; set; }

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
		public bool StretchToGrid { get; set; }

		[DataMember]
		public int ScalePercent { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int XOffset { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int YOffset { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ColorGradient Colors { get; set; }

		[DataMember]
		public int Direction { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int IncreaseBrightness { get; set; }

		[DataMember]
		public Curve IncreaseBrightnessCurve { get; set; }

		[DataMember]
		public bool FitToTime { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//The following is due to adding the PictureSource datamember and also no longer requiring the None selection for Embedded picture tiles.
			if (string.IsNullOrEmpty(FileName))
			{
				Source = PictureSource.Embedded;
			}
			if (TilePictures == TilePictures.None)
				TilePictures = TilePictures.BlueGlowDots;

			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (XOffsetCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(XOffset, 100, -100);
				XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				XOffset = 0;

				if (YOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(YOffset, 100, -100);
					YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					YOffset = 0;
				}

				if (IncreaseBrightnessCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(IncreaseBrightness, 100, 10);
					IncreaseBrightnessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					IncreaseBrightness = 0;
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			PictureData result = new PictureData
			{
				EffectType = EffectType,
				FitToTime = FitToTime,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				IncreaseBrightnessCurve = new Curve(IncreaseBrightnessCurve),
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
				Colors = Colors,
				Source = Source,
				StretchToGrid = StretchToGrid,
				CenterStop = CenterStop
			};
			return result;
		}
	}
}
