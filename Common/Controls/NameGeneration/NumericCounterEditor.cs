using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Rule.Name;

namespace Common.Controls.NameGeneration
{
	public partial class NumericCounterEditor : NameGeneratorEditor
	{
		private NumericCounter _counter;

		public NumericCounterEditor(NumericCounter counter)
		{
			InitializeComponent();
			_counter = counter;
			_oldStepValue = (int) numericUpDownStep.Value;
		}

		private void NumericCounterEditor_Load(object sender, EventArgs e)
		{
			numericUpDownStartNumber.Value = _counter.StartNumber;
			numericUpDownEndNumber.Value = _counter.EndNumber;
			numericUpDownEndNumber.Enabled = !_counter.Endless;
			numericUpDownStep.Value = _counter.Step;
			_oldStepValue = _counter.Step;
			checkBoxEndless.Checked = _counter.Endless;
		}

		private void numericUpDownStartNumber_ValueChanged(object sender, EventArgs e)
		{
			_counter.StartNumber = (int) numericUpDownStartNumber.Value;
			OnDataChanged();
		}

		private void numericUpDownEndNumber_ValueChanged(object sender, EventArgs e)
		{
			_counter.EndNumber = (int) numericUpDownEndNumber.Value;
			OnDataChanged();
		}

		private static int _oldStepValue;

		private void numericUpDownStep_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownStep.Value == 0) {
				if (_oldStepValue < 0)
					numericUpDownStep.Value = 1;
				else
					numericUpDownStep.Value = -1;
			}
			_counter.Step = (int) numericUpDownStep.Value;
			_oldStepValue = _counter.Step;
			OnDataChanged();
		}

		private void checkBoxEndless_CheckedChanged(object sender, EventArgs e)
		{
			_counter.Endless = checkBoxEndless.Checked;
			numericUpDownEndNumber.Enabled = !_counter.Endless;
			OnDataChanged();
		}
	}
}