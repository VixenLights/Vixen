using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	class ErrorLog : Log {
		public ErrorLog()
			: base("Error") {
		}

		public override void Write(Exception ex) {
			base.Write(ex);
			VixenSystem.Logging.Debug(ex);
		}

		public override void Write(string qualifyingMessage, Exception ex) {
			base.Write(qualifyingMessage, ex);
			VixenSystem.Logging.Debug(ex);
		}
	}
}
