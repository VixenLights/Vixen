using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.PostFilter;

namespace Grayscale {
	public class GrayscaleModule : PostFilterModuleInstanceBase {
		private GrayscaleData _data;

		public override Command Affect(Command command) {
			Lighting.Polychrome.SetColor setColorCommand = command as Lighting.Polychrome.SetColor;
			if(setColorCommand != null) {
				command = _BasicGrayscale(setColorCommand);
			}
			return command;
		}

		private Command _BasicGrayscale(Lighting.Polychrome.SetColor setColorCommand) {
			double luma = (setColorCommand.Color.R * 0.3 + setColorCommand.Color.G * 0.59 + setColorCommand.Color.B * 0.11);
			double level = luma / 255 * 100;
			Lighting.Monochrome.SetLevel setLevelCommand = new Lighting.Monochrome.SetLevel(level);
			return setLevelCommand;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (GrayscaleData)value; }
		}
	}
}
