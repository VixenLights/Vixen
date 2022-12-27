using Microsoft.AspNetCore.SignalR;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Hubs
{
	public class ContextHub:Hub
	{
		private ContextBroadcaster _broadcaster;

		public ContextHub(ContextBroadcaster broadcaster)
		{
			//This allows our broadcaster to get started when the hub is started. We don't have messaages
			//coming in from the clients yet, so there is not much to do here at the moment.
			_broadcaster = broadcaster;
		}

	}
}
