using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.WebServer
{
	public class WebSettingsEventArgs : EventArgs
	{
		public WebSettingsEventArgs(int port, bool isEnabled)
		{
			Port = port;
			IsEnabled = isEnabled;
		}

		public int Port { get; private set; }
		public bool IsEnabled { get; private set; }
	}
}
