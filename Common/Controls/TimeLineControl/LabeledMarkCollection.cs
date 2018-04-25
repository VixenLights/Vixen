using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Controls.TimelineControl
{
	public class LabeledMarkCollection
	{

		public LabeledMarkCollection(string name, bool isEnabled, bool bold, bool isSolidLine, Color color, int level)
		{
			Name = name;
			IsEnabled = isEnabled;
			IsBold = bold;
			IsSolidLine = isSolidLine;
			Color = color;
			Level = level;
			Marks = new List<LabeledMark>();
		}
		public string Name { get; set; }

		public bool IsEnabled { get; set; }

		public bool IsBold { get; set; }

		public bool IsSolidLine { get; set; }

		public Color Color { get; set; }

		public int Level { get; set; }

		public List<LabeledMark> Marks { get; set; }

	}
}
