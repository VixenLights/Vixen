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
		private IDisposable _server;
		private Data _data;
		private IApplication _application;
		
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
				AppCommand showCommand = new AppCommand(MENU_ID_ROOT, "Web Server");

				showCommand.Click += async (sender, e) =>
				{
					using (Settings cs = new Settings(_data))
					{
						cs.SettingsChanged += cs_SettingsChanged;
						if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{

							_data.HttpPort = cs.Port;
							_data.IsEnabled = cs.WebServerEnabled;
							await VixenSystem.SaveModuleConfigAsync();

						}
						_SetServerEnableState(_data.IsEnabled, _data.HttpPort);
					}
				};

				toolsMenu.Add(showCommand);
			}
			
		}

		private void _SetServerEnableState(bool value, int port)
		{
			if (value)
			{
				StopServer();
				var options = new StartOptions {ServerFactory = "Nowin", Port = port}; //use the Nowin _server to listen for connections
				try
				{
					_server = WebApp.Start<Startup>(options);
				}
				catch (Exception ex)
				{
					Logging.Error(ex, "Unable to start web _server.");
					return;
				}
				if (LiveContext == null)
				{
					LiveContext = VixenSystem.Contexts.CreateLiveContext("Web Server");
					LiveContext.Start();
				}
				
			}
			else
			{
				StopServer();
				if (LiveContext != null)
				{
					//We are the only current consumer of LiveContext, so shut it off when we are done.
					VixenSystem.Contexts.ReleaseContext(LiveContext);
					LiveContext = null;
				}
			}
		}

		private void StopServer()
		{
			if (_server != null)
			{
				_server.Dispose();
				_server = null;
			}	
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
			StopServer();
			//We are the only current consumer of LiveContext, so shut it off when we are done.
			if (LiveContext != null)
			{
				VixenSystem.Contexts.ReleaseContext(LiveContext);
				LiveContext = null;
			}
		}

		
	}

}
