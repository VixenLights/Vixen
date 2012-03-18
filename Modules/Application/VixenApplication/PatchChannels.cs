using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using VixenApplication.PatchingControls;

namespace VixenApplication {
	public partial class PatchChannels : Form {
		private ChannelNode[] _channelNodes;
		private IPatchingRule _patchingRule;

		public PatchChannels(IEnumerable<ChannelNode> channelNodes) {
			InitializeComponent();
			//***
			//Needs to be restricted to only the nodes that have channels, but for testing...
			//_channelNodes = channelNodes.Where(x => x.Channel != null).ToArray();
			_channelNodes = channelNodes.ToArray();
		}

		private void PatchChannels_Load(object sender, EventArgs e) {
			listBoxChannels.Items.AddRange(_channelNodes);

			IPatchingRule[] namingRules = Vixen.Services.ApplicationServices.GetAllPatchingRules();
			comboBoxPatchingMethod.DisplayMember = "Description";
			comboBoxPatchingMethod.ValueMember = "";
			comboBoxPatchingMethod.DataSource = namingRules;
		}

		private IPatchingRule _GetSelectedPatchingRule() {
			return (IPatchingRule)comboBoxPatchingMethod.SelectedValue;
		}

		private Control _GetPatchingRuleEditor(ToNOutputsAtOutput rule) {
			return new ToNOutputsAtOutputEditor();
		}

		private Control _GetPatchingRuleEditor(ToNOutputsAtOutputs rule) {
			return new ToNOutputsAtOutputsEditor();
		}

		private Control _GetPatchingRuleEditor(ToNOutputsOverControllers rule) {
			return new ToNOutputsOverControllersEditor();
		}

		private void buttonConfigurePatchingRule_Click(object sender, EventArgs e) {
			Control editorControl = _GetPatchingRuleEditor((dynamic)_GetSelectedPatchingRule());
			if(!(editorControl is IHasPatchRule)) {
				MessageBox.Show("The editor for this rule is not a valid editor.");
				return;
			}

			using(RuleEditorContainer ruleEditorContainer = new RuleEditorContainer(editorControl)) {
				if(ruleEditorContainer.ShowDialog() == DialogResult.OK) {
					_patchingRule = ((IHasPatchRule)editorControl).Rule;
				}
			}
		}

		private bool _Validate() {
			if(_patchingRule == null) {
				MessageBox.Show("Patching rule must be configured first.");
				return false;
			}

			return true;
		}

		private void buttonPreview_Click(object sender, EventArgs e) {
			if(Validate()) {
				ControllerReference[] controllerReferences = _patchingRule.GenerateControllerReferences(_channelNodes.Length);
				Queue<ControllerReference> referenceQueue = new Queue<ControllerReference>(controllerReferences);

				listViewPreview.Items.Clear();
				int referencesPerChannel = controllerReferences.Length / _channelNodes.Length;
				string lastChannelNode = null;
				foreach(ChannelNode channelNode in _channelNodes) {
					for(int i = 0; i < referencesPerChannel; i++) {
						ControllerReference reference = referenceQueue.Dequeue();
						string channelNodeName = lastChannelNode == channelNode.Name ? string.Empty : channelNode.Name;
						string[] referenceDetails = new[] { channelNodeName, reference.ToString() };
						listViewPreview.Items.Add(new ListViewItem(referenceDetails));
						lastChannelNode = channelNode.Name;
					}
				}
			}
		}

		private void buttonCommit_Click(object sender, EventArgs e) {
			if(Validate()) {
				Vixen.Services.ChannelNodeService.Instance.Patch(_channelNodes, _patchingRule);
				MessageBox.Show("Patched " + _channelNodes.Length + " channels.");
			}
		}
	}
}
