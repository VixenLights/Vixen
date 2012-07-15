using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ControlsEx.ValueControls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;

namespace VixenModules.App.ColorGradients
{
	[DefaultEvent("GradientChanged")]
	public partial class GradientEditPanel : UserControl
	{
		#region variables

		private XYZ _xyz = XYZ.White;
		private ColorPicker.Mode _mode = ColorPicker.Mode.HSV_RGB;
		private ColorPicker.Fader _fader = ColorPicker.Fader.HSV_H;

		#endregion


		public GradientEditPanel()
		{
			InitializeComponent();
			edit.SelectionDoubleClicked += edit_SelectionDoubleClicked;
			LockColorEditorHSV_Value = true;
		}


		#region handlers
		//updates all controls
		private void UpdateUI()
		{
			if (edit.Gradient == null)
			{
				grpStops.Enabled = false;
			}
			else
			{
				grpStops.Enabled = true;
				ColorPoint cpt = edit.Selection as ColorPoint;
				if (cpt != null)
				{
					vColorLoc.Enabled = true;
					lblColorSelect.Enabled = btnDeleteColor.Enabled = !edit.FocusSelection;
					//
					vColorLoc.Value = (int)((edit.FocusSelection ? cpt.Focus : cpt.Position) * 100.0);
					lblColorSelect.Color = edit.FocusSelection ? Color.DimGray :
						lblColorSelect.OldColor = (_xyz = cpt.Color).ToRGB().ToArgb();
				}
				else
				{
					lblColorSelect.Enabled = vColorLoc.Enabled = btnDeleteColor.Enabled = false;
					lblColorSelect.Color = lblColorSelect.OldColor = Color.DimGray;
					vColorLoc.Value = 0;
				}
			}
		}

		//triggered if gradient or selection changed
		private void edit_GradientChanged(object sender, EventArgs e)
		{
			UpdateUI();
			if (GradientChanged != null)
				GradientChanged(this, e);
		}

		//triggered if edit panel selection double clicked
		private void edit_SelectionDoubleClicked(object sender, EventArgs e)
		{
			if (ReadOnly)
				return;

			editSelectedColor();
		}


		//active color changed
		private void lblColorSelect_Click(object sender, EventArgs e)
		{
			if (ReadOnly)
				return;

			editSelectedColor();
		}

		// edits the selected color in the 'edit' control
		private void editSelectedColor()
		{
			if (edit.Gradient == null || edit.FocusSelection)
				return;
			ColorPoint pt = edit.Selection as ColorPoint;
			if (pt == null)
				return;
			using (ColorPicker frm = new ColorPicker(_mode, _fader)) {
				frm.LockValue_V = LockColorEditorHSV_Value;
				frm.Color = _xyz;
				if (frm.ShowDialog(this.FindForm()) == DialogResult.OK) {
					pt.Color = _xyz = frm.Color;
					lblColorSelect.Color = _xyz.ToRGB().ToArgb();
					_mode = frm.SecondaryMode;
					_fader = frm.PrimaryFader;
				}
			}
		}


		//active color dragged
		private void lblColorSelect_ColorChanged(object sender, EventArgs e)
		{
			if (edit.Gradient == null || edit.FocusSelection)
				return;
			ColorPoint pt = edit.Selection as ColorPoint;
			if (pt != null)
				pt.Color = XYZ.FromRGB(new RGB(lblColorSelect.Color));
		}

		//active color point location
		private void vColorLoc_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
		{
			if (edit.Gradient == null)
				return;
			ColorPoint pt = edit.Selection as ColorPoint;
			if (pt == null)
				return;
			if (edit.FocusSelection)
				pt.Focus = (double)vColorLoc.Value / 100.0;
			else
				pt.Position = (double)vColorLoc.Value / 100.0;
		}

		//delete active color point
		private void btnDeleteColor_Click(object sender, EventArgs e)
		{
			if (edit.Gradient == null || edit.FocusSelection || ReadOnly)
				return;
			int index = edit.SelectedColorIndex;
			if (index == -1) return;
			edit.Gradient.Colors.RemoveAt(index);
			UpdateUI();
		}

		#endregion


		#region properties

		/// <summary>
		/// gets or sets the gradient object
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorGradient Gradient
		{
			get { return edit.Gradient; }
			set { edit.Gradient = value; UpdateUI(); }
		}

		public bool LockColorEditorHSV_Value { get; set; }

		private bool _readonly;
		public bool ReadOnly
		{
			get { return _readonly; }
			set
			{
				_readonly = value;
				edit.ReadOnly = value;
				btnDeleteColor.Enabled = !value;
				vColorLoc.Enabled = !value;
			}
		}

		#endregion


		/// <summary>
		/// triggered if gradient changed
		/// </summary>
		[Description("triggered if gradient changed")]
		public event EventHandler GradientChanged;
	}
}
