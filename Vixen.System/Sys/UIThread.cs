using System;
using System.Threading;
using System.Windows.Forms;

namespace Vixen.Sys
{
	internal class UIThread
	{
		private Thread _thread;
		private Form _form;
		private ApplicationContext _applicationContext;
		private Action _exitAction;
		private Func<Form> _formThreadInit;
		private Func<ApplicationContext> _applicationContextThreadInit;
		private WindowsFormsSynchronizationContext _synchronizationContext;

		private ManualResetEvent _startupFinished;
		private const int START_TIMEOUT = 5000;	// 5 seconds to wait for the thread to start

		public UIThread(Func<ApplicationContext> threadInit)
		{
			if (threadInit == null) throw new ArgumentNullException();
			_applicationContextThreadInit = threadInit;

			_thread = new Thread(_ApplicationContextThread) {Name = "Application context thread"};
			_thread.SetApartmentState(ApartmentState.STA);

			_startupFinished = new ManualResetEvent(false);

			_exitAction = _ApplicationContextExit;
		}

		public UIThread(Func<Form> threadInit)
		{
			if (threadInit == null) throw new ArgumentNullException();
			_formThreadInit = threadInit;

			_thread = new Thread(_FormThread) {Name = "Form thread"};
			_thread.SetApartmentState(ApartmentState.STA);

			_startupFinished = new ManualResetEvent(false);

			_exitAction = _FormExit;
		}

		public void Start()
		{
			_startupFinished.Reset();
			if (_thread.ThreadState != ThreadState.Running) {
				_thread.Start();
				if (!_startupFinished.WaitOne(START_TIMEOUT)) {
					VixenSystem.Logging.Error("UIThread: timed out waiting for thread to start");
				}
			}
		}

		public void Stop()
		{
			if (_thread != null && _thread.ThreadState == ThreadState.Running) {
				_exitAction();
			}
		}

		public ThreadState ThreadState
		{
			get { return _thread.ThreadState; }
		}

		public void BeginInvoke(Action methodToInvoke)
		{
			if (_synchronizationContext == null) {
				VixenSystem.Logging.Debug("UIThread: BeginInvoke called before a _synchronizationContext has been created");
				return;
			}

			_synchronizationContext.Post(o => methodToInvoke(), null);
		}

		private void _FormThread()
		{
			_form = _formThreadInit();
			if (_form == null) throw new InvalidOperationException();

			_synchronizationContext = new WindowsFormsSynchronizationContext();
			_applicationContext = new ApplicationContext(_form);
			_startupFinished.Set();
			Application.Run(_applicationContext);
		}

		private void _ApplicationContextThread()
		{
			_applicationContext = _applicationContextThreadInit();
			if (_applicationContext == null) throw new InvalidOperationException();

			_synchronizationContext = new WindowsFormsSynchronizationContext();
			_startupFinished.Set();
			Application.Run(_applicationContext);
		}

		private void _FormExit()
		{
			_ApplicationContextExit();
		}

		private void _ApplicationContextExit()
		{
			_applicationContext.ExitThread();
		}
	}
}