using System;
using Vixen.Commands;

namespace Color {
	class ColorCommandDispatch : CommandDispatch {
		public Func<System.Drawing.Color, System.Drawing.Color> Filter;

		public override void DispatchCommand(Lighting.Polychrome.SetColor command) {
			System.Drawing.Color newColor = Filter(command.Color);
			Result = new Lighting.Polychrome.SetColor(newColor);
		}

		public Command Result;
	}
}
