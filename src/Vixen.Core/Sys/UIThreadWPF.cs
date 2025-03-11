using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

using NLog;

namespace Vixen.Sys
{
	internal class UIThreadWPF
	{
		private Thread _thread;
		private Window _window;
		private ApplicationContext _applicationContext;
		private Action _exitAction;
        private Func<Window> _windowThreadInit;
		private Func<ApplicationContext> _applicationContextThreadInit;
		private SynchronizationContext _synchronizationContext;
        private static Logger Logging = LogManager.GetCurrentClassLogger();

		private ManualResetEvent _startupFinished;
		private const int START_TIMEOUT = 5000; // 5 seconds to wait for the thread to start

		public UIThreadWPF(Func<ApplicationContext> threadInit)
		{
			if (threadInit == null) throw new ArgumentNullException();
			_applicationContextThreadInit = threadInit;

			_thread = new Thread(_ApplicationContextThread) { Name = "Application context thread" };
			_thread.SetApartmentState(ApartmentState.STA);

			_startupFinished = new ManualResetEvent(false);

			_exitAction = _ApplicationContextExit;
		}

		public UIThreadWPF(Func<Window> threadInit)
		{
			if (threadInit == null) throw new ArgumentNullException();
            _windowThreadInit = threadInit;

            _thread = new Thread(_WindowThread) { Name = "Window thread" };
			_thread.SetApartmentState(ApartmentState.STA);

			_startupFinished = new ManualResetEvent(false);

            _exitAction = _WindowExit;
		}

		public void Start()
		{
			_startupFinished.Reset();
			if (_thread.ThreadState != ThreadState.Running)
			{
				_thread.Start();
				if (!_startupFinished.WaitOne(START_TIMEOUT))
				{
					Logging.Error("UIThread: timed out waiting for thread to start");
				}
			}
		}

		public void Stop()
		{
			if (_thread != null && _thread.ThreadState == ThreadState.Running)
			{
				_exitAction();
			}
		}

		public ThreadState ThreadState
		{
			get { return _thread.ThreadState; }
		}

		public void BeginInvoke(Action methodToInvoke)
		{
			if (_synchronizationContext == null)
			{
				Logging.Debug("UIThread: BeginInvoke called before a _synchronizationContext has been created");
				return;
			}

			_synchronizationContext.Post(o => methodToInvoke(), null);
		}

        private void _WindowThread()
		{
            _window = _windowThreadInit();
            if (_window == null) throw new InvalidOperationException();

            _synchronizationContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);
			_startupFinished.Set();
            _window.Show();
            Dispatcher.Run();
		}

		private void _ApplicationContextThread()
		{
			_applicationContext = _applicationContextThreadInit();
			if (_applicationContext == null) throw new InvalidOperationException();

			_synchronizationContext = new WindowsFormsSynchronizationContext();
			_startupFinished.Set();
			System.Windows.Forms.Application.Run(_applicationContext);
		}

        private void _WindowExit()
        {
			
			_window.Dispatcher.Invoke(new Action(() =>
				{
					((IDisposable)_window).Dispose();
				}));

			_window.Dispatcher.InvokeShutdown();
        }

		private void _ApplicationContextExit()
		{
			_applicationContext.ExitThread();
		}
	}
}
