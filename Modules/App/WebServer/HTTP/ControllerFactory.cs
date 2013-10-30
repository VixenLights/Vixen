using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VixenModules.App.WebServer.Actions;


namespace VixenModules.App.WebServer.HTTP
{
	/// <summary>
	/// Returns the controller that handles the specified api verb
	/// </summary>
	public class ControllerFactory
	{
		public static IController Get(string actionVerb)
		{
			switch (actionVerb)
			{
				case "play" :
					return new PlayController();
				case "element":
					return new ElementController();
				default:
					return null;
			}
		}

	}
}
