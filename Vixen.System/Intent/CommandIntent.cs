using System;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent {
	public class CommandIntent : LinearIntent<CommandValue> {
		public CommandIntent(CommandValue command, TimeSpan timeSpan)
			: base(command, command, timeSpan, new StaticValueInterpolator<CommandValue>()) {
		}
	}
}
