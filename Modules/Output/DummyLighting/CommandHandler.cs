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

		public byte Value;
	}
}
