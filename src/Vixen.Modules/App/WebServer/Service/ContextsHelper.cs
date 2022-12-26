using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	public class ContextsHelper
	{
		private static readonly List<ContextStatus> _contextStatuses = new List<ContextStatus>();
		
		public static IEnumerable<ContextStatus> GetAllStates()
		{
			UpdateContextStatus();
			return _contextStatuses.Where(x => x.State.Equals(ContextStatus.States.Playing) ||
			                                   x.State.Equals(ContextStatus.States.Paused));
		}

		private static void UpdateContextStatus()
		{
			_contextStatuses.Clear();
			foreach (var context in VixenSystem.Contexts)
			{
				if (@"Web Server".Equals(context.Name))
				{
					//Skip the web server context.
					continue;
				}

				var status = new ContextStatus()
				{
					Sequence = new Sequence()
					{
						Name = context.Name
					},
					Position = context.GetTimeSnapshot()
				};

				var sequenceContext = context as ISequenceContext;
				if (sequenceContext != null)
				{
					status.Sequence.FileName = Path.GetFileName(sequenceContext.Sequence.FilePath);
				}

				if (context.IsPaused)
				{
					status.State = ContextStatus.States.Paused;

				}
				else if (context.IsRunning)
				{
					status.State = ContextStatus.States.Playing;
				}
				else
				{
					status.State = ContextStatus.States.Stopped;
					status.Position = TimeSpan.Zero; //Ensure reported time is set to zero when context is stopped
				}

				_contextStatuses.Add(status);
			}
		}
	}
}
