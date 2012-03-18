using System;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.PostFilter;

namespace VixenApplication {
	public partial class PostFilterTemplateControl : UserControl {
		private IModuleDataSet _dataSet;

		public PostFilterTemplateControl(IModuleDataSet dataSet) {
			InitializeComponent();
			_dataSet = dataSet;
		}

		private void PostFilterTemplateControl_Load(object sender, EventArgs e) {
			var modules = Vixen.Services.ApplicationServices.GetAvailableModules<IPostFilterModuleInstance>().ToArray();
			comboBoxFilters.DisplayMember = "Name";
			comboBoxFilters.ValueMember = "Value";
			comboBoxFilters.DataSource = modules;
		}

		private void _UpdateEnabledStates() {
			buttonMoveUp.Enabled = listBoxFilters.SelectedItems.Count == 1 && listBoxFilters.SelectedIndex > 0;
			buttonMoveDown.Enabled = listBoxFilters.SelectedItems.Count == 1 && listBoxFilters.SelectedIndex < listBoxFilters.Items.Count - 1;
			buttonRemove.Enabled = listBoxFilters.SelectedItems.Count < 0;
			buttonConfigure.Enabled = listBoxFilters.SelectedItems.Count < 0;
		}

		private void _AddModule(IPostFilterModuleInstance module) {
			listBoxFilters.Items.Add(module);
			_dataSet.AssignModuleInstanceData(module);
		}

		private void _RemoveModule(IPostFilterModuleInstance module) {
			listBoxFilters.Items.Remove(module);
			_dataSet.AssignModuleInstanceData(module);
		}

		private IPostFilterModuleInstance _SelectedAvailableFilter {
			get { return (IPostFilterModuleInstance)listBoxFilters.SelectedItem; }
		}

		private void listBoxFilters_SelectedIndexChanged(object sender, EventArgs e) {
			_UpdateEnabledStates();
		}

		private void comboBoxFilters_SelectedIndexChanged(object sender, EventArgs e) {
			buttonAdd.Enabled = _SelectedAvailableFilter != null && _SelectedAvailableFilter.HasSetup;
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
			IPostFilterModuleInstance module = Vixen.Services.ApplicationServices.Get<IPostFilterModuleInstance>(moduleId);
			_AddModule(module);
		}

		private Guid _GetSelectedAvailableModuleId() {
			return (Guid)comboBoxFilters.SelectedValue;
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			IPostFilterModuleInstance[] modulesToRemove = listBoxFilters.SelectedItems.Cast<IPostFilterModuleInstance>().ToArray();
			foreach(IPostFilterModuleInstance module in modulesToRemove) {
				_RemoveModule(module);
			}
		}

		private void buttonConfigure_Click(object sender, EventArgs e) {
			_SelectedAvailableFilter.Setup();
		}
	}
}
