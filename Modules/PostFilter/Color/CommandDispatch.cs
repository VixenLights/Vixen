using System;
using Vixen.Commands;
using Vixen.Sys;

namespace VixenModules.OutputFilter.Color {
	class CommandDispatch : Vixen.Sys.Dispatch.CommandDispatch {
		public Func<ICommand<LightingValue>, ICommand> Filter;
		public ICommand Command;

		public override void Handle(LightingValueCommand obj) {
			Command = Filter(obj);
		}
	}
}
