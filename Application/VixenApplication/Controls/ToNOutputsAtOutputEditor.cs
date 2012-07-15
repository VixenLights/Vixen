using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Controls {
	public partial class ToNOutputsAtOutputEditor : UserControl, IHasPatchRule {
		public ToNOutputsAtOutputEditor() {
			InitializeComponent();
			comboBoxController.Items.AddRange(VixenSystem.Controllers.ToArray());
		}

		public IPatchingRule Rule {
			get { return new ToNOutputsAtOutput(new ControllerReference(_SelectedController.Id, _StartingOutputIndex), _OutputsToPatch); }
		}

		private OutputController _SelectedController {
			get { return (OutputController)comboBoxController.SelectedItem; }
		}

		private int _StartingOutputIndex {
			get { return comboBoxOutputIndex.SelectedIndex; }
			set { comboBoxOutputIndex.SelectedIndex = value; }
		}

		private int _OutputsToPatch {
			get {
				int value;
				int.TryParse(textBoxOutputPatchCount.Text, out value);
				return value;
			}
		}

		private void comboBoxController_SelectedIndexChanged(object sender, EventArgs e) {
			int selectedOutputIndex = _StartingOutputIndex;

			comboBoxOutputIndex.Items.Clear();
			if(_SelectedController != null) {
				comboBoxOutputIndex.Items.AddRange(Enumerable.Range(1, _SelectedController.OutputCount).Cast<object>().ToArray());
			}

			_StartingOutputIndex = selectedOutputIndex;
		}

		private void textBoxOutputPatchCount_Validating(object sender, CancelEventArgs e) {
			if(_OutputsToPatch < 1) {
				MessageBox.Show("Number of outputs to patch must be at least 1.");
				e.Cancel = true;
			}
		}

		private void comboBoxController_Validating(object sender, CancelEventArgs e) {
			if(_SelectedController == null) {
				MessageBox.Show("A controller must be selected.");
				e.Cancel = true;
			}
		}

		private void comboBoxOutputIndex_Validating(object sender, CancelEventArgs e) {
			if(_StartingOutputIndex < 0) {
				MessageBox.Show("A starting output must be selected.");
				e.Cancel = true;
			}
		}
	}
}
