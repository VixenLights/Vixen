using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Text
{
	[DataContract]
	[KnownType(typeof (SerializableFont))]
	public class TextData: EffectTypeModuleData
	{
		public TextData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red)};
			BaseColor = Color.Lime;
			UseBaseColor = false;
			Direction = TextDirection.Left;
			Speed = 1;
			Position = 0;
			PositionX = 0;
			ScaleText = 0;
			CenterStop = false;
			Text = new List<string>{String.Empty};
			GradientMode = GradientMode.AcrossElement;
			Orientation=StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Arial", 8));
			LevelCurve = new Curve(CurveType.Flat100);
			BaseLevelCurve = new Curve(CurveType.Flat100);
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

		[DataMember]
		public int ScaleText { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[DataMember]
		public bool CenterText { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public TextMode TextMode { get; set; }

		[DataMember]
		public int Position { get; set; }

		[DataMember]
		public int PositionX { get; set; }

		[DataMember]
		public List<string> Text { get; set; }

		[DataMember]
		public SerializableFont Font { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (LevelCurve == null)
			{
				LevelCurve = new Curve(CurveType.Flat100);
			}

			if (BaseLevelCurve == null)
			{
				BaseLevelCurve = new Curve(CurveType.Flat100);
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
				Orientation = Orientation,
				Position = Position,
				PositionX = PositionX,
				Text = Text.ToList(),
				ScaleText = ScaleText,
				GradientMode = GradientMode,
				TextMode = TextMode,
				CenterText = CenterText,
				UseBaseColor = UseBaseColor,
				BaseColor = BaseColor,
				Font = new SerializableFont(Font.FontValue),
				LevelCurve = LevelCurve,
				BaseLevelCurve = BaseLevelCurve
			};
			return result;
		}
	}
}
