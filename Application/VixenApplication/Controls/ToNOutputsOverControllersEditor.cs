using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Controls {
	public partial class ToNOutputsOverControllersEditor : UserControl, IHasPatchRule {
		public ToNOutputsOverControllersEditor() {
			InitializeComponent();
			comboBoxStartingController.Items.AddRange(VixenSystem.Controllers.ToArray());
			checkedListBoxSubsequentControllers.Items.AddRange(VixenSystem.Controllers.ToArray());
		}

		public IPatchingRule Rule {
			get { return new ToNOutputsOverControllers(new ControllerReference(_StartingController.Id, _StartingOutputIndex), _OutputsToPatch, _SubsequentControllers); }
		}

		private OutputController _StartingController {
			get { return (OutputController)comboBoxStartingController.SelectedItem; }
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

		private IEnumerable<Guid> _SubsequentControllers {
			get { return checkedListBoxSubsequentControllers.CheckedItems.Cast<OutputController>().Select(x => x.Id); }
		}

		private void comboBoxStartingController_SelectedIndexChanged(object sender, EventArgs e) {
			int selectedOutputIndex = _StartingOutputIndex;

			comboBoxOutputIndex.Items.Clear();
			if(_StartingController != null) {
				comboBoxOutputIndex.Items.AddRange(Enumerable.Range(1, _StartingController.OutputCount).Cast<object>().ToArray());
			}

			_StartingOutputIndex = selectedOutputIndex;
		}

		private void textBoxOutputPatchCount_Validating(object sender, CancelEventArgs e) {
			if(_OutputsToPatch < 1) {
				MessageBox.Show("Number of outputs to patch must be at least 1.");
				e.Cancel = true;
			}
		}

		private void comboBoxStartingController_Validating(object sender, CancelEventArgs e) {
			if(_StartingController == null) {
				MessageBox.Show("An initial controller must be selected.");
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
