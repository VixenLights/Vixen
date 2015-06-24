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
			Line1 = String.Empty;
			Line2 = String.Empty;
			Line3 = String.Empty;
			Line4 = String.Empty;
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
		public bool FitToTime { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public int Position { get; set; }

		[DataMember]
		public int PositionX { get; set; }

		[DataMember]
		public string Line1 { get; set; }

		[DataMember]
		public string Line2 { get; set; }

		[DataMember]
		public string Line3 { get; set; }

		[DataMember]
		public string Line4 { get; set; }

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
				Line1 = Line1,
				Line2 = Line2,
				Line3 = Line3,
				Line4 = Line4,
				GradientMode = GradientMode,
				Font = new SerializableFont(Font.FontValue)
			};
			return result;
		}
	}
}
