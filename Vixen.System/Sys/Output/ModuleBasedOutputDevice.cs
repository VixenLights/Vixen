using System;
using Vixen.Module;

namespace Vixen.Sys.Output {
	public abstract class ModuleBasedOutputDevice<T> : OutputDeviceBase
		where T : class, IOutputModule, IHardwareModule {
		private Guid _moduleId;
		private T _module;
		private ModuleLocalDataSet _dataSet = new ModuleLocalDataSet();

		protected ModuleBasedOutputDevice(string name, Guid moduleId)
			: this(Guid.NewGuid(), name, moduleId) {
		}

		protected ModuleBasedOutputDevice(Guid id, string name, Guid moduleId)
			: base(id, name) {
			ModuleId = moduleId;
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
					_module = GetModule(_moduleId);

					if(_module != null) {
						_SetModuleData();
						ResetUpdateInterval();
					}
				}
				return _module;
			}
		}

		abstract protected T GetModule(Guid moduleId);

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
