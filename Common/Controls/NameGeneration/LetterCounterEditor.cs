using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Vixen.Rule.Name;

namespace Common.Controls.NameGeneration
{
	public partial class LetterCounterEditor : NameGeneratorEditor
	{
		private LetterCounter _counter;

		public LetterCounterEditor(LetterCounter counter)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_counter = counter;
			textBoxStartLetter.KeyPress += new KeyPressEventHandler(textBoxStartLetter_KeyPress);
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


		private void textBoxStartLetter_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			// Check for a unwanted character in the KeyDown event.
			if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[^A-Z^a-z^\b]"))
			{
				// Stop the character from being entered into the control since it is illegal.
				e.Handled = true;
			}
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