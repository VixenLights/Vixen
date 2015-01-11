using System;
using System.Collections.Generic;
using System.Threading;

namespace Vixen.Module.Input
{
	/// <summary>
	/// Base class for trigger module implementations.
	/// </summary>
	public abstract class InputModuleInstanceBase : ModuleInstanceBase, IInputModuleInstance,
	                                                IEquatable<InputModuleInstanceBase>, IEquatable<IInputModuleInstance>,
	                                                IEqualityComparer<IInputModuleInstance>,
	                                                IEqualityComparer<InputModuleInstanceBase>
	{
		//private Thread _stateUpdateThread;
		private ManualResetEvent _pause = new ManualResetEvent(true);

		public event EventHandler<InputValueChangedEventArgs> InputValueChanged;

		public abstract IInputInput[] Inputs { get; }

		public abstract string DeviceName { get; }

		public void Start()
		{
			if (!IsRunning) {
				// Call the subclass first in case its startup creates the triggers.
				DoStartup();
				// Subscribe to triggers.
				foreach (IInputInput input in Inputs) {
					input.ValueChanged += _InputValueChanged;
				}
				// Start monitoring the hardware.
				//_stateUpdateThread = new Thread(_StateUpdate);
				//_stateUpdateThread.IsBackground = true;
				//_stateUpdateThread.Start();
				IsRunning = true;
			}
		}

		protected virtual void DoStartup()
		{
		}

		public void Stop()
		{
			// In opposite order of Startup...
			if (IsRunning) {
				// Stop monitoring the hardware.
				Resume();
				IsRunning = false;
				// Unsubscribe to triggers.
				foreach (IInputInput input in Inputs) {
					input.ValueChanged -= _InputValueChanged;
				}
				// Notify the subclass.
				DoShutdown();
			}
		}

		protected virtual void DoShutdown()
		{
		}

		public void Pause()
		{
			if (!IsPaused) {
				IsPaused = true;
				_pause.Reset();
				DoPause();
			}
		}

		protected virtual void DoPause()
		{
		}

		public void Resume()
		{
			if (IsPaused) {
				IsPaused = false;
				_pause.Set();
				DoResume();
			}
		}

		protected virtual void DoResume()
		{
		}

		public bool IsRunning { get; private set; }

		public bool IsPaused { get; private set; }

		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			
			if (disposing)
			{
				Stop();
				if (_pause != null)
				{
					_pause.Dispose();
					_pause = null;
				}	
			}
			
		}

		~InputModuleInstanceBase()
		{
			Dispose(false);
		}

		private void _InputValueChanged(object sender, EventArgs e)
		{
			if (InputValueChanged != null) {
				InputValueChanged(this, new InputValueChangedEventArgs(this, sender as IInputInput));
			}
		}

		//private void _StateUpdate() {
		//    IsRunning = true;
		//    while(IsRunning) {
		//        UpdateState();
		//        Thread.Sleep(5);
		//        _pause.WaitOne();
		//    }
		//    _stateUpdateThread = null;
		//}

		public bool Equals(InputModuleInstanceBase other)
		{
			return Equals(other as IInputModuleInstance);
		}

		public bool Equals(IInputModuleInstance other)
		{
			return base.Equals(this, other);
		}

		public bool Equals(IInputModuleInstance x, IInputModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IInputModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(InputModuleInstanceBase x, InputModuleInstanceBase y)
		{
			return Equals(x as IInputModuleInstance, y as IInputModuleInstance);
		}

		public int GetHashCode(InputModuleInstanceBase obj)
		{
			return GetHashCode(obj as IInputModuleInstance);
		}
	}
}