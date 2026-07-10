using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Text
{
	[DataContract]
	[KnownType(typeof (SerializableFont))]
	public class TextData: EffectTypeModuleData
	{
		private TextCycleColorMode _cycleColorMode;
		private bool _cycleColorModeDeserialized;

		public TextData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red)};
			BaseColor = Color.Lime;
			UseBaseColor = false;
			Direction = TextDirection.Left;
			Speed = 1;
			Position = 0;
			PositionX = 0;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffSetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			FontScaleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			ScaleText = 0;
			CenterStop = false;
			EndStop = false;
			Text = new List<string>{String.Empty};
			GradientMode = GradientMode.AcrossElement;
			Orientation=StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Arial", 8));
			LevelCurve = new Curve(CurveType.Flat100);
			BaseLevelCurve = new Curve(CurveType.Flat100);
			TextSource = TextSource.None;
			TextFade = TextFade.Out;
			TimeVisibleLength = 750;
			DirectionPerWord = false;
			RepeatText = false;
			TextDuration = TextDuration.AutoFit;
			CycleColor = false;
			CycleColorMode = TextCycleColorMode.Character;
			ExplodePosition = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CycleCharacterColor = false;
			FallSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Color BaseColor { get; set; }

		[DataMember]
		public Curve BaseLevelCurve { get; set; }

		[DataMember]
		public bool UseBaseColor { get; set; }

		[DataMember]
		public TextDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int ScaleText { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[DataMember]
		public bool EndStop { get; set; }

		[DataMember]
		public bool CenterText { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public TextMode TextMode { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Position { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve YOffSetCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int PositionX { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve FontScaleCurve { get; set; }

		[DataMember]
		public Curve AngleCurve { get; set; }

		[DataMember]
		public List<string> Text { get; set; }

		[DataMember]
		public SerializableFont Font { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }
		
		[DataMember]
		public TextSource TextSource { get; set; }

		[DataMember]
		public TextFade TextFade { get; set; }
		
		[DataMember]
		public int TimeVisibleLength { get; set; }
		
		[DataMember]
		public bool DirectionPerWord { get; set; }

		[DataMember]
		public bool RepeatText { get; set; }
		
		[DataMember]
		public TextDuration TextDuration { get; set; }
		
		[DataMember]
		public bool CycleColor { get; set; }

		/// <summary>
		/// Gets or sets the mode that specifies how cycle color advances through rendered text.
		/// </summary>
		/// <value>One of the enumeration values that specifies the color cycling granularity. The default is <see cref="TextCycleColorMode.Character" />.</value>
		[DataMember]
		public TextCycleColorMode CycleColorMode
		{
			get { return _cycleColorMode; }
			set
			{
				_cycleColorMode = value;
				_cycleColorModeDeserialized = true;
			}
		}

		[DataMember]
		public bool CycleCharacterColor { get; set; }

		[DataMember]
		public Curve ExplodePosition { get; set; }

		[DataMember]
		public Curve FallSpeedCurve { get; set; }

		[OnDeserializing]
		private void OnDeserializing(StreamingContext c)
		{
			_cycleColorMode = TextCycleColorMode.Character;
			_cycleColorModeDeserialized = false;
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (TimeVisibleLength == 0) TimeVisibleLength = 750;

			if (LevelCurve == null)
			{
				LevelCurve = new Curve(CurveType.Flat100);
			}

			if (BaseLevelCurve == null)
			{
				BaseLevelCurve = new Curve(CurveType.Flat100);
			}

			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (FontScaleCurve == null)
			{
				AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));

				double value;

				if (FontScaleCurve == null)
				{
					FontScaleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
					ScaleText = 0;
				}

				if (XOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(PositionX, 100, -100);
					XOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					PositionX = 0;
				}

				if (YOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Position, 100, -100);
					YOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					Position = 0;
				}
			}

			if (YOffSetCurve == null)
			{
				YOffSetCurve = new Curve(YOffsetCurve);
				// Copy the old Y Offset curve and then Invert curve as this has been set the wrong way around so convert any existing curves.
				foreach (var p in YOffSetCurve.Points)
				{
					p.Y = 100 - p.Y;
				}
			}

			if (ExplodePosition == null)
			{
				ExplodePosition = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
				FallSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			}

			if (CycleCharacterColor && !_cycleColorModeDeserialized)
			{
				CycleColor = true;
				CycleColorMode = TextCycleColorMode.Character;
			}
			else if (CycleColor && TextSource != TextSource.None && !_cycleColorModeDeserialized)
			{
				CycleColorMode = TextCycleColorMode.Word;
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			TextData result = new TextData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				CenterStop = CenterStop,
				EndStop = EndStop,
				Orientation = Orientation,
				YOffSetCurve = new Curve(YOffSetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				AngleCurve = new Curve(AngleCurve),
				Text = Text.ToList(),
				ScaleText = ScaleText,
				GradientMode = GradientMode,
				TextMode = TextMode,
				CenterText = CenterText,
				UseBaseColor = UseBaseColor,
				BaseColor = BaseColor,
				Font = new SerializableFont(Font.FontValue),
				LevelCurve = LevelCurve,
				FontScaleCurve = new Curve(FontScaleCurve),
				BaseLevelCurve = BaseLevelCurve,
				TextSource = TextSource,
				MarkCollectionId = MarkCollectionId,
				TextFade = TextFade,
				TimeVisibleLength = TimeVisibleLength,
				DirectionPerWord = DirectionPerWord,
				RepeatText = RepeatText,
				TextDuration = TextDuration,
				CycleColor = CycleColor,
				CycleColorMode = CycleColorMode,
				ExplodePosition = new Curve(ExplodePosition),
				CycleCharacterColor = CycleCharacterColor,
				FallSpeedCurve = new Curve(FallSpeedCurve)
			};
			return result;
		}
	}
}
