using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Pixel;

namespace VixenModules.Effect.Text
{
	[DataContract]
	[KnownType(typeof (SerializableFont))]
	public class TextData: ModuleDataModelBase
	{
		public TextData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = TextDirection.Up;
			Speed = 5;
			Position = 0;
			PositionX = 0;
			CenterStop = false;
			FitToTime = true;
			Text = new List<string>{String.Empty};
			GradientMode = GradientMode.AcrossElement;
			Orientation=StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Arial", 8));
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public TextDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[DataMember]
		public bool CenterText { get; set; }

		[DataMember]
		public bool FitToTime { get; set; }

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

		public override IModuleDataModel Clone()
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
				GradientMode = GradientMode,
				Font = new SerializableFont(Font.FontValue)
			};
			return result;
		}
	}
}
