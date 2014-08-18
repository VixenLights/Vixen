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

		private const string MENU_ID_ROOT = "WebserverRoot";

		public Module()
		{
			scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
		}
		internal static LiveContext LiveContext { get; set; }

		private IScheduler scheduler;
		private IServer server;
		private Data _data;
		private IApplication _application;
		private AppCommand _showCommand;

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
			_AddApplicationMenu();
			_SetServerEnableState(_data.IsEnabled, _data.HttpPort);
		}
		private bool _AppSupportsCommands()
		{
			return _application != null && _application.AppCommands != null;
		}
		private AppCommand _GetToolsMenu()
		{
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if (toolsMenu == null) {
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
		private void _AddApplicationMenu()
		{
			if (_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Web Server");

				rootCommand.Add(_showCommand ?? (_showCommand = _CreateShowCommand()));

				toolsMenu.Add(rootCommand);
			}
		}

		private void _SetServerEnableState(bool value, int port)
		{
			if (value) {
				if (server != null) _SetServerEnableState(false, port);

				server = KayakServer.Factory.CreateHttp(new RequestDelegate(), scheduler);
				server.Listen(new IPEndPoint(IPAddress.Any, port));
				LiveContext = VixenSystem.Contexts.CreateLiveContext("Web Server");
				LiveContext.Start();
				Thread T = new Thread(new ThreadStart(scheduler.Start));
				T.Start();
			} else {
				scheduler.Stop();
				if (server != null) {
					server.Dispose();
					server = null;
				}
				if (LiveContext != null)
				{
					//We are the only current consumer of LiveContext, so shut it off when we are done.
					VixenSystem.Contexts.ReleaseContext(LiveContext);
					LiveContext = null;
				}
			}
		}



		private AppCommand _CreateShowCommand()
		{
			AppCommand showCommand = new AppCommand("WebserverConfigure", "Configure");
			showCommand.Click += (sender, e) => {
				using (Settings cs = new Settings(_data)) {
					cs.SettingsChanged += cs_SettingsChanged;
					if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

						_data.HttpPort = cs.Port;
						_data.IsEnabled = cs.WebServerEnabled;
						
					}
					_SetServerEnableState(_data.IsEnabled, _data.HttpPort);
				}
			};

			return showCommand;
		}

		void cs_SettingsChanged(object sender, WebSettingsEventArgs e)
		{
			_SetServerEnableState(e.IsEnabled, e.Port);
		}

		private void _RemoveApplicationMenu()
		{
			if (_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(MENU_ID_ROOT);
			}
		}

		public override void Unloading()
		{
			_SetServerEnableState(false,0);
			//We are the only current consumer of LiveContext, so shut it off when we are done.
			if (LiveContext != null)
			{
				VixenSystem.Contexts.ReleaseContext(LiveContext);	
			}
		}


	}


}
