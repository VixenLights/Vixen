using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonElements.ControlsEx.ValueControls;
using CommonElements.ColorManagement.ColorModels;
using CommonElements.ColorManagement.ColorPicker;

namespace CommonElements.ColorManagement.Gradients
{
	[DefaultEvent("GradientChanged")]
	public partial class GradientEditPanel : UserControl
	{
		#region variables
		private XYZ _xyz = XYZ.White;
		private ColorPicker.ColorPicker.Mode _mode = ColorPicker.ColorPicker.Mode.HSV_RGB;
		private ColorPicker.ColorPicker.Fader _fader = ColorPicker.ColorPicker.Fader.HSV_H;
		#endregion
		public GradientEditPanel()
		{
			InitializeComponent();
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
		//active color changed
		private void lblColorSelect_Click(object sender, EventArgs e)
		{
			if (edit.Gradient == null || edit.FocusSelection)
				return;
			ColorPoint pt = edit.Selection as ColorPoint;
			if (pt == null)
				return;
			using (ColorPicker.ColorPicker frm = new ColorPicker.ColorPicker(_mode, _fader))
			{
				frm.Color = _xyz;
				if (frm.ShowDialog(this.FindForm()) == DialogResult.OK)
				{
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
			if (edit.Gradient == null || edit.FocusSelection)
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
		public Gradient Gradient
		{
			get { return edit.Gradient; }
			set { edit.Gradient = value; UpdateUI(); }
		}
		#endregion
		/// <summary>
		/// triggered if gradient changed
		/// </summary>
		[Description("triggered if gradient changed")]
		public event EventHandler GradientChanged;

		private void buttonLoadPreset_Click(object sender, EventArgs e)
		{
			// TODO: once this has moved to an EffectEditor module, and is dependent on the RGB property, load
			// and present a GradientCollectionSelector from it. (It should be populated from the RGB static data.)
		}

		private void buttonSaveNewPreset_Click(object sender, EventArgs e)
		{
			// TODO: once this has moved to an EffectEditor module, and is dependent on the RGB property, get a name
			// from the user and save it to the RGB library.
		}
	}
}
