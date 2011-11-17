using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.InstrumentationPanel
{
	public class InstrumentationModule : AppModuleInstanceBase {
		private IApplication _application;
		private InstrumentationForm _form;

		private const string ID_MENU = "Instrumentation_Main";

		public override void Loading() {
			_form = new InstrumentationForm();
			_AddMenu();
		}

		public override void Unloading() {
			_form.Dispose();
			_form = null;
			_RemoveMenu();
		}

		public override IApplication Application {
			set { _application = value; }
		}

		private void _AddMenu() {
			if(_application != null && _application.AppCommands != null) {
				AppCommand mainMenu = new AppCommand(ID_MENU, "Instrumentation");
				mainMenu.Click += (sender, e) => _form.Show();

				_application.AppCommands.Add(mainMenu);
			}
		}

		private void _RemoveMenu() {
			if(_application != null && _application.AppCommands != null) {
				_application.AppCommands.Remove(ID_MENU);
			}
		}
	}
}
