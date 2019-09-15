using System.Windows.Forms.Integration;
using Catel.IoC;
using Vixen.Module.App;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Views;
using ConfigurationService = VixenModules.App.CustomPropEditor.Services.ConfigurationService;

namespace VixenModules.App.CustomPropEditor
{
	public class CustomPropEditorModule : AppModuleInstanceBase
	{
		private IApplication _application;
		private const string MenuIdRoot = "CustomPropRoot";

		public override void Loading()
		{
			var serviceLocator = ServiceLocator.Default;
			serviceLocator.AutoRegisterTypesViaAttributes = true;
			AddApplicationMenu();
			Configuration config = new Configuration((CustomPropEditorData)StaticModuleData);
			ConfigurationService service = ConfigurationService.Instance();
			service.Config = config;
		}

		public override void Unloading()
		{
			if (AppSupportsCommands)
			{
				AppCommand toolsMenu = GetToolsMenu();
				toolsMenu.Remove(MenuIdRoot);
			}
		}

		public override IApplication Application
		{
			set { _application = value; }
		}


		private bool AppSupportsCommands
		{
			get { return _application != null && _application.AppCommands != null; }
		}

		private void AddApplicationMenu()
		{
			if (AppSupportsCommands)
			{
				var toolsMenu = GetToolsMenu();
				var rootCommand = new AppCommand(MenuIdRoot, "Custom Prop Editor");
				rootCommand.Click += RootCommand_Click;
				toolsMenu.Add(rootCommand);
			}
		}

		private void RootCommand_Click(object sender, System.EventArgs e)
		{
			CustomPropEditorWindow mw = new CustomPropEditorWindow();
			//PropEditorWindow mw = new PropEditorWindow();
			ElementHost.EnableModelessKeyboardInterop(mw);
			mw.Show();
		}

		private AppCommand GetToolsMenu()
		{
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if (toolsMenu == null)
			{
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
	}
}