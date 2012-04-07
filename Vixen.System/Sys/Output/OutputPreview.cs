using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Preview;

namespace Vixen.Sys.Output {
	public class OutputPreview : OutputDeviceBase {
		private Guid _moduleId;
		private IPreviewModuleInstance _module;
		private ModuleLocalDataSet _dataSet = new ModuleLocalDataSet();

		public OutputPreview(string name, Guid moduleId)
			: this(Guid.NewGuid(), name, moduleId) {
		}

		public OutputPreview(Guid id, string name, Guid moduleId)
			: base(id, name) {
			ModuleId = moduleId;
		}

		protected override void _Start() {
			if(Module != null) {
				Module.Start();
			}
		}

		protected override void _Stop() {
			if(Module != null) {
				Module.Stop();
			}
		}

		protected override void _UpdateState() {
			if(Module != null && DataPolicy != null) {
				ChannelCommands channelCommands = new ChannelCommands(VixenSystem.Channels.ToDictionary(x => x.Id, x => DataPolicy.GenerateCommand(x.State)));
				Module.UpdateState(channelCommands);
			}
		}

		public Guid ModuleId {
			get { return _moduleId; }
			set {
				_moduleId = value;
				_module = null;
			}
		}

		public IPreviewModuleInstance Module {
			get {
				if(_module == null) {
					_module = Modules.ModuleManagement.GetPreview(_moduleId);

					if(_module != null) {
						_SetModuleData();
						ResetUpdateInterval();
						ResetDataPolicy();
					}
				}
				return _module;
			}
		}

		public ModuleLocalDataSet ModuleDataSet {
			get { return _dataSet; }
			set {
				_dataSet = value;
				_SetModuleData();
			}
		}

		override public bool IsRunning {
			get { return _module != null && _module.IsRunning; }
		}

		public void ResetUpdateInterval() {
			if(Module != null) {
				UpdateInterval = Module.UpdateInterval;
			}
		}

		public void ResetDataPolicy() {
			if(Module != null) {
				DataPolicy = Module.DataPolicy;
			}
		}

		public override string ToString() {
			return Name;
		}

		private void _SetModuleData() {
			if(_module != null && ModuleDataSet != null) {
				ModuleDataSet.AssignModuleTypeData(_module);
			}
		}
	}
}
