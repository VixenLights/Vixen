using System.Windows;

using Vixen.Sys;

namespace Vixen.Module.Preview
{

	public class WPFMultiThreadBehavior : IThreadBehavior
	{
		private Func<WPFUIThread> _threadInit;
		private WPFUIThread _thread;
		private Func<Window> _windowInit;
		private Func<ApplicationContext> _appContextInit;

		public WPFMultiThreadBehavior(Func<Window> windowInit)
		{
			if (windowInit == null) throw new InvalidOperationException();
			_windowInit = windowInit;
			_threadInit = _GetFormThread;
		}

		public WPFMultiThreadBehavior(Func<ApplicationContext> appContextInit)
		{
			if (appContextInit == null) throw new InvalidOperationException();
			_appContextInit = appContextInit;
			_threadInit = _GetApplicationContextThread;
		}

		public void Start()
		{
			_thread = _threadInit();
			if (_thread != null)
			{
				_thread.Start();
			}
		}

		public void Stop()
		{
			if (_thread != null)
			{
				_thread.Stop();
			}
		}

		public bool IsRunning
		{
			get { return _thread != null && _thread.ThreadState == ThreadState.Running; }
		}

		public void BeginInvoke(Action methodToInvoke)
		{
			_thread.BeginInvoke(methodToInvoke);
		}

		private WPFUIThread _GetFormThread()
		{
			return new WPFUIThread(_windowInit);
		}

		private WPFUIThread _GetApplicationContextThread()
		{
			return new WPFUIThread(_appContextInit);
		}		
	}	
}
