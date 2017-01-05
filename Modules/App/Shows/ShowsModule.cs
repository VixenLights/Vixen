using System.Threading;
using System.Timers;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using System;

namespace VixenModules.App.Shows
{
	public class ShowsModule : AppModuleInstanceBase
	{
		private ShowsData _data;
		private IApplication _application;
		//private const string MENU_ID_ROOT = "ShowsRoot";
		static public string MENU_ID_ROOT = "SuperSchedulerRoot";
		static public string MENU_ID_NAME = "Scheduler";

		public override void Loading()
		{
			_AddApplicationMenu();
		}

		public override void Unloading()
		{
		}

		public override IApplication Application
		{
			set { _application = value; }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = (ShowsData)value; }
		}

		private void _AddApplicationMenu()
		{
			if (_AppSupportsCommands()) {
				AppCommand schedulerMenu = _application.AppCommands.Find(MENU_ID_ROOT);
				if (schedulerMenu == null)
				{
					schedulerMenu = new AppCommand(MENU_ID_ROOT, MENU_ID_NAME);
					_application.AppCommands.Add(schedulerMenu);
				} 

				AppCommand rootCommand = new AppCommand("ShowEditor", "Shows"); 
				rootCommand.Click += (sender, e) =>
				{
					using (ShowListForm form = new ShowListForm(_data))
					{
						if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
						}
					}
				};
				if (schedulerMenu.Items.Length > 0)
					schedulerMenu.Add(new AppCommand("s1", "-"));
				schedulerMenu.Add(rootCommand);
			}
		}

		private bool _AppSupportsCommands()
		{
			return _application != null && _application.AppCommands != null;
		}

	}
}