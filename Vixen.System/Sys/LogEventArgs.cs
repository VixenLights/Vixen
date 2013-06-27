using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys
{
	public class LogEventArgs : EventArgs
	{
		public LogEventArgs(string logName, string text)
		{
			LogName = logName;
			Text = text;
		}

		public string LogName { get; private set; }
		public string Text { get; private set; }
	}
}