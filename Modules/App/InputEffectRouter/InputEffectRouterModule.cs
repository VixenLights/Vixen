using System.Windows.Forms;
using BasicInputManagement;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.InputEffectRouter {
	public class InputEffectRouterModule : AppModuleInstanceBase {
		private IApplication _application;
		private InputEffectRouterData _data;
		private InputManager _inputManagement;

		private const string ROOT_ID = "InputEffectRouter_Root";

		public override void Loading() {
			if(_AppSupportsCommands) {
				AppCommand toolsMenu = _GetToolsMenu();

				AppCommand rootMenuItem = new AppCommand(ROOT_ID, "Input Effect Router");
				rootMenuItem.Click += (sender, e) => {
					if(_inputManagement.ShowForm() == DialogResult.OK) {
					    _data.Map = _inputManagement.InputEffects;
					    _data.InputModules = _inputManagement.InputModules;
					}
				};
				
				toolsMenu.Add(rootMenuItem);

				_inputManagement.Start();
			}
		}

		public override void Unloading() {
			if(_AppSupportsCommands) {
				_inputManagement.Stop();

				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(ROOT_ID);
			}
		}

		public override IApplication Application {
			set { _application = value; }
		}

		public override IModuleDataModel StaticModuleData {
			get { return _data; }
			set { 
				_data = value as InputEffectRouterData;
				_inputManagement = new InputManager(_data.InputModules, _data.Map);
				_inputManagement.InputsChanged += _inputManagement_InputsChanged;
			}
		}

		private void _inputManagement_InputsChanged(object sender, InputsChangedEventArgs e) {
			Execution.Write(e.EffectNodes);
		}

		private bool _AppSupportsCommands {
			get { return _application != null && _application.AppCommands != null; }
		}

		private AppCommand _GetToolsMenu() {
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if(toolsMenu == null) {
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
	}
}
