using System.Windows;


namespace Vixen.Module.Preview
{
	public class WPFSingleThreadBehavior : IThreadBehavior
	{
		private Func<Window> _windowInit;
		private Window _window;

		public WPFSingleThreadBehavior(Func<Window> formInit)
		{
			if (formInit == null) throw new InvalidOperationException();
			_windowInit = formInit;
		}

		public void Start()
		{
			_window = _windowInit();
			_window.Show();
		}

		public void Stop()
		{
			_window.Close();
			((IDisposable)_window).Dispose();
			_window = null;
		}

		public bool IsRunning
		{
			get { return _window != null && _window.IsVisible; }
		}

		public void BeginInvoke(Action methodToInvoke)
		{
			_window.Dispatcher.BeginInvoke(methodToInvoke);
		}
	}	
}