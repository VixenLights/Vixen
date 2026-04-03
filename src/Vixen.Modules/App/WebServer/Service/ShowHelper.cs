using Common.Broadcast;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	internal class ShowHelper
	{
		/// <summary>
		/// Gets the list of available Shows.
		/// </summary>
		/// <returns>Returns a list of shows.</returns>
		public static List<Presentation> GetShows()
		{
			var service = new ShowHelperService();
			return service.GetShows();
		}

		/// <summary>
		/// Command to start playing a Show.
		/// </summary>
		/// <param name="show">Specifies the name of the Show to start.</param>
		public static ContextStatus PlayShow(Presentation show)
		{
			Broadcast.Publish<Presentation>("PlayShow", show);

			return new ContextStatus()
			{
				Message = string.Format($"Playing show {show.Name}")
			};
		}

		/// <summary>
		/// Command to pause playing a Show.
		/// </summary>
		/// <param name="show">Specifies the name of the Show to pause.</param>
		public static ContextStatus PauseShow(Presentation show)
		{
			Broadcast.Publish<Presentation>("PauseShow", show);

			return new ContextStatus()
			{
				Message = string.Format($"Pausing show {show.Name}")
			};
		}

		/// <summary>
		/// Command to stop playing a Show.
		/// </summary>
		/// <param name="show">Specifies the name of the Show to stop.</param>
		public static ContextStatus StopShow(Presentation show)
		{
			Broadcast.Publish<Presentation>("StopShow", show);

			return new ContextStatus()
			{
				Message = string.Format($"Stopping show {show.Name}")
			};
		}
	}
}
