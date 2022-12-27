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
		
		internal static LiveContext? LiveContext { get; set; }

		private Data? _data;
		private IApplication? _application;
		private WebHost? _webHost;
		
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
			_webHost = new WebHost(_data);
			_SetServerEnableState(_data.IsEnabled);
		}

		private bool _AppSupportsCommands()
		{
			return _application != null && _application.AppCommands != null;
		}

		private AppCommand? _GetToolsMenu()
		{
			if (_application == null) return null;
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
				AppCommand? toolsMenu = _GetToolsMenu();
				if (toolsMenu == null)
				{
					return;
				}
				AppCommand showCommand = new AppCommand(MENU_ID_ROOT, "Web Server");

				showCommand.Click += async (sender, e) =>
				{
					using (Settings cs = new Settings(_data))
					{
						bool settingsChanged = false;
						if (cs.ShowDialog() == DialogResult.OK)
						{
							if (_data.HttpPort != cs.Port)
							{
								_data.HttpPort = cs.Port;
								settingsChanged = true;
							}

							if (_data.IsEnabled != cs.WebServerEnabled)
							{
								_data.IsEnabled = cs.WebServerEnabled;
								settingsChanged = true;
							}
							await VixenSystem.SaveModuleConfigAsync();

						}

						if (settingsChanged)
						{
							_SetServerEnableState(_data.IsEnabled);
						}
					}
				};

				toolsMenu.Add(showCommand);
			}
			
		}

		private void _SetServerEnableState(bool enabled)
		{
			if (enabled)
			{
				StopServer();
				
				try
				{
					_webHost?.Start();		
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
			_webHost?.Stop();
		}

		private void _RemoveApplicationMenu()
		{
			if (_AppSupportsCommands())
			{
				AppCommand? toolsMenu = _GetToolsMenu();
				toolsMenu?.Remove(MENU_ID_ROOT);
			}
		}

		public override void Unloading()
		{
			_RemoveApplicationMenu();
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
