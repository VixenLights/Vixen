using Common.Broadcast;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	public class ShowHelperService
	{
		private List<Presentation> _showList = null;

		public ShowHelperService()
		{
			Broadcast.Subscribe<List<Presentation>>(this, "GetShowList", GetShowList);
			Broadcast.Subscribe<Presentation>(this, "PlayShow", PlayShow);
			Broadcast.Subscribe<Presentation>(this, "StopShow", StopShow);
			Broadcast.Subscribe<Presentation>(this, "PauseShow", PauseShow);
		}

		public List<Presentation> GetShows()
		{
			Broadcast.Publish<string>("RetrieveShowList", "");
			return _showList;
		}

		private void GetShowList(List<Presentation> ShowList)
		{
			if (_showList == null)
				_showList = new List<Presentation>();

			_showList = ShowList;
		}

		public void PlayShow(Presentation show)
		{
			IContext generalContext = VixenSystem.Contexts.FirstOrDefault(x => x.Name == show.Name);

			if (generalContext == null)
			{
				ShowContext showContext = VixenSystem.Contexts.CreateShowContext(show.Name, show.Info);
				showContext.Start();
			}

			else
			{
				ShowContext showContext = generalContext as ShowContext;
				showContext.Resume();
			}
		}

		public void PauseShow(Presentation show)
		{
			IEnumerable<IContext> contexts = VixenSystem.Contexts.Where(x => x.Name.Equals(show.Name) && (x.IsRunning || x.IsPaused));

			if (contexts.Any())
			{
				foreach (var context in contexts)
				{
					context.Pause();
				}
			}
		}

		public void StopShow(Presentation show)
		{
			IEnumerable<IContext> contexts = VixenSystem.Contexts.Where(x => x.Name.Equals(show.Name) && (x.IsRunning || x.IsPaused));

			if (contexts.Any())
			{
				foreach (var context in contexts)
				{
					context.Stop();
					VixenSystem.Contexts.ReleaseContext(context);
				}
			}
		}
	}
}
