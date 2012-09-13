using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Controls {
	public partial class ChannelsToControllerRuleEditor : UserControl, IHasPatchRule {
		public ChannelsToControllerRuleEditor() {
			InitializeComponent();
		}

		private void ChannelsToControllerRuleEditor_Load(object sender, EventArgs e) {
			listBoxChannels.Items.AddRange(VixenSystem.Channels.Cast<object>().ToArray());
			comboBoxController.Items.AddRange(VixenSystem.ControllerManagement.Devices.Cast<object>().ToArray());
		}

		public IPatchingRule PatchingRule {
			get {
				if(_Validate()) {
					return new ChannelsToSingleController(_SelectedChannels, _SelectedController, _StartingOutputIndex, _OutputsPerChannel);
				}
				return null;
			}
		}

		private int _GetSelectedControllerOutputCount() {
			if(comboBoxController.SelectedItem == null) return 0;
			return ((IHasOutputs)comboBoxController.SelectedItem).OutputCount;
		}

		private IEnumerable<Channel> _SelectedChannels {
			get { return listBoxChannels.SelectedItems.Cast<Channel>(); }
		}

		private IOutputDevice _SelectedController {
			get { return (IOutputDevice)comboBoxController.SelectedItem; }
		}

		private int _StartingOutputIndex {
			get {
				if(comboBoxStartingOutput.SelectedItem == null) return -1;
				return ((int)comboBoxStartingOutput.SelectedItem) - 1; 
			}
		}

		private int _OutputsPerChannel {
			get { return (int)nudOutputsPerChannels.Value; }
		}

		private bool _Validate() {
			if(!_SelectedChannels.Any()) {
				MessageBox.Show("No channels have been selected.");
				return false;
			}

			if(_SelectedController == null) {
				MessageBox.Show("No controller has been selected.");
				return false;
			}

			if(_StartingOutputIndex < 0) {
				MessageBox.Show("No starting output has been selected.");
				return false;
			}

			if(_GetSelectedControllerOutputCount() - _StartingOutputIndex - 1 < _OutputsPerChannel) {
				MessageBox.Show("Due to the number of outputs after the starting output relative to the number of outputs per channel, nothing would be patched.");
				return false;
			}

			return true;
		}

		private void buttonSelectAllChannels_Click(object sender, EventArgs e) {
			listBoxChannels.BeginUpdate();
			for(int i = 0; i < listBoxChannels.Items.Count; i++) {
				listBoxChannels.SetSelected(i, true);
			}
			listBoxChannels.EndUpdate();
		}

		private void buttonSelectNoChannels_Click(object sender, EventArgs e) {
			listBoxChannels.BeginUpdate();
			for(int i = 0; i < listBoxChannels.Items.Count; i++) {
				listBoxChannels.SetSelected(i, false);
			}
			listBoxChannels.EndUpdate();
		}

		private void comboBoxController_SelectedIndexChanged(object sender, EventArgs e) {
			comboBoxStartingOutput.Items.Clear();
			if(comboBoxController.SelectedItem != null) {
				comboBoxStartingOutput.Items.AddRange(Enumerable.Range(1, _GetSelectedControllerOutputCount()).Cast<object>().ToArray());
				comboBoxStartingOutput.SelectedIndex = 0;
				nudOutputsPerChannels.Maximum = _GetSelectedControllerOutputCount();
				comboBoxStartingOutput.Enabled = true;
				nudOutputsPerChannels.Enabled = true;
				buttonPreview.Enabled = true;
			} else {
				comboBoxStartingOutput.Enabled = false;
				nudOutputsPerChannels.Enabled = false;
				buttonPreview.Enabled = false;
			}
		}

		private void buttonPreview_Click(object sender, EventArgs e) {
			if(_Validate()) {
				listViewPreview.BeginUpdate();
				listViewPreview.Items.Clear();
				ChannelsToSingleController rule = new ChannelsToSingleController(_SelectedChannels, _SelectedController, _StartingOutputIndex, _OutputsPerChannel);
				foreach(DataFlowPatch patch in rule.GeneratePatches()) {
					listViewPreview.Items.Add(new ListViewItem(new[] { 
						VixenSystem.DataFlow.GetComponent(patch.SourceComponentId).Name,
						VixenSystem.DataFlow.GetComponent(patch.ComponentId).Name }));
				}
				listViewPreview.EndUpdate();
			}
		}
	}
}
