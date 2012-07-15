using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.OutputFilter;

namespace VixenApplication.Controls {
	public partial class OutputFilterTemplateControl : UserControl {
		private IModuleDataSet _dataSet;
		private List<IOutputFilterModuleInstance> _modules;

		public OutputFilterTemplateControl(IModuleDataSet dataSet, string title) {
			InitializeComponent();
			_dataSet = dataSet;
			_modules = new List<IOutputFilterModuleInstance>();
			labelTitle.Text = title;
		}

		public IEnumerable<IOutputFilterModuleInstance> Filters {
			get { return _modules; }
		}

		private void PostFilterTemplateControl_Load(object sender, EventArgs e) {
			var modules = Vixen.Services.ApplicationServices.GetAvailableModules<IOutputFilterModuleInstance>().Select(x => new KeyValuePair<string,Guid>(x.Value, x.Key)).ToArray();
			comboBoxFilters.DisplayMember = "Key";
			comboBoxFilters.ValueMember = "Value";
			comboBoxFilters.DataSource = modules;
		}

		private void _UpdateEnabledStates() {
			buttonMoveUp.Enabled = listBoxFilters.SelectedItems.Count == 1 && listBoxFilters.SelectedIndex > 0;
			buttonMoveDown.Enabled = listBoxFilters.SelectedItems.Count == 1 && listBoxFilters.SelectedIndex < listBoxFilters.Items.Count - 1;
			buttonRemove.Enabled = listBoxFilters.SelectedItems.Count > 0;
			buttonConfigure.Enabled = _SelectedFilter != null && _SelectedFilter.HasSetup;
		}

		private void _AddModule(IOutputFilterModuleInstance module) {
			_modules.Add(module);
			_dataSet.AssignModuleInstanceData(module);
			listBoxFilters.Items.Add(module.Descriptor.TypeName);
		}

		private void _RemoveModule(IOutputFilterModuleInstance module) {
			listBoxFilters.Items.RemoveAt(_modules.IndexOf(module));
			_dataSet.AssignModuleInstanceData(module);
			_modules.Remove(module);
		}

		private IOutputFilterModuleInstance _SelectedFilter {
			get {
				if(listBoxFilters.SelectedItems.Count == 1) {
					return _modules[listBoxFilters.SelectedIndex];
				}
				return null;
			}
		}

		private void listBoxFilters_SelectedIndexChanged(object sender, EventArgs e) {
			_UpdateEnabledStates();
		}

		private void comboBoxFilters_SelectedIndexChanged(object sender, EventArgs e) {
			buttonAdd.Enabled = comboBoxFilters.SelectedItem != null;
		}

		private void buttonMoveUp_Click(object sender, EventArgs e) {
			int index = listBoxFilters.SelectedIndex;
			object item = listBoxFilters.SelectedItem;
			listBoxFilters.Items.RemoveAt(index);
			listBoxFilters.Items.Insert(index - 1, item);
		}

		private void buttonMoveDown_Click(object sender, EventArgs e) {
			int index = listBoxFilters.SelectedIndex;
			object item = listBoxFilters.SelectedItem;
			listBoxFilters.Items.RemoveAt(index);
			listBoxFilters.Items.Insert(index + 1, item);
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			Guid moduleId = _GetSelectedAvailableModuleId();
			IOutputFilterModuleInstance module = Vixen.Services.ApplicationServices.Get<IOutputFilterModuleInstance>(moduleId);
			_AddModule(module);
		}

		private Guid _GetSelectedAvailableModuleId() {
			return (Guid)comboBoxFilters.SelectedValue;
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			IOutputFilterModuleInstance[] modulesToRemove = listBoxFilters.SelectedIndices.Cast<int>().Select(x => _modules[x]).ToArray();
			foreach(IOutputFilterModuleInstance module in modulesToRemove) {
				_RemoveModule(module);
			}
		}

		private void buttonConfigure_Click(object sender, EventArgs e) {
			_SelectedFilter.Setup();
		}
	}
}
