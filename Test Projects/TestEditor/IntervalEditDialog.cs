using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using Vixen.Module.Sequence;
using Vixen.Sys;

namespace TestEditor {
	public partial class IntervalEditDialog : Form {
		//private ISequenceModuleInstance _sequence;
		private ISequence _sequence;

		public IntervalEditDialog(ISequence sequence) {
			InitializeComponent();
			_sequence = sequence;
			listBoxIntervals.Items.AddRange(sequence.Data.IntervalValues.Cast<object>().ToArray());
		}

		private bool _internal = false;
		private void listBoxIntervals_SelectedIndexChanged(object sender, EventArgs e) {
			if(listBoxIntervals.SelectedIndex != -1) {
				_internal = true;
				// Set minimum based on surrounding values.
				if(listBoxIntervals.SelectedIndex > 0) {
					numericUpDownInterval.Minimum = (int)listBoxIntervals.Items[listBoxIntervals.SelectedIndex-1] + 1;
				} else {
					numericUpDownInterval.Minimum = 0;
				}
				// Set maximum based on surrounding values.
				if(listBoxIntervals.SelectedIndex < listBoxIntervals.Items.Count - 1) {
					numericUpDownInterval.Maximum = (int)listBoxIntervals.Items[listBoxIntervals.SelectedIndex + 1] - 1;
				} else {
					numericUpDownInterval.Maximum = _sequence.Length;
				}
				// Now set the value.
				numericUpDownInterval.Value = (int)listBoxIntervals.SelectedItem;
				_internal = false;
			}
		}

		private void numericUpDownInterval_ValueChanged(object sender, EventArgs e) {
			if(listBoxIntervals.SelectedIndex != -1 && !_internal) {
				listBoxIntervals.Items[listBoxIntervals.SelectedIndex] = (int)numericUpDownInterval.Value;
			}
		}

		private void buttonDone_Click(object sender, EventArgs e) {
			//assign back to interval collection
			_sequence.Data.IntervalValues = listBoxIntervals.Items.Cast<long>();
		}
	}
}
