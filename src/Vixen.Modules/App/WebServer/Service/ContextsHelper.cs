using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	/// <summary>
	/// Provides snapshots of the active execution contexts for the web server.
	/// </summary>
	public class ContextsHelper
	{
		/// <summary>
		/// Gets a snapshot of the contexts that are currently playing or paused.
		/// </summary>
		/// <returns>
		/// A materialized collection of the active context statuses.
		/// </returns>
		public static IEnumerable<ContextStatus> GetAllStates()
		{
			var contextStatuses = new List<ContextStatus>();
			foreach (var context in VixenSystem.Contexts)
			{
				if (Module.LiveContextName.Equals(context.Name))
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

				if (context is ISequenceContext sequenceContext)
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
					continue;
				}

				contextStatuses.Add(status);
			}

			return contextStatuses.ToArray();
		}
	}
}
