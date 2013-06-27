using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Wizard;
using Dataweb.NShape;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Services;

namespace VixenApplication.FiltersAndPatching
{
	public partial class PatchingWizard_2_Filters : WizardStage
	{
		private readonly PatchingWizardData _data;

		public PatchingWizard_2_Filters(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		private void PatchingWizard_2_Filters_Load(object sender, EventArgs e)
		{
			_populateComboBox();
			_updateButtonStatuses();
		}

		private void _populateComboBox()
		{
			comboBoxNewFilterTypes.Items.Clear();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IOutputFilterModuleInstance>()) {
				comboBoxNewFilterTypes.Items.Add(new ConfigFiltersAndPatching.FilterTypeComboBoxEntry(kvp.Key, kvp.Value));
			}
			if (comboBoxNewFilterTypes.Items.Count > 0) {
				comboBoxNewFilterTypes.SelectedIndex = 0;
			}
		}

		private void buttonAddFilter_Click(object sender, EventArgs e)
		{
			ConfigFiltersAndPatching.FilterTypeComboBoxEntry item =
				comboBoxNewFilterTypes.SelectedItem as ConfigFiltersAndPatching.FilterTypeComboBoxEntry;
			if (item == null) {
				MessageBox.Show("Please select a filter type first.", "Select filter type");
				return;
			}

			IOutputFilterModuleInstance moduleInstance = ApplicationServices.Get<IOutputFilterModuleInstance>(item.Guid);
			_data.Filters.Add(moduleInstance);

			listViewFilters.Items.Add(new ListViewItem {Text = moduleInstance.Name, Tag = moduleInstance});

			_WizardStageChanged();
		}

		private void buttonDeleteSelected_Click(object sender, EventArgs e)
		{
			if (listViewFilters.SelectedIndices.Count != 1)
				return;

			int index = listViewFilters.SelectedIndices[0];
			listViewFilters.Items.RemoveAt(index);
			_data.Filters.RemoveAt(index);
			_updateButtonStatuses();
			_WizardStageChanged();
		}

		private void buttonMoveUp_Click(object sender, EventArgs e)
		{
			if (listViewFilters.SelectedIndices.Count != 1 || listViewFilters.SelectedIndices[0] <= 0)
				return;

			int index = listViewFilters.SelectedIndices[0];

			ListViewItem item = listViewFilters.Items[index];
			listViewFilters.Items.RemoveAt(index);
			listViewFilters.Items.Insert(index - 1, item);

			IOutputFilterModuleInstance instance = _data.Filters[index];
			_data.Filters.RemoveAt(index);
			_data.Filters.Insert(index - 1, instance);

			_updateButtonStatuses();
			_WizardStageChanged();
		}

		private void buttonMoveDown_Click(object sender, EventArgs e)
		{
			if (listViewFilters.SelectedIndices.Count != 1 ||
			    listViewFilters.SelectedIndices[0] >= (listViewFilters.Items.Count - 1))
				return;

			int index = listViewFilters.SelectedIndices[0];

			ListViewItem item = listViewFilters.Items[index];
			listViewFilters.Items.RemoveAt(index);
			listViewFilters.Items.Insert(index + 1, item);

			IOutputFilterModuleInstance instance = _data.Filters[index];
			_data.Filters.RemoveAt(index);
			_data.Filters.Insert(index + 1, instance);

			_updateButtonStatuses();
			_WizardStageChanged();
		}

		private void _updateButtonStatuses()
		{
			if (listViewFilters.SelectedIndices.Count != 1) {
				groupBoxSelectedFilter.Enabled = false;
				buttonMoveDown.Enabled = false;
				buttonMoveUp.Enabled = false;
				buttonDeleteSelected.Enabled = false;
				buttonSetupFilter.Enabled = false;
			}
			else {
				groupBoxSelectedFilter.Enabled = true;
				buttonMoveDown.Enabled = listViewFilters.SelectedIndices[0] < (listViewFilters.Items.Count - 1);
				buttonMoveUp.Enabled = listViewFilters.SelectedIndices[0] > 0;
				buttonDeleteSelected.Enabled = true;
				buttonSetupFilter.Enabled = true;
			}

			buttonAddFilter.Enabled = comboBoxNewFilterTypes.Items.Count > 0 && comboBoxNewFilterTypes.SelectedIndex >= 0;
		}

		private void buttonSetupFilter_Click(object sender, EventArgs e)
		{
			IOutputFilterModuleInstance instance = listViewFilters.SelectedItems[0].Tag as IOutputFilterModuleInstance;
			if (instance != null)
				instance.Setup();
		}

		private void listViewFilters_DoubleClick(object sender, EventArgs e)
		{
			if (listViewFilters.SelectedItems.Count <= 0)
				return;

			IOutputFilterModuleInstance instance = listViewFilters.SelectedItems[0].Tag as IOutputFilterModuleInstance;
			if (instance != null)
				instance.Setup();
		}

		private void listViewFilters_SelectedIndexChanged(object sender, EventArgs e)
		{
			_updateButtonStatuses();
		}

		private void comboBoxNewFilterTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			_updateButtonStatuses();
		}
	}
}