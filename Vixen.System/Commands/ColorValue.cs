using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class ColorValue : Dispatchable<ColorValue>, ICommand {
		public ColorValue(Color value) {
			Value = value;
		}

		public Color Value { get; private set; }
	}
}
