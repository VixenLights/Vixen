using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Orientation {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(Orientation defaultOrientation) {
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			foreach (var item in Enum.GetValues(typeof(Orientation)))
			{
				comboBoxOrientation.Items.Add(item);
			}

			Orientation = defaultOrientation;
		}


		public Orientation Orientation
		{
			get { return (Orientation)comboBoxOrientation.SelectedItem; }
			set
			{
				comboBoxOrientation.SelectedItem = value;
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

		private void SetupForm_Load(object sender, EventArgs e)
		{
			
		}
	}
}
