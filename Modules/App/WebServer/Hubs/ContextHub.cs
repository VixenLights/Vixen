using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Hubs
{
	[HubName("ContextStates")]
	public class ContextHub:Hub
	{
		private readonly ContextStateFeeder _contextStateFeeder;

		public ContextHub() : this(ContextStateFeeder.Instance) { }

		public ContextHub(ContextStateFeeder contextStateFeeder)
        {
			_contextStateFeeder = contextStateFeeder;
        }

		public IEnumerable<ContextStatus> GetAllStates()
		{
			return _contextStateFeeder.GetAllStates();
        }
	}
}
