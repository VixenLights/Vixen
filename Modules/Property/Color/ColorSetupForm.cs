using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Color
{
	public partial class ColorSetupForm : BaseForm
	{
		public ColorSetupForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		public ColorModule ColorModule { get; set; }

		private void ColorSetupForm_Load(object sender, EventArgs e)
		{
			PopulateColorSetsComboBox();

			SelectRadioButton();

			colorPanelSingleColor.Color = ColorModule.SingleColor;

			colorPanelSingleColor.ColorChanged += colorPanelSingleColor_ColorChanged;
		}

		#region Overrides of Form

		/// <inheritdoc />
		protected override void OnClosing(CancelEventArgs e)
		{
			colorPanelSingleColor.ColorChanged -= colorPanelSingleColor_ColorChanged;
		}

		#endregion

		private void PopulateColorSetsComboBox()
		{
			comboBoxColorSet.BeginUpdate();
			comboBoxColorSet.Items.Clear();

			foreach (string colorSetName in (ColorModule.StaticModuleData as ColorStaticData).GetColorSetNames()) {
				comboBoxColorSet.Items.Add(colorSetName);
				if (colorSetName == ColorModule.ColorSetName) {
					comboBoxColorSet.SelectedIndex = comboBoxColorSet.Items.Count - 1;
				}
			}

			if (comboBoxColorSet.SelectedIndex < 0) {
				comboBoxColorSet.SelectedIndex = 0;
			}

			comboBoxColorSet.EndUpdate();
		}

		private void SelectRadioButton()
		{
			switch (ColorModule.ColorType) {
				case ElementColorType.SingleColor:
					radioButtonOptionSingle.Checked = true;
					break;

				case ElementColorType.MultipleDiscreteColors:
					radioButtonOptionMultiple.Checked = true;
					break;

				case ElementColorType.FullColor:
					radioButtonOptionFullColor.Checked = true;
					break;
			}
		}

		private void comboBoxColorSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			ColorModule.ColorSetName = comboBoxColorSet.SelectedItem.ToString();
		}

		private void AnyRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			colorPanelSingleColor.Enabled = radioButtonOptionSingle.Checked;
			comboBoxColorSet.Enabled = radioButtonOptionMultiple.Checked;
			buttonColorSetsSetup.Enabled = radioButtonOptionMultiple.Checked;

			if (radioButtonOptionSingle.Checked) {
				ColorModule.ColorType = ElementColorType.SingleColor;
			}
			else if (radioButtonOptionMultiple.Checked) {
				ColorModule.ColorType = ElementColorType.MultipleDiscreteColors;
			}
			else if (radioButtonOptionFullColor.Checked) {
				ColorModule.ColorType = ElementColorType.FullColor;
			}
		}

		private void buttonColorSetsSetup_Click(object sender, EventArgs e)
		{
			using (ColorSetsSetupForm cssf = new ColorSetsSetupForm(ColorModule.StaticModuleData as ColorStaticData)) {
				cssf.ShowDialog();
				PopulateColorSetsComboBox();
			}
		}

		private void colorPanelSingleColor_ColorChanged(object sender, ColorPanelEventArgs e)
		{
			ColorModule.SingleColor = colorPanelSingleColor.Color;
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}