using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class LogItemEventArgs : EventArgs {
		public LogItemEventArgs(string text) {
			Text = text;
		}

		public string Text { get; private set; }
	}
}
