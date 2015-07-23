using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	public partial class ColorGradientEditor : Form
	{
		private bool _discreteColors;
		private IEnumerable<Color> _validDiscreteColors;

		public ColorGradientEditor(ColorGradient gradient, bool discreteColors, IEnumerable<Color> validDiscreteColors)
		{
			InitializeComponent();
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonEditLibraryItem.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonLoadFromLibrary.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonOK.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonSaveToLibrary.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonUnlink.BackgroundImage = Resources.HeadingBackgroundImage;
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;

			gradientEditPanel.GradientChanged += GradientChangedHandler;
			Gradient = gradient;
			_discreteColors = discreteColors;
			_validDiscreteColors = validDiscreteColors;
			PopulateFormWithGradient(_gradient);
		}

		private ColorGradient _gradient;

		public ColorGradient Gradient
		{
			get { return _gradient; }
			set
			{
				_gradient = new ColorGradient(value);
				PopulateFormWithGradient(_gradient);
			}
		}

		private string _libraryItemName;

		public string LibraryItemName
		{
			get { return _libraryItemName; }
			set
			{
				_libraryItemName = value;
				PopulateFormWithGradient(Gradient);
			}
		}

		private ColorGradientLibrary _library;

		private ColorGradientLibrary Library
		{
			get
			{
				if (_library == null)
					_library =
						ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;

				return _library;
			}
		}


		public void GradientChangedHandler(object sender, EventArgs e)
		{
			// the editor panel should be operating on a reference to our gradient, so we don't need to do anything.
		}


		private void PopulateFormWithGradient(ColorGradient item)
		{
			gradientEditPanel.Gradient = item;
			gradientEditPanel.DiscreteColors = _discreteColors;
			gradientEditPanel.ValidDiscreteColors = _validDiscreteColors;

			// if we're editing one from the library, treat it special
			if (item.IsCurrentLibraryGradient) {
				if (LibraryItemName == null) {
					labelCurve.Text = "This gradient is a library gradient.";
					Text = "Color Gradient Editor: Library Gradient";
				}
				else {
					labelCurve.Text = string.Format("This gradient is the library gradient: {0}", LibraryItemName);
					Text = string.Format("Color Gradient Editor: Library Gradient {0}", LibraryItemName);
				}

				gradientEditPanel.ReadOnly = false;
				buttonSaveToLibrary.Enabled = false;
				buttonLoadFromLibrary.Enabled = false;
				buttonUnlink.Enabled = false;
				buttonEditLibraryItem.Enabled = false;
			}
			else {
				if (item.IsLibraryReference) {
					labelCurve.Text = string.Format("This gradient is linked to the library: {0}", item.LibraryReferenceName);
				}
				else {
					labelCurve.Text = "This gradient is not linked to any in the library.";
				}

				gradientEditPanel.ReadOnly = item.IsLibraryReference;
				buttonSaveToLibrary.Enabled = !item.IsLibraryReference;
				buttonLoadFromLibrary.Enabled = true;
				buttonUnlink.Enabled = item.IsLibraryReference;
				buttonEditLibraryItem.Enabled = item.IsLibraryReference;

				Text = @"Color Gradient Editor";
			}

			gradientEditPanel.Invalidate();
		}


		private void buttonLoadFromLibrary_Click(object sender, EventArgs e)
		{
			ColorGradientLibrarySelector selector = new ColorGradientLibrarySelector();
			if (selector.ShowDialog() == System.Windows.Forms.DialogResult.OK && selector.SelectedItem != null) {
				// make a new curve that references the selected library curve, and set it to the current Curve
				ColorGradient newGradient = new ColorGradient(selector.SelectedItem.Item2);
				newGradient.LibraryReferenceName = selector.SelectedItem.Item1;
				newGradient.IsCurrentLibraryGradient = false;
				Gradient = newGradient;
			}
		}

		private void buttonSaveToLibrary_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Gradient name?");

			while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				if (dialog.Response == string.Empty) {
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (Library.Contains(dialog.Response)) {
					DialogResult result = MessageBox.Show("There is already a gradient with that name. Do you want to overwrite it?",
					                                      "Overwrite gradient?", MessageBoxButtons.YesNoCancel);
					if (result == System.Windows.Forms.DialogResult.Yes) {
						Library.AddColorGradient(dialog.Response, new ColorGradient(Gradient));
						break;
					}
					else if (result == System.Windows.Forms.DialogResult.Cancel) {
						break;
					}
				}
				else {
					Library.AddColorGradient(dialog.Response, new ColorGradient(Gradient));
					break;
				}
			}
		}

		private void buttonUnlink_Click(object sender, EventArgs e)
		{
			Gradient.UnlinkFromLibrary();
			PopulateFormWithGradient(Gradient);
		}

		private void buttonEditLibraryItem_Click(object sender, EventArgs e)
		{
			string libraryName = Gradient.LibraryReferenceName;
			Library.EditLibraryItem(libraryName);
			PopulateFormWithGradient(Gradient);
		}

		private void ColorGradientEditor_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				DialogResult = DialogResult.OK;
			if (e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.Cancel;
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		#region Draw lines and GroupBox borders
		//set color for box borders.
		private Color _borderColor = Color.FromArgb(136, 136, 136);

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			//used to draw the boards and text for the groupboxes to change the default box color.
			//get the text size in groupbox
			Size tSize = TextRenderer.MeasureText((sender as GroupBox).Text, Font);

			e.Graphics.Clear(BackColor);
			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, _borderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 10;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
			e.Graphics.DrawString((sender as GroupBox).Text, Font, new SolidBrush(Color.FromArgb(221, 221, 221)), textRect);
		}
		#endregion
	}
}