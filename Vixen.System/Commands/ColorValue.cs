using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class ColorValue : Dispatchable<ColorValue>, ICommand<Color> {
		public ColorValue(Color value) {
			CommandValue = value;
		}

		public Color CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (Color)value; }
		}
	}
}
