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
	public partial class LetterIteratorEditor : NameGeneratorEditor
	{
		private LetterIterator _counter;

		public LetterIteratorEditor(LetterIterator counter)
		{
			InitializeComponent();
			_counter = counter;
		}

		private void NumericCounterEditor_Load(object sender, EventArgs e)
		{
			textBoxLetters.Text = _counter.Letters;
		}

		private void textBoxLetters_TextChanged(object sender, EventArgs e)
		{
			string text = textBoxLetters.Text.Trim();
			if (text.Length > 0)
				_counter.Letters = text;
			OnDataChanged();
		}
	}
}