using System;
using Vixen.Module;

namespace Vixen.Sys.Output {
	class OutputModuleConsumer : ModuleConsumer, IOutputModuleConsumer {
		private IOutputModule _outputModule;

		public OutputModuleConsumer(Guid moduleId, IModuleDataRetriever moduleDataRetriever)
			: base(moduleId, moduleDataRetriever) {
		}

		public override IModuleInstance Module {
			get {
				//*** will this go through the appropriate manager?
				_outputModule = (IOutputModule)base.Module;
				return _outputModule;
			}
		}

		public int UpdateInterval {
			get {
				if(_outputModule != null) {
					return _outputModule.UpdateInterval;
				}
				return 0;
			}
		}

		public void Start() {
			throw new NotImplementedException();
		}

		public void Stop() {
			throw new NotImplementedException();
		}

		public void Pause() {
			throw new NotImplementedException();
		}

		public void Resume() {
			throw new NotImplementedException();
		}

		public bool IsRunning {
			get { throw new NotImplementedException(); }
		}

		public bool IsPaused {
			get { throw new NotImplementedException(); }
		}

		public bool HasSetup {
			get { throw new NotImplementedException(); }
		}

		public bool Setup() {
			throw new NotImplementedException();
		}

		public IOutputDeviceUpdateSignaler UpdateSignaler {
			get { return _outputModule.UpdateSignaler; }
		}
	}
}
