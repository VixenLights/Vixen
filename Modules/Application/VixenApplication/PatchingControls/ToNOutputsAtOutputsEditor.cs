using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Patch;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.PatchingControls {
	public partial class ToNOutputsAtOutputsEditor : UserControl, IHasPatchRule {
		public ToNOutputsAtOutputsEditor() {
			InitializeComponent();
			checkedListBoxControllers.Items.AddRange(VixenSystem.Controllers.ToArray());
		}

		public IPatchingRule Rule {
			get { return new ToNOutputsAtOutputs(_Controllers.Select(x => new ControllerReference(x, _StartingOutputIndex)), _OutputsToPatch); }
		}

		private int _StartingOutputIndex {
			get { return comboBoxOutputIndex.SelectedIndex; }
			set { comboBoxOutputIndex.SelectedIndex = value; }
		}

		private IEnumerable<Guid> _Controllers {
			get { return checkedListBoxControllers.CheckedItems.Cast<OutputController>().Select(x => x.Id); }
		}

		private int _OutputsToPatch {
			get {
				int value;
				int.TryParse(textBoxOutputPatchCount.Text, out value);
				return value;
			}
		}

		private void checkedListBoxControllers_ItemCheck(object sender, ItemCheckEventArgs e) {
			// Only make the common set of output indexes available.
			int selectedOutputIndex = _StartingOutputIndex;

			List<OutputController> checkedControllers = checkedListBoxControllers.CheckedItems.Cast<OutputController>().ToList();
			// Account for the checked/unchecked item.
			OutputController controllerBeingChanged = (OutputController)checkedListBoxControllers.Items[e.Index];
			if(e.NewValue == CheckState.Checked) {
				checkedControllers.Add(controllerBeingChanged);
			} else {
				checkedControllers.Remove(controllerBeingChanged);
			}

			comboBoxOutputIndex.Items.Clear();
			if(checkedControllers.Count > 0) {
				comboBoxOutputIndex.Items.AddRange(Enumerable.Range(1, checkedControllers.Min(x => x.OutputCount)).Cast<object>().ToArray());
			}

			_StartingOutputIndex = selectedOutputIndex;
		}

		private void textBoxOutputPatchCount_Validating(object sender, CancelEventArgs e) {
			if(_OutputsToPatch < 1) {
				MessageBox.Show("Number of outputs to patch must be at least 1.");
				e.Cancel = true;
			}
		}

		private void checkedListBoxControllers_Validating(object sender, CancelEventArgs e) {
			if(checkedListBoxControllers.CheckedItems.Count == 0) {
				MessageBox.Show("No controllers have been selected.");
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
