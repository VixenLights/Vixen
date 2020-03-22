using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.Property.Color
{
	public partial class ColorSetsSetupForm : BaseForm
	{
		private ColorStaticData _data;

		public ColorSetsSetupForm(ColorStaticData colorStaticData)
		{
			_data = colorStaticData;
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			
			Icon = Resources.Icon_Vixen3;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonAddColor.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddColor.Text = "";
			buttonAddColorSet.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddColorSet.Text = "";
			buttonRemoveColorSet.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonRemoveColorSet.Text = "";
			ThemeUpdateControls.UpdateControls(this);
			
		}

		private void ColorSetsSetupForm_Load(object sender, EventArgs e)
		{
			UpdateColorSetsList();
			UpdateGroupBoxWithColorSet(null, null);
		}

		private void ResizeColumnHeaders()
		{

			for (int i = 0; i < listViewColorSets.Columns.Count; i++)
			{
				listViewColorSets.Columns[i].Width = listViewColorSets.ClientRectangle.Width;
			}
		}

		private void UpdateColorSetsList()
		{
			listViewColorSets.BeginUpdate();
			listViewColorSets.Items.Clear();
			foreach (string colorSetName in _data.GetColorSetNames()) {
				listViewColorSets.Items.Add(colorSetName);
			}
			listViewColorSets.EndUpdate();
			ResizeColumnHeaders();
		}

		private void UpdateGroupBoxWithColorSet(string name, ColorSet cs)
		{
			if (cs == null) {
				panelColorSet.Enabled = false;
				label2.ForeColor = ThemeColorTable.ForeColorDisabled;
				label3.ForeColor = ThemeColorTable.ForeColorDisabled;
				textBoxName.Text = string.Empty;
				tableLayoutPanelColors.Controls.Clear();
				return;
			}

			panelColorSet.Enabled = true;
			label2.ForeColor = ThemeColorTable.ForeColor;
			label3.ForeColor = ThemeColorTable.ForeColor;
			textBoxName.Text = name;

			tableLayoutPanelColors.Controls.Clear();

			foreach (System.Drawing.Color color in cs) {
				ColorPanel colorPanel = new ColorPanel(color);
				tableLayoutPanelColors.Controls.Add(colorPanel);
			}
		}

		private void listViewColorSets_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool selected = listViewColorSets.SelectedItems.Count > 0;

			if (selected) {
				string name = listViewColorSets.SelectedItems[0].Text;
				UpdateGroupBoxWithColorSet(name, _data.GetColorSet(name));
			}
			else {
				UpdateGroupBoxWithColorSet(null, null);
			}

			buttonRemoveColorSet.Enabled = selected;
		}

		private void buttonAddColorSet_Click(object sender, EventArgs e)
		{
			using (TextDialog textDialog = new TextDialog("New Color Set name?", "New Color Set")) {
				if (textDialog.ShowDialog() == DialogResult.OK) {
					string newName = textDialog.Response;

					if (_data.ContainsColorSet(newName)) {
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Color Set already exists.", "Error", false, false);
						messageBox.ShowDialog();
						return;
					}

					ColorSet newcs = new ColorSet();
					_data.SetColorSet(newName, newcs);
					UpdateGroupBoxWithColorSet(newName, newcs);
					UpdateColorSetsList();
				}
			}
		}

		private void buttonRemoveColorSet_Click(object sender, EventArgs e)
		{
			if (listViewColorSets.SelectedItems.Count > 0) {
				string item = listViewColorSets.SelectedItems[0].Text;
				if (!_data.RemoveColorSet(item)) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error removing Color Set!", "Error", false, false);
					messageBox.ShowDialog();
				}
			}
			UpdateColorSetsList();
			UpdateGroupBoxWithColorSet(null, null);
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (SaveDisplayedColorSet())
				UpdateColorSetsList();
		}

		private void buttonAddColor_Click(object sender, EventArgs e)
		{
			ColorPanel colorPanel = new ColorPanel(System.Drawing.Color.FromArgb(255,255,255));
			tableLayoutPanelColors.Controls.Add(colorPanel);
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			if (textBoxName.Text.Length <= 0)
				return;

			if (_data.ContainsColorSet(textBoxName.Text)) {
				buttonUpdate.Text = "Update Color Set";
			}
			else {
				buttonUpdate.Text = "Make New Color Set";
			}
		}

		private bool displayedColorSetHasDifferences()
		{
			if (!panelColorSet.Enabled)
				return false;

			if (_data.ContainsColorSet(textBoxName.Text)) {
				ColorSet cs = _data.GetColorSet(textBoxName.Text);

				int i = 0;
				foreach (var control in tableLayoutPanelColors.Controls) {
					if (cs.Count <= i)
						return true;

					ColorPanel cp = (ColorPanel)control;
					if (cs[i].ToArgb() != cp.Color.ToArgb())
						return true;

					i++;
				}

				if (cs.Count != i)
					return true;

				return false;
			}

			return true;
		}

		private bool SaveDisplayedColorSet()
		{
			string name = textBoxName.Text;
			ColorSet newColorSet = new ColorSet();

			if (name.Length <= 0) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("You must enter a name for the Color Set.", "Name Requred", false, false);
				messageBox.ShowDialog();
				return false;
			}

			foreach (var control in tableLayoutPanelColors.Controls) {
				ColorPanel cp = (ColorPanel)control;
				newColorSet.Add(cp.Color);
			}

			_data.SetColorSet(textBoxName.Text, newColorSet);

			return true;
		}

		private void ColorSetsSetupForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (displayedColorSetHasDifferences()) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Do you want to save changes to the displayed color set?", "Save Changes?", true, true);
				messageBox.ShowDialog();

				switch (messageBox.DialogResult)
				{
					case DialogResult.OK:
						if (!SaveDisplayedColorSet()) {
							e.Cancel = true;
						}
						break;

					case DialogResult.No:
						break;

					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}