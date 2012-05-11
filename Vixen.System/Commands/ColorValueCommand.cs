using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class ColorValueCommand : Dispatchable<ColorValueCommand>, ICommand<Color> {
		public ColorValueCommand(Color value) {
			CommandValue = value;
		}

		public Color CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (Color)value; }
		}
	}
}
