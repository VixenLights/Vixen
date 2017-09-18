using System;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace VixenModules.Preview.VixenPreview
{
	public partial class PreviewPixelSetupForm : Form
	{
		public PreviewPixelSetupForm(string prefixName, int startingIndex, int bulbSize)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			suffixIndexChooser.Minimum = 0;
			suffixIndexChooser.Maximum = Int32.MaxValue;
			suffixIndexChooser.Value = startingIndex;

			bulbSizeChooser.Minimum = 1;
			bulbSizeChooser.Maximum = 100;
			bulbSizeChooser.Value = bulbSize;

			txtPrefixName.Text = prefixName;
		}

		public int BulbSize
		{
			get { return decimal.ToInt32(bulbSizeChooser.Value); }
		}

		public int StartingIndex
		{
			get { return decimal.ToInt32(suffixIndexChooser.Value); }
		}

		public string PrefixName
		{
			get { return txtPrefixName.Text; }
		}

		#region Theme events

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Common.Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Common.Resources.Properties.Resources.ButtonBackgroundImage;
		}

		#endregion

	}
}
