using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using VixenModules.App.LipSyncApp;
using VixenModules.Property.Face;

namespace VixenModules.Property.Phoneme {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(FaceData data) {
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Phonemes = data.Phonemes;
		}


		public HashSet<PhonemeType> Phonemes { get; set; }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}
