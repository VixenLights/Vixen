using System.Threading;

namespace Vixen.Sys.Output {
	class OutputMediator<T> : IOutputMediator<T>
		where T : Output {
		private IHasOutputs<T> _outputCollection;
		private IUpdatableOutputCount _outputModule;

		public OutputMediator(IHasOutputs<T> outputCollection, IUpdatableOutputCount outputModule) {
			_outputCollection = outputCollection;
			_outputModule = outputModule;
		}

		public void AddOutput(T output) {
			_outputCollection.AddOutput(output);
			_outputModule.OutputCount = _outputCollection.OutputCount;
		}

		void IHasOutputs.AddOutput(Output output) {
			AddOutput((T)output);
		}

		public void RemoveOutput(T output) {
			_outputCollection.RemoveOutput(output);
			_outputModule.OutputCount = _outputCollection.OutputCount;
		}

		void IHasOutputs.RemoveOutput(Output output) {
			RemoveOutput((T)output);
		}

		public int OutputCount {
			get { return _outputCollection.OutputCount; }
		}

		public void LockOutputs() {
			Monitor.Enter(_outputCollection);
		}

		public void UnlockOutputs() {
			Monitor.Exit(_outputCollection);
		}

		public T[] Outputs {
			get { return _outputCollection.Outputs; }
		}

		Output[] IHasOutputs.Outputs {
			get { return Outputs; }
		}
	}
}
