using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Module.Effect;
using Vixen.Module.Input;
using Vixen.Sys;

namespace VixenModules.App.InputEffectRouter {
	public class InputEffectRouterModule : AppModuleInstanceBase {
		private IApplication _application;
		private InputEffectRouterData _data;

		private const string ROOT_ID = "InputEffectRouter_Root";

		public override void Loading() {
			if(_AppSupportsCommands) {
				AppCommand toolsMenu = _GetToolsMenu();

				AppCommand rootMenuItem = new AppCommand(ROOT_ID, "Input Effect Router");
				rootMenuItem.Click += (sender, e) => {
					using(SetupForm setupForm = new SetupForm(_data.Map, _data.InputModules)) {
						IInputModuleInstance[] inputModules = _data.InputModules.ToArray();

						if(setupForm.ShowDialog() == DialogResult.OK) {
							_data.Map = setupForm.Maps;
							_data.InputModules = setupForm.InputModules;

							// Turn off any removed modules.
							var removedModules = inputModules.Except(_data.InputModules);
							_StopModules(removedModules);

							// Turn on any added modules.
							var addedModules = _data.InputModules.Where(x => !x.IsRunning);
							_StartModules(addedModules);
						}
					}
				};
				
				toolsMenu.Add(rootMenuItem);

				_StartModules(_data.InputModules);
			}
		}

		public override void Unloading() {
			if(_AppSupportsCommands) {
				_StopModules(_data.InputModules);

				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(ROOT_ID);
			}
		}

		public override IApplication Application {
			set { _application = value; }
		}

		public override IModuleDataModel StaticModuleData {
			get { return _data; }
			set { _data = value as InputEffectRouterData; }
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

		private void _StartModules(IEnumerable<IInputModuleInstance> inputModules) {
			foreach(IInputModuleInstance inputModule in inputModules) {
				inputModule.Start();
				inputModule.InputValueChanged += inputModule_InputValueChanged;
			}
		}

		private void _StopModules(IEnumerable<IInputModuleInstance> inputModules) {
			foreach(IInputModuleInstance inputModule in inputModules) {
				inputModule.InputValueChanged -= inputModule_InputValueChanged;
				inputModule.Stop();
			}
		}

		private void inputModule_InputValueChanged(object sender, InputValueChangedEventArgs e) {
			IEnumerable<InputEffectMap> inputEffects = _data.Map.Where(x => x.IsMappedTo(e.InputModule, e.Input));

			List<EffectNode> effectNodes = new List<EffectNode>();
			foreach(InputEffectMap inputEffect in inputEffects) {
				IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(inputEffect.EffectModuleId);
				inputEffect.EffectParameterValues[inputEffect.InputValueParameterIndex] = e.Input.Value;
				effect.ParameterValues = inputEffect.EffectParameterValues;
				effect.TimeSpan = TimeSpan.FromMilliseconds(50);
				effect.TargetNodes = inputEffect.Nodes.Select(x => VixenSystem.Nodes.FirstOrDefault(y => y.Id == x)).ToArray();
				EffectNode effectNode = new EffectNode(effect, TimeSpan.Zero);
				effectNodes.Add(effectNode);
			}
			Execution.Write(effectNodes);
		}
	}
}
