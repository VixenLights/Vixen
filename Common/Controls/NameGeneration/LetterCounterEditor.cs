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
	public partial class LetterCounterEditor : NameGeneratorEditor
	{
		private LetterCounter _counter;

		public LetterCounterEditor(LetterCounter counter)
		{
			InitializeComponent();
			_counter = counter;
		}

		private void NumericCounterEditor_Load(object sender, EventArgs e)
		{
			numericUpDownSteps.Value = _counter.Count;
			textBoxStartLetter.Text = Char.ToString(_counter.StartLetter);
		}

		private void numericUpDownSteps_ValueChanged(object sender, EventArgs e)
		{
			_counter.Count = (int) numericUpDownSteps.Value;
			OnDataChanged();
		}

		private void textBoxStartLetter_TextChanged(object sender, EventArgs e)
		{
			string text = textBoxStartLetter.Text.Trim();
			if (text.Length > 0)
				_counter.StartLetter = text[0];
			OnDataChanged();
		}
	}
}