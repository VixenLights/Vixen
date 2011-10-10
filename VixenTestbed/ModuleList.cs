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
		public ModuleList() {
			InitializeComponent();
		}

		public void SetModuleType<T>()
			where T : class, Vixen.Module.IModuleInstance {
			Dictionary<Guid, string> modules = ApplicationServices.GetAvailableModules<T>();
			listBoxModules.DisplayMember = "Value";
			listBoxModules.ValueMember = "Key";
			listBoxModules.DataSource = modules.ToArray();
		}
	}
}
