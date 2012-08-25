using System;
using System.Threading;
using System.Windows.Forms;
using Vixen.Sys;

namespace Vixen.Module.Preview {
	public class MultiThreadBehavior : IThreadBehavior {
		private Func<UIThread> _threadInit;
		private UIThread _thread;
		private Func<Form> _formInit;
		private Func<ApplicationContext> _appContextInit;

		public MultiThreadBehavior(Func<Form> formInit) {
			if(formInit == null) throw new InvalidOperationException();
			_formInit = formInit;
			_threadInit = _GetFormThread;
		}

		public MultiThreadBehavior(Func<ApplicationContext> appContextInit) {
			if(appContextInit == null) throw new InvalidOperationException();
			_appContextInit = appContextInit;
			_threadInit = _GetApplicationContextThread;
		}

		public void Start() {
			_thread = _threadInit();
			if(_thread != null) {
				_thread.Start();
			}
		}

		public void Stop() {
			if(_thread != null) {
				_thread.Stop();
			}
		}

		public bool IsRunning {
			get { return _thread != null && _thread.ThreadState == ThreadState.Running; }
		}

		public void BeginInvoke(Action methodToInvoke) {
			_thread.BeginInvoke(methodToInvoke);
		}

		private UIThread _GetFormThread() {
			return new UIThread(_formInit);
		}

		private UIThread _GetApplicationContextThread() {
			return new UIThread(_appContextInit);
		}
	}
}
