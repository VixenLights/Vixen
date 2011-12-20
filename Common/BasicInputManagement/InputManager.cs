using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.Input;
using Vixen.Sys;

namespace BasicInputManagement {
	public class InputManager {
		private List<IInputModuleInstance> _inputModules;
		private List<InputEffectMap> _inputEffects;

		public event EventHandler<InputsChangedEventArgs> InputsChanged;

		public InputManager(IEnumerable<IInputModuleInstance> inputModules, IEnumerable<InputEffectMap> inputEffects) {
			_inputModules = inputModules.ToList();
			_inputEffects = inputEffects.ToList();
		}

		public TimeSpan EffectTimeSpan = TimeSpan.FromMilliseconds(50);

		public void Start() {
			_StartModules(_inputModules);
		}

		public void Stop() {
			_StopModules(_inputModules);
		}

		public DialogResult ShowForm() {
			using(SetupForm setupForm = new SetupForm(_inputEffects, _inputModules)) {
				IInputModuleInstance[] inputModules = _inputModules.ToArray();
				if(setupForm.ShowDialog() == DialogResult.OK) {
					_inputModules = setupForm.InputModules.ToList();
					_inputEffects = setupForm.Maps.ToList();

					// Turn off any removed modules.
					var removedModules = inputModules.Except(_inputModules);
					_StopModules(removedModules);

					// Turn on any added modules.
					var addedModules = _inputModules.Where(x => !x.IsRunning);
					_StartModules(addedModules);
				}
				return setupForm.DialogResult;
			}
		}

		public IEnumerable<IInputModuleInstance> InputModules {
			get { return _inputModules; }
		}

		public IEnumerable<InputEffectMap> InputEffects {
			get { return _inputEffects; }
		}

		protected virtual void OnInputsChanged(InputsChangedEventArgs e) {
			if(InputsChanged != null) {
				InputsChanged(this, e);
			}
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
			IEnumerable<InputEffectMap> inputEffects = _inputEffects.Where(x => x.IsMappedTo(e.InputModule, e.Input));
			IEnumerable<EffectNode> effectNodes = inputEffects.Select(x => x.GenerateEffect(e.Input, EffectTimeSpan));
			OnInputsChanged(new InputsChangedEventArgs(effectNodes));
		}
	}
}
