using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Dynamic;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public partial class ColorBreakdownSetup : BaseForm
	{
		private readonly ColorBreakdownData _data;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public ColorBreakdownSetup(ColorBreakdownData breakdownData)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_data = breakdownData;
		}

		public List<ColorBreakdownItem> BreakdownItems
		{
			get
			{
				return
					tableLayoutPanelControls.Controls.OfType<ColorBreakdownItemControl>().Select(
						itemControl => itemControl.ColorBreakdownItem).ToList();
			}
		}

		private void ColorBreakdownSetup_Load(object sender, EventArgs e)
		{
			tableLayoutPanelControls.Controls.Clear();

			foreach (ColorBreakdownItem breakdownItem in _data.BreakdownItems) {
				addControl(new ColorBreakdownItemControl(breakdownItem));
			}

			checkBoxMixColors.Checked = _data.MixColors;

			// let's just make up some hardcoded templates. Can expand on this later; probably don't need to,
			// people can request new ones and stuff if they want.
			comboBoxTemplates.Items.Clear();
			comboBoxTemplates.Items.Add("RGB");
			comboBoxTemplates.Items.Add("RGBW");
			comboBoxTemplates.Items.Add("RGBY");
			comboBoxTemplates.Items.Add("RGxB");
			comboBoxTemplates.Items.Add("GRBW");
			comboBoxTemplates.SelectedIndex = 0;
		}

		private void buttonAddColor_Click(object sender, EventArgs e)
		{
			addControl(new ColorBreakdownItemControl());
		}

		private void control_DeleteRequested(object sender, EventArgs e)
		{
			ColorBreakdownItemControl control = sender as ColorBreakdownItemControl;
			if (control == null)
				return;

			removeControl(control);
		}

		private void removeControl(ColorBreakdownItemControl control)
		{
			if (!tableLayoutPanelControls.Controls.Contains(control))
				return;

			tableLayoutPanelControls.Controls.Remove(control);
			control.DeleteRequested -= control_DeleteRequested;
		}

		private void addControl(ColorBreakdownItemControl control)
		{
			control.DeleteRequested += control_DeleteRequested;
			tableLayoutPanelControls.Controls.Add(control);
		}

		private void buttonApplyTemplate_Click(object sender, EventArgs e)
		{
			foreach (ColorBreakdownItemControl control in tableLayoutPanelControls.Controls.OfType<ColorBreakdownItemControl>()) {
				removeControl(control);
			}

			tableLayoutPanelControls.Controls.Clear();

			string template = comboBoxTemplates.SelectedItem.ToString();
			switch (template) {
				case "RGB":
					addControl(new ColorBreakdownItemControl(Color.Red, "Red"));
					addControl(new ColorBreakdownItemControl(Color.Lime, "Green"));
					addControl(new ColorBreakdownItemControl(Color.Blue, "Blue"));
					break;
				case "RGBW":
					addControl(new ColorBreakdownItemControl(Color.Red, "Red"));
					addControl(new ColorBreakdownItemControl(Color.Lime, "Green"));
					addControl(new ColorBreakdownItemControl(Color.Blue, "Blue"));
					addControl(new ColorBreakdownItemControl(Color.White, "White"));
					break;
				case "RGBY":
					addControl(new ColorBreakdownItemControl(Color.Red, "Red"));
					addControl(new ColorBreakdownItemControl(Color.Lime, "Green"));
					addControl(new ColorBreakdownItemControl(Color.Blue, "Blue"));
					addControl(new ColorBreakdownItemControl(Color.Yellow, "Yellow"));
					break;
				case "RGxB":
					addControl(new ColorBreakdownItemControl(Color.Red, "Red"));
					addControl(new ColorBreakdownItemControl(Color.Lime, "Green"));
					addControl(new ColorBreakdownItemControl(Color.Black, "[empty]"));
					addControl(new ColorBreakdownItemControl(Color.Blue, "Blue"));
					break;
				case "GRBW":
					addControl(new ColorBreakdownItemControl(Color.Lime, "Green"));
					addControl(new ColorBreakdownItemControl(Color.Red, "Red"));
					addControl(new ColorBreakdownItemControl(Color.Blue, "Blue"));
					addControl(new ColorBreakdownItemControl(Color.White, "White"));
					break;

				default:
					Logging.Error("Color Breakdown Setup: got an unknown template to apply: " + template);
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error applying template: Unknown template.", "Error", false, false);
					messageBox.ShowDialog();
					break;
			}
		}

		private void checkBoxMixColors_CheckedChanged(object sender, EventArgs e)
		{
			_data.MixColors = checkBoxMixColors.Checked;
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

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}