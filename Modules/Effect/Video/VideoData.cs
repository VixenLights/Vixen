using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Video
{
	[DataContract]
	public class VideoData : EffectTypeModuleData
	{

		public VideoData()
		{
			EffectType = EffectType.RenderPictureNone;
			EffectColorType = EffectColorType.RenderColor;
			ScalePercent = 0;
			ScaleToGrid = true;
			MaintainAspect = false;
			AdvancedSettings = false;
			Speed = 1;
			IncreaseBrightnessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			PlayBackSpeed = 0;
			StartTime = 0;
			RotateVideo = 0;
			Video_DataPath = string.Empty;
			FileName = String.Empty;
			Orientation = StringOrientation.Vertical;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		[DataMember]
		public EffectType EffectType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int IncreaseBrightness { get; set; }

		[DataMember]
		public Curve IncreaseBrightnessCurve { get; set; }

		[DataMember]
		public EffectColorType EffectColorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int RotateVideo { get; set; }

		[DataMember]
		public int PlayBackSpeed { get; set; }

		[DataMember]
		public double StartTime { get; set; }

		[DataMember]
		public bool FitToTime { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public bool ScaleToGrid { get; set; }

		[DataMember]
		public bool MaintainAspect { get; set; }

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
		public bool AdvancedSettings { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public int VideoLength { get; set; }

		[DataMember]
		public string Video_DataPath { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (XOffsetCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(XOffset, 100, -100);
				XOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				XOffset = 0;

				if (YOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(YOffset, 100, -100);
					YOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
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
			VideoData result = new VideoData
			{
				EffectType = EffectType,
				Video_DataPath = Video_DataPath,
				FitToTime = FitToTime,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				Speed = Speed,
				IncreaseBrightnessCurve = new Curve(IncreaseBrightnessCurve),
				VideoLength = VideoLength,
				RotateVideo =RotateVideo,
				AdvancedSettings = AdvancedSettings,
				PlayBackSpeed = PlayBackSpeed,
				StartTime = StartTime,
				ScalePercent = ScalePercent,
				ScaleToGrid = ScaleToGrid,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				FileName = FileName,
				EffectColorType = EffectColorType,
				MaintainAspect = MaintainAspect
			};
			return result;
		}
	}
}
