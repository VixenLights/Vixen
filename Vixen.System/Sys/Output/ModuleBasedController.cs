using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vixen.Module;

namespace Vixen.Sys.Output {
	abstract public class ModuleBasedController<T, U> : OutputDeviceBase
		where T : class, IOutputModule, IHasOutputs, IHardwareModule 
		where U : Output, new() {
		private Guid _moduleId;
		private T _module;
		private ModuleLocalDataSet _dataSet = new ModuleLocalDataSet();
		private OutputCollection<U> _outputs;

		protected ModuleBasedController(string name, int outputCount, Guid moduleId)
			: this(Guid.NewGuid(), name, outputCount, moduleId) {
		}

		protected ModuleBasedController(Guid id, string name, int outputCount, Guid moduleId)
			: base(id, name) {
			_outputs = new OutputCollection<U>(Id);
			ModuleId = moduleId;
			OutputCount = outputCount;
		}


		override protected void _Start() {
			if(Module != null) {
				Module.Start();
			}
		}

		override protected void _Stop() {
			if(Module != null) {
				Module.Stop();
			}
		}

		protected override void _Pause() {
			if(Module != null) {
				Module.Pause();
			}
		}

		protected override void _Resume() {
			if(Module != null) {
				Module.Resume();
			}
		}

		// Must be a property for data binding.
		public Guid ModuleId {
			get { return _moduleId; }
			set {
				_moduleId = value;
				_module = null;
			}
		}

		public T Module {
			get {
				if(_module == null) {
					_module = GetControllerModule(_moduleId);

					if(_module != null) {
						_SetOutputModuleOutputCount();
						_SetModuleData();
						ResetUpdateInterval();
					}
				}
				return _module;
			}
		}

		abstract protected T GetControllerModule(Guid moduleId);

		protected void BeginOutputChange() {
			Monitor.Enter(_outputs);
		}

		protected void EndOutputChange() {
			Monitor.Exit(_outputs);
		}

		protected void UpdateOutputStates(Action<U> outputUpdateAction) {
			_outputs.UpdateState(outputUpdateAction);
		}

		protected IEnumerable<V> ExtractFromOutputs<V>(Func<U,V> outputPropertySelector) {
			return _outputs.Select(outputPropertySelector);
		}

		private void _SetOutputModuleOutputCount() {
			if(_module != null && OutputCount != 0) {
				_module.OutputCount = OutputCount;
			}
		}

		private void _SetModuleData() {
			if(_module != null && ModuleDataSet != null) {
				ModuleDataSet.AssignModuleTypeData(_module);
			}
		}

		public ModuleLocalDataSet ModuleDataSet {
			get { return _dataSet; }
			set {
				_dataSet = value;
				_SetModuleData();
			}
		}

		public U[] Outputs {
			get { return _outputs.AsArray; }
		}

		public int OutputCount {
			get { return _outputs.Count; }
			set {
				lock(_outputs) {
					_outputs.Count = value;
				}
				_SetOutputModuleOutputCount();
			}
		}

		override public bool HasSetup {
			get { return _module.HasSetup; }
		}

		/// <summary>
		/// Runs the controller setup.
		/// </summary>
		/// <returns>True if the setup was successful and committed.  False if the user canceled.</returns>
		override public bool Setup() {
			if(_module != null) {
				if(_module.Setup()) {
					return true;
				}
			}
			return false;
		}

		public void AddSources(IOutputSourceCollection sources) {
			_outputs.AddSources(sources);
		}

		public void RemoveSources(IOutputSourceCollection sources) {
			_outputs.RemoveSources(sources);
		}

		public void ReloadSources() {
			_outputs.ReloadSources();
		}

		public void ReloadOutputSources(int outputIndex) {
			_outputs.ReloadOutputSources(outputIndex);
		}

		public void ClearSources(int outputIndex) {
			_outputs.ClearSources(outputIndex);
		}

		override public bool IsRunning {
			get { return _module != null && _module.IsRunning; }
		}

		public override bool IsPaused {
			get { return _module != null && _module.IsPaused; }
		}

		public void ResetUpdateInterval() {
			if(Module != null) {
				UpdateInterval = Module.UpdateInterval;
			}
		}

		public override string ToString() {
			return Name;
		}
	}
}
