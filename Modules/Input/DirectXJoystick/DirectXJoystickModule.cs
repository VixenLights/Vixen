using System;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Input;

namespace VixenModules.Input.DirectXJoystick {
	public class DirectXJoystickModule : InputModuleInstanceBase {
		// The joystick this instance represents, of the available ones.
		private Joystick _joystick;
		private DirectXJoystickData _data;

		public override IInputInput[] Inputs {
			get {
				if(_Joystick != null) {
					return _Joystick.Inputs;
				}
				return new IInputInput[0];
			}
		}

		public override string DeviceName {
			get {
				if(_Joystick != null) {
					return _Joystick.DeviceName;
				}
				return null;
			}
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as DirectXJoystickData; }
		}

		protected override void DoStartup() {
			if(_Joystick != null && !_Joystick.IsAcquired) {
				_Joystick.Acquire();
			}
		}

		protected override void DoShutdown() {
			if(_JoystickReady) {
				_Joystick.Release();
			}
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(SetupForm setupForm = new SetupForm(_data)) {
				if(setupForm.ShowDialog() == DialogResult.OK) {
					_joystick = null;
					return true;
				}
			}
			return false;
		}

		private bool _JoystickReady {
			get { return _Joystick != null && _Joystick.IsAcquired; }
		}

		private Joystick _Joystick {
			get {
				if(_joystick == null && _data.DeviceId != Guid.Empty) {
					try {
						_joystick = new Joystick(_data.DeviceId);
					} catch {
						_joystick = null;
					}
				}
				return _joystick;
			}
		}
	}
}
