using System;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Name;
using Vixen.Sys;
using VixenApplication.Controls;

//Just to test a concept, not permanent
namespace VixenApplication {
	public partial class CreateAndNameChannels : Form {
		private INamingRule _namingRule;
		private ChannelNode _parentNode;

		public CreateAndNameChannels() {
			InitializeComponent();
		}

		public CreateAndNameChannels(ChannelNode parentNode)
			: this() {
			_parentNode = parentNode;
		}

		private void CreateAndNameChannels_Load(object sender, EventArgs e) {
			INamingRule[] namingRules = Vixen.Services.ApplicationServices.GetAllNamingRules();
			comboBoxNamingMethod.DisplayMember = "Name";
			comboBoxNamingMethod.ValueMember = "";
			comboBoxNamingMethod.DataSource = namingRules;
		}

		private INamingRule _GetSelectedNamingRule() {
			return (INamingRule)comboBoxNamingMethod.SelectedValue;
		}

		private Control _GetNameRuleEditor(FlatNumericTemplate rule) {
			return new FlatNumericTemplateEditor();
		}

		private Control _GetNameRuleEditor(FlatLetterTemplate rule) {
			return new FlatStringTemplateEditor();
		}

		private Control _GetNameRuleEditor(GridTemplate rule) {
			return new GridEditor();
		}

		private int _ChannelCount {
			get {
				int value;
				int.TryParse(textBoxChannelCount.Text, out value);
				return value;
			}
		}

		private void buttonConfigureNamingRule_Click(object sender, EventArgs e) {
			Control editorControl = _GetNameRuleEditor((dynamic)_GetSelectedNamingRule());
			if(!(editorControl is IHasNameRule)) {
				MessageBox.Show("The editor for this rule is not a valid editor.");
				return;
			}

			using(RuleEditorContainer ruleEditorContainer = new RuleEditorContainer(editorControl)) {
				if(ruleEditorContainer.ShowDialog() == DialogResult.OK) {
					_namingRule = ((IHasNameRule)editorControl).Rule;
				}
			}
		}

		private bool _Validate() {
			if(_ChannelCount <= 0) {
				MessageBox.Show("Channel count must be greater than 0.");
				return false;
			}

			if(_namingRule == null) {
				MessageBox.Show("Naming rule must be configured first.");
				return false;
			}

			return true;
		}

		private void buttonPreview_Click(object sender, EventArgs e) {
			if(_Validate()) {
				string[] names = _namingRule.GenerateNames(_ChannelCount);
				listBoxPreview.Items.Clear();
				listBoxPreview.Items.AddRange(names);
			}
		}

		private void buttonCommit_Click(object sender, EventArgs e) {
			if(_Validate()) {
				ChannelNode[] nodes = Vixen.Services.ChannelNodeService.Instance.CreateMultiple(_parentNode, _ChannelCount, true, false);
				Vixen.Services.ChannelNodeService.Instance.Rename(nodes, _namingRule);
				MessageBox.Show("Created " + _ChannelCount + " channels and nodes.");
			}
		}

	}
}
