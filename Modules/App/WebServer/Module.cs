using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Owin.Hosting;
using NLog;
using Vixen.Execution.Context;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.WebServer
{
	public class Module : AppModuleInstanceBase
	{
		private static readonly NLog.Logger Logging = LogManager.GetCurrentClassLogger();
		private const string MENU_ID_ROOT = "WebserverRoot";

		internal static LiveContext LiveContext { get; set; }

		//Webserver object
		private IDisposable server;
		private Data _data;
		private IApplication _application;
		private AppCommand _showCommand;

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = (Data) value; }
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
			if (toolsMenu == null)
			{
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}

		private void _AddApplicationMenu()
		{
			if (_AppSupportsCommands())
			{
				AppCommand toolsMenu = _GetToolsMenu();
				AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Web Server");

				rootCommand.Add(_showCommand ?? (_showCommand = _CreateShowCommand()));

				toolsMenu.Add(rootCommand);
			}
		}

		private void _SetServerEnableState(bool value, int port)
		{
			if (value)
			{
				if (server != null) _SetServerEnableState(false, port);
				var options = new StartOptions {ServerFactory = "Nowin"}; //use the Nowin server to listen for connections
				options.Urls.Add(string.Format("http://*:{0}/", port));

				try
				{
					server = WebApp.Start<Startup>(options);
				}
				catch (Exception ex)
				{
					Logging.Error("Unable to start web server.", ex);
					return;
				}
				LiveContext = VixenSystem.Contexts.CreateLiveContext("Web Server");
				LiveContext.Start();
			}
			else
			{
				if (server != null)
				{
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
			showCommand.Click += (sender, e) =>
			{
				using (Settings cs = new Settings(_data))
				{
					cs.SettingsChanged += cs_SettingsChanged;
					if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{

						_data.HttpPort = cs.Port;
						_data.IsEnabled = cs.WebServerEnabled;

					}
					_SetServerEnableState(_data.IsEnabled, _data.HttpPort);
				}
			};

			return showCommand;
		}

		private void cs_SettingsChanged(object sender, WebSettingsEventArgs e)
		{
			_SetServerEnableState(e.IsEnabled, e.Port);
		}

		private void _RemoveApplicationMenu()
		{
			if (_AppSupportsCommands())
			{
				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(MENU_ID_ROOT);
			}
		}

		public override void Unloading()
		{
			_SetServerEnableState(false, 0);
			//We are the only current consumer of LiveContext, so shut it off when we are done.
			if (LiveContext != null)
			{
				VixenSystem.Contexts.ReleaseContext(LiveContext);
			}
		}

		
	}

}
