using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SlimDX.DirectInput;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Input;
using VixenModules.Input.DirectXJoystick.Input;
using Vixen.Sys;

namespace VixenModules.Input.DirectXJoystick {
	class Joystick {
		private SlimDX.DirectInput.Joystick _device;
		private AutoResetEvent _waitHandle;

		public Joystick(Guid deviceId) {
			DeviceId = deviceId;
			DeviceName = _GetDeviceName(deviceId);
		}

		static public Joystick[] AllJoysticks() {
			using(DirectInput directInput = new DirectInput()) {
				IList<DeviceInstance> devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
				return devices.Select(x => new Joystick(x.InstanceGuid)).ToArray();
			}
		}

		public string DeviceName { get; private set; }

		public IInputInput[] Inputs { get; private set; }

		public bool IsAcquired { get; private set; }

		public Guid DeviceId { get; private set; }

		public void Acquire() {
			if(!IsAcquired) {
				_waitHandle = new AutoResetEvent(false);
				_device = _CreateDevice();
				// Must be done before acquisition.
				_device.SetNotification(_waitHandle);
				_device.Acquire();
				// Must be set before any notifications come through.
				IsAcquired = true;
				new Thread(_UpdateThread).Start();
			}
		}

		public void Release() {
			if(_device != null && IsAcquired) {
				lock(_device) {
					_device.Unacquire();
					IsAcquired = false;
					_waitHandle.Set();
				}
			}
		}

		private void _UpdateThread() {
			while(IsAcquired) {
				lock(_device) {
					_UpdateState();
				}
				_waitHandle.WaitOne();
			}

			_device.Dispose();
			_device = null;

			_waitHandle.Close();
			_waitHandle.Dispose();
			_waitHandle = null;
		}

		private string _GetDeviceName(Guid instanceGuid) {
			using(DirectInput directInput = new DirectInput()) {
				IList<DeviceInstance> devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
				DeviceInstance deviceInstance = devices.FirstOrDefault(x => x.InstanceGuid == instanceGuid);
				return (deviceInstance != null) ? deviceInstance.InstanceName : null;
			}
		}

		private SlimDX.DirectInput.Joystick _CreateDevice() {
			SlimDX.DirectInput.Joystick device;

			using(DirectInput directInput = new DirectInput()) {
				// Try to get the device specified.
				DeviceInstance deviceInstance = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).FirstOrDefault(x => x.InstanceGuid == DeviceId);

				if(deviceInstance == null) {
					throw new Exception("Specified joystick not found.");
				}

				device = new SlimDX.DirectInput.Joystick(directInput, deviceInstance.InstanceGuid);
				// This needs System.Windows.Forms.Control.
				device.SetCooperativeLevel(IntPtr.Zero, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
				device.Properties.SetRange(Position.MinValue, Position.MaxValue);

				Inputs = _GetInputs(device).ToArray();
			}

			return device;
		}

		private void _UpdateState() {
			if(IsAcquired) {
				JoystickState currentState = _device.GetCurrentState();
				foreach(JoystickInput input in Inputs) {
					input.Update(currentState);
				}
			}
		}

		private bool _IsDeviceType(DeviceObjectInstance deviceObjectInstance, ObjectDeviceType objectDeviceType) {
			return (deviceObjectInstance.ObjectType & objectDeviceType) != 0;
		}

		private bool _IsAxis(DeviceObjectInstance deviceObjectInstance) {
			return _IsDeviceType(deviceObjectInstance, ObjectDeviceType.Axis);
		}

		private bool _IsButton(DeviceObjectInstance deviceObjectInstance) {
			return _IsDeviceType(deviceObjectInstance, ObjectDeviceType.Button);
		}

		private bool _IsPov(DeviceObjectInstance deviceObjectInstance) {
			return _IsDeviceType(deviceObjectInstance, ObjectDeviceType.PointOfViewController);
		}

		private IEnumerable<IInputInput> _GetInputs(SlimDX.DirectInput.Joystick device) {
			foreach(DeviceObjectInstance deviceObjectInstance in device.GetObjects()) {
				JoystickInput input = null;

				if(_IsButton(deviceObjectInstance)) {
					input = new Button(deviceObjectInstance.Name, deviceObjectInstance.Usage);
				} else if(_IsAxis(deviceObjectInstance)) {
					input = _IsRotationAxis(deviceObjectInstance) ? 
						_CreateRotationAxis(deviceObjectInstance) : 
						_CreateAxis(deviceObjectInstance);
				} else if(_IsPov(deviceObjectInstance)) {
					input = new Pov(deviceObjectInstance.Name, deviceObjectInstance.DesignatorIndex);
				}

				if(input != null) {
					yield return input;
				}
			}
		}

		private JoystickInput _CreateAxis(DeviceObjectInstance deviceObjectInstance) {
			switch(char.ToLower(deviceObjectInstance.Name[0])) {
				case 'x':
					return new XAxis(deviceObjectInstance.Name);
				case 'y':
					return new YAxis(deviceObjectInstance.Name);
				case 'z':
					return new ZAxis(deviceObjectInstance.Name);
			}
			return null;
		}

		private JoystickInput _CreateRotationAxis(DeviceObjectInstance deviceObjectInstance) {
			switch(char.ToLower(deviceObjectInstance.Name[0])) {
				case 'x':
					return new XRotationalAxis(deviceObjectInstance.Name);
				case 'y':
					return new YRotationalAxis(deviceObjectInstance.Name);
				case 'z':
					return new ZRotationalAxis(deviceObjectInstance.Name);
			}
			return null;
		}

		private bool _IsRotationAxis(DeviceObjectInstance deviceObjectInstance) {
			return deviceObjectInstance.Name.ContainsString("rotation");
		}

	}
}
