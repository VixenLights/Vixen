using System.ComponentModel;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Name;

namespace VixenApplication.NamingControls {
	public partial class FlatNumericTemplateEditor : UserControl, IHasNameRule {
		public FlatNumericTemplateEditor() {
			InitializeComponent();
			textBoxFormat.Text = "Channel-" + FlatNumericTemplate.NumericValuePlaceholder;
		}

		public INamingRule Rule {
			get {
				return new FlatNumericTemplate(_StartNumber, _Increment, _Format);
			}
		}

		private int _StartNumber {
			get {
				int value;
				int.TryParse(textBoxStartNumber.Text, out value);
				return value;
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

		private void textBoxStartNumber_Validating(object sender, CancelEventArgs e) {
			if(_StartNumber <= 0) {
				MessageBox.Show("Start number must be at least 1.");
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
			if(!_Format.Contains(FlatNumericTemplate.NumericValuePlaceholder)) {
				MessageBox.Show("Format does not contain the numeric placeholder.");
				e.Cancel = true;
			}
		}
	}
}
