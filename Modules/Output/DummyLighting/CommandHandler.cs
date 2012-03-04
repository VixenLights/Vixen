using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.DummyLighting {
	class CommandHandler : CommandDispatch {
		public override void Handle(ByteValue c) {
			Value = c.Value;
		}

		// No more color commands!
		// This would have to be a preview and pick up intents to be able to display color.
		// Or it would have to act like an actual piece of hardware and take color components
		// on separate channels and combine those values into a color interpretation.

		public byte Value;
	}
}
