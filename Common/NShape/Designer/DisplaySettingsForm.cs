/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/


using System;
using System.Drawing;
using System.Windows.Forms;

using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.Designer {

	public partial class DisplaySettingsForm : Form {

		public DisplaySettingsForm()
			: this(null) {
		}


		public DisplaySettingsForm(Form owner) {
			InitializeComponent();
			if (owner != null) {
				Owner = owner;
				Icon = Owner.Icon;
			}
			resizePointCombo.Items.Clear();
			connectionPointCombo.Items.Clear();
			foreach (ControlPointShape ptShape in Enum.GetValues(typeof(ControlPointShape))) {
				resizePointCombo.Items.Add(ptShape);
				connectionPointCombo.Items.Add(ptShape);
			}
		}


		public bool ShowGrid {
			get { return showGridCheckBox.Checked; }
			set { showGridCheckBox.Checked = value; }
		}


		public bool SnapToGrid {
			get { return snapToGridCheckBox.Checked; }
			set { snapToGridCheckBox.Checked = value; }
		}


		public int GridSize {
			get { return Convert.ToInt32(gridSizeUpDown.Value); }
			set { gridSizeUpDown.Value = value; }
		}


		public int SnapDistance {
			get { return Convert.ToInt32(snapDistanceUpDown.Value); }
			set { snapDistanceUpDown.Value = value; }
		}


		public Color GridColor {
			get { return gridColorLabel.BackColor; }
			set { gridColorLabel.BackColor = value; }
		}


		public ControlPointShape ResizePointShape {
			get { return (ControlPointShape)resizePointCombo.SelectedItem; }
			set { resizePointCombo.SelectedItem = value; }
		}


		public ControlPointShape ConnectionPointShape {
			get { return (ControlPointShape)connectionPointCombo.SelectedItem; }
			set { connectionPointCombo.SelectedItem = value; }
		}


		public int ControlPointSize {
			get { return Convert.ToInt32(pointSizeUpDown.Value); }
			set { pointSizeUpDown.Value = value; }
		}


		public bool HideDeniedMenuItems {
			get { return hideDeniedMenuItemsCheckBox.Checked;}
			set { hideDeniedMenuItemsCheckBox.Checked = value; }
		}


		public bool ShowDefaultContextMenu {
			get { return showDynamicContextMenu.Checked; }
			set { showDynamicContextMenu.Checked = value; }
		}


		private void chooseGridColorButton_Click(object sender, EventArgs e) {
			colorDialog.AllowFullOpen = true;
			colorDialog.FullOpen = true;
			colorDialog.AnyColor = true;
			colorDialog.Color = GridColor;

			// Add current Display color as CustomColor
			if (colorDialog.CustomColors != null) {
				int gridColorRGB = ColorToRGB(GridColor);
				int[] colors = colorDialog.CustomColors;
				int idx = Array.IndexOf(colors, gridColorRGB, 0);
				if (idx < 0) {
					int emptyArgb = ColorToRGB(Color.White);
					int maxIdx = colors.Length - 1;
					do ++idx; while (idx < maxIdx && colors[idx] != emptyArgb);
					colors[idx] = gridColorRGB;
				}
				colorDialog.CustomColors = colors;
			}

			if (colorDialog.ShowDialog(this) == DialogResult.OK)
				GridColor = colorDialog.Color;
		}


		private int ColorToRGB(Color color) {
			return (int)(((uint)((((color.R << 0x10) | (color.G << 8)) | color.B)) & uint.MaxValue));
		}

	
		private void cancelButton_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}


		private void okButton_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.OK;
		}

		private void snapDistanceUpDown_ValueChanged(object sender, EventArgs e) {

		}
	}
}