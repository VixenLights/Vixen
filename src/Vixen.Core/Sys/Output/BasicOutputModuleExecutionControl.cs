namespace Vixen.Sys.Output
{
	internal class BasicOutputModuleExecutionControl : IHardware
	{
		private IOutputModule _outputModule;

		public BasicOutputModuleExecutionControl(IOutputModule outputModule)
		{
			_outputModule = outputModule;
		}

		public void Start()
		{
			if (!IsRunning) {
				IsRunning = true;
				IsPaused = false;
				_Start();
			}
		}

		public void Stop()
		{
			if (IsRunning) {
				_Stop();
				IsRunning = false;
				IsPaused = false;
			}
		}

		public void Pause()
		{
			if (IsRunning && !IsPaused) {
				IsPaused = true;
				_Pause();
			}
		}

		public void Resume()
		{
			if (IsRunning && IsPaused) {
				IsPaused = false;
				_Resume();
			}
		}

		public bool IsRunning { get; private set; }

		public bool IsPaused { get; private set; }

		private void _Start()
		{
			_outputModule.Start();
		}

		private void _Stop()
		{
			_outputModule.Stop();
		}

		private void _Pause()
		{
			_outputModule.Pause();
		}

		private void _Resume()
		{
			_outputModule.Resume();
		}
	}
}