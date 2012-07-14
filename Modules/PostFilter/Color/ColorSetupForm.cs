using System;
using System.Windows.Forms;

namespace VixenModules.OutputFilter.Color {
	public partial class ColorSetupForm : Form {
		public ColorSetupForm(ColorData data) {
			InitializeComponent();
			_SelectedFilter = data.ColorFilter;
		}

		public ColorFilter SelectedColorFilter { get; private set; }

		private ColorFilter _SelectedFilter {
			get {
				if(radioButtonRed.Checked) {
					return ColorFilter.Red;
				}
				if(radioButtonGreen.Checked) {
					return ColorFilter.Green;
				}
				if(radioButtonBlue.Checked) {
					return ColorFilter.Blue;
				}
				if(radioButtonYellow.Checked) {
					return ColorFilter.Yellow;
				}
				if(radioButtonWhite.Checked) {
					return ColorFilter.White;
				}
				return ColorFilter.None;
			}
			set {
				switch(value) {
					case ColorFilter.Red:
						radioButtonRed.Checked = true;
						break;
					case ColorFilter.Green:
						radioButtonGreen.Checked = true;
						break;
					case ColorFilter.Blue:
						radioButtonBlue.Checked = true;
						break;
					case ColorFilter.Yellow:
						radioButtonYellow.Checked = true;
						break;
					case ColorFilter.White:
						radioButtonWhite.Checked = true;
						break;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			SelectedColorFilter = _SelectedFilter;

			if(SelectedColorFilter == ColorFilter.None) {
				DialogResult = DialogResult.None;
				MessageBox.Show("You have not selected a color.");
			}
		}
	}
}
