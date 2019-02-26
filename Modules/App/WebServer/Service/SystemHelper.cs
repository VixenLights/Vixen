using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	public class SystemHelper
	{
		public IEnumerable<Controller> GetControllers()
		{
			var controllers = VixenSystem.OutputControllers.Select(x => new Controller(x));
			return controllers;
		}

		public bool SetState(Guid id, bool on)
		{
			bool success = false;
			var c = VixenSystem.OutputControllers.Get(id);
			if (c != null)
			{
				if (on)
				{
					if (!c.IsRunning || c.IsPaused)
					{
						c.Start();
						success = true;
					}
				}
				else
				{
					if (c.IsRunning)
					{
						c.Stop();
						success = true;
					}
				}
			}

			return success;
		}

		public Controller Get(Guid id)
		{
			var c = VixenSystem.OutputControllers.Get(id);
			if (c != null)
			{
				return new Controller(c);
			}

			return null;
		}
	}
}
