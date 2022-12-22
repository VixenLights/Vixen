using Common.Controls.Theme;
using Vixen.Rule.Name;

namespace Common.Controls.NameGeneration
{
	public partial class WordIteratorEditor : NameGeneratorEditor
	{
		private WordIterator _counter;

		public WordIteratorEditor(WordIterator counter)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
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