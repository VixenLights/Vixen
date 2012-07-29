using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class ColorCommand : Dispatchable<ColorCommand>, ICommand<Color> {
		public ColorCommand(Color value) {
			CommandValue = value;
		}

		public Color CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (Color)value; }
		}
	}
}
