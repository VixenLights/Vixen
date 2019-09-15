using System;
using Vixen.Sys.Output;

namespace VixenModules.App.WebServer.Model
{
	public class Controller
	{
		public Controller(OutputController controller)
		{
			Id = controller.Id;
			Name = controller.Name;
			IsRunning = controller.IsRunning;
			IsPaused = controller.IsPaused;
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public bool IsRunning { get; set; }

		public bool IsPaused { get; set; }

	}
}
