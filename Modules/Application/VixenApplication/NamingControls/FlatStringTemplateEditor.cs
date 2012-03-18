using System.ComponentModel;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Name;

namespace VixenApplication.NamingControls {
	public partial class FlatStringTemplateEditor : UserControl, IHasNameRule {
		public FlatStringTemplateEditor() {
			InitializeComponent();
			textBoxFormat.Text = "Channel-" + FlatLetterTemplate.LetterValuePlaceholder;
		}

		public INamingRule Rule {
			get {
				return new FlatLetterTemplate((char)_StartLetter, _Increment, _Format);
			}
		}

		private int _StartLetter {
			get {
				if(textBoxStartLetter.Text.Length > 0 && char.IsLetter(textBoxStartLetter.Text[0])) return textBoxStartLetter.Text[0];
				return 0;
			}
		}

		private int _Increment {
			get {
				int value;
				int.TryParse(textBoxIncrement.Text, out value);
				return value;
			}
		}

		private string _Format {
			get { return textBoxFormat.Text; }
		}

		private void textBoxStartLetter_Validating(object sender, CancelEventArgs e) {
			if(_StartLetter == 0) {
				MessageBox.Show("Start letter must be between a-z or A-Z.");
				e.Cancel = true;
			}
		}

		private void textBoxIncrement_Validating(object sender, CancelEventArgs e) {
			if(_Increment <= 0) {
				MessageBox.Show("Increment must be at least 1.");
				e.Cancel = true;
			}
		}

		private void textBoxFormat_Validating(object sender, CancelEventArgs e) {
			if(!_Format.Contains(FlatLetterTemplate.LetterValuePlaceholder)) {
				MessageBox.Show("Format does not contain the letter placeholder.");
				e.Cancel = true;
			}
		}
	}
}
