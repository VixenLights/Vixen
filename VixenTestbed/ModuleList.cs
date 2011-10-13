using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenTestbed {
	public partial class ModuleList : UserControl {
		private Func<Dictionary<Guid, string>> _getMethod;

		public ModuleList() {
			InitializeComponent();
		}

		public void SetModuleType<T>()
			where T : class, Vixen.Module.IModuleInstance {
			_getMethod = ApplicationServices.GetAvailableModules<T>;
			_Reload();
		}

		private void _Reload() {
			listBoxModules.DataSource = null;

			Dictionary<Guid, string> modules = _getMethod();
			
			listBoxModules.DisplayMember = "Value";
			listBoxModules.ValueMember = "Key";
			listBoxModules.DataSource = modules.ToArray();
		}

		private void listBoxModules_SelectedIndexChanged(object sender, EventArgs e) {
			buttonReloadModule.Enabled = listBoxModules.SelectedItem != null;
		}

		private void buttonReloadModule_Click(object sender, EventArgs e) {
			Guid moduleId = (Guid)listBoxModules.SelectedValue;
			ApplicationServices.ReloadModule(moduleId);
			_Reload();
		}
	}
}
