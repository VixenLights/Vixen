using System;

namespace Vixen.Sys.Output {
	abstract public class OutputDeviceBase : IOutputDevice {
		private bool _isRunning;

		public void Start() {
			if(!IsRunning) {
				_isRunning = true;
				_Start();
			}
		}
		abstract protected void _Start();

		//public void Pause() {
		//    if(IsRunning) {
		//        _Pause();
		//    }
		//}
		//abstract protected void _Pause();

		//public void Resume() {
		//    if(IsRunning) {
		//        _Resume();
		//    }
		//}
		//abstract protected void _Resume();

		public void Stop() {
			if(IsRunning) {
				_Stop();
				_isRunning = false;
			}
		}
		abstract protected void _Stop();

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() {
			return false;
		}

		public Guid Id { get; protected set; }

		virtual public string Name { get; set; }

		abstract public int UpdateInterval { get; }

		virtual public bool IsRunning {
			get { return _isRunning; }
		}

		public void Update() {
			// First, get what we pull from to update...
			Execution.UpdateState();
			// Then we update ourselves from that.
			_UpdateState();
		}
		abstract protected void _UpdateState();
	}
}
