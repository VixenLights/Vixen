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
	public partial class NumericCounterEditor : UserControl
	{
		private NumericCounter _counter;

		public NumericCounterEditor(NumericCounter counter)
		{
			InitializeComponent();
			_counter = counter;
		}

		private void NumericCounterEditor_Load(object sender, EventArgs e)
		{
			numericUpDownStartNumber.Value = _counter.StartNumber;
			numericUpDownEndNumber.Value = _counter.EndNumber;
			numericUpDownStep.Value = _counter.Step;
			checkBoxEndless.Checked = _counter.Endless;
		}

		private void numericUpDownStartNumber_ValueChanged(object sender, EventArgs e)
		{
			_counter.StartNumber = (int)numericUpDownStartNumber.Value;
		}

		private void numericUpDownEndNumber_ValueChanged(object sender, EventArgs e)
		{
			_counter.EndNumber = (int)numericUpDownEndNumber.Value;
		}

		private void numericUpDownStep_ValueChanged(object sender, EventArgs e)
		{
			_counter.Step = (int)numericUpDownStep.Value;
		}

		private void checkBoxEndless_CheckedChanged(object sender, EventArgs e)
		{
			_counter.Endless = checkBoxEndless.Checked;
		}
	}
}
