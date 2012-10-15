using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Dynamic;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public partial class ColorBreakdownSetup : Form
	{
		private ColorBreakdownData _data;
		public ColorBreakdownSetup(ColorBreakdownData breakdownData)
		{
			InitializeComponent();
			_data = breakdownData;
		}

		public List<ColorBreakdownItem> BreakdownItems
		{
			get
			{
				return tableLayoutPanelControls.Controls.OfType<ColorBreakdownItemControl>().Select(itemControl => itemControl.ColorBreakdownItem).ToList();
			}
		}

		private void ColorBreakdownSetup_Load(object sender, EventArgs e)
		{
			tableLayoutPanelControls.Controls.Clear();
			
			foreach (ColorBreakdownItem breakdownItem in _data.BreakdownItems) {
				addControl(new ColorBreakdownItemControl(breakdownItem));
			}

			// let's just make up some hardcoded templates. Can expand on this later; probably don't need to,
			// people can request new ones and stuff if they want.
			comboBoxTemplates.Items.Clear();
			comboBoxTemplates.Items.Add("RGB");
			comboBoxTemplates.Items.Add("RGBW");
			comboBoxTemplates.Items.Add("RGBY");
			comboBoxTemplates.SelectedIndex = 0;
		}

		private void buttonAddColor_Click(object sender, EventArgs e)
		{
			addControl(new ColorBreakdownItemControl());
		}

		void control_DeleteRequested(object sender, EventArgs e)
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

				default:
					Vixen.Sys.VixenSystem.Logging.Error("Color Breakdown Setup: got an unknown template to apply: " + template);
					MessageBox.Show("Error applying template: Unknown template.");
					break;
			}
		}

	}
}
