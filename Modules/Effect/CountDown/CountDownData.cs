using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.CountDown
{
	[DataContract]
	[KnownType(typeof (SerializableFont))]
	public class CountDownData : EffectTypeModuleData
	{
		public CountDownData()
		{
			GradientColors = new List<ColorGradient> { new ColorGradient(Color.Red) };
			SpinnerColors = new List<ColorGradient> { new ColorGradient(Color.LightSalmon) };
			Direction = CountDownDirection.None;
			Speed = 1;
			Fade = true;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			FontScaleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			CenterStop = false;
			GradientMode = GradientMode.AcrossElement;
			SpinnerGradientMode = SpinnerGradientMode.Horizontal;
			Orientation=StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Arial", 20));
			LevelCurve = new Curve(CurveType.Flat100);
			TimeFormat = TimeFormat.Seconds;
			CountDownType = CountDownType.Effect;
			CountDownFade = CountDownFade.InOut;
			CountDownTime = "10";
			PerIteration = false;
			SizeMode = SizeMode.None;
			Spinner = false;
		}

		[DataMember(EmitDefaultValue = false)]
		public ColorGradient Colors { get; set; }

		[DataMember]
		public List<ColorGradient> GradientColors { get; set; }

		[DataMember]
		public List<ColorGradient> SpinnerColors { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public CountDownDirection Direction { get; set; }

		[DataMember]
		public TimeFormat TimeFormat { get; set; }

		[DataMember]
		public string CountDownTime { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[DataMember]
		public bool Fade { get; set; }

		[DataMember]
		public bool PerIteration { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public SpinnerGradientMode SpinnerGradientMode { get; set; }

		[DataMember]
		public TimeFormat TextMode { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve FontScaleCurve { get; set; }

		[DataMember]
		public Curve AngleCurve { get; set; }

		[DataMember]
		public SerializableFont Font { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public CountDownType CountDownType { get; set; }

		[DataMember]
		public CountDownFade CountDownFade { get; set; }

		[DataMember]
		public SizeMode SizeMode { get; set; }

		[DataMember]
		public bool Spinner { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (GradientColors == null)
			{
				GradientColors = new List<ColorGradient> { Colors };
			}

			if (SpinnerColors == null)
			{
				SpinnerColors = new List<ColorGradient> { Colors };
			}
		  }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			CountDownData result = new CountDownData
			{
				GradientColors = GradientColors.ToList(),
				SpinnerColors = SpinnerColors.ToList(),
				Direction = Direction,
				Speed = Speed,
				CenterStop = CenterStop,
				Orientation = Orientation,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				AngleCurve = new Curve(AngleCurve),
				GradientMode = GradientMode,
				SpinnerGradientMode = SpinnerGradientMode,
				Font = new SerializableFont(Font.FontValue),
				LevelCurve = LevelCurve,
				FontScaleCurve = new Curve(FontScaleCurve),
				TimeFormat = TimeFormat,
				Fade = Fade,
				CountDownType = CountDownType,
				CountDownFade = CountDownFade,
				CountDownTime = CountDownTime,
				PerIteration = PerIteration,
				SizeMode = SizeMode,
				Spinner = Spinner
		};
			return result;
		}
	}
}
