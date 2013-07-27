using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kayak;
using Kayak.Http;
using Vixen.Execution.Context;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using VixenModules.App.WebServer.HTTP;

namespace VixenModules.App.WebServer
{
	public class Module : AppModuleInstanceBase
	{

		public Module()
		{
			scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
		}
		internal static LiveContext LiveSystemContext {get;set;}

		private IScheduler scheduler;
		private IServer server;
		private Data _data;
		private IApplication _application;

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = (Data)value; }
		}

		public override IApplication Application
		{
			set { _application = value; }
		}

		public override void Loading()
		{
			server = KayakServer.Factory.CreateHttp(new RequestDelegate(), scheduler);
			server.Listen(new IPEndPoint(IPAddress.Any, _data.HttpPort));
			LiveSystemContext = VixenSystem.Contexts.GetSystemLiveContext(); 

			Thread T = new Thread(new ThreadStart(scheduler.Start));
			T.Start();
			
		}

		public override void Unloading()
		{
			scheduler.Stop();
			server.Dispose();
			server = null;
			LiveSystemContext = null;
		}


	}


}
