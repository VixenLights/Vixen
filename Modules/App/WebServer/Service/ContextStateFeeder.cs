using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.App.WebServer.Hubs;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	public class ContextStateFeeder
	{
		// Singleton instance
		private readonly static Lazy<ContextStateFeeder> _instance = new Lazy<ContextStateFeeder>(() => new ContextStateFeeder(GlobalHost.ConnectionManager.GetHubContext<ContextHub>().Clients));

		private readonly List<ContextStatus> _contextStatuses = new List<ContextStatus>(); 
		private readonly Timer _timer;
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);

		private ContextStateFeeder(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

			UpdateContextStatus(null);
			
			_timer = new Timer(UpdateContextStatus, null, _updateInterval, _updateInterval);

        }

		public static ContextStateFeeder Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		private IHubConnectionContext<dynamic> Clients{ get; set; }

		public IEnumerable<ContextStatus> GetAllStates()
		{
			return _contextStatuses.Where(x => x.State.Equals(ContextStatus.States.Playing) ||
			                                   x.State.Equals(ContextStatus.States.Paused));
		}

		private void UpdateContextStatus(object state)
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
					status.Sequence.FileName =  Path.GetFileName(sequenceContext.Sequence.FilePath);
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
			BroadcastContextStates();
		}

		private void BroadcastContextStates()
		{
			Clients.All.updateContextStates(new Sequence
			{
				FileName = "stuff.tim",
				Name = "stuff"
			});
			Clients.All.updatePlayingContextStates(_contextStatuses.Where(x => x.State.Equals(ContextStatus.States.Playing) ||
				x.State.Equals(ContextStatus.States.Paused)));
		}
	}
}
