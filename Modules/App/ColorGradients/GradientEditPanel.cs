using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ControlsEx.ValueControls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources.Properties;

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
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			edit.SelectionDoubleClicked += edit_SelectionDoubleClicked;
			LockColorEditorHSV_Value = false;
		}

		#region handlers

		//updates all controls
		private void UpdateUI()
		{
			if (edit.Gradient == null) {
				grpStops.Enabled = false;
			}
			else {
				grpStops.Enabled = true;
				//ColorPoint cpt = edit.Selection as ColorPoint;
				if (edit.Selection.Count > 0) {
					vColorLoc.Enabled = true;
					lblColorSelect.Enabled = btnDeleteColor.Enabled = !edit.FocusSelection;
					//
					vColorLoc.Value =
						(int) ((edit.FocusSelection ? edit.Selection.First().Focus : edit.Selection.First().Position)*100.0);

					if (edit.Selection.Count == 0)
						lblColorSelect.Color = Color.DimGray;
					else {
						ColorPoint cpt = edit.Selection.First() as ColorPoint;
						lblColorSelect.Color = edit.FocusSelection
						                       	? Color.DimGray
						                       	: lblColorSelect.OldColor = (_xyz = cpt.Color).ToRGB().ToArgb();
					}
				}
				else {
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

			editSelectedPoints();
		}


		//active color changed
		private void lblColorSelect_Click(object sender, EventArgs e)
		{
			if (ReadOnly)
				return;

			editSelectedPoints();
		}

		// edits the selected color in the 'edit' control
		private void editSelectedPoints()
		{
			if (edit.Gradient == null || edit.FocusSelection)
				return;

			if (DiscreteColors) {
				List<Color> selectedColors = new List<Color>();
				foreach (ColorGradient.Point point in edit.Selection) {
					ColorPoint pt = point as ColorPoint;
					if (pt == null)
						continue;
					selectedColors.Add(pt.Color.ToRGB().ToArgb());
				}

				using (DiscreteColorPicker picker = new DiscreteColorPicker()) {
					picker.ValidColors = ValidDiscreteColors;
					picker.SelectedColors = selectedColors;
					if (picker.ShowDialog() == DialogResult.OK) {
						if (picker.SelectedColors.Count() == 0) {
							DeleteColor();
						}
						//else if (picker.SelectedColors.Count() == selectedColors.Count) {
						//	int i = 0;
						//	foreach (Color selectedColor in picker.SelectedColors) {
						//		ColorPoint pt = edit.Selection[i] as ColorPoint;
						//		pt.Color = XYZ.FromRGB(selectedColor);
						//	}
						//}
						else {
							double position = edit.Selection.First().Position;

							foreach (ColorGradient.Point point in edit.Selection) {
								edit.Gradient.Colors.Remove(point as ColorPoint);
							}

							foreach (Color selectedColor in picker.SelectedColors) {
								ColorPoint newPoint = new ColorPoint(selectedColor, position);
								edit.Gradient.Colors.Add(newPoint);
							}
						}
					}
				}
			}
			else {
				if (edit.Selection.Count > 1)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Non-discrete color gradient, >1 selected point. oops! please report it.", "Delete library gradient?", false, false);
					messageBox.ShowDialog();
				}
				ColorPoint pt = edit.Selection.FirstOrDefault() as ColorPoint;
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
		}


		//active color dragged
		private void lblColorSelect_ColorChanged(object sender, EventArgs e)
		{
			if (edit.Gradient == null || edit.FocusSelection)
				return;

			if (edit.Selection.Count > 1)
				return;

			ColorPoint pt = edit.Selection.FirstOrDefault() as ColorPoint;
			if (pt != null)
				pt.Color = XYZ.FromRGB(new RGB(lblColorSelect.Color));
		}

		//active color point location
		private void vColorLoc_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
		{
			if (edit.Gradient == null)
				return;
			foreach (ColorGradient.Point point in edit.Selection) {
				ColorPoint pt = point as ColorPoint;
				if (pt == null)
					return;
				if (edit.FocusSelection)
					pt.Focus = (double) vColorLoc.Value/100.0;
				else
					pt.Position = (double) vColorLoc.Value/100.0;
			}
		}

		//delete active color point
		private void btnDeleteColor_Click(object sender, EventArgs e)
		{
			DeleteColor();
		}

		private void DeleteColor()
		{
			if (edit.Gradient == null || edit.FocusSelection || ReadOnly)
				return;
			foreach (int i in edit.SelectedColorIndex.Reverse()) {
				if (i == -1) return;
				edit.Gradient.Colors.RemoveAt(i);
				UpdateUI();
			}
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
			set
			{
				edit.Gradient = value;
				UpdateUI();
			}
		}

		public IEnumerable<Color> ValidDiscreteColors
		{
			get { return edit.ValidDiscreteColors; }
			set
			{
				edit.ValidDiscreteColors = value;
				UpdateUI();
			}
		}

		public bool DiscreteColors
		{
			get { return edit.DiscreteColors; }
			set
			{
				edit.DiscreteColors = value;
				UpdateUI();
			}
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

		private void edit_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				DeleteColor();
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

		#region Draw lines and GroupBox borders
		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
		#endregion
	}
}