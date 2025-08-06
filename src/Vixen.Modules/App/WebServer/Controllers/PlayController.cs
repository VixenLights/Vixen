using Microsoft.AspNetCore.Mvc;
using VixenModules.App.WebServer.Filter;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class PlayController : BaseApiController
	{
		[HttpGet]
		public IEnumerable<Status> Status()
		{
			return SequenceHelper.Status();
		}

		public IEnumerable<Presentation> GetSequences()
		{
			return SequenceHelper.GetSequences();
		}

		public IEnumerable<Presentation> GetShows()
		{
			return ShowHelper.GetShows();
		}

		[HttpPost]
		public Status PlayPresentation(Presentation presentation)
		{
			if (presentation.Info.EndsWith(".tim") == true)
			{
				return SequenceHelper.PlaySequence(presentation);
			}
			else
			{
				return ShowHelper.PlayShow(presentation);
			}
		}

		[HttpPost]
		public Status PausePresentation(Presentation presentation)
		{
			if (presentation.Info.EndsWith(".tim") == true)
			{
				return SequenceHelper.PauseSequence(presentation);
			}
			else
			{
				return ShowHelper.PauseShow(presentation);
			}
		}

		[HttpPost]
		public Status StopPresentation(Presentation presentation)
		{
			if (presentation.Info.EndsWith(".tim") == true)
			{
				return SequenceHelper.StopSequence(presentation);
			}
			else
			{
				return ShowHelper.StopShow(presentation);
			}
		}
	}
}
