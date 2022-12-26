using Microsoft.AspNetCore.SignalR;
using VixenModules.App.WebServer.Hubs;
using Timer = System.Threading.Timer;

namespace VixenModules.App.WebServer.Service
{
	public class ContextBroadcaster
	{
		private IHubContext<ContextHub> _hubContext;
		private readonly Timer _timer;
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);


		public ContextBroadcaster(IHubContext<ContextHub> hubContext)
		{
			_hubContext = hubContext;
			_timer = new Timer(BroadcastContextStates, null, _updateInterval, _updateInterval);
		}

		public async Task Update()
		{
			await _hubContext.Clients.All.SendAsync("updatePlayingContextStates", ContextsHelper.GetAllStates());
		}

		private void BroadcastContextStates(Object state)
		{
			//Call Update
			Update();
		}
	}
}
