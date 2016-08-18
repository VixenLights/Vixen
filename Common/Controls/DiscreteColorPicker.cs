using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class DiscreteColorPicker : BaseForm
	{
		public DiscreteColorPicker()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Properties.Resources.Icon_Vixen3;
		}

		private void DiscreteColorPicker_Load(object sender, EventArgs e)
		{
			if (ValidColors == null) {
				throw new Exception("Valid Colors not set");
			}

			if (_selectedColors == null) {
				_selectedColors = new List<Color>();
			}

			tableLayoutPanelColors.Controls.Clear();

			foreach (Color validColor in ValidColors) {
				DiscreteColorPickerItem control = new DiscreteColorPickerItem();
				control.Color = validColor;
				control.SingleColorOnly = SingleColorOnly;
				if (_selectedColors.Any(x => x.ToArgb() == validColor.ToArgb())) {
					control.Selected = true;
				}
				control.SelectedChanged += control_SelectedChanged;
				tableLayoutPanelColors.Controls.Add(control);
			}
			if (SingleColorOnly) {
				labelSelectPrompt.Text = "Select a discrete color.";
				this.Text = "Select Color";
			}
		}

		private void control_SelectedChanged(object sender, EventArgs e)
		{
			if (!SingleColorOnly)
				return;

			DiscreteColorPickerItem dcpi = sender as DiscreteColorPickerItem;
			if (dcpi == null)
				return;

			if (!dcpi.Selected)
				return;

			foreach (Control control in tableLayoutPanelColors.Controls) {
				if (control != sender) {
					(control as DiscreteColorPickerItem).Selected = false;
				}
			}
		}

		public IEnumerable<Color> ValidColors { get; set; }
		public bool SingleColorOnly { get; set; }

		private IEnumerable<Color> _selectedColors;

		public IEnumerable<Color> SelectedColors
		{
			get
			{
				List<Color> rv = new List<Color>();
				foreach (Control control in tableLayoutPanelColors.Controls) {
					DiscreteColorPickerItem dcpi = (DiscreteColorPickerItem) control;
					if (dcpi.Selected)
						rv.Add(dcpi.Color);
				}

				return rv;
			}
			set { _selectedColors = value; }
		}

		private void buttonOK_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonOK_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;
		}
	}
}