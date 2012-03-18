using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module;

namespace VixenTestbed {
	[DefaultEvent("SelectedModuleChanged")]
	public partial class ModuleList : UserControl {
		private Func<Dictionary<Guid, string>> _getMethod;
		private Dictionary<Guid, IModuleInstance> _moduleCache = new Dictionary<Guid, IModuleInstance>();

		public event EventHandler SelectedModuleChanged;

		public ModuleList() {
			InitializeComponent();
		}

		public void SetModuleType<T>()
			where T : class, IModuleInstance {
			_getMethod = ApplicationServices.GetAvailableModules<T>;
			_Reload();
		}

		public T GetSelectedModule<T>()
			where T : class, IModuleInstance {
			IModuleInstance module;
			Guid id = (Guid)listBoxModules.SelectedValue;
			if(!_moduleCache.TryGetValue(id, out module)) {
				module = ApplicationServices.Get<T>(id);
				_moduleCache[id] = module;
			}
			return module as T;
		}


		protected virtual void OnSelectedModuleChanged(EventArgs e) {
			if(SelectedModuleChanged != null) {
				SelectedModuleChanged(this, e);
			}
		}

		private void _Reload() {
			listBoxModules.DataSource = null;

			Dictionary<Guid, string> modules = _getMethod();
			
			listBoxModules.DisplayMember = "Value";
			listBoxModules.ValueMember = "Key";
			listBoxModules.DataSource = modules.ToArray();
		}

		private void listBoxModules_SelectedIndexChanged(object sender, EventArgs e) {
			buttonReloadModule.Enabled = listBoxModules.SelectedValue != null;
			OnSelectedModuleChanged(EventArgs.Empty);
		}

		private void buttonReloadModule_Click(object sender, EventArgs e) {
			Guid moduleId = (Guid)listBoxModules.SelectedValue;
			ApplicationServices.ReloadModule(moduleId);
			_Reload();
		}
	}
}
