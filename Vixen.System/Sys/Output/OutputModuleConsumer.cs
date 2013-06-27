using System;
using Vixen.Module;

namespace Vixen.Sys.Output
{
	internal class OutputModuleConsumer<T> : ModuleConsumer<T>, IOutputModuleConsumer<T>
		where T : class, IOutputModule
	{
		private T _outputModule;

		public OutputModuleConsumer(Guid moduleId, Guid moduleInstanceId, IModuleDataRetriever moduleDataRetriever)
			: base(moduleId, moduleInstanceId, moduleDataRetriever)
		{
		}

		public override T Module
		{
			get
			{
				_outputModule = base.Module;
				return _outputModule;
			}
		}

		public int UpdateInterval
		{
			get
			{
				if (_outputModule != null) {
					return _outputModule.UpdateInterval;
				}
				return 0;
			}
		}

		public void Start()
		{
			if (_outputModule != null) _outputModule.Start();
		}

		public void Stop()
		{
			if (_outputModule != null) _outputModule.Stop();
		}

		public void Pause()
		{
			if (_outputModule != null) _outputModule.Pause();
		}

		public void Resume()
		{
			if (_outputModule != null) _outputModule.Resume();
		}

		public bool IsRunning
		{
			get { return _outputModule != null && _outputModule.IsRunning; }
		}

		public bool IsPaused
		{
			get { return _outputModule != null && _outputModule.IsPaused; }
		}

		public bool HasSetup
		{
			get { return _outputModule != null && _outputModule.HasSetup; }
		}

		public bool Setup()
		{
			if (_outputModule != null) {
				return _outputModule.Setup();
			}
			return false;
		}

		public IOutputDeviceUpdateSignaler UpdateSignaler
		{
			get
			{
				if (_outputModule != null) {
					return _outputModule.UpdateSignaler;
				}
				return null;
			}
		}
	}
}