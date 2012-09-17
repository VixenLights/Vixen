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
	public partial class WordIteratorEditor : NameGeneratorEditor
	{
		private WordIterator _counter;

		public WordIteratorEditor(WordIterator counter)
		{
			InitializeComponent();
			_counter = counter;
		}

		private void NumericCounterEditor_Load(object sender, EventArgs e)
		{
			textBoxWords.Text = string.Join(", ", _counter.Words);
		}

		private void textBoxLetters_TextChanged(object sender, EventArgs e)
		{
			string text = textBoxWords.Text.Trim();
			string[] words = text.Split(',');
			_counter.Words = new List<string>(words.Select(x => x.Trim()));
			OnDataChanged();
		}
	}
}
