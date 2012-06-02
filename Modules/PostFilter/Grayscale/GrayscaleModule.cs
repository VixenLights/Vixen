using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.OutputFilter;

namespace Grayscale {
	public class GrayscaleModule : OutputFilterModuleInstanceBase {
		private GrayscaleData _data;
		private CommandHandler _commandHandler;

		public GrayscaleModule() {
			_commandHandler = new CommandHandler();
		}

		public override ICommand Affect(ICommand command) {
			command.Dispatch(_commandHandler);
			return _commandHandler.Result;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (GrayscaleData)value; }
		}
	}
}
