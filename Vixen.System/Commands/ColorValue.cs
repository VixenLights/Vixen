using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class ColorValue : Dispatchable<ColorValue>, ICommand<Color> {
		public ColorValue(Color value) {
			Value = value;
		}

		public Color Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (Color)value; }
		}
	}
}
