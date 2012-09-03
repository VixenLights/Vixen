using System.ComponentModel;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Rule.Name;

namespace VixenApplication.Controls {
	public partial class GridEditor : UserControl, IHasNameRule {
		public GridEditor() {
			InitializeComponent();
			textBoxFormat.Text = "Channel (" + GridTemplate.RowValuePlaceholder + ", " + GridTemplate.ColumnValuePlaceholder + ")";
		}

		public INamingRule NamingRule {
			get {
				return new GridTemplate(_Width, _Height, _Format, _RowStart, _ColStart);
			}
		}

		private int _Width {
			get {
				int value;
				int.TryParse(textBoxWidth.Text, out value);
				return value;
			}
		}

		private int _Height {
			get {
				int value;
				int.TryParse(textBoxHeight.Text, out value);
				return value;
			}
		}

		private string _Format {
			get { return textBoxFormat.Text; }
		}

		private int _RowStart {
			get {
				int value;
				int.TryParse(textBoxRowStart.Text, out value);
				return value;
			}
		}

		private int _ColStart {
			get {
				int value;
				int.TryParse(textBoxColStart.Text, out value);
				return value;
			}
		}

		private void textBoxWidth_Validating(object sender, CancelEventArgs e) {
			if(_Width < 2) {
				MessageBox.Show("Width must be at least 2.");
				e.Cancel = true;
			}
		}

		private void textBoxHeight_Validating(object sender, CancelEventArgs e) {
			if(_Height < 2) {
				MessageBox.Show("Height must be at least 2.");
				e.Cancel = true;
			}
		}

		private void textBoxRowStart_Validating(object sender, CancelEventArgs e) {
			if(_RowStart < 0) {
				MessageBox.Show("Starting row number must be at least 0.");
				e.Cancel = true;
			}
		}

		private void textBoxColStart_Validating(object sender, CancelEventArgs e) {
			if(_ColStart < 0) {
				MessageBox.Show("Starting column number must be at least 0.");
				e.Cancel = true;
			}
		}

		private void textBoxFormat_Validating(object sender, CancelEventArgs e) {
			if(!(_Format.Contains(GridTemplate.ColumnValuePlaceholder) || _Format.Contains(GridTemplate.RowValuePlaceholder))) {
				MessageBox.Show("Format does not contain the row or column placeholders.");
				e.Cancel = true;
			}
		}
	}
}
