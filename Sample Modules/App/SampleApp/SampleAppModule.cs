using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace SampleApp {
	public class SampleAppModule : AppModuleInstanceBase {
		private IApplication _application;

		public override void Loading() {
			_AddApplicationMenu();
		}

		public override void Unloading() {
			_RemoveApplicationMenu();
		}

		public override IApplication Application {
			set { _application = value; }
		}

		private const string MENU_NAME = "SampleAppModuleTopMenu";
		private void _AddApplicationMenu() {
			// Application may not support menus, so check AppCommands for null.
			if(_application != null && _application.AppCommands != null) {
				AppCommand topMenu = new AppCommand(MENU_NAME, "My Sample");

				AppCommand helloItem = new AppCommand("SampleAppModule_Hello", "Say Hello");
				helloItem.Click += (sender, e) => System.Windows.Forms.MessageBox.Show("Hello, world.");

				LatchedAppCommand checkedItem = new LatchedAppCommand("SampleAppModule_Checked", "I am checked");
				checkedItem.IsChecked = true;
				checkedItem.Checked += (sender, args) => (sender as LatchedAppCommand).Text = args.CheckedState ? "I am checked" : "I am not checked";

				topMenu.Add(helloItem);
				topMenu.Add(checkedItem);

				_application.AppCommands.Add(topMenu);
			}
		}

		private void _RemoveApplicationMenu() {
			if(_application != null && _application.AppCommands != null) {
				_application.AppCommands.Remove(MENU_NAME);
			}
		}
	}
}
