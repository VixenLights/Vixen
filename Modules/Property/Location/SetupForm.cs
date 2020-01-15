using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Location {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(LocationData data) {
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			X = data.X;
			Y = data.Y;
			Z = data.Z;
		}


		public int X {
			get { return (int)numericUpDownXPosition.Value; }
			set {
				numericUpDownXPosition.Value = value;

			}
		}
		public int Y {
			get { return (int)numericUpDownYPosition.Value; }
			set {
				numericUpDownYPosition.Value = value;
			}
		}
		public int Z {
			get { return (int)numericUpDownZPosition.Value; }
			set {
				numericUpDownZPosition.Value = value;
			}
		}

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
