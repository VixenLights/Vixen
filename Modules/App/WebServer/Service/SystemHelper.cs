using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		public bool SetControllerState(Guid id, bool on)
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

		public bool SetAllControllersState(bool on)
		{
			if (on)
			{
				foreach (var outputController in VixenSystem.OutputControllers)
				{
					outputController.Start();
				}
			}
			else
			{
				foreach (var outputController in VixenSystem.OutputControllers)
				{
					outputController.Stop();
				}
			}

			return true;
		}

		public Controller GetController(Guid id)
		{
			var c = VixenSystem.OutputControllers.Get(id);
			if (c != null)
			{
				return new Controller(c);
			}

			return null;
		}

		public async Task<bool> Save()
		{
			return await VixenSystem.SaveSystemConfigAsync();
		}
	}
}
