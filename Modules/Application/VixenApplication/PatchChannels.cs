using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using VixenApplication.Controls;

namespace VixenApplication {
	public partial class PatchChannels : Form {
		private ChannelNode[] _channelNodes;
		private IPatchingRule _patchingRule;

		public PatchChannels(IEnumerable<ChannelNode> channelNodes) {
			InitializeComponent();
			//Needs to be restricted to only the nodes that have channels.
			_channelNodes = channelNodes.Where(x => x.Channel != null).ToArray();
		}

		private void PatchChannels_Load(object sender, EventArgs e) {
			listBoxChannels.Items.AddRange(_channelNodes);

			IPatchingRule[] namingRules = Vixen.Services.ApplicationServices.GetAllPatchingRules();
			comboBoxPatchingMethod.DisplayMember = "Description";
			comboBoxPatchingMethod.ValueMember = "";
			comboBoxPatchingMethod.DataSource = namingRules;

			_ReloadPostFilterTemplates();
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

		private bool _DoApplyFilterTemplate {
			get { return checkBoxApplyFilterTemplate.Checked; }
		}

		private bool _DoApplyPatching {
			get { return !checkBoxFilterTemplateOnly.Checked; }
		}

		private string _SelectedFilterTemplate {
			get { return (string)comboBoxFilterTemplate.SelectedItem; }
		}

		private void _ReloadPostFilterTemplates() {
			IEnumerable<PostFilterTemplate> postFilterTemplates = PostFilterTemplate.GetAll();
			comboBoxFilterTemplate.Items.AddRange(postFilterTemplates.Select(x => Path.GetFileNameWithoutExtension(x.FilePath)).ToArray());
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
				MessageBox.Show("Rule must be configured first.");
				return false;
			}

			if(_DoApplyFilterTemplate && _SelectedFilterTemplate == null) {
				MessageBox.Show("You selected to apply a template, but not the template to apply.");
				return false;
			}

			return true;
		}

		private void buttonPreview_Click(object sender, EventArgs e) {
			if(Validate()) {
				IEnumerable<ControllerReferenceCollection> controllerReferences = _patchingRule.GenerateControllerReferenceCollections(_channelNodes.Length);
				Queue<ControllerReferenceCollection> referenceQueue = new Queue<ControllerReferenceCollection>(controllerReferences);

				listViewPreview.Items.Clear();

				string lastChannelNode = null;
				foreach(ChannelNode channelNode in _channelNodes) {
					//for(int i = 0; i < _patchingRule.OutputCountToPatch; i++) {
						if(referenceQueue.Count == 0) continue;

						ControllerReferenceCollection references = referenceQueue.Dequeue();
						foreach(ControllerReference reference in references) {
							string channelNodeName = (lastChannelNode == channelNode.Name) ? string.Empty : channelNode.Name;
							string[] referenceDetails = new[] {channelNodeName, reference.ToString()};
							listViewPreview.Items.Add(new ListViewItem(referenceDetails));
							lastChannelNode = channelNode.Name;
						}
					//}
				}
			}
		}

		private void checkBoxApplyFilterTemplate_CheckedChanged(object sender, EventArgs e) {
			buttonCreateFilterTemplate.Enabled = 
			comboBoxFilterTemplate.Enabled = checkBoxApplyFilterTemplate.Checked;
		}

		private void buttonCommit_Click(object sender, EventArgs e) {
			if(_Validate()) {
				Cursor = Cursors.WaitCursor;
				try {
					if(_DoApplyPatching) {
						Vixen.Services.ChannelNodeService.Instance.Patch(_channelNodes, _patchingRule, checkBoxClearExistingPatches.Checked);
					}
					if(_DoApplyFilterTemplate) {
						Vixen.Services.PostFilterService.Instance.ApplyTemplateMany(_SelectedFilterTemplate, _patchingRule, _channelNodes.Length, checkBoxClearExistingFilters.Checked);
					}
					MessageBox.Show(_channelNodes.Length + " channels done.");
				} catch(Exception ex) {
					MessageBox.Show(ex.Message);
				} finally {
					Cursor = Cursors.Default;
				}
			}
		}

		private void buttonCreateFilterTemplate_Click(object sender, EventArgs e) {
			using(PostFilterTemplateForm form = new PostFilterTemplateForm()) {
				form.ShowDialog();
				_ReloadPostFilterTemplates();
			}
		}
	}
}
